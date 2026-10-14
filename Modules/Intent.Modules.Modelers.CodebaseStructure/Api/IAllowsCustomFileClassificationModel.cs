using System;

namespace Intent.Modules.Modelers.CodebaseStructure.Api
{
    /// <summary>
    /// The generated interface for the "Allows File Classification" trait before 1.2.0 renamed it. Kept by hand
    /// because modules compiled against earlier versions (e.g. Intent.VisualStudio.Projects 4.1.10) implement it,
    /// and fail to load without it.
    /// </summary>
    [Obsolete("Use IAllowsFileClassificationModel.")]
    public interface IAllowsCustomFileClassificationModel : IAllowsFileClassificationModel
    {
    }
}
