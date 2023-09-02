/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />

/**
 * Ensures that for the provided contract, it has the provided field creating it if necessary and
 * then ensure the field has the correct typeReference details and order.
 * The field's element is returned.
 */
function ensureHasField(options: {
    contract: IElementApi,
    fieldDetail: IFieldDetails,
    mappingSettingsId?: string
    order?: number
}): IElementApi {
    const { contract, fieldDetail, mappingSettingsId, order } = options;

    let field = fieldDetail.existingId != null
        ? contract.getChildren("DTO-Field").find(x => x.id === fieldDetail.existingId)
        : createElement("DTO-Field", fieldDetail.name, contract.id);

    field.typeReference.setType(fieldDetail.typeId);
    field.typeReference.setIsCollection(fieldDetail.isCollection);
    field.typeReference.setIsNullable(fieldDetail.isNullable);

    if (order != null) {
        field.setOrder(order);
    }

    if (mappingSettingsId != null) {
        field.setMapping(fieldDetail.mappingPath, mappingSettingsId);
    }

    return field;
}