using System;
using System.Runtime.InteropServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Keys.IdentityGenerator", Version = "1.0")]

namespace MyCompany.MyMovies.Domain.Common
{
    public static class IdentityGenerator
    {
        /// <summary>
        /// Generates sequential GUIDs for SQL Server.
        /// https://github.com/richardtallent/RT.Comb
        /// </summary>
        /// <returns></returns>
        public static Guid NewSequentialId()
        {
            return RT.Comb.Provider.Sql.Create();
        }
    }
}