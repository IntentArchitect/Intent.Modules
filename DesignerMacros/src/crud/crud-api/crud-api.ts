/// <reference path="strategy-cqrs.ts" />
/// <reference path="strategy-traditional.ts" />

let CrudApi = {
  createCQRSService,
  createTraditionalService,
  createCQRSServiceForOperation,
  createTraditionalServiceForOperation
};

async function createCQRSService(element: IElementApi, preselectedClass?: IElementApi, diagramElement?: IElementApi | undefined): Promise<void> {
    let strategy = new CQRSCrudStrategy();
    await strategy.execute(element, preselectedClass, diagramElement);
} 

async function createTraditionalService(element: IElementApi, preselectedClass?: IElementApi, diagramElement?: IElementApi | undefined): Promise<void> {
    let strategy = new TraditionalServicesStrategy();
    await strategy.execute(element, preselectedClass, diagramElement);
}

async function createCQRSServiceForOperation(domainOperationElement: IElementApi, diagramElement?: IDiagramApi | undefined): Promise<void> {
    let strategy = new CQRSCrudStrategy();
    await strategy.executeForOperation(domainOperationElement, diagramElement);
}

async function createTraditionalServiceForOperation(domainOperationElement: IElementApi, diagramElement?: IDiagramApi | undefined): Promise<void> {
    let strategy = new TraditionalServicesStrategy();
    await strategy.executeForOperation(domainOperationElement, diagramElement);
}