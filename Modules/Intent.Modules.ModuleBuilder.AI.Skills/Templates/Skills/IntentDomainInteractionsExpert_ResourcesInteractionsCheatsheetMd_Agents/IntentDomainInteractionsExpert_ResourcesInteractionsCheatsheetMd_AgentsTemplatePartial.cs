using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.IntentDomainInteractionsExpert_ResourcesInteractionsCheatsheetMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntentDomainInteractionsExpert_ResourcesInteractionsCheatsheetMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.IntentDomainInteractionsExpert_ResourcesInteractionsCheatsheetMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntentDomainInteractionsExpert_ResourcesInteractionsCheatsheetMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("interactions-cheatsheet", relativeLocation: "intent-domain-interactions-expert/resources")
                .FromMarkdown(""""""
                    # Intent Domain Interactions Expert Cheatsheet

                    ## Workflow Overview
                    ```
                    Designer model:                                          Generated code (handler body):
                    Operation "PlaceOrder"                                 ┌─ var customer = _customers.FindByIdAsync(...);
                    ├─ QueryEntity → Customer            ──ImplementInteractions──►
                    ├─ CreateEntity → Order                                ├─ _orders.Add(order);
                    ├─ Publish → OrderPlacedEvent                          └─ _bus.Publish(new OrderPlacedEvent(...));
                    ```

                    ===

                    ## Strategy Roles

                    ```
                    ┌──────────────────────────────────┐
                    │  Handler Factory Extension       │  ◄── lives in the handler's module (e.g. MediatR)
                    │  - OnBeforeTemplateExecution     │      finds handler templates, attaches mapping resolvers,
                    │    iterates handlers,            │      calls method.ImplementInteractions(handler.Model)
                    │    sets up mapping resolvers     │
                    └──────────────┬───────────────────┘
                                   │
                                   ▼
                    ┌──────────────────────────────────┐
                    │  ImplementInteractions ext.      │  ◄── from Intent.Modules.Common.CSharp.Interactions
                    │  - iterates interaction elements │      foreach interaction in operation.OwnedAssociations:
                    │  - finds IsMatch strategy        │        InteractionStrategyProvider.Instance
                    │  - calls ImplementInteraction    │          .FirstOrDefault(s => s.IsMatch(interaction))
                    └──────────────┬───────────────────┘          .ImplementInteraction(method, interaction)
                                   │
                                   ▼
                    ┌──────────────────────────────────┐
                    │  IInteractionStrategy            │  ◄── strategy logic
                    │  - IsMatch(IElement)             │
                    │  - ImplementInteraction(...)     │
                    │    └─ emits phased statements    │
                    │       into method body           │
                    └──────────────────────────────────┘
                    ```

                    ===

                    ## Strategy Skeleton

                    ```csharp
                    public class MyInteractionStrategy : IInteractionStrategy
                    {
                        public bool IsMatch(IElement interaction)
                        {
                            if (!interaction.IsMyInteractionKindTargetEndModel())
                                return false;

                            var action = interaction.AsMyInteractionKindTargetEndModel();
                                return action?.TypeReference?.Element != null && action.Mappings.Any();
                        }

                        public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
                        {
                            ArgumentNullException.ThrowIfNull(method);

                            var interaction = (IAssociationEnd)interactionElement;
                            var action = interaction.AsMyInteractionKindTargetEndModel();
                            var handlerClass = method.Class;
                            var template = (ICSharpFileBuilderTemplate)handlerClass.File.Template;

                            // 1. Inject required services
                            handlerClass.InjectService(template.GetTypeName(SomeServiceTemplateId), "someService");

                            // 2. Register mapping resolvers
                            var mapping = method.GetMappingManager();
                            mapping.AddMappingResolver(new MyMappingResolver(template));

                            // 3. Register type sources
                            template.AddTypeSource(SomeReferencedTemplate.TemplateId);

                            // 4. Emit body with explicit phases
                            var creationStatement = mapping.GenerateCreationStatement(action.Mappings.Single());
                            method.AddStatement(ExecutionPhases.BusinessLogic, creationStatement);
                        }
                    }
                    ```

                    ===

                    ## Built-in Strategies

                    | Strategy | Module | Match predicate | Purpose |
                    |---|---|---|---|
                    | `QueryInteractionStrategy` | `Application.DomainInteractions` | `IsQueryEntityActionTargetEndModel` | Load entity (single or list) into a local var |
                    | `CreateEntityInteractionStrategy` | `Application.DomainInteractions` | `IsCreateEntityActionTargetEndModel` | New-up entity, mapping fields, add to repository |
                    | `UpdateEntityInteractionStrategy` | `Application.DomainInteractions` | `IsUpdateEntityActionTargetEndModel` | Load + apply mapping + (no save — UoW handles it) |
                    | `DeleteEntityInteractionStrategy` | `Application.DomainInteractions` | `IsDeleteEntityActionTargetEndModel` | Load + repository.Remove |
                    | `CallDomainServiceInteractionStrategy` | `Application.DomainInteractions` | `IsCallDomainServiceActionTargetEndModel` | Inject domain service + invoke method |
                    | `CallEntityServiceInteractionStrategy` | `Application.DomainInteractions` | `IsCallEntityServiceActionTargetEndModel` | Call an instance method on a previously-queried entity |
                    | `PublishIntegrationMessageInteractionStrategy` | `Eventing.Contracts` | `IsPublishIntegrationEventTargetEndModel` OR `IsSendIntegrationCommandTargetEndModel` | Map source → new message, then publish/send |

                    ===

                    ## Handler Discovery in a Factory Extension

                    ```csharp
                    protected override void OnBeforeTemplateExecution(IApplication application)
                    {
                    var templates = application
                        .FindTemplateInstances<ITemplate>(TemplateRoles.Application.Eventing.EventHandler)
                        .OfType<ICSharpFileBuilderTemplate>();

                        foreach (var template in templates)
                        {
                            foreach (var handler in template.CSharpFile.GetProcessingHandlers())
                            {
                                var method = handler.Method;
                                var mappingManager = method.GetMappingManager();

                                mappingManager.AddMappingResolver(new ProcessingHandlerDomainUpdateMappingTypeResolver(template));
                                mappingManager.AddMappingResolver(new InvocationMappingTypeResolver(template));

                                template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);

                                mappingManager.SetFromReplacement(handler.Model, "message");
                                method.ImplementInteractions(handler.Model);
                            }
                        }
                    }
                    ```

                    ===

                    ## Execution Phases

                    | Phase | Typical contents |
                    |---|---|
                    | `Initialise` | Variable declarations, guards, early returns |
                    | `BusinessLogic` | Domain queries, entity mutations, service calls |
                    | `IntegrationEvents` | `_bus.Publish(...)`, `_bus.Send(...)` |
                    | `Return` | The terminal `return` expression |

                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
