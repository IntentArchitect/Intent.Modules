/// <reference path="./common.ts" />
/// <reference path="./icons.ts" />

class StoredProceduresImportStrategy {
    public async execute(repositoryElement: MacroApi.Context.IElementApi): Promise<void> {
        let defaults = this.getDialogDefaults(repositoryElement);
        let capturedInput = await this.presentImportDialog(defaults);
        if (capturedInput == null) {
            return;
        }
        let importModel = await this.createImportModel(capturedInput);
        if (importModel == null) {
            return;
        }
        let executionResult = await executeImporterModuleTask("Intent.Modules.SqlServerImporter.Tasks.StoredProcedureImport", importModel);
        
        if (executionResult.errors?.length > 0) {
            await displayExecutionResultErrors(executionResult);
        } else if (executionResult.warnings?.length > 0) {
            await displayExecutionResultWarnings(executionResult, "Import Complete.");
        } else {
            await dialogService.info("Import Complete.");
        }
    }

    private getDialogDefaults(element: MacroApi.Context.IElementApi): ISqlStoredProceduresImportPackageSettings {
        let domainPackage = element.getPackage();

        let result: ISqlStoredProceduresImportPackageSettings = {
            inheritedConnectionString: this.getSettingValue(domainPackage, "sql-import:connectionString", null),
            connectionString: this.getSettingValue(domainPackage, "sql-import-repository:connectionString", null),
            storedProcedureType: this.getSettingValue(domainPackage, "sql-import-repository:storedProcedureType", ""),
            storedProcNames: "",
            settingPersistence: this.getSettingValue(domainPackage, "sql-import-repository:settingPersistence", "None")
        };
        return result;
    }

    private async presentImportDialog(defaults: ISqlStoredProceduresImportPackageSettings): Promise<any> {
        let formConfig: MacroApi.Context.IDynamicFormConfig = {
            title: "Sql Server Import",
            fields: [
                {
                    id: "connectionString",
                    fieldType: "text",
                    label: "Connection String",
                    placeholder: "(optional if inherited setting)",
                    hint: null,
                    value: defaults.connectionString
                },
                {
                    id: "storedProcedureType",
                    fieldType: "select",
                    label: "Stored Procedure Representation",
                    value: defaults.storedProcedureType,
                    selectOptions: [
                        { id: "", description: "(default or inherited setting)" },
                        { id: "StoredProcedureElement", description: "Stored Procedure Element" },
                        { id: "RepositoryOperation", description: "Stored Procedure Operation" }
                    ]
                },
                {
                    id: "storedProcNames",
                    fieldType: "text",
                    label: "Stored Procedure Names",
                    placeholder: "Enter Stored Procedure names (comma-separated) or use Browse button",
                    hint: "Enter Stored procedure names (comma-separated) or use the browse button.",
                    value: defaults.storedProcNames,
                    isRequired: true
                },
                {
                    id: "storedProcBrowse",
                    fieldType: "button",
                    label: "Browse",
                    onClick: async (form: MacroApi.Context.IDynamicFormApi) => {
                        const connectionStringValue = form.getField("connectionString").value as string;
                        const settingPersistenceValue = form.getField("settingPersistence").value as string;

                        if (settingPersistenceValue != "InheritDb" && (connectionStringValue == null || connectionStringValue?.trim() === "")) {
                            await dialogService.error("Please enter a connection string (or inherit DB settings) before browsing stored procedures.");
                            return;
                        }

                        let connectionStringStr = settingPersistenceValue == "InheritDb" ? defaults.inheritedConnectionString : connectionStringValue;

                        try {
                            let storedProcNames = form.getField("storedProcNames").value as string;
                            let capturedStoredProcs = (storedProcNames).split(",").map(x => x.trim());

                            const selectedProcs = await this.openStoredProcedureBrowseDialog(connectionStringStr, capturedStoredProcs);
                            if (selectedProcs.length > 0) {
                                const storedProcNamesField = form.getField("storedProcNames");
                                storedProcNamesField.value = selectedProcs.join(", ");
                            }
                        } catch (e) {
                            await dialogService.error("Error browsing stored procedures: " + e);
                        }
                    }
                },
                {
                    id: "settingPersistence",
                    fieldType: "select",
                    label: "Persist Settings",
                    hint: "Remember these settings for next time you run the import",
                    value: defaults.settingPersistence,
                    selectOptions: [
                        { id: "None", description: "(None)" },
                        { id: "InheritDb", description: "Inherit Database Settings" },
                        { id: "All", description: "All Settings" },
                        { id: "AllSanitisedConnectionString", description: "All (with Sanitized connection string, no password))" },
                        { id: "AllWithoutConnectionString", description: "All (without connection string))" }
                    ]
                }
            ]
        }

        let capturedInput = await dialogService.openForm(formConfig);
        return capturedInput;
    }

