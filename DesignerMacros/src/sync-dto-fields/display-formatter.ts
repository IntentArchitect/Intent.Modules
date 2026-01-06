/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

function formatDiscrepancy(discrepancy: IFieldDiscrepancy): MacroApi.Context.IDisplayTextComponent[] {
    const components: MacroApi.Context.IDisplayTextComponent[] = [];
    const statusInfo = getDiscrepancyStatusInfo(discrepancy.type);
    
    switch (discrepancy.type) {
        case "NEW":
            // EntityName: type [NEW]
            components.push({ text: discrepancy.entityAttributeName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.entityAttributeType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: "[NEW]", color: statusInfo.color });
            break;
            
        case "RENAMED":
            // CurrentName → EntityName: type [RENAMED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            components.push({ text: discrepancy.entityAttributeName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.entityAttributeType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: "[RENAMED]", color: statusInfo.color });
            break;
            
        case "TYPE_CHANGED":
            // FieldName: currentType → entityType [TYPE CHANGED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.dtoFieldType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            components.push({ text: discrepancy.entityAttributeType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: "[TYPE CHANGED]", color: statusInfo.color });
            break;
            
        case "DELETED":
            // FieldName: type [DELETED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.dtoFieldType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: "[DELETED]", color: statusInfo.color });
            break;
    }
    
    return components;
}

function getDiscrepancyStatusInfo(type: "NEW" | "DELETED" | "RENAMED" | "TYPE_CHANGED"): { color: string; cssClass: string } {
    switch (type) {
        case "NEW":
            return { color: "#22c55e", cssClass: "keyword" }; // Green
        case "DELETED":
            return { color: "#ef4444", cssClass: "typeref" }; // Red
        case "RENAMED":
            return { color: "#007777", cssClass: "annotation" }; // Teal
        case "TYPE_CHANGED":
            return { color: "#f97316", cssClass: "muted" }; // Orange
        default:
            return { color: "#6b7280", cssClass: "" }; // Gray
    }
}

function createDiscrepancyDisplayFunction(discrepancy: IFieldDiscrepancy): (context: any) => MacroApi.Context.IDisplayTextComponent[] {
    return (context: any) => formatDiscrepancy(discrepancy);
}
