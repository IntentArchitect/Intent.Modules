const validOperators = [
    "is equal to",
    "is less than",
    "is greater than",
    "is greater than or equal to",
    "is less than or equal to",
];

function getDisplay(element: MacroApi.Context.IElementApi): string {
    if (element.typeReference.getType().getName() !== "Simple") {
        var childDisplays = element.getChildren(element.specialization)
            .map(e => getDisplay(e))
            .join(", ");

        return `${element.typeReference.getType().getName()} (${childDisplays})`;
    }

    const stereotype = element.getStereotype("Conditional Statement Settings");
    const isComparisonOperator = validOperators.some(x => x === stereotype.getProperty("Operator").value);

    const not = stereotype.getProperty("NOT").value ? "NOT " : "";
    const variable = stereotype.getProperty("Variable").value ?? "";
    const operator  = stereotype.getProperty("Operator").value ?? "";
    const type = operator === "is of type" ? " " + stereotype.getProperty("Type").value ?? "UNKNOWN" : "";
    const valueType = isComparisonOperator ? " " + stereotype.getProperty("Value Type").value ?? "UNKNOWN" : "";
    const value = operator == "matches string" || (isComparisonOperator && !valueType.startsWith("Boolean")) ? " " + stereotype.getProperty("Value").value ?? "UNKNOWN" : "";
    const booleanValue = isComparisonOperator && valueType.startsWith("Boolean") ? " " + stereotype.getProperty("Boolean Value").value ?? "UNKNOWN" : "";

    return `${not}${variable} ${operator}${type}${valueType}${value}${booleanValue}`;
}