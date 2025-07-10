/// <reference path="./common.ts" />

class StoredProceduresImportStrategy {
    public async execute(repositoryElement: MacroApi.Context.IElementApi): Promise<void> {
        let defaults = this.getDialogDefaults(repositoryElement);
        let capturedInput = await this.presentImportDialog(defaults);
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
                            await dialogService.error("Please enter a connection string before browsing stored procedures.");
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

            const databaseIcon: string = "data:image/png;base64,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";
            const storedProcIcon: string = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFAAAABQCAYAAACOEfKtAAAAAXNSR0IArs4c6QAAEi1JREFUeF7tXQt4lNWZft9z/pkEghcEBa2AZCYgskUFbauuVVdbL63tdivVWtvSdUWBJNLaZ9utdTe2unarLYVJUKm63qpd3LXV7dqrWG1rbb3VclFIAqJYKBcxICGZOed8+5x/MjCBSWaS+RNin57nGRKeOec97/n+7z+X73JClFEaGqAu24TRiMePEOdGkzIe4N8AqBHBkaSMBTgWQMWebgSyX5cEu31P2QJwkwCbIPK6giwXcA2htqRNZmsQw8aaFDrLoB5Z073E+wC5bi7GGhX7pIJcLERSgEovJAJBH2D6WtVBkAbQKZQ3FfCYg72/JoVn+goUZf2SBLhuFirdwcFpTnABBR8gMVUA1U8iLmwnXVrHUCP7iwWIbAbxSwf82In92bGN+FM/efWrWY8CFICvzschYvVMB3wd4JgSeugUhFqSYfazCZAVEL4BoJ1EO8h2ca4dRIcAMYoMd0oNpyD8QMlI53gciSSAOICAQLxLy4sKWkTut7D/av+MN6Y+FHIZ0FJQgKvmYUIcugHE2SDH9ciAMBC8LJBfK+A3znGtgt7GoLONbWibeDc6+steZkKvOhqHDBccnEnHDoV279LCk4RyOsD3QlCF/LkzryMC7U7k91RYlDjMPsIGZLV+AEo3AW6eixE7dGwWIF8DMLJAf50QrCVlhTg8XNFhfzbuTrw5ALx6hVzZgHiwOXiPVrhIiFMI1BTiK4ChyP+I6K/XNKVXDgTPPQJcPwcj0zpYRmLa/nOSdABMKWeaRgi2Hb4Yu+hnsSFQ/PycHoURqkOfD+Imku/an5a8BcFVyUb7X1FTDgX4Si0mBgx+CITCyy/NELckDvfA+EGenPsz0JUzEY+P0eeBnAPgnG67AiLtnLt6UqO7rT/YPbXhystxWMVw/SLA8XmVdhO4dVinue6oJWiPssPBwBIB183XFznLJhKj/YIY9isQreRjExfZR6LiwZb64Itw+GZuQvYrpxN8MjnaPDWQk29UA+gNZ918HGOtvhPg3+XqUbC8w5lTpy7G21FwYEtd4PdNR3aB7QRxXnKReToK8KGAIQ2It2wNfkrizC4+DhbnJBebJ6Lg5wW4dzEQSSVS9mpmN7d/MaW5NjiTwLLcW6aAG6pT5rooBpgvQFF0ddWLXFMUwEMJY80cVKsgaN3LSR5MpuylUXDspoEi+N2Gt82ZZ5WxAY6CVNQYzfNic6hkcQ5XIEtrUvbiKPrpLkCEJ4ubaxrNV6IAHwoYrXX6QwL+J4DDB1yAeSp+n7b2nycu9mfZd2Z5/fMYls6ozwrVAmStRXvKgGlgtz6AbXDu2mSTW/JOE2FrvbdHBvcCOBmA3pf/YAgw26c3fhIrANynnFla3YT1Q1WY/hzfFmhvbrsY4Ie7LDkF6Q6eAPNVUpABZJkibwmMeX7tGOw8qwHmQAnUny62zEXVTosjEQ/mCmQWwENL4XNABJhHzAlkE8GXIXiaIs+YjP395CXYWgr5cuo8NxuxkXEcD8ROA91pDnw3gQSAWF9wD7QAC3F1BFq8DQ6KqwBppeNWKLbBZNqMQrsmMoFGZkeATLAemYemwpwBqGQbYh0GsWEOsYwg1uFQWcH4wRJzBzsjIzVlvCjl3QYnAXIywL3+lb5IbZAXkX5S69bMeKu0wL/6sACcAEKExk3/8b/7I4+3Miv/UwEUCSf9mBCxLmtKSW6HvhAeihrYF/4HvO5fBVjmI/irAMsX4PdqUvayMmHC5t2tMVEgvgMwKO5fEo3uG1FQZUttsArElCjA3iEYGQVzcnUKL0XBl831+tMU3uX9r1EADnUMAr9SO80Hy3G55o+R/tDdmdE/B3naUB98BPze1qJOntiYfiUCrOwc6P9prsPBFH03yI9FBTzUcITYpYCPJhaZx6PkRh8o5M1WXhM7jG5U4MUCVEXZyQHG8u6JF7Soy7zmNdf5SLF4oiaVXhUFL7bUqWuSKfctD7Z0JvTxYzFDIzQFTY6igwOMsZvirq2qdEvG3oJdnkvoA4f+crLRXhkFN7+N+UFik7mID4XHrbD4VxrQFxH0e6WzouhocDHkdZIPwqn7qxvTK/KjKFrrg7MBmZVYZD8dBScvwK2w5sTkYrxeCLBlDmYw0N8AOEOAQ8oKRYuCcQ8YPqAIkM0CWZBMuUWFqkkDVOu22IMCh0h9IgKsYMacnbwNmwt17CNRP7MF4xz1iSA+CeACgCMGUB7FobPGXv/WvCQODwQxLsNbmVd62p74aIXWevVFgfp3QB6OVIAhW5HH4rBXlBIDs3Qm4tPG4G8DBOeCodl8NCFHOHBUlwXFT9xRWVFyWG2AbPV2VIisFoXHA2N/Xorf5unPY9gYo2YJ1C0Ahg/cWViwwTleOGlx5g/FVSBbQxoQrNqMyoMrEKfB8E6lT4CT6Q6cJOAkDUkKWChUrnAXRBoOrUJpUWCzE1kVc/bpjA4Ntj4uurMv8dHect1Sp+8j6N+cMEBz4ASYHdJuiNyndPDt6oWdq0sVZG/1Xvo0qoYfiuEug3hFgHgnUBEAMUdYWqQ7DdLDhqFTdqOjegl2RBE61zobh7i4+hTBa0BW5/MbaAHu6YuUxZK21yfG4k0eQP9HqQ8xq204CFZfSI1vQXhEoSjWQRNgF/HtpDwnIo92trsHph6AiNRiAvS+kkPj+hwQfmvizf4+vnqvIgCvQLBSKBsF3Kic/CHRZB8rhlvK9300Z4kP2v6RAP+nlFounZn1iduwJYpXrhSyuTp/mo3h6SA+wWpbLcL3C+RTeZGp3um1noKnQCyzgX188gL4IPcBKX0UYDcOPoB8lwjWEPiFAL9RafNMYgnaombqX831dTjBQPkA8zOA0PBRRWCYdHOcSysFX0La/uK+o7CzYQCDy3NjLEeA+8nJO4y8y1PADT5/gwLvmdsizm0DsRNgJ4kMBJ2OyFCgFREHEReReCgQUYcJZXQ2loWHExgLyDG9euNE1lPLN6t3u+9ySejE2lP82Vdc7ARFdwwoY0hVBQndoJ2A2yHCjVC2JXEYVvUnoNQbVF1P6QJRa1LUeGEUPvCUwFxSk8KWHH6YGFSF8QL9BVGcxazfuMccEwJWIFvpuMApc9eGUdheatCA10AfAz0s6sENBp4Q17R1mNRJeVrnDcQQzqXgBBIVe+KjSyPkN+1thPyKirdULzRPFWvmBbgQQH2xikPpewLbnciVNY32oRyv5jocR+qbITw/ilMQgd1OsFBr853qhfhzT+PnmvmYQhs8Q3gLzDujEHJpdcp+P7f6r63Xn3DC76K/Y8ieqwsePUXwsjPmA5NvK7ySZy3StcG5pNwP0E/eQ7lYJ7h+UqP5uifprSvN22K1CnIjgP4YN4xQlsLiESjeQeCgwoOXLRp25sQUntz3ez7RgMBPmOvqYu+zkJ8ga7IakoXA4w7mQ7mzcHN9bB6dpPqzCBKy0QkvrWk0v/SDXVsbfM0BX+0ZS3Zra0+duBjd7ARsnac/nGiyP/Igr9UhkRZ9A8hLhp4E5bVKZ6cf3YRtnltLvf4YhPegR63pcQQOkHvTzjYclxfvGKZDbAunshN7aimQdVrZU/LnRL+I3JhMmWvzGzXX6o+Q/BKyYENjhRZXn2x0Kc9zXS2OdQx+K0BJ8YDhHtRhNYkREJeqHu1uy+35vBvjfWMwJo3gMhA+Nrz3N1DwW6bN+bkDgxfgSyMqzKk5n0FOkE/MQuXRB8WmKLqbRejN+kVzdQdOa6VVYKfueXXr9KMELyypP8EqS14aH5VZXbUZwRFdGUreDLdmm/6cBud3JRr5h1GSDVPovlKzyN3k+2drXWCc4OYX/2y++ok8v0iOXGjJvTo+FeLOBXAeBKeCGF4S+SgqCURELqlpskvDBW+u/hQ17y8FWkTWxq09acKt2J6r/6drMHpXWs2m8HNgd6NDKZjZOrLRxu20yd/C1r1HOeeuvP9wd0ex82PLVTiCMfVxCR1O9J47L0wf9DggGiqQDYG27574HbzlTxjmoOBJAu8pYbBpiDz8Vtp+Jn+j3VIbfA1EuVlK/qHenWy0l+efhdsFcld7hf3y8V0uwGIkm+twOBkb78SN1UomwXGawH9kMsH++Zb37sl8POYWCh6uHm3n+TlrzVxMUTp4sdstIAVJynYI5nSOtj+Y2tA97b+1XjeJcG6xsRX7XoCttGZ6IWPCrzXw1Wc3mV8XeqWLAee+f/1yHJYZHhsnyo2zVo5WRJUIYqTyyXkxH4mqibTA7bbAbiX0KbZvK82NJm02MMAb+5rum2v1HSQvL8Lh2bQzM/0Ku/rzeJe2wXwIZliHGyc3mceb58WnUrvfQnra85U6Qlgl8g89WWN8av8yipmfaMKakiEHsGL4+o4IdpA9BpSLCFYEgXm/f93X1OqPU/E2Ckb5uV6ADenAHH/ct7G9tT64GcA15dOV24uZs3aL4HGl5OGYsz8txWNXPqnCCM3zYtOp5Pke8QXLM2LOGhFHZYfV11PCoID8gHRHcXWJRrc4G94RvEDguDL5ri4mwK5FJzwrpgXyQ9LerBVat7VjV/7kXCaRos1ba/XHhfzvHio6EVwwjGZlh+gnel5dZafttNU+JaO5Vl9O8o6iHRepUJoA94J4c4/xy7gIXyXgrxb5iRPzbE0KO8ol01v7llpVB6qCEQcAlinwCw7y47zk8YJwinLPcxvt5dUjUXFoReDNVTPK4d1XAfbUl4PIqyCXQ/BHoaym4E0l3JUm2ysD7lKW6V2ZThvEYNIWJhgGo9KgVlCKUBkbpjdUWBWv0sZVKYWqjJjWYxuxznfaXBd8hYA3GuxXnOAWAqeTeG9RYQjanFOnTFqcfnl9HY5LI/gd+2eICLsaMIt01rwPC4FF9oIe/9Mn5Pgifo+SG2zXL+yKavCRstr/LnDzkil3u6/XWhv8mxANhQREyHoBJxQVXlcFEbyUHG2m+/+2bNXfJzmz1Lbd6gkkKg3sV/8lNLoumTI3dAnwy0KEx6f9Si/2vJ77kM8mU/Zen83OIPg9Ea7WfS5srg2eJPH+PrcclAbSmEzZOt/V2jpV66BCY0IURQTLvRb6gIGWulgtIH5+LeksnN8/W2r1+QAfBrsnJUdBslwMAv+bSJmPhK9arf57kD8oFzO/vQhu8tn5/kQF6D8QPKov+NmrpWYj1hLXt5P8XF8aD0pd4o3EQjPO3yLyyhxMC4IgktSEPO47tJj3TmzEK83z9Eep6H0sfcn8/GOost6007pNLwBYOyiC6UMnpJmQWITXvN3uxLF6K0vMCS69C7kjmbJXhHE1tcEPSYQaX1LJrv7Z4uNLDonFZlO5awHmLuIpCWcgK1nnrpzcde1ARJaUfekaEuf56H2v5bEguEOyVwUUKxkS5+83aXpzFWLBAwBO7y1tvhh6hN8/kRhlPugn+1fn40hjw/tforaS/zIxynzA9+GPeRS9FGTvmijYDGN8wvf+xR/c7YhgBsh5gPjAxANZtmScmTKlCdv8fL22Uv9YhGdHTGj5rgpzSs6M11IXuxKQXm95I931iUWuoeiyvXo2jtXx4KpQI4ljuwyoEfPvHY6C6xONJtxEt9YF5/josGjfjtBo+56t7Wg7pAJVlODJXvMHRVoSo+0Ur7FFBZgbmk/EkQxGZhj4GLx/lOytkSW3L0fi/l7WuDFTJtyKtX6yb60NFoC4uhzMfdr6DHrv7fM/vWXd+8cLj827GMArahozd3qMfgmg64LaCc7q6UJOF5HJAGsASfbbEt27NHYD7rpcQtBrtTgqTf3kvoGUEQq0Ryh/yS1o/yln7O2XAHtC949mTT2mKegZACdQZALIY0RkPMlKZu9EUF3O6+yVCdljmDdHOUL8Lb/bFeRlJ1xJJSuc2D8mU3hj3yDOV7+IiaZDPw/2IYC9fAm/0GnNGfl3D0YqwN74ZfdxCNorEGR2IBhWBV25C/bt3TDThiGNJeF80qdr98JTFPnoIKXqrjRiLsxZh3JjHTQBlv/wCyOsrcPpjvpBSKHLZyPqlXjeODNzX+H1ew6MiFZUMFxZj3EVLngMxNSoQEMcb65S8r2quL1q38CDvxgNzA1kwzyM6lDBFwhcJcBhZQtSZC1E/YeozD29Jfa841/hfQXVWo/xIsFCIc6mhCFvfRmjz73bLg5Lkoeb67mPT7nQQ+kLeNkPdbAAfHLkJdsrauIwJ4tV5wrc6WCXxTr/z3Fkgyr9BWr+uqqnBPhZZWBfGLcg/IMGJS1o/w/AK9ivBEvoXAAAAABJRU5ErkJggg==";
            const schemaIcon: string = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAANpJREFUOE+tk0ESgjAMRX9mdC9cRG4ALPA8ehI8jyzAG8AJHC8A7mUmTklbgRFt1a4IbV9/8hOCWWW3wR1bG6uPNRqkwW3ybxaQjYsuAbic7lOKLKj8AFko0KJl4BsAYF5M/ACnLgLxEUCkJddg2mMX1G4pmFNFKwqyMHl30ey5FVGg8QSoa/UCQKkcHBxR8pWNEQjXIaUhRc6xCGAS6cQVCA1YA4CLBahaLSsYC9U2OqXwcxHFxnxm48Hdxmcrn7WY2K+RDOAPrTyqos8sqHHubRsLZIX60zg/AOCebhHZxIffAAAAAElFTkSuQmCC";

            storedProcSelection.treeViewOptions.rootNode = {
                id: "database",
                specializationId: "Database",
                label: "Database",
                icon: databaseIcon,
                children: Object.keys(spListResult.storedProcs).map(schemaName => {
                    return {
                        id: `schema.${schemaName}`,
                        label: schemaName,
                        specializationId: "Schema",
                        icon: schemaIcon,
                        isSelected: inputProcs.some(x => x.startsWith(`sp.${schemaName}`)),
                        children: spListResult.storedProcs[schemaName].map(sp => {
                            return {
                                id: `sp.${schemaName}.${sp}`,
                                label: sp,
                                specializationId: "Stored-Procedure",
                                icon: storedProcIcon,
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