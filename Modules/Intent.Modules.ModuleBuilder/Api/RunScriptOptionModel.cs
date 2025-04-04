using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using IconType = Intent.IArchitect.Common.Types.IconType;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge)]
    public class RunScriptOptionModel : ScriptModel, IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public new const string SpecializationType = "Run Script Option";
        public new const string SpecializationTypeId = "345d46fb-c500-409a-88c7-26720572c9af";

        public RunScriptOptionModel(IElement element) : base(element, SpecializationType)
        {
        }

        public ContextMenuModel SubmenuMenu => _element.ChildElements
            .GetElementsOfType(ContextMenuModel.SpecializationTypeId)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        public ContextMenuModel MenuOptions => _element.ChildElements
            .GetElementsOfType(ContextMenuModel.SpecializationTypeId)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(RunScriptOptionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RunScriptOptionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Ignore)]
        public RunScriptOption ToPersistable()
        {
            if (HasScriptSettings())
            {
                return new RunScriptOption
                {
                    Type = ContextMenuOptionType.RunScript,
                    MenuGroup = this.GetOptionSettings().MenuGroup().GetValueOrDefault(0),
                    Order = this.GetOptionSettings().Order()?.ToString(),
                    Text = this.Name,
                    Shortcut = this.GetOptionSettings().Shortcut(),
                    MacShortcut = this.GetOptionSettings().ShortcutMacOS(),
                    TriggerOnDoubleClick = this.GetOptionSettings().TriggerOnDoubleClick(),
                    Icon = this.GetOptionSettings().Icon()?.ToPersistable() ?? new IconModelPersistable() { Type = IconType.FontAwesome, Source = "code" },
                    Script = this.Script,
                    Dependencies = this.Dependencies,
                    IsOptionVisibleFunction = this.GetOptionSettings().IsOptionVisibleFunction(),
                    HasTopDivider = this.GetOptionSettings().TopDivider(),
                    HasBottomDivider = this.GetOptionSettings().BottomDivider(),
                    SubMenuOptions = MenuOptions?.ToPersistable()
                };
            }
            else
            {
                return new RunScriptOption
                {
                    Type = ContextMenuOptionType.RunScript,
                    MenuGroup = this.GetOptionSettings().MenuGroup().GetValueOrDefault(0),
                    Order = this.GetOptionSettings().Order()?.ToString(),
                    Text = this.Name,
                    Shortcut = this.GetOptionSettings().Shortcut(),
                    MacShortcut = this.GetOptionSettings().ShortcutMacOS(),
                    TriggerOnDoubleClick = this.GetOptionSettings().TriggerOnDoubleClick(),
                    Icon = this.GetOptionSettings().Icon()?.ToPersistable() ?? new IconModelPersistable() { Type = IconType.FontAwesome, Source = "code" },
                    ScriptReference = new TargetReferencePersistable() { Id = InternalElement.TypeReference.ElementId, Name = InternalElement.TypeReference.Element.Name },
                    IsOptionVisibleFunction = this.GetOptionSettings().IsOptionVisibleFunction(),
                    HasTopDivider = this.GetOptionSettings().TopDivider(),
                    HasBottomDivider = this.GetOptionSettings().BottomDivider(),
                    SubMenuOptions = MenuOptions?.ToPersistable()
                };
            }
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class RunScriptOptionModelExtensions
    {

        public static bool IsRunScriptOptionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == RunScriptOptionModel.SpecializationTypeId;
        }

        public static RunScriptOptionModel AsRunScriptOptionModel(this ICanBeReferencedType type)
        {
            return type.IsRunScriptOptionModel() ? new RunScriptOptionModel((IElement)type) : null;
        }
    }
}