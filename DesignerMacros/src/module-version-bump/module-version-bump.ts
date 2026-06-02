/// <reference path="../../typings/elementmacro.context.api.d.ts" />

const MODULE_SETTINGS_STEREOTYPE_ID = "76256322-88f1-4efb-8bd6-7cc4a6c54cb9";
const VERSION_PROPERTY_NAME = "Version";
const INCLUDE_RELEASE_NOTES_PROPERTY_NAME = "Include Release Notes";

interface IVersionInfo {
    major: number;
    minor: number;
    patch: number;
    preIndex: number | null;
}

function parseVersion(version: string): IVersionInfo | null {
    const match = version.trim().match(/^(\d+)\.(\d+)\.(\d+)(?:-pre\.(\d+))?$/);
    if (!match) return null;
    return {
        major: parseInt(match[1], 10),
        minor: parseInt(match[2], 10),
        patch: parseInt(match[3], 10),
        preIndex: match[4] !== undefined ? parseInt(match[4], 10) : null
    };
}

// Returns positive if a > b, negative if a < b, 0 if equal.
// A release version (preIndex = null) is considered higher than a pre-release
// with the same major.minor.patch.
function compareVersions(a: IVersionInfo, b: IVersionInfo): number {
    if (a.major !== b.major) return a.major - b.major;
    if (a.minor !== b.minor) return a.minor - b.minor;
    if (a.patch !== b.patch) return a.patch - b.patch;
    if (a.preIndex === null && b.preIndex === null) return 0;
    if (a.preIndex === null) return 1;   // release > pre-release
    if (b.preIndex === null) return -1;  // pre-release < release
    return a.preIndex - b.preIndex;
}

function bumpVersion(info: IVersionInfo, bumpType: "patch" | "minor" | "major"): string {
    if (info.preIndex !== null && bumpType === "patch") {
        // Pre-release patch: just increment the pre index
        return `${info.major}.${info.minor}.${info.patch}-pre.${info.preIndex + 1}`;
    }
    switch (bumpType) {
        case "patch": return `${info.major}.${info.minor}.${info.patch + 1}-pre.0`;
        case "minor": return `${info.major}.${info.minor + 1}.0-pre.0`;
        case "major": return `${info.major + 1}.0.0-pre.0`;
    }
}

function changeTypePrefix(changeType: string): string {
    switch (changeType) {
        case "fix":         return "Fixed";
        case "improvement": return "Improvement";
        case "feature":     return "New Feature";
        default:            return changeType;
    }
}

