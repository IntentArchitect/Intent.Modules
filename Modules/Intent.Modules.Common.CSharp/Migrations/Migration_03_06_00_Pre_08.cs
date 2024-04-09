using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Plugins;
using Intent.Utils;

namespace Intent.Modules.Common.CSharp.Migrations;

public class Migration_03_06_00_Pre_08 : IModuleMigration
{
    private readonly IApplicationConfigurationProvider _configurationProvider;
    private const string DateTypeId = "1fbaa056-b666-4f25-b8fd-76fe3165acc8";
    private const string DateTimeTypeId = "a4107c29-7851-4121-9416-cf1236908f1e";
    private const string MetadataKey = "common-charp-migration-date-to-datetime";
    
    public Migration_03_06_00_Pre_08(IApplicationConfigurationProvider configurationProvider)
    {
        _configurationProvider = configurationProvider;
    }

    public string ModuleId => "Intent.Common.CSharp";
    public string ModuleVersion => "3.6.0-pre.8";
    
    public void Up()
    {
        var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

        var packages = app.GetDesigners().SelectMany(x => x.GetPackages());

        foreach (var package in packages)
        {
            var elements = GetMatchedElements(package.Classes, p => p.TypeReference?.TypeId == DateTypeId);
            if (!elements.Any())
            {
                continue;
            }

            foreach (var element in elements)
            {
                element.TypeReference.TypeId = DateTimeTypeId;
                element.AddMetadata(MetadataKey, "true");
            }

            package.Save(true);
        }
    }
    
    public void Down()
    {
        var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
        
        var packages = app.GetDesigners().SelectMany(x => x.GetPackages());

        foreach (var package in packages)
        {
            var elements = GetMatchedElements(package.Classes, p => p.TypeReference?.TypeId == DateTimeTypeId && p.HasMetadata(MetadataKey));
            
            if (!elements.Any())
            {
                continue;
            }

            foreach (var element in elements)
            {
                element.TypeReference.TypeId = DateTypeId;
                element.Metadata.Remove(element.Metadata.Single(p => p.Key == MetadataKey));
            }

            package.Save(true);
        }
    }
    
    private static IEnumerable<ElementPersistable> GetMatchedElements(IList<ElementPersistable> elements, Predicate<ElementPersistable> predicate)
    {
        foreach (var element in elements)
        {
            if (predicate(element))
            {
                yield return element;
            }

            foreach (var result in GetMatchedElements(element.ChildElements, predicate))
            {
                yield return result;
            }
        }
    }
}