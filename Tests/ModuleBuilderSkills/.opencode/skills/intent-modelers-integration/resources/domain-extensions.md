---
contentHash: EB6CB983F0C5E00C804A2388571DF5656D15C1E3E4900D75DCE2C975761DFA22
---
# Domain Designer — Extension Modules

The modules below are installed separately from the base Domain designer (`Intent.Modules.Modelers.Domain`). Each one adds
its own element types, package/folder extensions or navigation on top of it. Read `domain.md`
first for the base designer; this file covers only what the extension modules add. Load it only when
your module actually installs one of them.

Every extension module has its **own** install identity. Never infer one from its parent designer or
from a sibling (`SKILL.md` Must #6).

## 1. Install identities

| Extension | NuGet PackageId | Intent module id | API namespace | Adds |
|---|---|---|---|---|
| Domain Events | `Intent.Modules.Modelers.Domain.Events` | `Intent.Modelers.Domain.Events` | `Intent.Modelers.Domain.Events.Api` | `DomainEventModel` (`0814e459-...`), `DomainEventHandlerModel` (`d80e61c5-...`, implements `IProcessingHandlerModel`), `PropertyModel` (`b4d69073-...`), plus four association families: `DomainEventAssociationModel`, `DomainEventHandlerAssociationModel`, `DomainEventGeneralizationModel`, `DomainEventOriginAssociationModel` |
| Repositories | `Intent.Modules.Modelers.Domain.Repositories` | `Intent.Modelers.Domain.Repositories` | `Intent.Modelers.Domain.Repositories.Api` | `RepositoryModel` (`96ffceb2-...`, reuses Domain's own `OperationModel`), `PackageExtensionsModel`, `FolderExtensionsModel` |
| Value Objects | `Intent.Modules.Modelers.Domain.ValueObjects` | `Intent.Modelers.Domain.ValueObjects` | `Intent.Modelers.Domain.ValueObjects.Api` | `ValueObjectModel` (`5fe6bb0a-...`, reuses Domain's own `AttributeModel`), `DomainPackageExtensionModel`, `FolderExtensionModel`. **Also defines its own stereotype**: `ValueObjectModelStereotypeExtensions.SerializationSettings` (`4ced3df6-...`) |
| Domain Services | `Intent.Modelers.Domain.Services` ⚠️ **no `Modules.` segment** | `Intent.Modelers.Domain.Services` | `Intent.Modelers.Domain.Services.Api` | `DomainServiceModel` (`07f936ea-...`, reuses Domain's own `OperationModel`), `DomainPackageExtensionModel`, `FolderExtensionModel` |
| Stored Procedures | `Intent.Modules.Modelers.Domain.StoredProcedures` | `Intent.Modules.Modelers.Domain.StoredProcedures` ⚠️ **keeps `Modules.` in BOTH module id and namespace, unlike all four siblings above** | `Intent.Modules.Modelers.Domain.StoredProcedures.Api` | `StoredProcedureModel` (`575edd35-...`, implements `IInvokableModel`), `StoredProcedureParameterModel`, `PackageExtensionModel`, `FolderElementExtensionModel`, plus association `StoredProcedureInvocationModel` (navigation directly under `Api/`, not `Api/Extensions/`) |

This table is exactly why `SKILL.md`'s Must #6 / Must Not #5 exist: five sibling extension modules
of the *same* designer, and no two of them are guaranteed to follow the same `Modules.`-segment
convention. Always check the specific module's own `.csproj`/`.imodspec`.

## 2. Model API reference

Every public type and member each extension module ships — extracted mechanically from source, not
sampled. **Prefer this to reflecting over the assembly:** if a member is not listed here, it is
not part of the version shown. The stubs follow the conventions defined in
`domain.md` §9 — `// + common element members`, the omitted `Is<X>Model()` / `As<X>Model()`
casts, and `// + common association-end members` mean exactly what they mean there.

### 2.1 Domain.Events — `Intent.Modules.Modelers.Domain.Events` (source at `4.4.7`)

```csharp
namespace Intent.Modelers.Domain.Events.Api;

public static class ApiMetadataProviderExtensions
{
    static IList<DomainEventModel> GetDomainEventModels(this IDesigner designer);
    static IList<DomainEventHandlerModel> GetDomainEventHandlerModels(this IDesigner designer);
}

public class DomainPackageExtensionModel : DomainPackageModel
{
    // + everything inherited from DomainPackageModel
    IList<DomainEventModel> DomainEvents { get; }
}

public class ServicesPackageExtensionModel : ServicesPackageModel
{
    // + everything inherited from ServicesPackageModel
    IList<DomainEventHandlerModel> DomainEventHandlers { get; }
}

// "Domain Event Association" · 8014b71d-627b-4349-b6d7-5011dfa1bb09
public class DomainEventAssociationModel : IMetadataModel
{
    static DomainEventAssociationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    DomainEventAssociationSourceEndModel SourceEnd { get; }
    DomainEventAssociationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

// "Domain Event Association Source End" · ad8998ec-ee85-4544-9267-8ec150a2f257
public class DomainEventAssociationSourceEndModel : DomainEventAssociationEndModel { /* common association-end members */ }

// "Domain Event Association Target End" · fe0d0f78-fb7e-4685-ab2d-bae88054a78e
public class DomainEventAssociationTargetEndModel : DomainEventAssociationEndModel
{
    // + common association-end members
    IEnumerable<IElementToElementMapping> Mappings { get; }
}

public class DomainEventAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

// "Domain Event Generalization" · fa57ec52-536d-46a8-8aa0-4589812665c1
public class DomainEventGeneralizationModel : IMetadataModel
{
    static DomainEventGeneralizationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    DomainEventGeneralizationSourceEndModel SourceEnd { get; }
    DomainEventGeneralizationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

// "Domain Event Generalization Source End" · 9d77f404-ca00-46ea-9ec8-8734274cb3f7
public class DomainEventGeneralizationSourceEndModel : DomainEventGeneralizationEndModel { /* common association-end members */ }

// "Domain Event Generalization Target End" · 1540252f-51ba-48e4-a7ad-f52ac32389e6
public class DomainEventGeneralizationTargetEndModel : DomainEventGeneralizationEndModel { /* common association-end members */ }

public class DomainEventGeneralizationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

// "Domain Event Handler Association" · 90831494-f069-44eb-b488-ab2dba7518ea
public class DomainEventHandlerAssociationModel : IMetadataModel
{
    static DomainEventHandlerAssociationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    DomainEventHandlerAssociationSourceEndModel SourceEnd { get; }
    DomainEventHandlerAssociationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

// "Domain Event Handler Association Source End" · 79f048d4-4c09-4405-be8f-95473a981556
public class DomainEventHandlerAssociationSourceEndModel : DomainEventHandlerAssociationEndModel { /* common association-end members */ }

// "Domain Event Handler Association Target End" · f45dfee9-f62b-45ac-bfce-a3878e04b73f
public class DomainEventHandlerAssociationTargetEndModel : DomainEventHandlerAssociationEndModel, IProcessingHandlerModel { /* common association-end members */ }

public class DomainEventHandlerAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper, IProcessingHandlerModel { /* common association-end members */ }

// "Domain Event Handler" · d80e61c5-7e4c-4175-9df1-0413f664824c
public class DomainEventHandlerModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder, IProcessingHandlerModel
{
    // + common element members
    FolderModel Folder { get; }
}

// "Domain Event" · 0814e459-fb9b-47db-b7eb-32ce30397e8a
public class DomainEventModel : IMetadataModel, IHasStereotypes, IHasName, IHasFolder, IElementWrapper
{
    // + common element members
    FolderModel Folder { get; }
    bool IsMapped { get; }
    IElementMapping Mapping { get; }
    IList<PropertyModel> Properties { get; }
}

public static class DomainEventModelExtensions
{
    static bool HasMapFromClassMapping(this DomainEventModel type);
    static IElementMapping GetMapFromClassMapping(this DomainEventModel type);
}

// "Domain Event Origin Association" · 4c0cc50b-8a9d-43cd-b731-9f354f69f3c9
public class DomainEventOriginAssociationModel : IMetadataModel
{
    static DomainEventOriginAssociationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    DomainEventOriginAssociationSourceEndModel SourceEnd { get; }
    DomainEventOriginAssociationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

// "Domain Event Origin Association Source End" · 2495d3ef-f7a2-441d-b749-b51b2546b45e
public class DomainEventOriginAssociationSourceEndModel : DomainEventOriginAssociationEndModel { /* common association-end members */ }

// "Domain Event Origin Association Target End" · 17046427-14e2-4081-8463-ef16c0fda399
public class DomainEventOriginAssociationTargetEndModel : DomainEventOriginAssociationEndModel
{
    // + common association-end members
    IEnumerable<IElementToElementMapping> Mappings { get; }
}

public class DomainEventOriginAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

public static class DomainEventAssociationModelAssociationExtensions
{
    static IList<DomainEventAssociationTargetEndModel> AssociatedClasses(this DomainEventModel model);
    static IList<DomainEventAssociationSourceEndModel> AssociatedDomainEvents(this ClassModel model);
}

public static class DomainEventGeneralizationModelAssociationExtensions
{
    static IList<DomainEventGeneralizationTargetEndModel> Generalizations(this DomainEventModel model);
    static IList<DomainEventGeneralizationSourceEndModel> Specializations(this DomainEventModel model);
    static IList<DomainEventGeneralizationEndModel> DomainEventGeneralizationEnds(this DomainEventModel model);
}

public static class DomainEventHandlerAssociationModelAssociationExtensions
{
    static IList<DomainEventHandlerAssociationTargetEndModel> HandledDomainEvents(this DomainEventHandlerModel model);
    static IList<DomainEventHandlerAssociationSourceEndModel> DomainEventHandlers(this DomainEventModel model);
}

public static class DomainEventOriginAssociationModelAssociationExtensions
{
    static IList<DomainEventOriginAssociationTargetEndModel> PublishedDomainEvents(this OperationModel model);
    static IList<DomainEventOriginAssociationTargetEndModel> PublishedDomainEvents(this ClassConstructorModel model);
    static IList<DomainEventOriginAssociationSourceEndModel> DomainEventSources(this DomainEventModel model);
}

public class FolderExtensionModel : FolderModel
{
    // + everything inherited from FolderModel
    IList<DomainEventModel> DomainEvents { get; }
}

// "Property" · b4d69073-5abb-4968-b41b-545b2f7408ed
public class PropertyModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IElementWrapper
{
    // + common element members
    ITypeReference TypeReference { get; }
}
```

### 2.2 Domain.Repositories — `Intent.Modules.Modelers.Domain.Repositories` (source at `3.6.3`)

```csharp
namespace Intent.Modelers.Domain.Repositories.Api;

public static class ApiMetadataProviderExtensions
{
    static IList<RepositoryModel> GetRepositoryModels(this IDesigner designer);
}

public class FolderExtensionsModel : FolderModel
{
    // + everything inherited from FolderModel
    IList<RepositoryModel> Repositories { get; }
}

public class PackageExtensionsModel : DomainPackageModel
{
    // + everything inherited from DomainPackageModel
    IList<RepositoryModel> Repositories { get; }
}

// "Repository" · 96ffceb2-a70a-4b69-869b-0df436c470c3
public class RepositoryModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference, IHasFolder
{
    // + common element members
    FolderModel Folder { get; }
    ITypeReference TypeReference { get; }
    ITypeReference EntityType { get; }
    IList<OperationModel> Operations { get; }
}
```

### 2.3 Domain.Services — `Intent.Modules.Modelers.Domain.Services` (source at `1.3.1`)

```csharp
namespace Intent.Modelers.Domain.Services.Api;

public static class ApiMetadataProviderExtensions
{
    static IList<DomainServiceModel> GetDomainServiceModels(this IDesigner designer);
}

public class DomainPackageExtensionModel : DomainPackageModel
{
    // + everything inherited from DomainPackageModel
    IList<DomainServiceModel> DomainServices { get; }
}

// "Domain Service" · 07f936ea-3756-48c8-babd-24ac7271daac
public class DomainServiceModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
{
    // + common element members
    FolderModel Folder { get; }
    IList<OperationModel> Operations { get; }
}

public class FolderExtensionModel : FolderModel
{
    // + everything inherited from FolderModel
    IList<DomainServiceModel> DomainServices { get; }
}
```

### 2.4 Domain.StoredProcedures — `Intent.Modules.Modelers.Domain.StoredProcedures` (source at `1.2.4`)

```csharp
namespace Intent.Modules.Modelers.Domain.StoredProcedures.Api;

public class FolderElementExtensionModel : FolderModel
{
    // + everything inherited from FolderModel
    IList<StoredProcedureModel> StoredProcedures { get; }
}

public class PackageExtensionModel : DomainPackageModel
{
    // + everything inherited from DomainPackageModel
    IList<StoredProcedureModel> StoredProcedures { get; }
}

// "Stored Procedure Invocation" · adf062ed-c0a4-421f-9940-318a91e9a52c
public class StoredProcedureInvocationModel : IMetadataModel
{
    static StoredProcedureInvocationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    StoredProcedureInvocationSourceEndModel SourceEnd { get; }
    StoredProcedureInvocationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

// "Stored Procedure Invocation Source End" · 7b7d3fd8-5e32-4f8c-b4cc-7b92f45a8577
public class StoredProcedureInvocationSourceEndModel : StoredProcedureInvocationEndModel { /* common association-end members */ }

// "Stored Procedure Invocation Target End" · d0b0b24a-db0f-4aff-873a-a0e9c2dce12d
public class StoredProcedureInvocationTargetEndModel : StoredProcedureInvocationEndModel
{
    // + common association-end members
    IEnumerable<IElementToElementMapping> Mappings { get; }
}

public class StoredProcedureInvocationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

public static class StoredProcedureInvocationModelAssociationExtensions
{
    static IList<StoredProcedureInvocationTargetEndModel> StoredProcedureInvocationTargets(this OperationModel model);
    static IList<StoredProcedureInvocationSourceEndModel> StoredProcedureInvocationSources(this StoredProcedureModel model);
    static IElementToElementMapping GetMapInvocationMapping(this StoredProcedureInvocationTargetEndModel model);
    static IElementToElementMapping GetMapResultMapping(this StoredProcedureInvocationTargetEndModel model);
}

// "Stored Procedure" · 575edd35-9438-406d-b0a7-b99d6f29b560
public class StoredProcedureModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IInvokableModel, IHasTypeReference, IHasFolder
{
    // + common element members
    FolderModel Folder { get; }
    ITypeReference TypeReference { get; }
    ITypeReference ReturnType { get; }
    IList<StoredProcedureParameterModel> Parameters { get; }
}

// "Stored Procedure Parameter" · 5823b192-eb03-47c8-90d8-5501c922e9a5
public class StoredProcedureParameterModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
{
    // + common element members
    ITypeReference TypeReference { get; }
}
```

### 2.5 Domain.ValueObjects — `Intent.Modules.Modelers.Domain.ValueObjects` (source at `3.6.3`)

```csharp
namespace Intent.Modelers.Domain.ValueObjects.Api;

public static class ApiMetadataProviderExtensions
{
    static IList<ValueObjectModel> GetValueObjectModels(this IDesigner designer);
}

public class DomainPackageExtensionModel : DomainPackageModel
{
    // + everything inherited from DomainPackageModel
    IList<ValueObjectModel> ValueObjects { get; }
}

public class FolderExtensionModel : FolderModel
{
    // + everything inherited from FolderModel
    IList<ValueObjectModel> ValueObjects { get; }
}

// "Value Object" · 5fe6bb0a-7fc3-42ae-a351-d9188f5b8bc5
public class ValueObjectModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
{
    // + common element members
    FolderModel Folder { get; }
    IList<AttributeModel> Attributes { get; }
}

public static class ValueObjectModelStereotypeExtensions
{
    static SerializationSettings GetSerializationSettings(this ValueObjectModel model);
    static bool HasSerializationSettings(this ValueObjectModel model);
    static bool TryGetSerializationSettings(this ValueObjectModel model, out SerializationSettings stereotype);
    public class SerializationSettings
    {
        const string DefinitionId = "4ced3df6-2827-461d-bf1b-6512f521f2c6";
        string Name { get; }
        TypeOptions Type();
        public class TypeOptions
        {
            TypeOptionsEnum AsEnum();
            bool IsJSON();
        }
        public enum TypeOptionsEnum { JSON }
    }
}
```

## 3. Name collisions and gaps

- **`FolderExtensionModel`** is declared in Domain, Domain Events, Domain Services and Value Objects, and **`DomainPackageExtensionModel`** in Domain Events, Domain Services and Value Objects — the same simple name in different namespaces. Alias them if your template imports more than one of these modules.
- Stored Procedures names its equivalents differently (`FolderElementExtensionModel`, `PackageExtensionModel`), and so does Repositories (`FolderExtensionsModel`, `PackageExtensionsModel` — plural). Copy the name from the stub above; don't infer it from a sibling.
