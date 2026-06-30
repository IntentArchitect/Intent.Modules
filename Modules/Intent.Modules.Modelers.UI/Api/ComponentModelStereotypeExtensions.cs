using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    public static class ComponentModelStereotypeExtensions
    {
        public static Composable GetComposable(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(Composable.DefinitionId);
            return stereotype != null ? new Composable(stereotype) : null;
        }


        public static bool HasComposable(this ComponentModel model)
        {
            return model.HasStereotype(Composable.DefinitionId);
        }

        public static bool TryGetComposable(this ComponentModel model, out Composable stereotype)
        {
            if (!HasComposable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Composable(model.GetStereotype(Composable.DefinitionId));
            return true;
        }
        public static Dialog GetDialog(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(Dialog.DefinitionId);
            return stereotype != null ? new Dialog(stereotype) : null;
        }


        public static bool HasDialog(this ComponentModel model)
        {
            return model.HasStereotype(Dialog.DefinitionId);
        }

        public static bool TryGetDialog(this ComponentModel model, out Dialog stereotype)
        {
            if (!HasDialog(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Dialog(model.GetStereotype(Dialog.DefinitionId));
            return true;
        }

        public static Page GetPage(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(Page.DefinitionId);
            return stereotype != null ? new Page(stereotype) : null;
        }


        public static bool HasPage(this ComponentModel model)
        {
            return model.HasStereotype(Page.DefinitionId);
        }

        public static bool TryGetPage(this ComponentModel model, out Page stereotype)
        {
            if (!HasPage(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Page(model.GetStereotype(Page.DefinitionId));
            return true;
        }

        public static Secured GetSecured(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(Secured.DefinitionId);
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static IReadOnlyCollection<Secured> GetSecureds(this ComponentModel model)
        {
            var stereotypes = model
                .GetStereotypes(Secured.DefinitionId)
                .Select(stereotype => new Secured(stereotype))
                .ToArray();

            return stereotypes;
        }


        public static bool HasSecured(this ComponentModel model)
        {
            return model.HasStereotype(Secured.DefinitionId);
        }

        public static bool TryGetSecured(this ComponentModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype(Secured.DefinitionId));
            return true;
        }

        public class Composable
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "5a2ba6fc-8512-4801-8b14-9d532c9c2616";

            public Composable(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class Dialog
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1f4165ee-41a0-4520-a193-9ae4d3413d1f";

            public Dialog(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class Page
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "ea4adc09-8978-4ede-ba5f-265debb2b60c";

            public Page(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Route()
            {
                return _stereotype.GetProperty<string>("Route");
            }

            public string Title()
            {
                return _stereotype.GetProperty<string>("Title");
            }

        }

        public class Secured
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";

            public Secured(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Roles()
            {
                return _stereotype.GetProperty<string>("Roles");
            }

            public string Policy()
            {
                return _stereotype.GetProperty<string>("Policy");
            }

        }

    }
}