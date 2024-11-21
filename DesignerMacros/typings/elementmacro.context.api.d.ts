/// <reference path="core.context.types.d.ts"/>

/**
 * Returns information about the application and its settings.
 */
declare const application: IApplication;

/**
 * Returns the element that triggered this script's execution.
 */
declare let element: IElementApi;

/**
 * Returns the element that triggered this script's execution.
 */
declare let association: IAssociationApi;

/**
 * Obsolete. Use {@link getCurrentDiagram} instead. This would only return the diagram which was open at the time the script execution began.
 */
declare const currentDiagram: IDiagramApi;

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
 * Finds the element with the specified id across all loaded packages.
 */
declare function lookup(id: string): IElementApi;

/**
 * Finds the elements of the specified type(s) across all loaded packages.
 * @param includeReferences Defaults to false if unspecified
 */
declare function lookupTypesOf(type: string | string[], includeReferences?: boolean): IElementApi[];

/**
 * Removes the specified {@link prefixes} from the provided {@link string}.
 */
declare function removePrefix(string: string, ...prefixes: string[]): string;

/**
 * Removes the specified {@link suffixes} from the provided {@link string}.
 * @param string The string from which the suffixes should be removed.
 * @param suffixes An array of suffix strings to remove.
 */
declare function removeSuffix(string: string, ...suffixes: string[]): string;

/**
 * Returns the plural form of the specified word.
 */
declare function pluralize(word: string): string;

/**
 * Returns the singular form of the specified word.
 */
declare function singularize(word: string): string;

/**
 * Converts the provided {@link word} to {@link https://en.wikipedia.org/wiki/Letter_case#Camel_case camelCase}.
 */
declare function toCamelCase(word: string): string;

/**
 * Converts the provided {@link word} to {@link https://en.wikipedia.org/wiki/Letter_case#Camel_case PascalCase}.
 */
declare function toPascalCase(word: string): string;

/**
 * Converts the provided {@link word} to {@link https://en.wikipedia.org/wiki/Letter_case#Kebab_case kebab-case}.
 */
declare function toKebabCase(word: string): string;

/**
 * Converts the provided {@link word} to {@link https://en.wikipedia.org/wiki/Letter_case#Snake_case snake_case}.
 */
declare function toSnakeCase(word: string): string;
/**
 * Converts the provided {@link word} to {@link https://en.wikipedia.org/wiki/Letter_case#Sentence_case snake_case}.
 */
declare function toSentenceCase(word: string): string;
/**
 * Converts the provided {@link word} to {@link https://en.wikipedia.org/wiki/Letter_case#Title_case snake_case}.
 */
declare function toTitleCase(word: string): string;
/**
 * Executes the Module Task with the provided {@link taskTypeId} with the supplied {@link args}.
 * Will asynchronously return a string.
 */
declare function executeModuleTask(taskTypeId: string, ...args: string[]): Promise<string>

/**
 * Present a popup dialog for user feedback or intervention.
 */
declare const dialogService: IDialogService;

/**
 * Logs messages to the log window.
 * JPS & GB: Commented out as conflicts with console declared in lib.dom.d.ts
 */
//declare const console: { debug(message: string): void, log(message: string): void, warn(message: string): void, error(message: string): void };
