/// <reference path="core.context.types.d.ts"/>

/**
 * Returns information about the application and its settings.
 */
declare const application: IApplication;

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

declare const theme: IThemeApi;