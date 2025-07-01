/// <reference path="common.ts" />
/// <reference path="../../common/domainHelper.ts" />

class CrudCreationContext implements ICrudCreationContext {
  constructor(
    public element: IElementApi,
    public dialogOptions: ICrudCreationResult,
    public primaryKeys: IAttributeWithMapPath[],
    ) {}

  hasPrimaryKey(): boolean {
    return this.primaryKeys.length > 0;
  }
}


async function presentCrudOptionsDialog(preselectedClass?: IElementApi) : Promise<ICrudCreationResult>{
    let dialogResult: ICrudCreationResult = null;
    if (!preselectedClass) {
        dialogResult = await openCrudCreationDialog();

        if (!dialogResult) {
            return null; 
        }
    } else {
        dialogResult = {
            selectedEntity: preselectedClass,
            canCreate: true,
            canUpdate: true,
            canQueryById: true,
            canQueryAll: true,
            canDelete: true,
            canDomain: true,
            selectedDomainOperationIds: []
        };
    }

    return dialogResult;
}

function ClassSelectionFilter(element: MacroApi.Context.IElementApi) : boolean {

    let entity = element;
    if (!entity || entity.specialization != "Class") {
        return false;
    }

    if (DomainHelper.isAggregateRoot(element)){
        return true;
    }

    let results = entity.getAssociations("Association").filter(x => DomainHelper.isOwnedByAssociation(x));       
    for (let i = 0; i < results.length; i++) {

        if (results[i].getOtherEnd().typeReference.isCollection){
            return true;
        }
    }        
    return false;
}

async function openCrudCreationDialog(): Promise<ICrudCreationResult|null> {
    let classes = lookupTypesOf("Class").filter(x => ClassSelectionFilter(x));
    if (classes.length == 0) {
        await dialogService.info("No Domain types could be found. Please ensure that you have a reference to the Domain package and that at least one class exists in it.");
        return null;
    }
    
    let dialogResult = await dialogService.openForm({
        title: "CRUD Creation Options",
        fields: [
            {
                id: "entityId",
                fieldType: "select",
                label: "Entity for CRUD operations",
                selectOptions: classes.map(x => {
                    return {
                        id: x.id,
                        description: x.getName(),
                        additionalInfo: getClassAdditionalInfo(x)
                    };
                }),
                isRequired: true
            },
            {
                id: "create",
                fieldType: "checkbox",
                label: "Create",
                value: "true",
                hint: "Generate the \"Create\" operation"
            },
            {
                id: "update",
                fieldType: "checkbox",
                label: "Update",
                value: "true",
                hint: "Generate the \"Update\" operation"
            },
            {
                id: "queryById",
                fieldType: "checkbox",
                label: "Query By Id",
                value: "true",
                hint: "Generate the \"Query By Id\" operation"
            },
            {
                id: "queryAll",
                fieldType: "checkbox",
                label: "Query All",
                value: "true",
                hint: "Generate the \"Query All\" operation"
            },
            {
                id: "delete",
                fieldType: "checkbox",
                label: "Delete",
                value: "true",
                hint: "Generate the \"Delete\" operation"
            },
            {
                id: "domain",
                fieldType: "checkbox",
                label: "Domain Operations",
                value: "true",
                hint: "Generate operations for Domain Entity operations"
            }
        ]
    });

    let foundEntity = lookup(dialogResult.entityId);

    var result: ICrudCreationResult = {
        selectedEntity: foundEntity,
        canCreate: dialogResult.create == "true",
        canUpdate: dialogResult.update == "true",
        canQueryById: dialogResult.queryById == "true",
        canQueryAll: dialogResult.queryAll == "true",
        canDelete: dialogResult.delete == "true",
        canDomain: dialogResult.domain == "true",
        selectedDomainOperationIds: []
    };

    if (result.canDomain && foundEntity.getChildren("Operation").length > 0) {
        dialogResult = await dialogService.openForm({
            title: "Select Domain Operations",
            fields: [
                {
                    id: "tree",
                    fieldType: "tree-view",
                    label: "Domain Operations",
                    hint: "Generate operations from selected domain entity operations",
                    treeViewOptions: {
                        rootId: foundEntity.id,
                        submitFormTriggers: ["double-click", "enter"],
                        isMultiSelect: true,
                        selectableTypes: [
                            {
                                specializationId: "Class",
                                autoExpand: true,
                                autoSelectChildren: false,
                                isSelectable: (x) => false
                            },
                            {
                                specializationId: "Operation",
                                isSelectable: (x) => true
                            }
                        ]
                    }
                }
            ]
        });

        result.selectedDomainOperationIds = dialogResult.tree?.filter((x:any) => x != "0") ?? [];
    }

    return result;

    function getClassAdditionalInfo(element: MacroApi.Context.IElementApi): any {
        let aggregateEntity = DomainHelper.getOwningAggregate(element);
        let prefix = aggregateEntity ? `: ${aggregateEntity.getName()}  ` : "";
        return `${prefix}(${element.getParents().map(item => item.getName()).join("/")})`;
    }
}

interface ICrudCreationResult {
    selectedEntity: MacroApi.Context.IElementApi,
    selectedDomainOperationIds: string[],
    canCreate: boolean,
    canUpdate: boolean,
    canQueryById: boolean,
    canQueryAll: boolean,
    canDelete: boolean,
    canDomain: boolean
}

interface IAttributeWithMapPath {
    id: string,
    name: string,
    typeId: string,
    mapPath: string[],
    isNullable: boolean,
    isCollection: boolean,
}

