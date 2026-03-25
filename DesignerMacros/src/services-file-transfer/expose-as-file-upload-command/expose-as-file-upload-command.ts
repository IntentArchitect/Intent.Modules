/// <reference path="../common/common-file-transfer.ts" />
/// <reference path="../../services-expose-as-http-endpoint/cqrs/command-expose-as-http-endpoint.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/expose-as-file-upload-command/expose-as-file-upload-command.ts
 */

function configureUpload(element: MacroApi.Context.IElementApi): void{   
    applyFileTransferStereoType(element);
    addUploadFields(element, "DTO-Field"); 
    exposeCommandAsHttpEndPoint(element);   
    makePost(element);
}