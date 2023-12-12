/// <reference path="isAggregateRoot.ts" />
/// <reference path="getDefaultIdType.ts" />

function updatePartitionKeyAsPrimary(element: IElementApi, packageLoad : boolean = false): void {
    if (!isCosmosDb(element)) return;

    let allowedTypes: Array<string> = ["Class", "Folder", "Domain Package"];
    if (!allowedTypes.includes(element.specialization)) {
        return;
    }
    const primaryKeyStereotypeId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74";
    const containerStereotypeId = "ef9b1772-18e1-44ad-b606-66406221c805";
    const multiTenancyStereotypeId = "586eb05b-d647-4430-ac05-8d096fe3f79e";    

    let partitionKey : string = getPartitionKey(element);

    if (!packageLoad && ["Folder", "Domain Package"].includes(element.specialization)){
        //prevent unnecessary package scans on every package change event.
        if (element.hasMetadata("currentPartitionKey") && element.getMetadata("currentPartitionKey") === partitionKey){
            return;
        }
    }
    
    if (element.specialization == "Class"){
        ensureCorrectPartitionKey(element, partitionKey, packageLoad);
    } else {
        updateDescendants(element, partitionKey, packageLoad);
    }
    element.setMetadata("currentPartitionKey", partitionKey);

    function getPartitionKey(element: IElementApi) : string{
        let result : string = "";

        while(element)
        {
            if (element.hasStereotype(containerStereotypeId) && element.getStereotype(containerStereotypeId).getProperty("Partition Key")?.value){
                let partitionKey = element.getStereotype(containerStereotypeId).getProperty("Partition Key")?.value;
                if (!partitionKey) partitionKey = "";
                return partitionKey;
            }    
            element = element.getParent();
        }
        return result;
    }

    function updateDescendants(element: IElementApi, partitionKeyName: string, packageLoad: boolean) : void {
        let children = element.getChildren().filter(c => allowedTypes.includes( c.specialization));
        
        children.forEach(child => {
            if (child.hasStereotype(containerStereotypeId)){
                if (packageLoad){
                    //On package load we check the entire tree not just the scope of the edited item
                    partitionKeyName = child.getStereotype(containerStereotypeId).getProperty("Partition Key")?.value
                } else {
                    return;
                }
            }

            if (child.specialization == "Class"){
                ensureCorrectPartitionKey(child, partitionKeyName, packageLoad);
            } else {
                updateDescendants(child, partitionKeyName, packageLoad);
            }                
        });
    }    

    function ensureCorrectPartitionKey(element: IElementApi, partitionKeyName: string, packageLoad: boolean) : void {
        if (element.specialization !== "Class") {
            return;
        } 
        if (!partitionKeyName)
        {
            let toRemove = element.getChildren("Attribute")
            .find(x => x.hasStereotype(primaryKeyStereotypeId) && x.hasMetadata("partition-key"));    
            if (toRemove != null) {
                toRemove.delete();
            }    
        }
        else
        {
            let toFix = element.getChildren("Attribute")
            .find(x => x.hasStereotype(primaryKeyStereotypeId) && x.hasMetadata("partition-key") && x.getName().toLowerCase() != partitionKeyName.toLowerCase());    
            if (toFix != null) {
                toFix.removeMetadata("partition-key");
                toFix.removeStereotype(primaryKeyStereotypeId);
            }    

            let toAdjust = element.getChildren("Attribute")
            .find(x => x.getName().toLowerCase() == partitionKeyName.toLowerCase());    
            if (toAdjust != null) {
                if (!toAdjust.hasStereotype(primaryKeyStereotypeId)) {
                    toAdjust.addStereotype(primaryKeyStereotypeId);
                }        
                let partitionPkStereotype = toAdjust.getStereotype(primaryKeyStereotypeId);      
                if (!element.hasStereotype(multiTenancyStereotypeId)){
                    partitionPkStereotype.getProperty("Data source")?.setValue("User supplied");
                }
                toAdjust.setMetadata("partition-key", "true");
            }    
        }
    }

    function isCosmosDb(element: IElementApi): boolean {
        const documentStoreId : string = "8b68020c-6652-484b-85e8-6c33e1d8031f";
        const cosmosDbProvider : string = "3e1a00f7-c6f1-4785-a544-bbcb17602b31";
    
        let docDbStereotype = element.getPackage().getStereotype(documentStoreId);
        let providers = lookupTypesOf("Document Db Provider").filter((elem, index) => lookupTypesOf("Document Db Provider").findIndex(obj => obj.id == elem.id) === index && elem.getName() != "Custom");
    
        return ((!docDbStereotype.getProperty("Provider")?.getValue() && providers.length == 1 && providers[0].id == cosmosDbProvider)|| 
            (docDbStereotype.getProperty("Provider")?.getValue() as MacroApi.Context.IElementApi)?.id == cosmosDbProvider);
    }
    
}

