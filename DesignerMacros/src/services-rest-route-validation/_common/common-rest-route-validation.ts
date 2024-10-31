/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function validateRestRoutes(element: MacroApi.Context.IElementApi): String {

    let routeToValidate = RestRoute.create(element);
    if (!routeToValidate) {
        return "";
    }

    let routesToCompareAgainst = lookupTypesOf("Command", false)
        .concat(lookupTypesOf("Query", false))
        .concat(lookupTypesOf("Operation", false))
        .map(x => RestRoute.create(x))
        .filter(x => x != null) as RestRoute[];

    let message: string = __findOneDuplicate(routeToValidate, routesToCompareAgainst);
    if (message && message != "") {
        return message;
    }

    message = __findMissingParameters(routeToValidate);
    if (message && message != "") {
        return message;
    }

    return "";
}

function __findOneDuplicate(routeToValidate: RestRoute, routesToCompareAgainst: RestRoute[]): string {
    for (let possibleDuplicate of routesToCompareAgainst) {
        // Make sure we're not checking the same route with itself.
        if (routeToValidate.underlyingElement.id == possibleDuplicate.underlyingElement.id) {
            continue;
        }
        if (possibleDuplicate.isDuplicate(routeToValidate)) {
            return `Duplicate rest route ${routeToValidate.underlyingElement.getName()}(${routeToValidate.underlyingElement.specialization}) with ${possibleDuplicate.underlyingElement.getName()}(${possibleDuplicate.underlyingElement.specialization}) - ${possibleDuplicate.originalRoute}`;
        }
    }

    return "";
}

function __findMissingParameters(routeToValidate: RestRoute): string {
    let element = routeToValidate.underlyingElement;
    let elementType = element.specialization;
    let elementChildren;

    if (elementType === "Command" || elementType === "Query") {
        elementChildren = element.getChildren("DTO-Field");
    } else if (elementType === "Operation") {
        elementChildren = element.getChildren("Parameter");
    } else {
        return "";
    }

    let missingParameters: string[] = [];
    for (let routeParam of routeToValidate.routeParams) {
        let routeParamName = routeParam.toLowerCase();
        if (!elementChildren.some(e => e.getName().toLowerCase() === routeParamName)) {
            missingParameters.push(routeParam);
        }
    }

    if (missingParameters.length === 0) {
        return "";
    }

    return `Route mismatch: some route parameters do not match element's properties/parameters. Unmatched parameters: ${missingParameters.join(", ")}`;
}

// this method is used to define any route parameters which should be excluded from the validation checks
function __getRouteParameterExclusions() {
    let routeExclusions = [];

    // get the multi-tenancy route parameter configured
    let multiTenancyRouteStrategy = application.getSettings("41ae5a02-3eb2-42a6-ade2-322b3c1f1115")?.getField("e15fe0fb-be28-4cc5-8b85-37a07b7ca160")?.value;
    let multiTenancyRoute = application.getSettings("41ae5a02-3eb2-42a6-ade2-322b3c1f1115")?.getField("c8ff4af6-68b6-4e31-a291-43ada6a0008a")?.value;

    if(multiTenancyRouteStrategy && multiTenancyRouteStrategy != "" && multiTenancyRouteStrategy === "route" 
        && multiTenancyRoute && multiTenancyRoute != "") {
        routeExclusions.push(multiTenancyRoute);
    }

    // version is always excluded
    routeExclusions.push("version");

    return routeExclusions;
}

class RestVersionSet {
    private versionHashTable: { [key: string]: string } = {};

    constructor(public isVersioned: boolean, versionIds: string[]) {
        for (let v of versionIds) {
            this.versionHashTable[v] = v;
        }
    }

    public matches(versionSet: RestVersionSet): boolean {
        // Only engage with versioning checks if both REST routes are versioned
        return !this.isVersioned || !versionSet.isVersioned || this.intersects(versionSet);
    }

    private intersects(versionSet: RestVersionSet): boolean {
        for (let v in this.versionHashTable) {
            if (versionSet.versionHashTable[v]) {
                return true;
            }
        }
        return false;
    }

    public static create(element: MacroApi.Context.IElementApi): RestVersionSet {
        const apiVersionSettingId = "20855f03-c663-4ec6-b106-de06be98f1fe";
        let versionSetting = element.getStereotype(apiVersionSettingId);
        if (!versionSetting) { return new RestVersionSet(false, []); }
        
        let versionIds = JSON.parse(versionSetting.getProperty("Applicable Versions").value) as string[];
        return new RestVersionSet(true, versionIds ?? []);
    }
}

class RestRoute {
    public hashedRoute: string;
    public routeParams: string[];
    private versionSet: RestVersionSet;
    
    constructor(public verb: string, public originalRoute: string, public underlyingElement: MacroApi.Context.IElementApi) {
        this.routeParams = [];
        this.versionSet = RestVersionSet.create(underlyingElement);
        
        let counter = 0;
        let localRouteParams = this.routeParams;
        let actionName = underlyingElement.getName();
        let routeParameterExclusions = __getRouteParameterExclusions();
        this.hashedRoute = originalRoute
            .replace(/\{([^}]*)\}/g, function (match, g1) { 
                if (routeParameterExclusions.indexOf(g1) < 0) {
                    localRouteParams.push(g1);
                }
                return (counter++).toString(); 
            })
            .replace(/(\[action\])/g, actionName);
    }

    public isDuplicate(possibleDuplicateRoute: RestRoute): boolean {
        return this.hashedRoute === possibleDuplicateRoute.hashedRoute && 
            this.verb === possibleDuplicateRoute.verb &&
            this.versionSet.matches(possibleDuplicateRoute.versionSet);
    }

    public static create(element: MacroApi.Context.IElementApi): RestRoute | null {
        if (!element) { return null; }

        const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
        let httpSettings = element.getStereotype(httpSettingsId);
        if (!httpSettings) { return null; }

        let absoluteRouteToCheck = getAbsoluteRoute(element, httpSettings);
        if (!absoluteRouteToCheck) { return null; }
        let verbToCheck = httpSettings.getProperty("Verb").value;

        return new RestRoute(
            verbToCheck, 
            absoluteRouteToCheck, 
            element);

        function getAbsoluteRoute(element: MacroApi.Context.IElementApi, httpSettings: MacroApi.Context.IStereotypeApi): string | null {
            if (element.specialization == "Operation") {
                const serviceHttpServiceSettingsId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd";
                
                let serviceElement = element.getParent();
                if (!serviceElement) { return null; }
                
                let operationPath = httpSettings.getProperty("Route").value;
                let servicePath = serviceElement.getStereotype(serviceHttpServiceSettingsId)?.getProperty("Route").value;
                if (servicePath && servicePath != "") {
                    if (servicePath.toLocaleLowerCase().includes('[controller]')) {
                        servicePath = servicePath.replace(/\[controller\]/gi, `[${serviceElement.getName()}]`);
                    } 
                    return `${servicePath}/${operationPath}`;
                } 

                //We don't know how the service name will be transformed so we add [{ServiceName}] to represent the transform
                return `[${serviceElement.getName()}]/${operationPath}`;
            }

            return httpSettings.getProperty("Route").value;
        }
    }
}