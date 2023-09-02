/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/getMappedRequestDetails.ts" />
/// <reference path="ensureHasField.ts" />

function ensureHasKeys(options: {
    contract: IElementApi,
    keyFields: IFieldDetails[],
    mappingSettingsId?: string
}): void {
    const { contract, keyFields, mappingSettingsId } = options;

    let order = 0;

    for (const keyField of keyFields) {
        ensureHasField({
            contract: contract,
            fieldDetail: keyField,
            mappingSettingsId: mappingSettingsId,
            order: order++
        });
    }
}