async function bumpModuleVersion(element: MacroApi.Context.IElementApi): Promise<void> {
    const moduleSettings = element.getStereotype(MODULE_SETTINGS_STEREOTYPE_ID);
    if (!moduleSettings) {
        await dialogService.error(
            "Module Settings stereotype not found on this element.\n" +
            "Please run this script from an Intent Module package."
        );
        return;
    }

    const settingsVersionRaw = moduleSettings.getProperty(VERSION_PROPERTY_NAME).getValue() as string;
    if (!settingsVersionRaw) {
        await dialogService.error("Version property is empty in Module Settings.");
        return;
    }

    const settingsVersionInfo = parseVersion(settingsVersionRaw);
    if (!settingsVersionInfo) {
        await dialogService.error(`Cannot parse Module Settings version: "${settingsVersionRaw}". Expected X.Y.Z or X.Y.Z-pre.N`);
        return;
    }

    // Query the imodspec on disk to get its version
    const checkerResult = await executeModuleTask(
        "Intent.ModuleBuilder.IModSpecChecker",
        JSON.stringify({ applicationId: application.id })
    );

    let imodspecVersionInfo: IVersionInfo | null = null;
    let imodspecVersionRaw: string | null = null;
    try {
        const parsed = JSON.parse(checkerResult);
        if (parsed?.version && !parsed?.errorMessage) {
            imodspecVersionRaw = parsed.version as string;
            imodspecVersionInfo = parseVersion(imodspecVersionRaw);
        }
    } catch { /* ignore — fall back to settings version */ }

    // Use whichever version is higher as the effective current version to bump from
    let effectiveVersionInfo = settingsVersionInfo;
    let effectiveVersionRaw = settingsVersionRaw;
    let versionSource = "Module Settings";

    if (imodspecVersionInfo && compareVersions(imodspecVersionInfo, settingsVersionInfo) > 0) {
        effectiveVersionInfo = imodspecVersionInfo;
        effectiveVersionRaw = imodspecVersionRaw!;
        versionSource = ".imodspec on disk";
    }

    const isPreRelease = effectiveVersionInfo.preIndex !== null;
    const includeReleaseNotes = moduleSettings.getProperty(INCLUDE_RELEASE_NOTES_PROPERTY_NAME)?.getValue() as boolean ?? false;

    const versionHint = imodspecVersionInfo && imodspecVersionRaw !== settingsVersionRaw
        ? `Bumping from ${effectiveVersionRaw} (${versionSource}). Module Settings: ${settingsVersionRaw}, .imodspec: ${imodspecVersionRaw}.`
        : `Current version: ${effectiveVersionRaw}`;

    // Build form fields
    const fields: MacroApi.Context.IDynamicFormFieldConfig[] = [
        {
            id: "bumpType",
            fieldType: "select",
            label: "Version Bump Type",
            isRequired: true,
            hint: isPreRelease
                ? `Currently at pre-release (${effectiveVersionRaw}).`
                : versionHint,
            hintType: isPreRelease ? "info" : "primary",
            value: "patch",
            selectOptions: [
                { id: "patch",  description: "Patch — Bug fixes and minor corrections" },
                { id: "minor",  description: "Minor — New backwards-compatible functionality" },
                { id: "major",  description: "Major — Breaking changes" }
            ]
        }
    ];

    if (includeReleaseNotes) {
        fields.push({
            id: "changeType",
            fieldType: "select",
            label: "Change Type",
            isRequired: true,
            value: "improvement",
            selectOptions: [
                { id: "fix",         description: "Fix — Bug fix or correction" },
                { id: "improvement", description: "Improvement — Enhancement to existing functionality" },
                { id: "feature",     description: "New Feature — Brand new functionality" }
            ]
        });
        fields.push({
            id: "description",
            fieldType: "textarea",
            label: "Description",
            isRequired: true,
            placeholder: "Describe the change..."
        });
    }

    const result = await dialogService.openForm({
        title: "Bump Module Version",
        submitButtonText: "Bump Version",
        minWidth: "520px",
        fields: fields
    });

    if (!result) return;

    const bumpType = result.bumpType as "patch" | "minor" | "major";
    const newVersion     = bumpVersion(effectiveVersionInfo, bumpType);
    const newBaseVersion = newVersion.split("-")[0];

    // Always update the Module Settings version
    moduleSettings.getProperty(VERSION_PROPERTY_NAME).setValue(newVersion);

    if (includeReleaseNotes) {
        const changeType  = result.changeType as string;
        const description = (result.description as string)?.trim();

        if (!description) {
            await dialogService.error("A description is required.");
            return;
        }

        const prefix = changeTypePrefix(changeType);
        const taskConfig = {
            applicationId: application.id,
            version: newBaseVersion,
            changePrefix: prefix,
            description: description
        };

        const taskResult = await executeModuleTask(
            "Intent.ModuleBuilder.ReleaseNoteUpdater",
            JSON.stringify(taskConfig)
        );

        let parsedResult: any;
        try { parsedResult = JSON.parse(taskResult); } catch { parsedResult = null; }

        if (parsedResult?.errorMessage) {
            await dialogService.error(`Version updated to ${newVersion}, but release-notes.md could not be updated:\n\n${parsedResult.errorMessage}`);
            return;
        }

        await dialogService.info(`Version bumped from ${effectiveVersionRaw} to ${newVersion}.\n\nrelease-notes.md updated with:\n- ${prefix}: ${description}`);
    } else {
        await dialogService.info(`Version bumped from ${effectiveVersionRaw} to ${newVersion}.`);
    }
}

/**
 * Bump Module Version accelerator.
 * Run from the context menu of an Intent Module package.
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/module-version-bump/module-version-bump.ts
 */

//Uncomment below
//await bumpModuleVersion(element);
