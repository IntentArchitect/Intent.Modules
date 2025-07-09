/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

interface IExecutionResult {
    result?: any;
    warnings: string[];
    errors: string[];
}

async function executeImporterModuleTask(taskTypeId: string, input: any): Promise<IExecutionResult> {
    let inputJsonString = JSON.stringify(input);
    console.log(`Executing Module Task ${taskTypeId} => ${inputJsonString}`);
    
    let moduleTaskResultString = await executeModuleTask(taskTypeId, inputJsonString);
    
    console.log(`Module Task ${taskTypeId} Completed => ${moduleTaskResultString}`);
    let executionResult = JSON.parse(moduleTaskResultString) as IExecutionResult;
    return executionResult;
}

async function displayExecutionResultErrors(executionResult: IExecutionResult): Promise<void> {
    if (executionResult.errors.length === 0) {
        return;
    }
    await dialogService.error(executionResult.errors.join("\r\n"));
}

async function displayExecutionResultWarnings(executionResult: IExecutionResult, title: string): Promise<void> {
    if (executionResult.warnings.length === 0) {
        return;
    }
    await dialogService.warn(title + "\r\n\r\n" + executionResult.warnings.join("\r\n"));
}