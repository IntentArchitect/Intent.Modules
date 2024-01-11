using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Common
{
    /// <summary>
    /// Extension methods for <see cref="IStereotype"/> and <see cref="IHasStereotypes"/>.
    /// </summary>
    public static class StereotypeExtensions
    {
        /// <summary>
        /// Obsolete. Use <see cref="GetStereotypeProperty{T}"/> instead.
        /// </summary>
        [Obsolete("Use GetStereotypeProperty")]
        public static T GetPropertyValue<T>(this IHasStereotypes model, string stereotypeName, string propertyName, T defaultIfNotFound = default)
        {
            return model.GetStereotypeProperty(stereotypeName, propertyName, defaultIfNotFound);
        }

        /// <summary>
        /// Retrieve the value of the property with the provided <paramref name="propertyName"/>
        /// on the provided <paramref name="stereotypeName"/> on the provided <paramref name="model"/>.
        /// </summary>
        public static T GetStereotypeProperty<T>(this IHasStereotypes model, string stereotypeName, string propertyName, T defaultIfNotFound = default)
        {
            try
            {
                return model.GetStereotype(stereotypeName).GetProperty(propertyName, defaultIfNotFound);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get stereotype property for element [{model}]", e);
            }
        }

        /// <summary>
        /// Lookup only one stereotype with a given name. If more than one is found with the same name an exception is thrown.
        /// </summary>
        public static IStereotype GetStereotype(this IHasStereotypes model, string stereotypeNameOrId)
        {
            var stereotypes = model.Stereotypes.Where(x => x.Name == stereotypeNameOrId || x.DefinitionId == stereotypeNameOrId).ToArray();
            if (stereotypes.Length > 1)
            {
                throw new Exception(model is IMetadataModel metadataModel
                    ? $"More than one stereotype found with the name '{stereotypeNameOrId}' on element with ID {metadataModel.Id}"
                    : $"More than one stereotype found with the name '{stereotypeNameOrId}'");
            }

            return stereotypes.SingleOrDefault();
        }

        /// <summary>
        /// Look up multiple stereotypes by the same name or definitionId.
        /// </summary>
        public static IReadOnlyCollection<IStereotype> GetStereotypes(this IHasStereotypes model, string stereotypeNameOrId)
        {
            return model.Stereotypes.Where(p => p.Name == stereotypeNameOrId || p.DefinitionId == stereotypeNameOrId).ToArray();
        }

        /// <summary>
        /// Retrieve the value of the property with the provided <paramref name="propertyName"/>
        /// on the provided <paramref name="stereotype"/>.
        /// </summary>
        public static T GetProperty<T>(this IStereotype stereotype, string propertyName, T defaultIfNotFound = default)
        {
            if (stereotype == null)
            {
                return defaultIfNotFound;
            }
            //foreach (var property in stereotype.Properties)
            if (stereotype.TryGetProperty(propertyName, out var property) && !string.IsNullOrWhiteSpace(property.Value))
            {
                //if (property.Key != propertyName || string.IsNullOrWhiteSpace(property.Value)) continue;

                if (Nullable.GetUnderlyingType(typeof(T)) != null) // is nullable type
                {
                    return (T)Convert.ChangeType(property.Value, Nullable.GetUnderlyingType(typeof(T)));
                }

                if (property is IStereotypeProperty<T> stereotypeProperty)
                {
                    return stereotypeProperty.Value;
                }
                return (T)Convert.ChangeType(property.Value, typeof(T));
            }
            return defaultIfNotFound;
        }

        /// <summary>
        /// Used to query whether or not a stereotype with a particular name or definition Id is present.
        /// </summary>
        public static bool HasStereotype(this IHasStereotypes model, string stereotypeNameOrId)
        {
            return model.Stereotypes.Any(x => x.Name == stereotypeNameOrId || x.DefinitionId == stereotypeNameOrId);
        }
    }
}
