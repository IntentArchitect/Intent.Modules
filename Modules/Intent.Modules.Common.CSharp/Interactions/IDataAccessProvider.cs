#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.Interactions;

public interface IDataAccessProvider
{
    bool IsUsingProjections { get; }
    CSharpStatement SaveChangesAsync();
    CSharpStatement AddEntity(string entityName);
    CSharpStatement Update(string entityName);
    CSharpStatement Remove(string entityName);
    CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps);
    CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps);
    CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements);
    CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNUllable, out IList<CSharpStatement> prerequisiteStatements);
    CSharpStatement FindAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements);
    CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageSize, string? cursorToken, out IList<CSharpStatement> prerequisiteStatements);
}