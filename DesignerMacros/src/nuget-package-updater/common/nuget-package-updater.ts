/// <reference path="../../../typings/elementmacro.context.api.d.ts" />


interface ITaskConfig {
    nugetPackageIds: string[];
}

interface ITaskResponse {
    nugetPackages: INugetPackage[];
}

interface INugetPackage {
    name: string;
    versions: INugetVersion[];
}

interface INugetVersion {
    version: string;
    targetFramework: string;
    dependencies: INugetDependency[];
}

interface INugetDependency {
    packageName: string;
    version: string;
}

async function getLatestNugetPackages(nugetPackageIds: string[]): Promise<ITaskResponse> {

    let taskConfig: ITaskConfig = {
        nugetPackageIds: nugetPackageIds,
    };
    let jsonResponse = await executeModuleTask("Intent.ModuleBuilder.CSharp.Tasks.NugetUpdater", JSON.stringify(taskConfig));
    let result = JSON.parse(jsonResponse);

    if (result?.errorMessage) {
        await dialogService.error(result?.errorMessage);
        return {
            nugetPackages: []
        };
    }

    // Assuming the response matches the ITaskResponse structure
    let taskResponse: ITaskResponse = {
        nugetPackages: result.nugetPackages.map((pkg: any) => ({
            name: pkg.name,
            versions: pkg.versions.map((ver: any) => ({
                version: ver.version,
                targetFramework: ver.targetFramework,
                dependencies: ver.dependencies.map((dep: any) => ({
                    packageName: dep.packageName,
                    version: dep.version,    
                }))
            }))
        }))
    };

    return taskResponse; // Return the parsed ITaskResponse
}

function updateNugetPackageElements(elements: MacroApi.Context.IElementApi[], latestVersions : ITaskResponse) : void {

    elements.filter(x => !isPackageLocked(x)).forEach(e => {
        let packageName = e.getName() ;
        let nugetPackage = latestVersions.nugetPackages.find(pkg => pkg.name === packageName );
        console.log("synchronizing:" + packageName);
        synchronizeModel(e, nugetPackage);
    });
}

function synchronizeModel(packageElement : MacroApi.Context.IElementApi, nugetPackage : INugetPackage) : void{
    const packageVersionSettings = "7af88c37-ce54-49fc-b577-bde869c23462";

    const dict = new Map<string, INugetVersion>();
    nugetPackage.versions.forEach(v => {
        dict.set(v.targetFramework, v);
    });

    dict.forEach((value, key) => {
        console.log(`NuGet:${value.version}(${key})`);
    });

    packageElement.getChildren().forEach(e => {

        let versionSettings = e.getStereotype(packageVersionSettings);
        let targetFramework = versionSettings.getProperty("Minimum Target Framework").value;
        if (versionSettings.getProperty("Locked").getValue() == true){
            if (dict.has(targetFramework)){
                dict.delete(targetFramework);
            }
        } else {
            if (dict.has(targetFramework) ){                
                //The version no is the same, no work to do
                let packageInfo = dict.get(targetFramework);
                if (packageInfo.version == e.getName()){
                    //If the dependencies are out of sync, this fixes legacy setups and is Ok for normal ones
                    if (packageInfo.dependencies.length == e.getChildren().length){
                        dict.delete(targetFramework);
                    }
                }
            } 
        }
    });

    dict.forEach((value, key) => {
        let existingElement = packageElement.getChildren().find(c => c.getStereotype(packageVersionSettings).getProperty("Minimum Target Framework").value == key);
        if (existingElement){
            //Update Version No
            existingElement.setName(value.version, false);
            updateNugetDependencies(existingElement, value.dependencies);
        }else{
            //Create new framework element
            let element = createElement("Package Version", value.version, packageElement.id);
            let versionSettings = element.getStereotype(packageVersionSettings);
            versionSettings.getProperty("Minimum Target Framework").setValue(key);
            updateNugetDependencies(element, value.dependencies);
        }
    });
}

function updateNugetDependencies(element: MacroApi.Context.IElementApi, dependencies: INugetDependency[]) : void {
    element.getChildren().forEach(child => child.delete());
    dependencies.forEach(dep =>{
        let child = createElement("NuGet Dependency", dep.packageName, element.id);
        child.setValue(dep.version);
    });
}

function isPackageLocked(element: MacroApi.Context.IElementApi) : boolean {
    let packageSettings = "265221a5-779c-46c9-a367-8b07b435803b";
    return  element.getStereotype(packageSettings).getProperty("Locked")?.getValue() == true;
}


