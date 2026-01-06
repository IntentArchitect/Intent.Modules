/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

// Discrepancy status colors
const DISCREPANCY_COLORS = {
    NEW: "#22c55e",      // Green
    DELETED: "#ef4444",  // Red
    RENAMED: "#007777",  // Teal
    TYPE_CHANGED: "#f97316"  // Orange
} as const;

const DISCREPANCY_LABELS = {
    NEW: "[NEW]",
    DELETED: "[DELETED]",
    RENAMED: "[RENAMED]",
    TYPE_CHANGED: "[TYPE CHANGED]"
} as const;

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
            components.push({ text: DISCREPANCY_LABELS.NEW, color: statusInfo.color });
            break;
            
        case "RENAMED":
            // CurrentName → EntityName: type [RENAMED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            components.push({ text: discrepancy.entityAttributeName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.entityAttributeType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.RENAMED, color: statusInfo.color });
            break;
            
        case "TYPE_CHANGED":
            // FieldName: currentType → entityType [TYPE CHANGED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.dtoFieldType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " → ", cssClass: "text-highlight muted" });
            components.push({ text: discrepancy.entityAttributeType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.TYPE_CHANGED, color: statusInfo.color });
            break;
            
        case "DELETED":
            // FieldName: type [DELETED]
            components.push({ text: discrepancy.dtoFieldName, cssClass: "text-highlight" });
            components.push({ text: ": ", cssClass: "text-highlight annotation" });
            components.push({ text: discrepancy.dtoFieldType || "", cssClass: "text-highlight keyword" });
            components.push({ text: " " });
            components.push({ text: DISCREPANCY_LABELS.DELETED, color: statusInfo.color });
            break;
    }
    
    return components;
}

function getDiscrepancyStatusInfo(type: "NEW" | "DELETED" | "RENAMED" | "TYPE_CHANGED"): { color: string; cssClass: string } {
    switch (type) {
        case "NEW":
            return { color: DISCREPANCY_COLORS.NEW, cssClass: "keyword" };
        case "DELETED":
            return { color: DISCREPANCY_COLORS.DELETED, cssClass: "typeref" };
        case "RENAMED":
            return { color: DISCREPANCY_COLORS.RENAMED, cssClass: "annotation" };
        case "TYPE_CHANGED":
            return { color: DISCREPANCY_COLORS.TYPE_CHANGED, cssClass: "muted" };
        default:
            return { color: "#6b7280", cssClass: "" }; // Gray fallback
    }
}

function createDiscrepancyDisplayFunction(discrepancy: IFieldDiscrepancy): () => MacroApi.Context.IDisplayTextComponent[] {
    return () => formatDiscrepancy(discrepancy);
}
