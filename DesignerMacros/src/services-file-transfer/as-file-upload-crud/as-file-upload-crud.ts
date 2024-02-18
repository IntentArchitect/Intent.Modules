/// <reference path="../common/common-file-transfer.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/as-file-upload-crud/as-file-upload-crud.ts
 */

function configureUpload(element: MacroApi.Context.IElementApi): void{   
    applyFileTransferStereoType(element);
    addUploadFields(element, "DTO-Field");    
}

configureUpload(element);