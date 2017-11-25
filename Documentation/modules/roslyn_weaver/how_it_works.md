# Roslyn Weaver 

## How it works

The Roslyn Weaver is an Intent Architect extension, more specifically a Factory Extension. The Roslyn Weaver works by modifying the standard Intent Architect process.

![Simplified Intent Architect Process](images/simplified_ia_process.png)
*Simplified Intent Architect execution process*

Roslyn weavers intercepts the template output, finds the existing solution file ( if present) and weaves them together, typically preserving the *non-generated* code and merging in the generated template output.

![Simplified Roslyn Weave Intent Architect Process](images/simplified_roslyn_weave_ia_process.png) 
*Roslyn Weaver's modified Intent Architect execution process*

As you can imagine the process of merging the code is complex, having said that developers do it all the time. Roslyn Weaver merges the code by allowing developers to annotate the code with attributes which dictate how the merging should be performed. These attributes are typically used in one of two ways:-
- By template designers to design extend points in their templates, i.e. the "put your code here" scenario. [Roslyn Weaver for Template Designers](how_to_implement.md)
- By developers consuming the templates to extend or deviate from the templatized pattern. [Roslyn Weaver for Template Consumers](how_to_work_with.md)




