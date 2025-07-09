/// <reference path="strategy-cqrs.ts" />
/// <reference path="strategy-traditional.ts" />

let CrudApi = {
  createCQRSService,
  createTraditionalService
};

async function createCQRSService(element: IElementApi, preselectedClass?: IElementApi) {
    let strategy = new CQRSCrudStrategy();
    await strategy.execute(element, preselectedClass);
} 

async function createTraditionalService(element: IElementApi, preselectedClass?: IElementApi) {
    let strategy = new TraditionalServicesStrategy();
    await strategy.execute(element, preselectedClass);
}
