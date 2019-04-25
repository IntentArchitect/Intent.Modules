﻿using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modelers.Services.Api
{
    public interface IDTOModel : IMetaModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }

        IEnumerable<string> GenericTypes { get; } 

        bool IsMapped { get; }

        IClassMapping MappedClass { get; }

        IApplication Application { get; }

        IEnumerable<IAttribute> Fields { get; }

        string Comment { get; }
    }
}