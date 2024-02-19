/// <reference path="../common/common-file-transfer.ts" />
/// <reference path="../../services-expose-as-http-endpoint/operation/shared.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/expose-as-file-upload-operation/expose-as-file-upload-operation.ts
 */

function configureUpload(element: MacroApi.Context.IElementApi): void{   
    applyFileTransferStereoType(element);
    addUploadFields(element, "Parameter");    
    exposeAsHttpEndPoint(element);
    makePost(element);
}

configureUpload(element);