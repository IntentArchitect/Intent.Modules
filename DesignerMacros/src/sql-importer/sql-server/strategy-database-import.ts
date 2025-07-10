/// <reference path="./common.ts" />

class DatabaseImportStrategy {
    public async execute(packageElement: MacroApi.Context.IElementApi): Promise<void> {
        let defaults = this.getDialogDefaults(packageElement);
        let capturedInput = await this.presentImportDialog(defaults);
        let importModel = this.createImportModel(capturedInput);
        let executionResult = await executeImporterModuleTask("Intent.Modules.SqlServerImporter.Tasks.DatabaseImport", importModel);
        
        if (executionResult.errors?.length > 0) {
            await displayExecutionResultErrors(executionResult);
        } else if (executionResult.warnings?.length > 0) {
            await displayExecutionResultWarnings(executionResult, "Import Complete.");
        } else {
            await dialogService.info("Import Complete.");
        }
    }

    private getDialogDefaults(element: MacroApi.Context.IElementApi): ISqlDatabaseImportPackageSettings {
        let domainPackage = element.getPackage();
        let persistedValue = this.getSettingValue(domainPackage, "sql-import:typesToExport", "");
        let includeTables = "true";
        let includeViews = "true";
        let includeStoredProcedures = "true";
        let includeIndexes = "true";
        if (persistedValue) {
            includeTables = "false";
            includeViews = "false";
            includeStoredProcedures = "false";
            includeIndexes = "false";
            persistedValue.split(";").forEach(i => {
                switch (i.toLocaleLowerCase()) {
                    case 'table':
                        includeTables = "true";
                        break;
                    case 'view':
                        includeViews = "true";
                        break;
                    case 'storedprocedure':
                        includeStoredProcedures = "true";
                        break;
                    case 'index':
                        includeIndexes = "true";
                        break;
                    default:
                        break;
                }
            });
        }
        let result: ISqlDatabaseImportPackageSettings = {
            entityNameConvention: this.getSettingValue(domainPackage, "sql-import:entityNameConvention", "SingularEntity"),
            tableStereotypes: this.getSettingValue(domainPackage, "sql-import:tableStereotypes", "WhenDifferent"),
            includeTables: includeTables,
            includeViews: includeViews,
            includeStoredProcedures: includeStoredProcedures,
            includeIndexes: includeIndexes,
            importFilterFilePath: this.getSettingValue(domainPackage, "sql-import:importFilterFilePath", null),
            connectionString: this.getSettingValue(domainPackage, "sql-import:connectionString", null),
            storedProcedureType: this.getSettingValue(domainPackage, "sql-import:storedProcedureType", ""),
            settingPersistence: this.getSettingValue(domainPackage, "sql-import:settingPersistence", "None")
        };
        return result;
    }

