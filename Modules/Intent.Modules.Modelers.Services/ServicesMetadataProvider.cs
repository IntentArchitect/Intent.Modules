﻿using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modelers.Services
{
    public class ServicesMetadataProvider
    {
        private readonly IMetadataManager _metaDataManager;

        public ServicesMetadataProvider(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public IEnumerable<IDTOModel> GetAllDTOs()
        {
            var classes = _metaDataManager.GetMetaData<IClass>("Services").Where(x => x.SpecializationType == "DTO").ToList();
            return classes.Select(x => new DTOModel(x)).ToList();
        }

        public IEnumerable<IDTOModel> GetDTOs(Engine.IApplication application)
        {
            return GetAllDTOs().Where(x => x.Application.Name == application.ApplicationName);
        }

        public IEnumerable<IServiceModel> GetServices(Engine.IApplication application)
        {
            var classes = _metaDataManager.GetMetaData<IClass>("Services").Where(x => x.Application.Name == application.ApplicationName
                && x.SpecializationType == "DTO").ToList();
            return classes.Select(x => new ServiceModel(x)).ToList();
        }
    }

    public static class ServicesMetadataManagerExtensions
    {
        public static IEnumerable<IDTOModel> GetDTOs(this IMetadataManager metadataManager, IApplication application)
        {
            return new ServicesMetadataProvider(metadataManager).GetDTOs(application);
        }

        public static IEnumerable<IServiceModel> GetServices(this IMetadataManager metadataManager, IApplication application)
        {
            return new ServicesMetadataProvider(metadataManager).GetServices(application);
        }
    }
}