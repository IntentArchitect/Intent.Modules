function executeClassOnCreated(classElement: MacroApi.Context.IElementApi): void {
    if (application?.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")?.getField("Key Creation Mode")?.value != "explicit" ||
        !classElement.getPackage().hasStereotype(relationalDatabaseId)
    ) {
        return;
    }

    updatePrimaryKeys(classElement);
}