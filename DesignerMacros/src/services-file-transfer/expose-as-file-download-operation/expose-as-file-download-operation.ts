/// <reference path="../common/common-file-transfer.ts" />
/// <reference path="../../services-expose-as-http-endpoint/services/operation-expose-as-http-endpoint.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/expose-as-file-download-operation/expose-as-file-download-operation.ts
 */

function configureDownload(element: MacroApi.Context.IElementApi): void{   

    applyFileTransferStereoType(element);    
    makeReturnTypeFileDownloadDto(element);
    exposeOperationAsHttpEndPoint(element);
}