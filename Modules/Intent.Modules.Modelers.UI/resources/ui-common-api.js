/// <reference path="../../typings/elementmacro.context.api.d.ts" />
let BackendServiceHelperApi = {
    setupCallServiceFromComponent,
    remapServiceCall,
};
function setupCallServiceFromComponent(component, serviceRequest) {
    let helper = new BackendServiceHelper();
    helper.callService(component, serviceRequest);
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
    helper.callService(component, serviceRequest, association, existingOperation, existingProperty);
}
class BackendServiceHelper {
    callService(component, serviceRequest, existingCallServiceAssociation, existingOperation, existingProperty) {
        var _a;
        const diagram = getCurrentDiagram();
        const componentVisual = diagram.getVisual(component.id);
        const space = diagram.findEmptySpace({ x: componentVisual.getDimensions().right + 250, y: componentVisual.getDimensions().getCenter().y }, { width: 200, height: 100 });
        if (serviceRequest.specialization == "Query") {
            const query = serviceRequest;
            diagram.layoutVisuals([query.id], space, false);
            const operation = existingOperation !== null && existingOperation !== void 0 ? existingOperation : createElement("Component Operation", "Load" + removePrefix(removeSuffix(query.getName(), "Query"), "Get"), component.id);
            query.getChildren("DTO-Field").forEach(param => {
                const parameter = operation.addChild("Parameter", toCamelCase(param.getName()));
                parameter.typeReference.setType(param.typeReference.toModel());
            });
            const property = existingProperty !== null && existingProperty !== void 0 ? existingProperty : component.addChild("Property", "Model");
            property.typeReference.setType(query.typeReference.toModel());
            property.typeReference.setIsNullable(true);
            const callService = existingCallServiceAssociation !== null && existingCallServiceAssociation !== void 0 ? existingCallServiceAssociation : createAssociation("Call Service Operation Action", operation.id, query.id);
            diagram.layoutVisuals([callService.id]);
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
            diagram.layoutVisuals([command.id], space, false);
            const operation = existingOperation !== null && existingOperation !== void 0 ? existingOperation : createElement("Component Operation", removeSuffix(command.getName(), "Command"), component.id);
            const callService = existingCallServiceAssociation !== null && existingCallServiceAssociation !== void 0 ? existingCallServiceAssociation : createAssociation("Call Service Operation Action", operation.id, command.id);
            diagram.layoutVisuals([callService.id]);
            const mapping = callService.createAdvancedMapping(operation.id, command.id, "e4a4111b-cf13-4efe-8a5d-fea9457ac8ad");
            mapping.addMappedEnd("ab447316-1252-49bc-a695-f34cb00a3cdc", [operation.id, callService.id], [command.id]);
            this.addViewModel(command, component, mapping, ["Command", "Create", "Update"]);
            // if (command.typeReference.isTypeFound())  {
            //     const property = component.addChild("Property", "Model");
            //     property.typeReference.setType(command.typeReference.toModel());
            //     const returnMapping = callService.createAdvancedMapping(operation.id, command.id, "e60890c6-7ce7-47be-a0da-dff82b8adc02");
            //     returnMapping.addMappedEnd("9cccf6be-31be-4ac0-a026-ed0c4f5578bf", [operation.id, callService.id, "2f68b1a4-a523-4987-b3da-f35e6e8e146b"], [property.id]);
            // }
            mapping.launchDialog();
        }
        else if (serviceRequest.specialization == "Operation") {
            const serviceOp = serviceRequest; // Service Operation
            diagram.layoutVisuals([serviceOp.getParent().id], space, false);
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
                    const property = existingProperty !== null && existingProperty !== void 0 ? existingProperty : component.addChild("Property", "Model");
                    property.typeReference.setType(serviceOp.typeReference.toModel());
                    property.typeReference.setIsNullable(true);
                    const returnMapping = callService.createAdvancedMapping(operation.id, serviceOp.id, "e60890c6-7ce7-47be-a0da-dff82b8adc02");
                    returnMapping.addMappedEnd("9cccf6be-31be-4ac0-a026-ed0c4f5578bf", [operation.id, callService.id, "2f68b1a4-a523-4987-b3da-f35e6e8e146b"], [property.id]);
                }
            }
            else {
                const dtoParam = serviceOp.getChild(x => x.specialization == "Parameter" && x.typeReference.getType().specialization == "DTO");
                const dto = dtoParam === null || dtoParam === void 0 ? void 0 : dtoParam.typeReference.getType();
                this.addViewModel(dto, component, mapping, ["DTO", "Dto"]);
                mapping.launchDialog();
            }
        }
    }
    addViewModel(contract, component, mapping, suffixesToRemove) {
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
            this.addChildElementsRecursivly(modelDefinition, contract, component, mapping, [property.id], [contract.id]);
        }
    }
    addChildElementsRecursivly(model, contract, component, mapping, srcPath, dstPath) {
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
                this.addChildElementsRecursivly(complexChildType, field.typeReference.getType(), component, mapping, nextSrcPath, nextDstPath);
            }
            else {
                modelProperty.typeReference.setType(field.typeReference.toModel());
                mapping.addMappedEnd("ce70308a-e29d-4644-8410-f9e6bbd214fc", nextSrcPath, nextDstPath);
            }
        });
    }
}
