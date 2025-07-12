/// <reference path="./common.ts" />
/// <reference path="./icons.ts" />

class DatabaseImportStrategy {
    public async execute(packageElement: MacroApi.Context.IElementApi): Promise<void> {
        let defaults = this.getDialogDefaults(packageElement);
        let capturedInput = await this.presentImportDialog(defaults, packageElement.id);
        if (capturedInput == null) {
            return;
        }
        let importModel = this.createImportModel(capturedInput);
        let executionResult = await executeImporterModuleTask("Intent.Modules.SqlServerImporter.Tasks.DatabaseImport", importModel);
        
        if ((executionResult.errors ?? []).length > 0) {
            await displayExecutionResultErrors(executionResult);
        } else if ((executionResult.warnings ?? []).length > 0) {
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

    private async presentImportDialog(defaults: ISqlDatabaseImportPackageSettings, packageId: string): Promise<any> {
        let formConfig: MacroApi.Context.IDynamicFormConfig = {
            title: "Sql Server Import",
            fields: [],
            sections: [
                {
                    name: "Connection & Settings",
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
                                
                                if ((executionResult.errors ?? []).length > 0) {
                                    form.getField("connectionStringTest").hint = "Failed to connect.";
                                    await displayExecutionResultErrors(executionResult);
                                } else {
                                    form.getField("connectionStringTest").hint = "Connection established successfully.";
                                    await dialogService.info("Connection established successfully.");
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
                                { id: "All", description: "All Settings" },
                                { id: "AllSanitisedConnectionString", description: "All (with Sanitized connection string, no password))" },
                                { id: "AllWithoutConnectionString", description: "All (without connection string))" }
                            ]
                        }
                    ],
                    isCollapsed: false,
                    isHidden: false
                },
                {
                    name: "Database Objects",
                    fields: [
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
                        }
                    ],
                    isCollapsed: true,
                    isHidden: false
                },
                {
                    name: "Import Options",
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
                },
                {
                    name: 'Filtering',
                    fields: [
                        {
                            id: "importFilterFilePath",
                            fieldType: "open-file",
                            label: "Import Filter File",
                            hint: "Path to import filter JSON file (see [documentation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-sqlserverimporter/intent-sqlserverimporter.html#import-filter-file))",
                            placeholder: "(optional)",
                            value: defaults.importFilterFilePath,
                            openFileOptions: {
                                fileFilters: [{ name: "JSON", extensions: ["json"] }]
                            },
                            onChange: (form) => {
                                
                            }
                        },
                        {
                            id: "manageIncludeFilters",
                            fieldType: "button",
                            label: "Manage Filters",
                            onClick: async (form: MacroApi.Context.IDynamicFormApi) => {
                                const connectionString = form.getField("connectionString").value as string;
                                if (!connectionString) {
                                    await dialogService.error("Please enter a connection string first.");
                                    return;
                                }
                                let importFilterFilePath = form.getField("importFilterFilePath").value as string;
                                
                                await this.presentManageFiltersDialog(connectionString, packageId, importFilterFilePath);
                            }
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

    private async presentManageFiltersDialog(connectionString: string, packageId: string, importFilterFilePath: string): Promise<void> {
        try {
            const metadata = await this.fetchDatabaseMetadata(connectionString);
            if (!metadata) {
                return;
            }

            // Get package location for relative path resolution
            const packageLocationModel = {
                applicationId: application.id,
                packageId: packageId
            };
            const packageLocationResult = await executeImporterModuleTask(
                "Intent.Modules.SqlServerImporter.Tasks.PackageLocationGet",
                packageLocationModel
            );
            
            const packageFileName = packageLocationResult.result?.PackageDirectory || null;

            // Load existing filter data if file path exists
            let existingFilter: ImportFilterModel | null = null;
            if (importFilterFilePath) {
                try {
                    const filterLoadModel = {
                        importFilterFilePath: importFilterFilePath,
                        packageFileName: packageFileName
                    };
                    const filterLoadResult = await executeImporterModuleTask(
                        "Intent.Modules.SqlServerImporter.Tasks.FilterLoad",
                        filterLoadModel
                    );
                    
                    if ((filterLoadResult.errors ?? []).length === 0 && filterLoadResult.result) {
                        existingFilter = filterLoadResult.result as ImportFilterModel;
                    }
                } catch (error) {
                    console.warn("Could not load existing filter file:", error);
                }
            }

            const inclusiveSelection: MacroApi.Context.IDynamicFormFieldConfig = {
                id: "inclusiveSelection",
                fieldType: "tree-view",
                label: "Objects to Include in Import",
                isRequired: false,
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
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Table",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Stored-Procedure",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "View",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        }
                    ]
                }
            };

            const exclusiveSelection: MacroApi.Context.IDynamicFormFieldConfig = {
                id: "exclusiveSelection",
                fieldType: "tree-view",
                label: "Objects to Exclude from Import",
                isRequired: false,
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
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Table",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "Stored-Procedure",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        },
                        {
                            specializationId: "View",
                            autoSelectChildren: false,
                            isSelectable: (x) => true
                        }
                    ]
                }
            };

            let allSchemas = [...Object.keys(metadata.tables), ...Object.keys(metadata.storedProcedures), ...Object.keys(metadata.views)]
            let distinctSchemas = [...new Set(allSchemas)];

            // Create tree nodes with pre-selected states for inclusive filter
            inclusiveSelection.treeViewOptions.rootNode = {
                id: "Database",
                label: "Database",
                specializationId: "Database",
                icon: Icons.databaseIcon,
                children: distinctSchemas.map(schemaName => {
                    return {
                        id: `schema.${schemaName}`,
                        label: schemaName,
                        specializationId: "Schema",
                        icon: Icons.schemaIcon,
                        isSelected: this.isSchemaIncluded(schemaName, existingFilter),
                        children: this.createSchemaTreeNodes(schemaName, metadata, existingFilter, "include")
                    };
                })
            };

            // Create tree nodes with pre-selected states for exclusive filter
            exclusiveSelection.treeViewOptions.rootNode = {
                id: "Database",
                label: "Database",
                specializationId: "Database",
                icon: Icons.databaseIcon,
                children: distinctSchemas.map(schemaName => {
                    return {
                        id: `schema.${schemaName}`,
                        label: schemaName,
                        specializationId: "Schema",
                        icon: Icons.schemaIcon,
                        isSelected: this.isSchemaExcluded(schemaName, existingFilter),
                        children: this.createSchemaTreeNodes(schemaName, metadata, existingFilter, "exclude")
                    };
                })
            };

            const formConfig: MacroApi.Context.IDynamicFormConfig = {
                title: "Manage Filters",
                fields: [],
                sections: [
                    {
                        name: "Inclusive Objects",
                        fields: [inclusiveSelection],
                        isCollapsed: false,
                        isHidden: false
                    },
                    {
                        name: "Exclusive Objects", 
                        fields: [exclusiveSelection],
                        isCollapsed: true,
                        isHidden: false
                    }
                ]
            };
            
            try {
                const result = await dialogService.openForm(formConfig);
                if (result) {
                    // Handle the save operation when dialog is closed with OK
                    await this.saveFilterData(result, packageId, importFilterFilePath, packageFileName);
                }
            } catch (error) {
                console.error("Error in filter selection dialog:", error);
            }
        } catch (error) {
            await dialogService.error(`Error loading database metadata: ${error}`);
        }
    }

