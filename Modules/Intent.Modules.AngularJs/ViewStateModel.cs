using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Common;
using Intent.SoftwareFactory.Plugins;

namespace Intent.Modules.AngularJs
{
    public class ViewStateModel
    {
        public string Name { get; set; }
        public IList<CommandModel> Commands { get; set; }
        public IList<Stereotype> Stereotypes { get; set; }
        public ViewStateModel Parent { get; set; }
        public Application Application { get; set; }
    }

    public class CommandModel
    {
        public string Name { get; set; }
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
                    Commands = new List<CommandModel>(),
                    Stereotypes = new List<Stereotype>()
                    {
                        new Stereotype("AngularState", new[]
                        {
                            new StereotypeProperty("Url", "", StereotypePropertyType.String),
                        })
                    },
                }
            };
            models[1].Parent = models[0];
            return new List<IMetaDataResults>()
            {
                new MetaDataResults<ViewStateModel>("ViewState", models)
            };
        }
    }
}
