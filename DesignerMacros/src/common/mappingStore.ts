/**
 * Helper class to build up source and target mapping paths for
 * advanced mapping scenarios.
 * 
 * @remarks
 * 
 * Source Path and Target Path is maintained separately since not all mapping scenarios are
 * straightforward.
 * 
 * @example
 * 
 * When to Push/Pop the Paths
 * 
    let leftField = createField(...);

    mappingStore.pushSourcePath(leftField.id);
    mappingStore.pushTargetPath(rightField.id);

    let leftFieldDto = replicateDto(rightField.typeReference.getType(), folder, mappingStore);

    mappingStore.popSourcePath();
    mappingStore.popTargetPath();

    leftField.typeReference.setType(leftFieldDto.id);
 * 
 * Adding mappings
 * 
    function replicateDto(existingDto: MacroApi.Context.IElementApi, ...) {
        let newDto = createElement("DTO", existingDto.getName(), folder.id);
        existingDto.getChildren("DTO-Field").forEach(existingField => {
            let newField = createElement("DTO-Field", existingField.getName(), newDto.id);
            // ...
            mappingStore.addMapping(newField.id, existingField.id);
            // ...
        }
    }
 */
class MappingStore {
    private mappings: IMappingEntry[] = [];
    private sourcePath: string[] = [];
    private targetPath: string[] = [];

    /** 
     * Get all the recorded mapping entries
     */
    getMappings(): IMappingEntry[] {
        return this.mappings;
    }

    /**
     * Keep track of this element id on the source end
     * when navigating inside it's type hierarchy.
     */
    pushSourcePath(id: string): void {
        this.sourcePath.push(id);
    }

    /**
     * Remove the last tracked element on the source path stack
     * when done navigating down its type hierarchy.
     */
    popSourcePath(): void {
        this.sourcePath.pop();
    }

    /**
     * Keep track of this element id on the target end
     * when navigating inside it's type hierarchy.
     */
    pushTargetPath(id: string): void {
        this.targetPath.push(id);
    }

    /**
     * Remove the last tracked element on the target path stack
     * when done navigating down its type hierarchy.
     */
    popTargetPath(): void {
        this.targetPath.pop();
    }

    /**
     * Record a mapping between a source element id and target element id.
     * Target and Source path stack will be used to build up the whole path.
     */
    addMapping(sourceId: string, targetId: string): void {
        this.mappings.push({
            sourcePath: this.sourcePath.concat([sourceId]),
            targetPath: this.targetPath.concat([targetId])
        });
    }
}

interface IMappingEntry {
    sourcePath: string[];
    targetPath: string[];
}