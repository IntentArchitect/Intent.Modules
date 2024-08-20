/// <reference path="../common/nuget-package-updater.ts" />



async function updateAllNuGetPackages(element: MacroApi.Context.IElementApi): Promise<void> {
    let packages = element.getChildren();
    let result = await getLatestNugetPackages(packages.filter(x => !isPackageLocked(x)).map(x => x.getName()));
    updateNugetPackageElements(packages, result);
    await dialogService.info("Update complete.");
}

/**
 * Used by Intent.Modules\Modules\Intent.ModuleBuilder.CSharp
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/nuget-package-updater/update-all-package/update-all-package.ts
 */

//Uncomment below
//await updateAllNuGetPackages(element);