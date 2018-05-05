using Intent.SoftwareFactory.MetaModels.UMLModel;

namespace Intent.Modules.RichDomain.Auditing
{
    public static class Utils
    {
        public static bool HasParentClassWhichIsAggregateRoot(Class @class)
        {
            return @class.ParentClass != null && (@class.ParentClass.IsAggregateRoot() ||
                                                  HasParentClassWhichIsAggregateRoot(@class.ParentClass));
        }
    }
}
