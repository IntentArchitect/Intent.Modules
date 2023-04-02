// services-cqrs-crud script (see ~/DesignerMacros/scr/services-cqrs-crud folder in Intent.Modules)
async function execute() { 
    let entity = await DomainHelper.openSelectEntityDialog();
    if (!entity) { return; }
};
