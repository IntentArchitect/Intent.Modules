// The FilePerModel and SingleFileListModel template-registration scaffolds emit
// "using Intent.ModuleBuilder.Api;" unconditionally, because in the normal case a registration's
// model type comes from there (FileTemplateModel, CSharpTemplateModel, ElementSettingsModel, ...).
//
// This module is the abnormal case: its registration model types are local marker classes
// (HarnessFolderModel, GateSourceFileModel) or plain object, so nothing is consumed from that
// namespace - but the using is still generated, and without the namespace existing the module does
// not compile. Removing the using by hand does not hold: the Software Factory re-adds it on the
// next run, so every regeneration would break the build again.
//
// Declaring the namespace empty resolves the using at compile time and costs nothing. The
// alternative - referencing the Intent.Modules.ModuleBuilder package, which is what actually
// provides it - would force every consumer of this module to install the whole Module Builder
// module to satisfy a using that is never used.
//
// Delete this file if the scaffold is ever changed to emit that using only when the model type
// genuinely needs it.
namespace Intent.ModuleBuilder.Api
{
}
