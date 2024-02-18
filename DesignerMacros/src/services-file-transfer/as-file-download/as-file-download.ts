/// <reference path="../common/common-file-transfer.ts" />

/**
 * Used by Intent.Modules\Modules\Intent.Modules.Metadata.WebApi
 *
 * Source code here:
 * https://github.com/IntentArchitect/Intent.Modules/blob/master/DesignerMacros/src/as-file-download/as-file-download.ts
 */

function configureDownload(element: MacroApi.Context.IElementApi): void{   

    applyFileTransferStereoType(element);    
    makeReturnTypeFileDownloadDto(element);
}

configureDownload(element);