/**
 * Will run code against a package as needed based on the current (which is stored in metadata on the package) and a target version.
 * 
 * If the package is already at the {@link targetVersion}, {@link action} is not executed. After execution of {@link targetVersion} is recorded
 * in metadata on the package under the {@link metadataKeyName} key.
 * 
 * @param package The package to ensure is at the latest version and on which the version will be stored as metadata.
 * @param metadataKeyName The name of the metadata key in which to store the version.
 * @param targetVersion The version that should be saved in the metadata this method is complete.
 * @param action A delegate which is called if the current version in the metadata is unspecified or less than the {@link targetVersion}.
 * The delegate is passed the current version as specified in the metadata, or if there is no version yet it is passed -1.
 */
function ensureIsAtTargetVersion(package: MacroApi.Context.IPackageApi, metadataKeyName: string, targetVersion: number, action: (currentVersion: number) => void): void {
    if (!Number.isInteger(targetVersion)) {
        throw new Error("targetVersion must be an integer");
    }

    var currentVersion = package.hasMetadata(metadataKeyName) &&
                         Number.isInteger(Number.parseInt(package.getMetadata(metadataKeyName)))
        ? Number.parseInt(package.getMetadata(metadataKeyName))
        : -1;

    if (currentVersion >= targetVersion) {
        return;
    }

    action(currentVersion);

    package.removeMetadata(metadataKeyName);
    package.addMetadata(metadataKeyName, targetVersion.toString());
}