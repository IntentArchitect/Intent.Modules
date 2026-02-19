/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
// Stereotype IDs for RDBMS Join Table and Table transformations
const JOIN_TABLE_STEREOTYPE_ID = "5679fb86-e403-4dc0-bf25-8446ef2d1d03";
const TABLE_STEREOTYPE_ID = "dd205b32-b48b-4c77-98f5-faefb2c047ce";
function convertIntermediateEntity(association) {
    // Determine source and target ends - source end determines ownership
    const sourceEnd = association.isSourceEnd() ? association : association.getOtherEnd();
    const targetEnd = sourceEnd.getOtherEnd();
    // Get the participating entities
    const sourceEntity = sourceEnd.typeReference.getType();
    const targetEntity = targetEnd.typeReference.getType();
    // Generate link entity name
    const linkEntityName = `${sourceEntity.getName()}${targetEntity.getName()}`;
    // Store original navigation names
    const originalSourceCollectionName = targetEnd.getName();
    const originalTargetCollectionName = sourceEnd.getName();
    // Check if entities are on the diagram
    const diagram = getCurrentDiagram();
    const entitiesOnDiagram = diagram &&
        diagram.isVisual(sourceEntity.id) &&
        diagram.isVisual(targetEntity.id);
    // Only get visuals and calculate position if entities are on diagram
    let sourceVisual = null;
    let targetVisual = null;
    let linkEntityPosition = undefined;
    if (entitiesOnDiagram) {
        sourceVisual = diagram.getVisual(sourceEntity.id);
        targetVisual = diagram.getVisual(targetEntity.id);
        // Calculate position for the new link entity (between source and target)
        const sourceDims = sourceVisual.getDimensions();
        const targetDims = targetVisual.getDimensions();
        linkEntityPosition = {
            x: (sourceDims.getCenter().x + targetDims.getCenter().x) / 2,
            y: (sourceDims.getCenter().y + targetDims.getCenter().y) / 2
        };
    }
    // Check if the association has a Join Table stereotype and capture its name
    let joinTableName = undefined;
    if (association.hasStereotype(JOIN_TABLE_STEREOTYPE_ID)) {
        const joinTableStereotype = association.getStereotype(JOIN_TABLE_STEREOTYPE_ID);
        if (joinTableStereotype.hasProperty("Name")) {
            const nameProperty = joinTableStereotype.getProperty("Name");
            joinTableName = nameProperty.value;
        }
    }
    // Create the Link Entity in the same package as the source entity
    const linkEntity = createElement("Class", linkEntityName, sourceEntity.getPackage().id);
    // Remove the many-to-many association
    association.delete();
    // Create many-to-one from Link to Source (Source is the owner)
    // This creates a source end on Link pointing to Source (composite ownership)
    const linkToSourceAssoc = createAssociation("Association", linkEntity.id, sourceEntity.id);
    const sourceSideEnd = linkToSourceAssoc.getOtherEnd();
    // Configure Link's end pointing to Source (many-to-one: Link * -> Source 1)
    linkToSourceAssoc.typeReference.setIsCollection(false); // Link points to one Source
    linkToSourceAssoc.typeReference.setIsNullable(false); // Required
    linkToSourceAssoc.setName(sourceEntity.getName()); // Navigation property on Link
    // Configure Source's end pointing to Link (one-to-many: Source 1 -> Link *)
    sourceSideEnd.typeReference.setIsCollection(true); // Source has many Links
    sourceSideEnd.typeReference.setIsNullable(false);
    sourceSideEnd.typeReference.setIsNavigable(true);
    sourceSideEnd.setName(originalTargetCollectionName || `${linkEntityName}s`);
    // Create many-to-one from Link to Target (Link * -> 1 Target)
    const linkToTargetAssoc = createAssociation("Association", linkEntity.id, targetEntity.id);
    const targetSideEnd = linkToTargetAssoc.getOtherEnd();
    // Configure Link's end pointing to Target (many-to-one: Link * -> Target 1)
    linkToTargetAssoc.typeReference.setIsCollection(false); // Link points to one Target
    linkToTargetAssoc.typeReference.setIsNullable(false); // Required
    linkToTargetAssoc.setName(targetEntity.getName()); // Navigation property on Link
    // Configure Target's end pointing to Link (one-to-many: Target 1 -> Link *)
    targetSideEnd.typeReference.setIsCollection(true); // Target has many Links
    targetSideEnd.typeReference.setIsNullable(false);
    targetSideEnd.typeReference.setIsNavigable(false);
    targetSideEnd.setName(originalSourceCollectionName || `${linkEntityName}s`);
    // Apply Table stereotype if we captured a Join Table name from the original association
    if (joinTableName) {
        const tableStereotype = linkEntity.addStereotype(TABLE_STEREOTYPE_ID);
        if (tableStereotype.hasProperty("Name")) {
            tableStereotype.getProperty("Name").setValue(joinTableName);
        }
    }
    // Add to diagram only if the original entities were visible
    if (entitiesOnDiagram) {
        diagram.addElement(linkEntity.id, linkEntityPosition);
        diagram.addAssociation(linkToSourceAssoc.id);
        diagram.addAssociation(linkToTargetAssoc.id);
        // Select the new link entity
        selectElement(linkEntity.id);
    }
}
