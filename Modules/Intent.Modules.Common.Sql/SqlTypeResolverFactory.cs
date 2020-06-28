//using System.Collections.Generic;
//using System.ComponentModel;
//using Intent.Modules.Common.TypeResolution;
//using Intent.Modules.Common.Types.Contracts;

//namespace Intent.Modules.Common.Sql
//{
//    [Description("SQL Data Type Resolver")]
//    public class SqlTypeResolverFactory : ITypeResolverFactory
//    {
//        public string Name => "SQL";

//        public int Priority => 0;

//        public IEnumerable<string> SupportedFileTypes
//        {
//            get
//            {
//                return new[] { "sql" };
//            }
//        }

//        public ITypeResolver Create()
//        {
//            return new SqlTypeResolver();
//        }
//    }
//}