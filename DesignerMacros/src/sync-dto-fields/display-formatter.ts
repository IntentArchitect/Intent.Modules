/// <reference path="types.ts" />
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../typings/core.context.types.d.ts" />

function formatDiscrepancy(discrepancy: IFieldDiscrepancy): MacroApi.Context.IDisplayTextComponent[] {
    const components: MacroApi.Context.IDisplayTextComponent[] = [];
    
    // Status badge with color and icon
    const statusInfo = getDiscrepancyStatusInfo(discrepancy.type);
    components.push({
        text: `[${discrepancy.type}]`,
        cssClass: `text-highlight ${statusInfo.cssClass}`,
        color: statusInfo.color
    });
    
    components.push({
        text: " ",
        cssClass: ""
    });
    
    // DTO field name
    components.push({
        text: discrepancy.dtoFieldName,
        cssClass: "text-highlight keyword"
    });
    
    // Separator and arrow
    components.push({
        text: " → ",
        cssClass: "text-highlight muted"
    });
    
    // Entity attribute name
    components.push({
        text: discrepancy.entityAttributeName,
        cssClass: "text-highlight typeref"
    });
    
    // Type information if applicable
    if (discrepancy.dtoFieldType || discrepancy.entityAttributeType) {
        components.push({
            text: " ",
            cssClass: ""
        });
        
        if (discrepancy.type === "TYPE_CHANGED") {
            components.push({
                text: `(${discrepancy.dtoFieldType} → ${discrepancy.entityAttributeType})`,
                cssClass: "text-highlight annotation"
            });
        } else if (discrepancy.dtoFieldType) {
            components.push({
                text: `(${discrepancy.dtoFieldType})`,
                cssClass: "text-highlight muted"
            });
        }
    }
    
    // Reason if provided
    if (discrepancy.reason) {
        components.push({
            text: " - ",
            cssClass: "text-highlight muted"
        });
        
        components.push({
            text: discrepancy.reason,
            cssClass: "text-highlight muted"
        });
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
