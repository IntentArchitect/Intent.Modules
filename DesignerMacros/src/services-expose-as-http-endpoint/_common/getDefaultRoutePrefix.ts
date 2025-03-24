function getDefaultRoutePrefix(includeLastPathSeparator: boolean): string {
    const defaultApiRoutePrefix = "api/";
    const apiSettingsId = "4bd0b4e9-7b53-42a9-bb4a-277abb92a0eb";

    let settingsGroup = application.getSettings(apiSettingsId);
    let route = settingsGroup ? settingsGroup.getField("Default API Route Prefix").value : null;

    // if the group is not present, use the default value
    if(!settingsGroup)
    {
        route = defaultApiRoutePrefix;
    }
    
    // if the route is null (or set to blank in settings, which results in null)
    // set it to blank (the actual value in settings)
    if (!route || route == "") {
        return "";
    }

    if (includeLastPathSeparator && !route.endsWith("/")) {
        route += "/";
    } else if (!includeLastPathSeparator && route.endsWith("/")) {
        route = removeSuffix(route, "/");
    }

    return route;
}
