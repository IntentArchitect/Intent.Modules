function getDefaultRoutePrefix(includeLastPathSeparator: boolean): string {
    const defaultApiRoutePrefix = "api/";
    const apiSettingsId = "4bd0b4e9-7b53-42a9-bb4a-277abb92a0eb";

    let settingsGroup = application.getSettings(apiSettingsId);
    if (!settingsGroup) { return defaultApiRoutePrefix; }
    
    let field = settingsGroup.getField("Default API Route Prefix");
    if (!field) { return defaultApiRoutePrefix; }
    
    let route = field.value ?? "";

    if (includeLastPathSeparator && !route.endsWith("/")) {
        route += "/";
    } else if (!includeLastPathSeparator && route.endsWith("/")) {
        route = removeSuffix(route, "/");
    }

    return route;
}
