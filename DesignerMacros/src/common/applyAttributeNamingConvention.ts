/// <reference path="../../typings/elementmacro.context.api.d.ts" />

/**
 * Applies camelCasing or PascalCasing naming convention according to the setting configured for the current application.
 */
function applyAttributeNamingConvention(str : string) : string {
    let convention = application.getSettings("c4d1e35c-7c0d-4926-afe0-18f17563ce17")
    ?.getField("Attribute Naming Convention")?.value ?? "pascal-case";

    switch (convention) {
        case "pascal-case":
            return toPascalCase(str);
        case "camel-case":
            return toCamelCase(str);
    }

    return str;
}
