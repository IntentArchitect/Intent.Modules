#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Interactions;

public record EntityDetails(IElement ElementModel, string VariableName, IDataAccessProvider DataAccessProvider, bool IsNew, string ProjectedType = null, bool IsCollection = false);