# Roslyn Weaver 

## Terminology

### Terms
|Term|Description|
|-|-|
|Roslyn Weaver|Intent Architect plugin designed to merge existing code (Solution Code) with generated code (Template Output) to provide a more powerful and interactive code generation experience.|
|Code Element|The refers to a code fragment, e.g a method or property, within a code file which can be targeted for code weaving. [List of Code Elements](xref:RoslynWeaver_Appendix#list-of-roslyn-weave-code-elements)|
|Mode|This describes the strategy the Roslyn Weaver will use for weaving a specific code element when it is processing it.|
|Factory Extension|This is a plugin type supported by Intent Architect. This plugin type allows for the extension of the code generation process. Roslyn Weaver is an example of such a plugin, it specifically modifies the code being generated, i.e. after template execution but before persistence of the solution file.|
|Solution Code File|Refers to a code file in a solution. These are the actual code files you would compile to produce your software product. The outputs from Intent Architect, i.e. the generated code, are typically solution code files. Within the context of the Roslyn Weaver it is important to note that these are also inputs, as the previous Intent Architects executions outputs become inputs to the following execution.|
|Template Output|Refers the generated code produced by Intent Architect. The Roslyn Weaver will weave this output with it's corresponding Solution Code Files to produce a new versions of the Solution Code Files. |
|Weaving|The process of taking a 2 pieces of code and merging them together to form a single piece of code.|

