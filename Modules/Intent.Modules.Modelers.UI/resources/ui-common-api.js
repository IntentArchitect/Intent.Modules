/// <reference path="../../typings/elementmacro.context.api.d.ts" />
let BackendServiceHelperApi = {
    setupCallServiceFromComponent,
    remapServiceCall,
};
function setupCallServiceFromComponent(component, serviceRequest, presentMappingDialogs) {
    let helper = new BackendServiceHelper();
    helper.callService(component, serviceRequest, presentMappingDialogs);
}
function remapServiceCall(association) {
    var _a, _b;
    const component = association.getOtherEnd().typeReference.getType().getParent();
    const serviceRequest = association.typeReference.getType();
    const existingOperation = association.getOtherEnd().typeReference.getType();
    existingOperation.getChildren("Parameter").forEach(x => x.delete());
    const modelProperty = component.getChildren("Property").find(x => x.typeReference.getType().specialization == "Model Definition" && x.getName() == "Model");
    if (modelProperty != null) {
        const processType = (type) => {
            // Traverse children recursively
            type.getChildren().forEach(x => {
                const childType = x.typeReference.getType();
                if (childType.specialization === "Model Definition") {
                    processType(childType); // recursive call
                }
            });
            // Delete if it's a Model Definition
            if (type.specialization === "Model Definition") {
                type.delete();
            }
        };
        // Start with the root
        const type = modelProperty.typeReference.getType();
        processType(type);
        modelProperty.delete();
    }
    const existingPropertyId = (_b = (_a = association.getAdvancedMapping("e60890c6-7ce7-47be-a0da-dff82b8adc02")) === null || _a === void 0 ? void 0 : _a.getMappedEnds()[0]) === null || _b === void 0 ? void 0 : _b.getTargetElement().id;
    const existingProperty = lookup(existingPropertyId);
    let helper = new BackendServiceHelper();
    helper.callService(component, serviceRequest, true, association, existingOperation, existingProperty);
}
class BackendServiceHelper {
    callService(component, serviceRequest, presentMappingDialogs, existingCallServiceAssociation, existingOperation, existingProperty) {
        var _a;
        const diagram = getCurrentDiagram();
        const componentVisual = diagram === null || diagram === void 0 ? void 0 : diagram.getVisual(component.id);
        const space = diagram === null || diagram === void 0 ? void 0 : diagram.findEmptySpace({ x: componentVisual.getDimensions().right + 250, y: componentVisual.getDimensions().getCenter().y }, { width: 200, height: 100 });
        if (serviceRequest.specialization == "Query") {
            const query = serviceRequest;
            diagram === null || diagram === void 0 ? void 0 : diagram.layoutVisuals([query.id], space, false);
            let conceptName = removePrefix(removeSuffix(query.getName(), "Query"), "Get");
            const operation = existingOperation !== null && existingOperation !== void 0 ? existingOperation : createElement("Component Operation", "Load" + conceptName, component.id);
            query.getChildren("DTO-Field").forEach(param => {
                const parameter = operation.addChild("Parameter", toCamelCase(param.getName()));
                parameter.typeReference.setType(param.typeReference.toModel());
            });
            const property = existingProperty !== null && existingProperty !== void 0 ? existingProperty : component.addChild("Property", conceptName + "Models");
            property.typeReference.setType(query.typeReference.toModel());
            property.typeReference.setIsNullable(true);
            const callService = existingCallServiceAssociation !== null && existingCallServiceAssociation !== void 0 ? existingCallServiceAssociation : createAssociation("Call Service Operation Action", operation.id, query.id);
            diagram === null || diagram === void 0 ? void 0 : diagram.layoutVisuals([callService.id]);
            const mapping = callService.createAdvancedMapping(operation.id, query.id, "e4a4111b-cf13-4efe-8a5d-fea9457ac8ad");
            mapping.addMappedEnd("ab447316-1252-49bc-a695-f34cb00a3cdc", [operation.id, callService.id], [query.id]);
            query.getChildren("DTO-Field").forEach((param, $index) => {
                mapping.addMappedEnd("ce70308a-e29d-4644-8410-f9e6bbd214fc", [operation.id, operation.getChildren()[$index].id], [serviceRequest.id, param.id]);
            });
            const returnMapping = callService.createAdvancedMapping(operation.id, query.id, "e60890c6-7ce7-47be-a0da-dff82b8adc02");
            returnMapping.addMappedEnd("9cccf6be-31be-4ac0-a026-ed0c4f5578bf", [operation.id, callService.id, "2f68b1a4-a523-4987-b3da-f35e6e8e146b"], [property.id]);
        }
        else if (serviceRequest.specialization == "Command") {
            const command = serviceRequest;
            diagram === null || diagram === void 0 ? void 0 : diagram.layoutVisuals([command.id], space, false);
            const operation = existingOperation !== null && existingOperation !== void 0 ? existingOperation : createElement("Component Operation", removeSuffix(command.getName(), "Command"), component.id);
            const callService = existingCallServiceAssociation !== null && existingCallServiceAssociation !== void 0 ? existingCallServiceAssociation : createAssociation("Call Service Operation Action", operation.id, command.id);
            diagram === null || diagram === void 0 ? void 0 : diagram.layoutVisuals([callService.id]);
            const mapping = callService.createAdvancedMapping(operation.id, command.id, "e4a4111b-cf13-4efe-8a5d-fea9457ac8ad");
            mapping.addMappedEnd("ab447316-1252-49bc-a695-f34cb00a3cdc", [operation.id, callService.id], [command.id]);
            this.addViewModel(command, component, mapping, ["Command", "Create", "Update"]);
            if (presentMappingDialogs) {
                mapping.launchDialog();
            }
        }
        else if (serviceRequest.specialization == "Operation") {
            const serviceOp = serviceRequest; // Service Operation
            diagram === null || diagram === void 0 ? void 0 : diagram.layoutVisuals([serviceOp.getParent().id], space, false);
            const operation = existingOperation !== null && existingOperation !== void 0 ? existingOperation : createElement("Component Operation", serviceOp.getName(), component.id);
            const callService = existingCallServiceAssociation !== null && existingCallServiceAssociation !== void 0 ? existingCallServiceAssociation : createAssociation("Call Service Operation Action", operation.id, serviceOp.id);
            diagram === null || diagram === void 0 ? void 0 : diagram.layoutVisuals([callService.id]);
            const mapping = callService.createAdvancedMapping(operation.id, serviceOp.id, "e4a4111b-cf13-4efe-8a5d-fea9457ac8ad");
            mapping.addMappedEnd("ab447316-1252-49bc-a695-f34cb00a3cdc", [operation.id, callService.id], [serviceOp.id]);
            serviceOp.getChildren("Parameter").filter(x => x.typeReference.getType().specialization != "DTO").forEach(param => {
                const parameter = operation.addChild("Parameter", toCamelCase(param.getName()));
                parameter.typeReference.setType(param.typeReference.toModel());
                mapping.addMappedEnd("ce70308a-e29d-4644-8410-f9e6bbd214fc", [operation.id, parameter.id], [serviceOp.id, param.id]);
            });
            if (((_a = serviceOp.getStereotype("Http Settings")) === null || _a === void 0 ? void 0 : _a.getProperty("Verb").getValue()) == "GET") {
                if (serviceOp.typeReference.getType() != null
                    && (serviceOp.typeReference.getType().specialization != "Type-Definition" || (serviceOp.typeReference.getType().specialization == "Type-Definition") && serviceOp.typeReference.toModel().genericTypeParameters.length > 0)) {
                    let conceptName = removePrefix(serviceOp.getName(), "Find", "Get");
                    const property = existingProperty !== null && existingProperty !== void 0 ? existingProperty : component.addChild("Property", conceptName + "Models");
                    property.typeReference.setType(serviceOp.typeReference.toModel());
                    property.typeReference.setIsNullable(true);
                    const returnMapping = callService.createAdvancedMapping(operation.id, serviceOp.id, "e60890c6-7ce7-47be-a0da-dff82b8adc02");
                    returnMapping.addMappedEnd("9cccf6be-31be-4ac0-a026-ed0c4f5578bf", [operation.id, callService.id, "2f68b1a4-a523-4987-b3da-f35e6e8e146b"], [property.id]);
                }
            }
            else {
                const dtoParam = serviceOp.getChild(x => x.specialization == "Parameter" && x.typeReference.getType().specialization == "DTO");
                const dto = dtoParam === null || dtoParam === void 0 ? void 0 : dtoParam.typeReference.getType();
                this.addViewModel(dto, component, mapping, ["DTO", "Dto"], [serviceOp.id, dtoParam.id]);
                if (presentMappingDialogs) {
                    mapping.launchDialog();
                }
            }
        }
    }
    addViewModel(contract, component, mapping, suffixesToRemove, targetPath) {
        if (targetPath == null) {
            targetPath = [contract.id];
        }
        if (!contract)
            return;
        if (!component.getChildren("Property").some(x => x.typeReference.getType().specialization == "Model Definition" && x.getName() == "Model")) {
            const modelDefinition = component.addChild("Model Definition", removePrefix(removeSuffix(contract.getName(), ...suffixesToRemove)) + "Model");
            const property = component.addChild("Property", "Model");
            property.typeReference.setType(modelDefinition.id);
            if (contract.getName().startsWith("Create")) {
                property.setValue("new()");
            }
            else {
                property.typeReference.setIsNullable(true);
            }
            this.addChildElementsRecursively(modelDefinition, contract, component, mapping, [property.id], targetPath);
        }
    }
    addChildElementsRecursively(model, contract, component, mapping, srcPath, dstPath) {
        contract.getChildren("DTO-Field").forEach(field => {
            var _a, _b;
            const modelProperty = model.addChild("Property", field.getName());
            const nextSrcPath = srcPath.concat(modelProperty.id);
            const nextDstPath = dstPath.concat(field.id);
            if (((_b = (_a = field.typeReference) === null || _a === void 0 ? void 0 : _a.getType()) === null || _b === void 0 ? void 0 : _b.specialization) == "DTO") {
                const complexChildType = component.addChild("Model Definition", removePrefix(removeSuffix(field.typeReference.getType().getName(), "DTO", "Dto")) + "Model");
                modelProperty.typeReference.setType(complexChildType.id);
                modelProperty.typeReference.setIsNullable(field.typeReference.isNullable);
                modelProperty.typeReference.setIsCollection(field.typeReference.isCollection);
                if (field.typeReference.isCollection) {
                    mapping.addMappedEnd("ce70308a-e29d-4644-8410-f9e6bbd214fc", nextSrcPath, nextDstPath);
                }
                this.addChildElementsRecursively(complexChildType, field.typeReference.getType(), component, mapping, nextSrcPath, nextDstPath);
            }
            else {
                modelProperty.typeReference.setType(field.typeReference.toModel());
                if (!field.typeReference.isCollection && this.makeNullableInModel(field)) {
                    //Make Model Definition primitives nullable for better UI binding support
                    modelProperty.typeReference.setIsNullable(true);
                }
                mapping.addMappedEnd("ce70308a-e29d-4644-8410-f9e6bbd214fc", nextSrcPath, nextDstPath);
            }
        });
    }
    makeNullableInModel(field) {
        /*
        Lots of UI controls require nullable bindings to represent "Not select / No selection"
        Switches and Check boxes tend to not support nullable, hence removing the bool
        */
        const primitiveTypeIds = new Set([
            //"e6f92b09-b2c5-4536-8270-a4d9e5bbd930", // bool
            "A4E9102F-C1C8-4902-A417-CA418E1874D2", // byte
            "C1B3A361-B1C6-48C3-B34C-7999B3E071F0", // char
            "1fbaa056-b666-4f25-b8fd-76fe3165acc8", // date
            "a4107c29-7851-4121-9416-cf1236908f1e", // datetime
            "f1ba4df3-a5bc-427e-a591-4f6029f89bd7", // datetimeoffset
            "675c7b84-997a-44e0-82b9-cd724c07c9e6", // decimal
            "24A77F70-5B97-40DD-8F9A-4208AD5F9219", // double
            "341929E9-E3E7-46AA-ACB3-B0438421F4C4", // float
            "6b649125-18ea-48fd-a6ba-0bfff0d8f488", // guid
            "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74", // int
            "33013006-E404-48C2-AC46-24EF5A5774FD", // long
            "2ABF0FD3-CD56-4349-8838-D120ED268245", // short
            "46dbdc6c-aaa7-4d2e-ba1f-81abdb87a888", // TimeSpan
        ]);
        return primitiveTypeIds.has(field.typeReference.typeId);
    }
}
