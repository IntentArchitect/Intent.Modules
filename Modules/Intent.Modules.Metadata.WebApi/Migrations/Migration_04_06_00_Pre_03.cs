using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Plugins;

namespace Intent.Modules.Metadata.WebApi.Builder.Migrations;

public class Migration_04_06_00_Pre_03 : IModuleMigration
{
    private readonly IApplicationConfigurationProvider _configurationProvider;
    private const string ServicesDesignerId = "81104ae6-2bc5-4bae-b05a-f987b0372d81";
    private const string HttpServiceSettingsStereotypeId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd";
    private const string ServiceModelSpecializationTypeId = "b16578a5-27b1-4047-a8df-f0b783d706bd";
    private const string WebApiDefinitionPackageId = "0011387a-b122-45d7-9cdb-8e21b315ab9f";
    private const string RoutePropertyDefinitionId = "1e223bd0-7a72-435a-8741-a612d88e4a12";

    public Migration_04_06_00_Pre_03(IApplicationConfigurationProvider configurationProvider)
    {
        _configurationProvider = configurationProvider;
    }
    
    public string ModuleId => "Intent.Metadata.WebApi";
    public string ModuleVersion => "4.6.0-pre.3";
    
    // This migration deals with the legacy ASP.NET Core Controller logic where a missing
    // Http Service Settings stereotype would mean that the Route assigned to the Controller
    // will be `api/[controller]` which is a bad take on convention and tech stacks.
    // So this will create a Http Service Settings stereotype with that base route if missing
    // and only when Intent.AspNetCore.Controllers are installed.
    
    public void Up()
    {
        var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
        
        if (!app.Modules.Any(p => p.ModuleId == "Intent.AspNetCore.Controllers"))
        {
            return;
        }
        
        var designer = app.GetDesigner(ServicesDesignerId);
        var packages = designer.GetPackages();
        
        foreach (var package in packages)
        {
            var needsSave = false;

            foreach (var element in GetAllElements(package).Where(p => p.SpecializationTypeId == ServiceModelSpecializationTypeId))
            {
                var httpServiceSettings = element.Stereotypes.FirstOrDefault(p => p.DefinitionId == HttpServiceSettingsStereotypeId);
                if (httpServiceSettings is null)
                {
                    httpServiceSettings = new StereotypePersistable
                    {
                        DefinitionId = HttpServiceSettingsStereotypeId,
                        Name = "Http Service Settings",
                        DefinitionPackageId = WebApiDefinitionPackageId,
                        DefinitionPackageName = "Intent.Metadata.WebApi",
                        Properties =
                        [
                            new StereotypePropertyPersistable
                            {
                                DefinitionId = RoutePropertyDefinitionId,
                                Name = "Route",
                                Value = "api/[controller]",
                                IsActive = true
                            }
                        ]
                    };
                    
                    element.Stereotypes.Add(httpServiceSettings);
                    needsSave = true;
                }

                if (httpServiceSettings is not null)
                {
                    var routeProp = httpServiceSettings.Properties.FirstOrDefault(p => p.DefinitionId == RoutePropertyDefinitionId);
                    if (routeProp is null)
                    {
                        routeProp = new StereotypePropertyPersistable
                        {
                            DefinitionId = RoutePropertyDefinitionId,
                            Name = "Route",
                            Value = "api/[controller]",
                            IsActive = true
                        };
                        httpServiceSettings.Properties.Add(routeProp);
                        needsSave = true;
                    }
                    else if (string.IsNullOrWhiteSpace(routeProp.Value))
                    {
                        routeProp.Value = "api/[controller]";
                        needsSave = true;
                    }
                }
            }
            
            if (needsSave)
            {
                package.Save(true);
            }
        }
    }
    
    public void Down()
    {
        var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
    
        if (!app.Modules.Any(p => p.ModuleId == "Intent.AspNetCore.Controllers"))
        {
            return;
        }
    
        var designer = app.GetDesigner(ServicesDesignerId);
        var packages = designer.GetPackages();
    
        foreach (var package in packages)
        {
            var needsSave = false;

            foreach (var element in GetAllElements(package).Where(p => p.SpecializationTypeId == ServiceModelSpecializationTypeId))
            {
                var httpServiceSettings = element.Stereotypes.FirstOrDefault(p => p.DefinitionId == HttpServiceSettingsStereotypeId);
                if (httpServiceSettings != null)
                {
                    var routeProperty = httpServiceSettings.Properties.FirstOrDefault(p => p.DefinitionId == RoutePropertyDefinitionId);
                    if (routeProperty != null && routeProperty.Value == "api/[controller]")
                    {
                        element.Stereotypes.Remove(httpServiceSettings);
                        needsSave = true;
                    }
                }
            }
        
            if (needsSave)
            {
                package.Save(true);
            }
        }
    }
    
    private static IEnumerable<ElementPersistable> GetAllElements(PackageModelPersistable package)
    {
        return package.ChildElements.SelectMany(GetAllElements);

        static IEnumerable<ElementPersistable> GetAllElements(ElementPersistable element)
        {
            yield return element;

            foreach (var descendentElement in element.ChildElements.SelectMany(GetAllElements))
            {
                yield return descendentElement;
            }
        }
    }
}