/// <reference path="strategy-cqrs.ts" />
/// <reference path="strategy-traditional.ts" />

let CrudApi = {
  createCQRSService,
  createTraditionalService
};

async function createCQRSService(element: IElementApi, preselectedClass?: IElementApi, diagramElement?: IElementApi | undefined) {
    let strategy = new CQRSCrudStrategy();
    await strategy.execute(element, preselectedClass, diagramElement);
} 

async function createTraditionalService(element: IElementApi, preselectedClass?: IElementApi, diagramElement?: IElementApi | undefined) {
    let strategy = new TraditionalServicesStrategy();
    await strategy.execute(element, preselectedClass, diagramElement);
}






