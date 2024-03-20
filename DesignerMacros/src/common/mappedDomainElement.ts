/// <reference path="domainHelper.ts" />

class EntityDomainElementDetails {
    public readonly owningEntity?: MacroApi.Context.IElementApi;
    
    constructor (public readonly entity: MacroApi.Context.IElementApi) {
        this.owningEntity = DomainHelper.getOwningAggregate(entity);
    }

    public getOwningOrTargetEntityName(): string {
        return (this.owningEntity ?? this.entity).getName();
    }

    public hasOwningEntity(): Boolean {
        return this.owningEntity != null;
    }
}

class MappedDomainElement {
    public readonly entityDomainElementDetails?: EntityDomainElementDetails;

    constructor (private originalElement: MacroApi.Context.IElementApi) {
        this.entityDomainElementDetails = this.isEntityDomainElement() ? new EntityDomainElementDetails(originalElement) : null;
    }

    public isEntityDomainElement(): Boolean {
        return this.originalElement.specialization == "Class";
    }

    public getId(): string {
        return this.originalElement.id;
    }

    public getName(): string {
        return this.originalElement.getName();
    }
}