    private async createImportModel(capturedInput: any): Promise<IStoredProceduresImportModel|null> {
        if (capturedInput.settingPersistence != "InheritDb" && (!capturedInput.connectionString || capturedInput.connectionString?.trim() === "")) {
            await dialogService.error("Connection String was not set.");
            return null;
        }

        const storedProcNamesArray = capturedInput.storedProcNames.split(',').map((name: string) => name.trim());

        const domainDesignerId: string = "6ab29b31-27af-4f56-a67c-986d82097d63";

        let importConfig: IStoredProceduresImportModel = {
            applicationId: application.id,
            designerId: domainDesignerId,
            packageId: element.getPackage().id,
            storedProcedureType: capturedInput.storedProcedureType,
            connectionString: capturedInput.connectionString,
            storedProcNames: storedProcNamesArray,
            repositoryElementId: element.id,
            settingPersistence: capturedInput.settingPersistence
        };

        return importConfig;
    }

    private getSettingValue(domainPackage: MacroApi.Context.IPackageApi, key: string, defaultValue: string): string {
        let persistedValue = domainPackage.getMetadata(key);
        return persistedValue ? persistedValue : defaultValue;
    }

    private async openStoredProcedureBrowseDialog(connectionString: string, preSelectedStoredProcs: string[]): Promise<string[]> {
        let inputProcs = this.sanitizePreSelectedStoredProcs(preSelectedStoredProcs);

        let storedProcSelection: MacroApi.Context.IDynamicFormFieldConfig = {
            id: "storedProcSelection",
            fieldType: "tree-view",
            label: "Stored Procedure Selection",
            isRequired: true,
            treeViewOptions: {
                isMultiSelect: true,
                selectableTypes: [
                    {
                        specializationId: "Database",
                        autoExpand: true,
                        isSelectable: (x) => false
                    },
                    {
                        specializationId: "Schema",
                        autoExpand: true,
                        autoSelectChildren: true,
                        isSelectable: (x) => true
                    },
                    {
                        specializationId: "Stored-Procedure",
                        isSelectable: (x) => true
                    }
                ]
            }
        };

        try {
            let executionResult = await executeImporterModuleTask("Intent.Modules.SqlServerImporter.Tasks.StoredProcList", { "connectionString": connectionString });
            
            if (executionResult.errors?.length > 0) {
                await displayExecutionResultErrors(executionResult);
                return [];
            }
            
            let spListResult = executionResult.result as IStoredProcListResultModel;

            storedProcSelection.treeViewOptions.rootNode = {
                id: "database",
                specializationId: "Database",
                label: "Database",
                icon: Icons.databaseIcon,
                children: Object.keys(spListResult.storedProcs).map(schemaName => {
                    return {
                        id: `schema.${schemaName}`,
                        label: schemaName,
                        specializationId: "Schema",
                        icon: Icons.schemaIcon,
                        isSelected: inputProcs.some(x => x.startsWith(`sp.${schemaName}`)),
                        children: spListResult.storedProcs[schemaName].map(sp => {
                            return {
                                id: `sp.${schemaName}.${sp}`,
                                label: sp,
                                specializationId: "Stored-Procedure",
                                icon: Icons.storedProcIcon,
                                isSelected: inputProcs.some(x => x == `sp.${schemaName}.${sp}`)
                            } as MacroApi.Context.ISelectableTreeNode;
                        })
                    } as MacroApi.Context.ISelectableTreeNode;
                })
            };
            
        } catch (e) {
            await dialogService.error(e);
            return [];
        }

        let browseFormConfig: MacroApi.Context.IDynamicFormConfig = {
            title: "Browse Stored Procedures",
            fields: [
                storedProcSelection
            ]
        };

        let browseInputs = await dialogService.openForm(browseFormConfig);

        if (browseInputs && browseInputs.storedProcSelection) {
            let selection = browseInputs.storedProcSelection as string[];
            let filteredSelection = selection.filter(x => !x.startsWith("schema."));
            return filteredSelection
                .map(x => {
                    let parts = x.split(".");
                    return `${parts[1]}.${parts[2]}`;
                });
        }

        return [];
    }

    private sanitizePreSelectedStoredProcs(preSelectedStoredProcs: string[]): string[] {
        if (preSelectedStoredProcs == null || preSelectedStoredProcs.filter(x => x != "").length === 0) {
            return [];
        }

        return preSelectedStoredProcs.map(x => !x.startsWith("dbo.") ? `sp.dbo.${x}` : `sp.${x}`);
    }
}

interface ISqlStoredProceduresImportPackageSettings {
    inheritedConnectionString: string;
    connectionString: string;
    storedProcedureType: string;
    storedProcNames: string;
    settingPersistence: string;
}

interface IStoredProceduresImportModel {
    applicationId: string;
    designerId: string;
    packageId: string;
    storedProcedureType: string;
    connectionString: string;
    storedProcNames: string[];
    repositoryElementId: string;
    settingPersistence: string;
}

interface IStoredProcListResultModel {
    storedProcs: { [key: string]: string[] };
}