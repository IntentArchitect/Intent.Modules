using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Common;
using Intent.SoftwareFactory.Plugins;

namespace Intent.Modules.AngularJs
{
    public class ViewStateModel : IHasStereotypes
    {
        public string Name { get; set; }
        public IList<CommandModel> Commands { get; set; }
        public IEnumerable<IStereotype> Stereotypes { get; set; }
        public ViewStateModel Parent { get; set; }
        public Application Application { get; set; }
    }

    public class CommandModel
    {
        public string Name { get; set; }
        public IEnumerable<IStereotype> Stereotypes { get; set; }
    }

    public class MetadataProvider : IMetaDataProvider
    {
        public IEnumerable<IMetaDataResults> GetMetaData()
        {
            var models = new[]
            {
                new ViewStateModel()
                {
                    Name = "Shell",
                    Application = new Application("", "Test"),
                    Commands = new List<CommandModel>()
                    {
                        new CommandModel() {Name = "logout"},
                        new CommandModel() {Name = "selectSolution"},
                        new CommandModel() {Name = "createSolution"},
                        new CommandModel() {Name = "openExistingSolution"},
                        new CommandModel() {Name = "runSoftwareFactory"},
                        new CommandModel() {Name = "openSettings"},
                    },
                    Stereotypes = new List<Stereotype>()
                    {
                        new Stereotype("AngularState", new[]
                        {
                            new StereotypeProperty("Url", "/", StereotypePropertyType.String),
                        })
                    },
                },
                new ViewStateModel()
                {
                    Name = "Home",
                    Application = new Application("", "Test"),
                    Commands = new List<CommandModel>()
                    {
                        new CommandModel() {Name = "print", Stereotypes = new List<IStereotype>
                        {
                            new Stereotype("Service Call", new []
                            {
                                new StereotypeProperty("ServiceName", "PrintService", StereotypePropertyType.String),
                                new StereotypeProperty("OperationName", "Print(vm.ajsd, vm.asdlkfj)", StereotypePropertyType.String),
                            })
                        }
                        }
                    },
                    Stereotypes = new List<Stereotype>()
                    {
                        new Stereotype("AngularState", new[]
                        {
                            new StereotypeProperty("Url", "", StereotypePropertyType.String),
                        })
                    },
                },
                new ViewStateModel()
                {
                    Name = "About",
                    Application = new Application("", "Test"),
                    Commands = new List<CommandModel>()
                    {
                    },
                    Stereotypes = new List<Stereotype>()
                    {
                        new Stereotype("AngularState", new[]
                        {
                            new StereotypeProperty("Url", "about", StereotypePropertyType.String),
                        })
                    },
                }
            };
            models[1].Parent = models[0];
            models[2].Parent = models[0];
            return new List<IMetaDataResults>()
            {
                new MetaDataResults<ViewStateModel>("ViewState", models)
            };
        }
    }
}
