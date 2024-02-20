/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />

function makeReturnTypeFileDownloadDto(element: MacroApi.Context.IElementApi): void{   

    const commonTypes = {
        string: "d384db9c-a279-45e1-801e-e4e8099625f2",
        stream: "fd4ead8e-92e9-47c2-97a6-81d898525ea0"
    };

    let returnResultType = lookupTypesOf("DTO").find(x => x.getName() == "FileDownloadDto");
    if (!returnResultType){
        let folderName = "Common";
        
        const folder = element.getPackage().getChildren("Folder").find(x => x.getName() == folderName) ?? createElement("Folder", folderName, element.getPackage().id);

        returnResultType = createElement("DTO", "FileDownloadDto", folder.id);
        returnResultType.id

        let stream = createElement("DTO-Field", "Content", returnResultType.id);
        stream.typeReference.setType(commonTypes.stream)

        let filename = createElement("DTO-Field", "Filename", returnResultType.id);
        filename.typeReference.setType(commonTypes.string)
        filename.typeReference.setIsNullable(true)

        let contentType = createElement("DTO-Field", "ContentType", returnResultType.id);
        contentType.typeReference.setType(commonTypes.string)
        contentType.typeReference.setIsNullable(true)
    }
    element.typeReference.setType(returnResultType.id);
    element.typeReference.setIsCollection(false);
    element.typeReference.setIsNullable(false);
}

function applyFileTransferStereoType(element: MacroApi.Context.IElementApi): void{   
    const fileTransferId = "d30e48e8-389e-4b70-84fd-e3bac44cfe19";
    element.getStereotype(fileTransferId) ?? element.addStereotype(fileTransferId);
}

function makePost(element: MacroApi.Context.IElementApi){
    const httpSettingsId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";
    const httpSettings = element.getStereotype(httpSettingsId) ?? element.addStereotype(httpSettingsId);
    httpSettings.getProperty("Verb").setValue("POST");
}

function addUploadFields(element: MacroApi.Context.IElementApi, childType: string): void{   
    const commonTypes = {
        string: "d384db9c-a279-45e1-801e-e4e8099625f2",
        long: "33013006-E404-48C2-AC46-24EF5A5774FD",
        stream: "fd4ead8e-92e9-47c2-97a6-81d898525ea0"
    };

    const parameterSettingId = "d01df110-1208-4af8-a913-92a49d219552";
    
    var existing = element.getChildren().find(x => x.getName() == "Content")
    if (!existing){    
        let stream = createElement(childType, "Content", element.id);
        stream.typeReference.setType(commonTypes.stream)
    }

    var existing = element.getChildren().find(x => x.getName() == "Filename")
    if (!existing){    
        let filename = createElement(childType, "Filename", element.id);
        filename.typeReference.setType(commonTypes.string)
        filename.typeReference.setIsNullable(true)
    }

    var existing = element.getChildren().find(x => x.getName() == "ContentType")
    if (!existing){    
        let contentType = createElement(childType, "ContentType", element.id);
        contentType.typeReference.setType(commonTypes.string)
        contentType.typeReference.setIsNullable(true)
        let parameterSetting = contentType.addStereotype(parameterSettingId);
        parameterSetting.getProperty("Source").setValue("From Header");
        parameterSetting.getProperty("Header Name").setValue("Content-Type");
    }
    var existing = element.getChildren().find(x => x.getName() == "ContentLength")
    if (!existing){    
        let contentType = createElement(childType, "ContentLength", element.id);
        contentType.typeReference.setType(commonTypes.long)
        contentType.typeReference.setIsNullable(true)
        let parameterSetting = contentType.addStereotype(parameterSettingId);
        parameterSetting.getProperty("Source").setValue("From Header");
        parameterSetting.getProperty("Header Name").setValue("Content-Length");
    }
}