/// <reference path="../common/nuget-package-updater.ts" />

async function updateNuGetPackage(element: MacroApi.Context.IElementApi): Promise<void> {

    if (isPackageLocked(element)) return;
    let result = await getLatestNugetPackages([element.getName()]);
    updateNugetPackageElements([element], result);
    await dialogService.info("Update complete.");
}

/**
 * Used by Intent.Modules\Modules\Intent.ModuleBuilder.CSharp
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/nuget-package-updater/update-package/update-package.ts
 */

//Uncomment below
//await updateNuGetPackage(element);