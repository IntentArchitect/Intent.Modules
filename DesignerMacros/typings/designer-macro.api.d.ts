/// <reference path="core.context.types.d.ts"/>
/// <reference path="designer-common.api.d.ts"/>

/**
 * Obsolete. Use {@link getCurrentDiagram} instead. This would only return the diagram which was open at the time the script execution began.
 */
//declare const currentDiagram: IDiagramApi;

/**
 * Returns the currently opened and displayed diagram.
 */
declare function getCurrentDiagram(): IDiagramApi;

/**
 * Selects an element in the currently visible diagram with the provided {@link elementId}.
 */
declare function selectElement(elementId: string | any): void;

/**
 * Selects elements in the currently visible diagram with the provided {@link elementIds}.
 */
declare function selectElements(elementIds: string[] | any): void;

/**
 * Creates an element of specialization type with the specified name, as a child of the specified parent.
 */
declare function createElement(specialization: string, name: string, parentId: string): IElementApi;

/**
 * Creates an association of specialization type with from a sourceElementId and optionally to a targetElementId.
 */
declare function createAssociation(specialization: string, sourceElementId: string, targetElementId?: string): IAssociationApi;

/**
 * Returns the packages currently loaded into the designer.
 */
declare function getPackages(): IPackageApi[];

/**
 * Executes the Module Task with the provided {@link taskTypeId} with the supplied {@link args}.
 * Will asynchronously return a string.
 */
declare function executeModuleTask(taskTypeId: string, ...args: string[]): Promise<string>

/**
 * Launches a Hosted Module Task with the provided {@link taskTypeId} with the supplied {@link args}.
 * Supply options to 
 */
declare function launchHostedModuleTask(taskTypeId: string, args: string[], options?: { taskName?: string, openWindow?: boolean, attachDebugger?: boolean }): void;

/**
 * Copies the provided {@param text} to the clipboard. A toast confirms that this has been done.
 */
declare function copyToClipboard(text: string): void;

/**
 * Navigates to the designer with the specified {@param designerId}. 
 * Can automatically select an element with {@param options.selectionId} and optionally run {@param options.executeScript} for this element. 
 * {@param options.scriptDependencies} can be provided as an array of script ids to support the provided {@param options.executeScript} script.
 */
declare function navigateToDesigner(designerId: string, options?: { selectionId?: string, executeScript?: string, scriptDependencies?: string[] }): void;

/**
 * Navigates to the designer in the application with the specified {@param designerId} and {@param applicationId}. 
 * Can automatically select an element with {@param options.selectionId} and optionally run {@param options.executeScript} for this element. 
 * {@param options.scriptDependencies} can be provided as an array of script ids to support the provided {@param options.executeScript} script.
 */
declare function navigateToApplicationDesigner(applicationId: string, designerId: string, options?: { selectionId?: string, executeScript?: string, scriptDependencies?: string[] }): void;

/**
 * Checks for unsaved changes and if any are found then presents a prompt to optionally save, not save or cancel.
 * @returns {boolean} Whether the user wants to proceed. Returns false if the dialog was presented and the user pressed cancel, otherwise true.
 */
declare function promptIfUnsavedChangesAsync(): Promise<boolean>;

/**
 * Present a popup dialog for user feedback or intervention.
 */
declare const dialogService: IDialogService;

/**
 * Provides access to local user settings.
 */
declare const userSettings: IUserSettingsAccessor;

/**
 * Logs messages to the log window.
 */
//declare const console: { debug(message: string): void, log(message: string): void, warn(message: string): void, error(message: string): void };
