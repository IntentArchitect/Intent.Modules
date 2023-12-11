/// <reference path="isAggregateRoot.ts" />
/// <reference path="getDefaultIdType.ts" />

function updatePrimaryKey(element: IElementApi): void {
    if (element.specialization !== "Class") {
        return;
    }

    const primaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    let idAttr = element.getChildren("Attribute")
        .find(x => x.hasStereotype(primaryKeyStereotypeId));

    const isToCompositionalOneRelationshipTarget = () => element.getAssociations("Association")
        .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable && !x.getOtherEnd().typeReference.isCollection);
    if ((!isAggregateRoot(element) && isToCompositionalOneRelationshipTarget()) || derivedFromTypeHasPk(element)) {
        if (idAttr != null) {
            idAttr.delete();
        }

        updateDerivedTypePks(element);
        return;
    }

    if (isTableStorage(element)){
        updateTableStoragePk(element)
    } else {
        if (idAttr == null) {
            const classNameWithId = `${element.getName()}Id`.toLowerCase();

            idAttr = element.getChildren("Attribute")
                .find(attribute => {
                    const attributeName = attribute.getName().toLowerCase();
                    return attributeName === "id" || attributeName === classNameWithId;
                });
        }

        if (idAttr == null) {
            idAttr = createElement("Attribute", "Id", element.id);
            idAttr.setOrder(0);
            if (idAttr.typeReference == null) throw new Error("typeReference is not defined");
            idAttr.typeReference.setType(getDefaultIdType());
        }

        if (!idAttr.hasStereotype(primaryKeyStereotypeId)) {
            idAttr.addStereotype(primaryKeyStereotypeId);
        }
    }

    updateDerivedTypePks(element);

    function derivedFromTypeHasPk(element: IElementApi): boolean {
        const primaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    
        return element.getAssociations("Generalization")
            .some(generalization => {
                if (!generalization.isTargetEnd()) {
                    return false;
                }
    
                const baseType = generalization.typeReference.getType();
                if (baseType.getChildren("Attribute").some(attribute => attribute.hasStereotype(primaryKeyStereotypeId))) {
                    return true;
                }
    
                return derivedFromTypeHasPk(baseType);
            });
    }

    function updateDerivedTypePks(element: IElementApi) {
        const derivedTypes = element.getAssociations("Generalization")
            .filter(generalization => generalization.isSourceEnd())
            .map(generalization => generalization.typeReference.getType());
    
        for (const derivedType of derivedTypes) {
            updatePrimaryKey(derivedType);
        }
    }
}

function isTableStorage(element: IElementApi): boolean {
    const documentStoreId : string = "8b68020c-6652-484b-85e8-6c33e1d8031f";
    const tableStorageProvider : string = "1d05ee8e-747f-4120-9647-29ac784ef633";

    var docDbStereotype = element.getPackage().getStereotype(documentStoreId);
    let providers = lookupTypesOf("Document Db Provider").filter((elem, index) => lookupTypesOf("Document Db Provider").findIndex(obj => obj.id == elem.id) === index && elem.getName() != "Custom");

    return ((!docDbStereotype.getProperty("Provider")?.getValue() && providers.length == 1 && providers[0].id == tableStorageProvider)|| 
        (docDbStereotype.getProperty("Provider")?.getValue() as MacroApi.Context.IElementApi)?.id == tableStorageProvider);
}

function updateTableStoragePk(element: IElementApi): void {
    const primaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    const stringTypeId: string = "d384db9c-a279-45e1-801e-e4e8099625f2";

    let idAttrs = element.getChildren("Attribute").filter(x => x.hasStereotype(primaryKeyStereotypeId));

    if (!isAggregateRoot(element)){
        if (idAttrs.length > 0){
            idAttrs.forEach(key => key.delete());
        }
    }else{
        //Keys are not right
        if (!(idAttrs.length == 2 && idAttrs[0].getName() == "PartitionKey"&& idAttrs[1].getName() == "RowKey")){
            if (idAttrs.length > 0){
                idAttrs.forEach(key => key.delete());
            }
            let rowKeyAttr = createElement("Attribute", "RowKey", element.id);
            rowKeyAttr.setOrder(0);
            rowKeyAttr.typeReference.setType(stringTypeId);
            let idPkSterotype = rowKeyAttr.addStereotype(primaryKeyStereotypeId);
            idPkSterotype.getProperty("Data source")?.setValue("User supplied");
            
            let partitionKeyAttr = createElement("Attribute", "PartitionKey", element.id);
            partitionKeyAttr.setOrder(0);
            partitionKeyAttr.typeReference.setType(stringTypeId);
            let partitionPkStereotype = partitionKeyAttr.addStereotype(primaryKeyStereotypeId);                    
            partitionPkStereotype.getProperty("Data source")?.setValue("User supplied");
        }
    }        
}
