using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Plugins;
using Intent.Plugins.FactoryExtensions;

// ReSharper disable once CheckNamespace
namespace Intent.Modules.Common.Plugins
{
    /// <summary>
    /// Use implementations of this to extend the Software Factory execution process by overriding
    /// one or more of the following:
    /// <list type="bullet">
    /// <item><description><see cref="OnStep"/></description></item>
    /// <item><description><see cref="OnStart"/></description></item>
    /// <item><description><see cref="OnBeforeMetadataLoad"/></description></item>
    /// <item><description><see cref="OnAfterMetadataLoad"/></description></item>
    /// <item><description><see cref="OnBeforeTemplateRegistrations"/></description></item>
    /// <item><description><see cref="OnAfterTemplateRegistrations"/></description></item>
    /// <item><description><see cref="OnBeforeTemplateExecution"/></description></item>
    /// <item><description><see cref="OnAfterTemplateExecution"/></description></item>
    /// <item><description><see cref="OnBeforeCommitChanges"/></description></item>
    /// <item><description><see cref="OnAfterCommitChanges"/></description></item>
    /// </list>
    /// <example>
    /// For example, to override <see cref="OnStart"/>, add a member to your implementation as follows:
    /// <code>
    /// <![CDATA[
    /// protected override void OnStart(IApplication application)
    /// {
    ///     base.OnStart(application);
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>
    /// The Software Factory uses assembly scanning and reflection to automatically instantiate and
    /// register implementations of this class.
    /// </remarks>
    public abstract class FactoryExtensionBase : IExecutionLifeCycle, IFactoryExtension, ISupportsConfiguration
    {
        /// <inheritdoc />
        public abstract string Id { get; }

        /// <inheritdoc />
        public virtual int Order { get; set; }

        /// <inheritdoc />
        public virtual void Configure(IDictionary<string, string> settings)
        {
            if (settings.ContainsKey(nameof(Order)) && !string.IsNullOrWhiteSpace(settings[nameof(Order)]))
            {
                Order = int.Parse(settings[nameof(Order)]);
            }
        }

        void IExecutionLifeCycle.OnStep(IApplication application, string step)
        {
            OnStep(application, step);

            switch (step)
            {
                case ExecutionLifeCycleSteps.Start:
                    OnStart(application);
                    break;
                case ExecutionLifeCycleSteps.BeforeMetadataLoad:
                    OnBeforeMetadataLoad(application);
                    break;
                case ExecutionLifeCycleSteps.AfterMetadataLoad:
                    OnAfterMetadataLoad(application);
                    break;
                case ExecutionLifeCycleSteps.BeforeTemplateRegistrations:
                    OnBeforeTemplateRegistrations(application);
                    break;
                case ExecutionLifeCycleSteps.AfterTemplateRegistrations:
                    OnAfterTemplateRegistrations(application);
                    break;
                case ExecutionLifeCycleSteps.BeforeTemplateExecution:
                    OnBeforeTemplateExecution(application);
                    break;
                case ExecutionLifeCycleSteps.AfterTemplateExecution:
                    OnAfterTemplateExecution(application);
                    break;
                case ExecutionLifeCycleSteps.BeforeCommitChanges:
                    OnBeforeCommitChanges(application);
                    break;
                case ExecutionLifeCycleSteps.AfterCommitChanges:
                    OnAfterCommitChanges(application);
                    break;
                default:
                    throw new Exception($"Unknown step: {step}");
            }
        }

        /// <summary>
        /// Called for each phase of the Software Factory execution. The
        /// <paramref name="step"/> parameter will be populated with the current phase/step of the
        /// Software Factory. See <see cref="ExecutionLifeCycleSteps"/> for possible values.
        /// </summary>
        /// <remarks>
        /// After <see cref="OnStep"/>, has been called, then the step specific virtual method
        /// (for example <see cref="OnBeforeTemplateExecution"/>) is also always called.
        /// </remarks>
        public virtual void OnStep(IApplication application, string step)
        {
        }

        /// <summary>
        /// Called during the <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software
        /// Factory execution.
        /// </summary>
        /// <remarks>
        /// <see cref="ExecutionLifeCycleSteps.Start"/> is the first phase of the Software Factory
        /// execution process and precedes the
        /// <see cref="ExecutionLifeCycleSteps.BeforeMetadataLoad"/> phase.
        /// </remarks>
        protected virtual void OnStart(IApplication application)
        {
        }