    private async fetchDatabaseMetadata(connectionString: string): Promise<IDatabaseMetadata|null> {
        // Get database metadata
        const metadataModel = { connectionString: connectionString };
        const metadataExecutionResult = await executeImporterModuleTask(
            "Intent.Modules.SqlServerImporter.Tasks.DatabaseMetadataExtract", 
            metadataModel);
        
        if ((metadataExecutionResult.errors ?? []).length > 0) {
            await displayExecutionResultErrors(metadataExecutionResult);
            return null;
        }
        
        const metadata = metadataExecutionResult.result as IDatabaseMetadata;
        if (!metadata) {
            await dialogService.error("No database metadata received.");
            return null;
        }

        return metadata;
    }

    private createSchemaTreeNodes(
        schemaName: string, 
        metadata: { 
            tables: Record<string, string[]>, 
            storedProcedures: Record<string, string[]>, 
            views: Record<string, string[]> 
        },
        existingFilter: ImportFilterModel | null = null,
        filterType: "include" | "exclude"
    ): MacroApi.Context.ISelectableTreeNode[] {
        const nodes: MacroApi.Context.ISelectableTreeNode[] = [];

        if (metadata.tables[schemaName]?.some(x => x)) {
            nodes.push(this.createCategoryNode(
                schemaName,
                "tables",
                "Tables",
                "Table",
                Icons.tableIcon,
                metadata.tables[schemaName],
                existingFilter,
                filterType
            ));
        }

        if (metadata.storedProcedures[schemaName]?.some(x => x)) {
            nodes.push(this.createCategoryNode(
                schemaName,
                "storedProcedures",
                "Stored Procedures",
                "Stored-Procedure",
                Icons.storedProcIcon,
                metadata.storedProcedures[schemaName],
                existingFilter,
                filterType
            ));
        }

        if (metadata.views[schemaName]?.some(x => x)) {
            nodes.push(this.createCategoryNode(
                schemaName,
                "views",
                "Views",
                "View",
                Icons.viewIcon,
                metadata.views[schemaName],
                existingFilter,
                filterType
            ));
        }

        return nodes;
    }

