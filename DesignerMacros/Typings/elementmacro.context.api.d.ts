/// <reference path="core.context.types.d.ts"/>

/**
 * Returns the element that triggered this script's execution.
 */
declare let element: MacroApi.Context.IElementApi;

/**
 * Returns information about the application and it's settings.
 */
declare const application: MacroApi.Context.IApplication;

/**
 * Returns the currently opened and displayed diagram.
 */
declare const currentDiagram: MacroApi.Context.IDiagramApi;

/**
 * Creates an element of specialization type with the specified name, as a child of the specified parent.
 */
declare function createElement(specialization: string, name: string, parentId: string): MacroApi.Context.IElementApi;

/**
 * Creates an association of specialization type with from a sourceElementId and optionally to a targetElementId.
 */
declare function createAssociation(specialization: string, sourceElementId: string, targetElementId?: string): MacroApi.Context.IAssociationApi;

/**
 * Returns the packages currently loaded into the designer.
 */
declare function getPackages(): MacroApi.Context.IPackageApi[];

/**
 * Returns the singular form of the specified word.
 */
declare const dialogService: MacroApi.Context.IDialogService;

/**
 * Finds the element with the specified id across all loaded packages.
 */
declare function lookup(id: string): MacroApi.Context.IElementApi;

/**
 * Finds the elements of the specified type across all loaded packages.
 */
declare function lookupTypesOf(type: string): MacroApi.Context.IElementApi[];

/**
 * Removes specified suffixes from the provided string.
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

declare function toCamelCase(word: string): string;
declare function toPascalCase(word: string): string;
declare function toKebabCase(word: string): string;
declare function toSnakeCase(word: string): string;