    private async presentImportDialog(defaults: ISqlDatabaseImportPackageSettings): Promise<any> {
        let formConfig: MacroApi.Context.IDynamicFormConfig = {
            title: "Sql Server Import",
            fields: [
                {
                    id: "settingPersistence",
                    fieldType: "select",
                    label: "Persist Settings",
                    hint: "Remember these settings for next time you run the import",
                    value: defaults.settingPersistence,
                    selectOptions: [
                        { id: "None", description: "(None)" },
                        { id: "All", description: "All Settings" },
                        { id: "AllSanitisedConnectionString", description: "All (with Sanitized connection string, no password))" },
                        { id: "AllWithoutConnectionString", description: "All (without connection string))" }
                    ]
                }
            ],
            sections: [
                {
                    name: 'Connection string',
                    fields: [
                        {
                            id: "connectionString",
                            fieldType: "text",
                            label: "Connection String",
                            placeholder: null,
                            hint: null,
                            isRequired: true,
                            value: defaults.connectionString
                        },
                        {
                            id: "connectionStringTest",
                            fieldType: "button",
                            label: "Test Connection",
                            hint: "Test whether the Connection String is valid to access the Database Server",
                            onClick: async (form: MacroApi.Context.IDynamicFormApi) => {
                                let testConnectionModel = {
                                    connectionString: form.getField("connectionString").value as string
                                };
                                
                                let executionResult = await executeImporterModuleTask(
                                    "Intent.Modules.SqlServerImporter.Tasks.TestConnection",
                                    testConnectionModel);
                                
                                if (executionResult.errors?.length > 0) {
                                    form.getField("connectionStringTest").hint = "Failed to connect.";
                                    await displayExecutionResultErrors(executionResult);
                                } else {
                                    form.getField("connectionStringTest").hint = "Connection established successfully.";
                                    await dialogService.info("Connection established successfully.");
                                }
                            }
                        }
                    ],
                    isCollapsed: false,
                    isHidden: false
                },
                {
                    name: 'Import settings',
                    fields: [
                        {
                            id: "entityNameConvention",
                            fieldType: "select",
                            label: "Entity name convention",
                            placeholder: "",
                            hint: "",
                            value: defaults.entityNameConvention,
                            selectOptions: [{ id: "SingularEntity", description: "Singularized table name" }, { id: "MatchTable", description: "Table name, as is" }]
                        },
                        {
                            id: "tableStereotypes",
                            fieldType: "select",
                            label: "Apply Table Stereotypes",
                            placeholder: "",
                            hint: "When to apply Table stereotypes to your domain entities",
                            value: defaults.tableStereotypes,
                            selectOptions: [{ id: "WhenDifferent", description: "If They Differ" }, { id: "Always", description: "Always" }]
                        },
                        {
                            id: "includeTables",
                            fieldType: "checkbox",
                            label: "Include Tables",
                            hint: "Export SQL tables",
                            value: defaults.includeTables
                        },
                        {
                            id: "includeViews",
                            fieldType: "checkbox",
                            label: "Include Views",
                            hint: "Export SQL views",
                            value: defaults.includeViews
                        },
                        {
                            id: "includeStoredProcedures",
                            fieldType: "checkbox",
                            label: "Include Stored Procedures",
                            hint: "Export SQL stored procedures",
                            value: defaults.includeStoredProcedures
                        },
                        {
                            id: "includeIndexes",
                            fieldType: "checkbox",
                            label: "Include Indexes",
                            hint: "Export SQL indexes",
                            value: defaults.includeIndexes
                        },
                        {
                            id: "importFilterFilePath",
                            fieldType: "open-file",
                            label: "Import Filter File",
                            hint: "Path to import filter JSON file (see [documentation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-sqlserverimporter/intent-sqlserverimporter.html#import-filter-file))",
                            placeholder: "(optional)",
                            value: defaults.importFilterFilePath,
                            openFileOptions: {
                                fileFilters: [{ name: "JSON", extensions: ["json"] }]
                            }
                        },
                        {
                            id: "manageFilters",
                            fieldType: "button",
                            label: "Manage Filters",
                            hint: "",
                            onClick: async () => {
                                
                            }
                        },
                        {
                            id: "storedProcedureType",
                            fieldType: "select",
                            label: "Stored Procedure Representation",
                            value: defaults.storedProcedureType,
                            selectOptions: [
                                { id: "Default", description: "(Default)" },
                                { id: "StoredProcedureElement", description: "Stored Procedure Element" },
                                { id: "RepositoryOperation", description: "Stored Procedure Operation" }
                            ]
                        }
                    ],
                    isCollapsed: true,
                    isHidden: false
                }
            ],
            height: "70%"
        }
        
        let capturedInput = await dialogService.openForm(formConfig);
        return capturedInput;
    }

    private createImportModel(capturedInput: any): IDatabaseImportModel {
        const typesToExport: string[] = [];
        if (capturedInput.includeTables == "true") {
            typesToExport.push("Table");
        }
        if (capturedInput.includeViews == "true") {
            typesToExport.push("View");
        }
        if (capturedInput.includeStoredProcedures == "true") {
            typesToExport.push("StoredProcedure");
        }
        if (capturedInput.includeIndexes == "true") {
            typesToExport.push("Index");
        }

        const domainDesignerId: string = "6ab29b31-27af-4f56-a67c-986d82097d63";

        let importConfig: IDatabaseImportModel = {
            applicationId: application.id,
            designerId: domainDesignerId,
            packageId: element.getPackage().id,
            entityNameConvention: capturedInput.entityNameConvention,
            tableStereotype: capturedInput.tableStereotypes,
            typesToExport: typesToExport,
            importFilterFilePath: capturedInput.importFilterFilePath,
            storedProcedureType: capturedInput.storedProcedureType,
            connectionString: capturedInput.connectionString,
            settingPersistence: capturedInput.settingPersistence
        };

        return importConfig;
    }

    private getSettingValue(domainPackage: MacroApi.Context.IPackageApi, key: string, defaultValue: string): string {
        let persistedValue = domainPackage.getMetadata(key);
        return persistedValue ? persistedValue : defaultValue;
    }
}

interface ISqlDatabaseImportPackageSettings {
    entityNameConvention: string;
    tableStereotypes: string;
    includeTables: string;
    includeViews: string;
    includeStoredProcedures: string;
    includeIndexes: string;
    importFilterFilePath: string;
    storedProcedureType: string;
    connectionString: string;
    settingPersistence: string;
}

interface IDatabaseImportModel {
    applicationId: string;
    designerId: string;
    packageId: string;
    entityNameConvention: string;
    tableStereotype: string;
    typesToExport: string[];
    importFilterFilePath: string;
    storedProcedureType: string;
    connectionString: string;
    // Ignoring PackageFileName
    settingPersistence: string;
}