(async () => {

let targetClass = association.typeReference.getType();

const GuidTypeId : string = "6b649125-18ea-48fd-a6ba-0bfff0d8f488";

if (!isAggregateRoot(targetClass)) {
    targetClass.getChildren("Attribute").filter(x => x.hasMetadata("id")).forEach(x => x.typeReference.setType(GuidTypeId));
}

function isAggregateRoot(element : MacroApi.Context.IElementApi) : boolean {
    return ! element.getAssociations("Association")
        .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
}

})();