    private createCategoryNode(
        schemaName: string,
        category: string,
        label: string,
        specializationId: string,
        icon: any,
        items: string[],
        existingFilter: ImportFilterModel | null = null,
        filterType: "include" | "exclude"
    ): MacroApi.Context.ISelectableTreeNode {
        return {
            id: `${schemaName}.${category}`,
            label: label,
            specializationId: specializationId,
            icon: icon,
            isSelected: this.isCategorySelected(category, existingFilter, filterType),
            children: items.map(item => ({
                id: `${schemaName}.${category}.${item}`,
                label: item,
                specializationId: specializationId,
                icon: icon,
                isSelected: this.isItemSelected(schemaName, category, item, existingFilter, filterType)
            } as MacroApi.Context.ISelectableTreeNode))
        };
    }

    private getSettingValue(domainPackage: MacroApi.Context.IPackageApi, key: string, defaultValue: string): string {
        let persistedValue = domainPackage.getMetadata(key);
        return persistedValue ? persistedValue : defaultValue;
    }

    private isSchemaIncluded(schemaName: string, existingFilter: ImportFilterModel | null): boolean {
        if (!existingFilter) {
            return false;
        }
        return existingFilter.schemas.includes(schemaName);
    }

    private isSchemaExcluded(schemaName: string, existingFilter: ImportFilterModel | null): boolean {
        if (!existingFilter) {
            return false;
        }
        // Check if any tables, views, or stored procedures from this schema are in exclude lists
        const excludedTables = existingFilter.exclude_tables?.some(table => table.startsWith(`${schemaName}.`)) ?? false;
        const excludedViews = existingFilter.exclude_views?.some(view => view.startsWith(`${schemaName}.`)) ?? false;
        const excludedStoredProcs = existingFilter.exclude_stored_procedures?.some(sp => sp.startsWith(`${schemaName}.`)) ?? false;
        return excludedTables || excludedViews || excludedStoredProcs;
    }

    private isCategorySelected(
        category: string, 
        existingFilter: ImportFilterModel, 
        filterType: "include" | "exclude"
    ): boolean {
        if (!existingFilter) {
            return false;
        }

        if (filterType === "include") {
            switch (category) {
                case "tables":
                    return existingFilter.include_tables?.length > 0;
                case "views":
                    return existingFilter.include_views?.length > 0;
                case "storedProcedures":
                    return existingFilter.include_stored_procedures?.length > 0;
                default:
                    return false;
            }
        } else {
            switch (category) {
                case "tables":
                    return existingFilter.exclude_tables?.length > 0;
                case "views":
                    return existingFilter.exclude_views?.length > 0;
                case "storedProcedures":
                    return existingFilter.exclude_stored_procedures?.length > 0;
                default:
                    return false;
            }
        }
    }

    private isItemSelected(
        schemaName: string, 
        category: string, 
        item: string, 
        existingFilter: ImportFilterModel | null, 
        filterType: "include" | "exclude"
    ): boolean {
        if (!existingFilter) {
            return false;
        }

        const fullItemName = `${schemaName}.${item}`;

        if (filterType === "include") {
            switch (category) {
                case "tables":
                    return existingFilter.include_tables?.some(table => table.name === fullItemName) ?? false;
                case "views":
                    return existingFilter.include_views?.some(view => view.name === fullItemName) ?? false;
                case "storedProcedures":
                    return existingFilter.include_stored_procedures?.includes(fullItemName) ?? false;
                default:
                    return false;
            }
        } else {
            switch (category) {
                case "tables":
                    return existingFilter.exclude_tables?.includes(fullItemName) ?? false;
                case "views":
                    return existingFilter.exclude_views?.includes(fullItemName) ?? false;
                case "storedProcedures":
                    return existingFilter.exclude_stored_procedures?.includes(fullItemName) ?? false;
                default:
                    return false;
            }
        }
    }

