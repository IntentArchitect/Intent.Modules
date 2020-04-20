using System;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface ICreatableType
    {
        string Id { get; }
        string Name { get; }
        string ApiClassName { get; }
    }

    public static class CreatableTypeFactory
    {
        public static ICreatableType Create(IElement element)
        {
            switch (element.SpecializationType)
            {
                case ElementSettingsModel.SpecializationType:
                    return new ElementSettingsModel(element);
                case AssociationSettingsModel.SpecializationType:
                    return new AssociationSettingsModel(element);
                case CoreTypeModel.SpecializationType:
                    return new CoreTypeModel(element);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}