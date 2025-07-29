/// <reference path="../../typings/elementmacro.context.api.d.ts" />

class PaginateHelper {

    public static showCursorPaginateMenuItem(element: MacroApi.Context.IElementApi) : boolean {

        const packages = lookupTypesOf("Domain Package");

        if(packages.length == 0) {
            return false;
        }

        const providers = lookupTypesOf("Document Db Provider");

        // if no providers could be found, it means the reference is not added or an older version of the module is installed
        // default to not showing the menu
        if(providers.length == 0) {
            return false;
        }

        // if there is only 1 package, we will try find the selected provider
        if(packages.length == 1) {
            return PaginateHelper.packageQualifiesForPagination(packages[0], providers, "Cursor Enabled")
        }

        // if more than one package, then need to look at the entity linked to the query
        let queryAssociation = element.getAssociations().find(x => x.isTargetEnd() && x?.typeReference.getType()?.specialization == "Class");

        if(queryAssociation == null){

            // try lookup the package of the element the return DTO is mapped to
            var returnDtoPackage = element.typeReference?.getType()?.getMapping()?.getElement()?.getPackage();

            if(returnDtoPackage != null){
                return PaginateHelper.packageQualifiesForPagination(returnDtoPackage, providers, "Cursor Enabled")
            }

            // finally check all domain packages to see if any one has cursor pagination enabled
            let anyEnabled = packages.some(domainPackage =>
                PaginateHelper.packageQualifiesForPagination(domainPackage, providers, "Cursor Enabled")
            );

            return anyEnabled;
        }else{
            if(PaginateHelper.packageQualifiesForPagination(queryAssociation?.typeReference.getType().getPackage(), providers, "Cursor Enabled") == false) {
                return false;
            } else {
                return true;
            }
        }
    }

    public static showOffsetPaginateMenuItem(element: MacroApi.Context.IElementApi) : boolean {

        const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
        const packages = lookupTypesOf("Domain Package");

        if(packages.length == 0) {
            return false;
        }

        const providers = lookupTypesOf("Document Db Provider");

        // if no providers could be found, means only EF installed, and normal offset paging  should be shown
        if(providers.length == 0) {
            return true;
        }

        // if there is only 1 package, and its a document store, then try find the setting
        if(packages.length == 1 && packages[0].hasStereotype(documentStoreId)) {
            return PaginateHelper.packageQualifiesForPagination(packages[0], providers, "Offset Enabled")
        }

        // if more than one package, then need to look at the entity linked to the query
        let queryAssociation = element.getAssociations().find(x => x.isTargetEnd() && x?.typeReference.getType()?.specialization == "Class");

        if(queryAssociation == null){
            // try lookup the package of the element the return DTO is mapped to
            var returnDtoPackage = element.typeReference?.getType()?.getMapping()?.getElement()?.getPackage();

            if(returnDtoPackage != null){
                return PaginateHelper.packageQualifiesForPagination(returnDtoPackage, providers, "Offset Enabled")
            }

            return true;
        }else{
            const linkedPackage = queryAssociation?.typeReference.getType().getPackage();

            // if not a document db, then show the menu item
            if(!linkedPackage.hasStereotype(documentStoreId)){
                return true;
            }

            // else, actually check the values to see if it should be shown
            if(PaginateHelper.packageQualifiesForPagination(linkedPackage, providers, "Offset Enabled") == false) {
                return false;
            } else {
                return true;
            }
        }
    }

    // looks at the domain package and tries to determine based on a number of factors, if the menu option should be shown
    private static packageQualifiesForPagination(domainPackage: MacroApi.Context.IPackageApi | MacroApi.Context.IElementApi, 
        providers: MacroApi.Context.IElementApi[], paginationPropertyName: string): boolean {
        const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
        const paginationSupportId = "03385443-bbd6-46d9-87f2-6813f7833a38";

        // make sure its a documentDB store for now. This is the only one supported
        if(!domainPackage.hasStereotype(documentStoreId)) {
            return false;
        }
        // get the provider selected on the package
        let docDatabase = domainPackage.getStereotype(documentStoreId);
        let selectedProvider = docDatabase.getProperty("Provider");

        // if no provider selected, then take the first provider of type "Document Db Provider".
        let provider = null;
        if(selectedProvider?.getValue() == '' || selectedProvider?.getValue() == null) {
            provider = providers[0];
        } else if(providers.filter(p => p.id == selectedProvider?.getValue()).length > 0) {
            // if we have a "Document Db Provider" for the selected value
            provider = providers.filter(p => p.id == selectedProvider?.getValue())[0];
        }

        if(provider == null) {
            return false;
        }

        if(!provider.hasStereotype(paginationSupportId)){
            return false;
        }

        // if cursor is not enabled OR it is enabled AND offset is enabled
        if(provider.getStereotype(paginationSupportId).getProperty(paginationPropertyName)?.getValue() == null || 
            provider.getStereotype(paginationSupportId).getProperty(paginationPropertyName)?.getValue() != "true") {
            return false;
        }

        return true;
    }

    public static getCursorFiltering(element: MacroApi.Context.IElementApi) : CursorFiltering {
        const packages = lookupTypesOf("Domain Package");

        if(packages.length == 0) {
            return new CursorFiltering(false, false);
        }

        const providers = lookupTypesOf("Document Db Provider");
        if(packages.length == 1) {
            return PaginateHelper.getPackageCursorFilter(packages[0], providers)
        }

        // if more than one package, then need to look at the entity linked to the query
        let queryAssociation = element.getAssociations().find(x => x.isTargetEnd() && x?.typeReference.getType()?.specialization == "Class");

        if(queryAssociation == null){
            return new CursorFiltering(false, false);
        }else{
            return PaginateHelper.getPackageCursorFilter(queryAssociation?.typeReference.getType().getPackage(), providers)
        }
    }

    private static getPackageCursorFilter(domainPackage: MacroApi.Context.IPackageApi | MacroApi.Context.IElementApi, 
        providers: MacroApi.Context.IElementApi[]) :  CursorFiltering {

        const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
        const paginationSupportId = "03385443-bbd6-46d9-87f2-6813f7833a38";

        // make sure its a documentDB store for now. This is the only one supported
        if(!domainPackage.hasStereotype(documentStoreId)) {
            return new CursorFiltering(false, false);
        }
        // get the provider selected on the package
        let docDatabase = domainPackage.getStereotype(documentStoreId);
        let selectedProvider = docDatabase.getProperty("Provider");

        // if no provider selected, then take the first provider of type "Document Db Provider".
        let provider = null;
        if(selectedProvider?.getValue() == '' || selectedProvider?.getValue() == null) {
            provider = providers[0];
        } else if(providers.filter(p => p.id == selectedProvider?.getValue()).length > 0) {
            // if we have a "Document Db Provider" for the selected value
            provider = providers.filter(p => p.id == selectedProvider?.getValue())[0];
        }

        if(provider == null) {
            return new CursorFiltering(false, false);
        }

        if(!provider.hasStereotype(paginationSupportId)){
            return new CursorFiltering(false, false);
        }

        let cursorFiltering = provider.getStereotype(paginationSupportId).getProperty("Cursor Filtering")?.getValue();

        if(cursorFiltering == "Disabled") {
            return new CursorFiltering(false, false);
        }

        if(cursorFiltering == "Enabled (Nullable)") {
            return new CursorFiltering(true, true);
        }

        if(cursorFiltering == "Enabled (Not Nullable)") {
            return new CursorFiltering(true, false);
        }

        return new CursorFiltering(false, false);
    }
}

class CursorFiltering {
    constructor(public enabled: boolean, public nullable: boolean) {
    }
}