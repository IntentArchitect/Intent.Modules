(async () => {

    const StringTypeId : string = "d384db9c-a279-45e1-801e-e4e8099625f2";
    const GuidTypeId : string = "6b649125-18ea-48fd-a6ba-0bfff0d8f488";
    
    let targetClass = association.typeReference.getType();
    
    if (isAggregateRoot(targetClass)) {
        targetClass.getChildren("Attribute").filter(x => x.hasMetadata("id")).forEach(x => x.typeReference.setType(StringTypeId));
    }
    
    function isAggregateRoot(element : MacroApi.Context.IElementApi) : boolean {
        return ! element.getAssociations("Association")
            .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
    }
    
    })();