    private async saveFilterData(formResult: any, packageId: string, importFilterFilePath: string, packageFileName: string | null): Promise<void> {
        try {
            // Extract selections from form result
            const inclusiveSelections = formResult.inclusiveSelection || [];
            const exclusiveSelections = formResult.exclusiveSelection || [];


            // Create filter model from selections
            const filterModel: ImportFilterModel = {
                schemas: [],
                include_tables: [],
                include_views: [],
                include_stored_procedures: [],
                exclude_tables: [],
                exclude_views: [],
                exclude_stored_procedures: []
            };

            // Process inclusive selections
            inclusiveSelections.forEach((selection: string) => {
                if (selection.startsWith('schema.')) {
                    const schemaName = selection.replace('schema.', '');
                    filterModel.schemas.push(schemaName);
                } else if (selection.includes('.tables.')) {
                    // Extract schema and table name from ID like "schema.tables.tableName"
                    const parts = selection.split('.');
                    console.log(`INCLUDE ${selection} => ${JSON.stringify(parts)}`);
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const tableName = parts[2];
                        const fullTableName = `${schemaName}.${tableName}`;
                        const filterTableModel: FilterTableModel = { name: fullTableName, exclude_columns: [] };
                        filterModel.include_tables.push(filterTableModel);
                    }
                } else if (selection.includes('.views.')) {
                    // Extract schema and view name from ID like "schema.views.viewName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const viewName = parts[2];
                        const fullViewName = `${schemaName}.${viewName}`;
                        filterModel.include_views.push({ name: fullViewName, exclude_columns: [] });
                    }
                } else if (selection.includes('.storedProcedures.')) {
                    // Extract schema and stored procedure name from ID like "schema.storedProcedures.spName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const spName = parts[2];
                        const fullSpName = `${schemaName}.${spName}`;
                        filterModel.include_stored_procedures.push(fullSpName);
                    }
                }
            });

            // Process exclusive selections
            exclusiveSelections.forEach((selection: string) => {
                if (selection.includes('.tables.')) {
                    // Extract schema and table name from ID like "schema.tables.tableName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const tableName = parts[2];
                        const fullTableName = `${schemaName}.${tableName}`;
                        filterModel.exclude_tables.push(fullTableName);
                    }
                } else if (selection.includes('.views.')) {
                    // Extract schema and view name from ID like "schema.views.viewName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const viewName = parts[2];
                        const fullViewName = `${schemaName}.${viewName}`;
                        filterModel.exclude_views.push(fullViewName);
                    }
                } else if (selection.includes('.storedProcedures.')) {
                    // Extract schema and stored procedure name from ID like "schema.storedProcedures.spName"
                    const parts = selection.split('.');
                    if (parts.length >= 3) {
                        const schemaName = parts[0];
                        const spName = parts[2];
                        const fullSpName = `${schemaName}.${spName}`;
                        filterModel.exclude_stored_procedures.push(fullSpName);
                    }
                }
            });

            // Determine save path
            let savePath = importFilterFilePath;
            if (!savePath) {
                // Prompt for file name using input dialog
                const fileNameConfig: MacroApi.Context.IDynamicFormConfig = {
                    title: "Save Filter File",
                    fields: [
                        {
                            id: "fileName",
                            fieldType: "text",
                            label: "File Name",
                            placeholder: "filter.json",
                            isRequired: true,
                            value: "filter.json"
                        }
                    ]
                };
                
                const fileNameResult = await dialogService.openForm(fileNameConfig);
                if (!fileNameResult || !fileNameResult.fileName) {
                    return;
                }
                // Note: Package directory resolution will be handled by the backend
                savePath = fileNameResult.fileName;
            }

            // Save the filter data
            const saveModel = {
                importFilterFilePath: savePath,
                packageFileName: packageFileName,
                filterData: filterModel
            };

            const saveResult = await executeImporterModuleTask(
                "Intent.Modules.SqlServerImporter.Tasks.FilterSave",
                saveModel
            );

            if ((saveResult.errors ?? []).length > 0) {
                await displayExecutionResultErrors(saveResult);
            } else {
                await dialogService.info("Filters saved successfully.");
            }

        } catch (error) {
            await dialogService.error(`Error saving filters: ${error}`);
        }
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

interface IDatabaseMetadata {
    tables: { [key: string]: string[] };
    views: { [key: string]: string[] };
    storedProcedures: { [key: string]: string[] };
}

interface ImportFilterModel {
    schemas: string[];
    include_tables: FilterTableModel[];
    include_views: FilterViewModel[];
    include_stored_procedures: string[];
    exclude_tables: string[];
    exclude_views: string[];
    exclude_stored_procedures: string[];
}

interface FilterTableModel {
    name: string;
    exclude_columns: string[];
}

interface FilterViewModel {
    name: string;
    exclude_columns: string[];
}