        /// <summary>
        /// Called during the <see cref="ExecutionLifeCycleSteps.BeforeMetadataLoad"/> phase of the Software
        /// Factory execution.
        /// </summary>
        /// <remarks>
        /// The <see cref="ExecutionLifeCycleSteps.BeforeMetadataLoad"/> phase proceeds the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase and precedes the
        /// <see cref="ExecutionLifeCycleSteps.AfterMetadataLoad"/> phase.
        /// </remarks>
        /// <remarks>
        /// Called after <see cref="OnStart"/> and before <see cref="OnAfterMetadataLoad"/>.
        /// </remarks>
        protected virtual void OnBeforeMetadataLoad(IApplication application)
        {
        }

        /// <summary>
        /// Called during the <see cref="ExecutionLifeCycleSteps.AfterMetadataLoad"/> phase of the Software
        /// Factory execution.
        /// </summary>
        /// <remarks>
        /// The <see cref="ExecutionLifeCycleSteps.AfterMetadataLoad"/> phase proceeds the
        /// <see cref="ExecutionLifeCycleSteps.BeforeMetadataLoad"/> phase and precedes the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateRegistrations"/> phase.
        /// </remarks>
        protected virtual void OnAfterMetadataLoad(IApplication application)
        {
        }

        /// <summary>
        /// Called during the <see cref="ExecutionLifeCycleSteps.BeforeTemplateRegistrations"/> phase of the Software
        /// Factory execution.
        /// </summary>
        /// <remarks>
        /// The <see cref="ExecutionLifeCycleSteps.BeforeTemplateRegistrations"/> phase proceeds the
        /// <see cref="ExecutionLifeCycleSteps.AfterMetadataLoad"/> phase and precedes the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase.
        /// </remarks>
        protected virtual void OnBeforeTemplateRegistrations(IApplication application)
        {
        }

        /// <summary>
        /// Called during the <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software
        /// Factory execution.
        /// </summary>
        /// <remarks>
        /// The <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase proceeds the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateRegistrations"/> phase and precedes the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase.
        /// </remarks>
        protected virtual void OnAfterTemplateRegistrations(IApplication application)
        {
        }

        /// <summary>
        /// Called during the <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase of the Software
        /// Factory execution.
        /// </summary>
        /// <remarks>
        /// The <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase proceeds the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase and precedes the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateExecution"/> phase.
        /// </remarks>
        protected virtual void OnBeforeTemplateExecution(IApplication application)
        {
        }

        /// <summary>
        /// Called during the <see cref="ExecutionLifeCycleSteps.AfterTemplateExecution"/> phase of the Software
        /// Factory execution.
        /// </summary>
        /// <remarks>
        /// The <see cref="ExecutionLifeCycleSteps.AfterTemplateExecution"/> phase proceeds the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase and precedes the
        /// <see cref="ExecutionLifeCycleSteps.BeforeCommitChanges"/> phase.
        /// </remarks>
        protected virtual void OnAfterTemplateExecution(IApplication application)
        {
        }

        /// <summary>
        /// Called during the <see cref="ExecutionLifeCycleSteps.BeforeCommitChanges"/> phase of the Software
        /// Factory execution.
        /// </summary>
        /// <remarks>
        /// The <see cref="ExecutionLifeCycleSteps.BeforeCommitChanges"/> phase proceeds the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateExecution"/> phase and precedes the
        /// <see cref="ExecutionLifeCycleSteps.AfterCommitChanges"/> phase.
        /// </remarks>
        protected virtual void OnBeforeCommitChanges(IApplication application)
        {
        }

        /// <summary>
        /// Called during the <see cref="ExecutionLifeCycleSteps.AfterCommitChanges"/> phase of the Software
        /// Factory execution.
        /// </summary>
        /// <remarks>
        /// The <see cref="ExecutionLifeCycleSteps.AfterCommitChanges"/> phase proceeds the
        /// <see cref="ExecutionLifeCycleSteps.BeforeCommitChanges"/> phase and is the last phase of the Software
        /// Factory execution process.
        /// </remarks>
        protected virtual void OnAfterCommitChanges(IApplication application)
        {
        }
    }
}
