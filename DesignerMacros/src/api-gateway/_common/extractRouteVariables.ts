function extractRouteVariables(routePath: string): string[] {
    // Match any text between curly braces
    const variablePattern = /\{([^}]+)\}/g;
    const variables: string[] = [];
    
    // Find all matches in the route path
    let match: RegExpExecArray | null;
    while ((match = variablePattern.exec(routePath)) !== null) {
        variables.push(match[1]);
    }
    
    return variables;
}