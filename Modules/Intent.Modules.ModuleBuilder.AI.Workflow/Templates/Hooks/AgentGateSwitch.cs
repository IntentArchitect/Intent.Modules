using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Utils;

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks
{
    /// <summary>
    /// The single on/off decision every hook template routes through, so the six registrations
    /// cannot drift apart - and so the "off" case is reported once rather than six times or, as
    /// before, not at all.
    /// </summary>
    internal static class AgentGateSwitch
    {
        /// <summary>
        /// The harness folders this module generates into. Used only to decide whether the "off"
        /// case is worth mentioning - which templates actually emit is decided by each template
        /// from the anchor it landed in, never from disk.
        /// </summary>
        private static readonly string[] HarnessFolders =
        {
            ".claude", ".codex", ".cursor", ".kiro", ".opencode",
        };

        private static readonly object WarnedLock = new();
        private static string _warnedForApplicationId;

        /// <summary>
        /// Whether the agent gate should be generated for this application.
        /// </summary>
        /// <remarks>
        /// When the setting is absent or unparseable this is false, because the generated accessor is
        /// "bool.TryParse(...) &amp;&amp; result". That default is fine; the SILENCE around it was not.
        /// With the switch off, all six templates declined and the Software Factory reported nothing
        /// at all - no warning, no log line, no entry in the result - and a template that has stopped
        /// running is indistinguishable from one that runs and produces identical output. Diagnosing
        /// that in a consuming application took most of a session and was only settled by bypassing
        /// the check in a throwaway build.
        /// <para>
        /// So when the switch is off but the application clearly expects otherwise - it has at least
        /// one AI harness folder - say so, once. Applications with no harness folders stay silent,
        /// because for them "off" is simply correct and a warning would be noise.
        /// </para>
        /// </remarks>
        public static bool IsOn(IApplication application)
        {
            var settings = Settings.ModuleSettingsExtensions.GetAIWorkflowSettings(application.Settings);
            if (settings.InstallAgentGateHooks())
            {
                return true;
            }

            WarnOnce(application);
            return false;
        }

        private static void WarnOnce(IApplication application)
        {
            lock (WarnedLock)
            {
                if (_warnedForApplicationId == application.Id)
                {
                    return;
                }

                _warnedForApplicationId = application.Id;
            }

            string[] present;
            try
            {
                present = HarnessFolders
                    .Where(folder => Directory.Exists(Path.Combine(application.OutputRootDirectory, folder)))
                    .ToArray();
            }
            catch (Exception)
            {
                // A diagnostic must never be the thing that fails a consumer's run.
                return;
            }

            if (present.Length == 0)
            {
                return;
            }

            Logging.Log.Warning(
                $"Intent.ModuleBuilder.AI.Workflow: '{application.Name}' has AI harness folders " +
                $"({string.Join(", ", present)}) but the 'Install Agent Gate Hooks' application setting is off, " +
                "so no gate scripts or hook configuration were generated. Turn the setting on if that was not " +
                "intended - nothing else reports this, because the templates simply decline.");
        }
    }
}
