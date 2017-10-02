# Roslyn Weaver 

## Roslyn Weaver Modes

The Roslyn Weaver goes through all the [code elements](xref:RoslynWeaver_Appendix#list-of-roslyn-weave-code-elements) in the existing solution file and merges in all code elements from the template output. The way in which the code elements are weaved is determined by the mode specified for the code element. The mode under which a code element is operating is controlled by using either the [IntentManaged](xref:RoslynWeaver_Attributes#intenttemplate-attribute) or [DefaultIntentManaged](xref:RoslynWeaver_Attributes#defaultintentmanaged-attribute) attributes. A code element can be operating under one of three modes as detailed below.

|Mode|Meaning|
|-|-|
|Fully|The code element from the Template Output will be used and the code element from the Solution Code File will be discarded. If the code element exists in the Solution Code File but not in the Template Output the code element will be removed.|
|Ignore|The code element from the Solution Code File will be used and the code element from the Template Output will be discarded.|
|Merge|The code element existing in both the Template Output and the Solution Code File the code elements will be merged to produce a new code element. How the merging occurs is code element specific and can be further controller by specifying a Signature and/or Body mode. If either side does  not have a version of the code element it will be introduced, in this way merge mode is always additive code will typically not be removed by the weaver. |

If a code element is running under *Merge* mode you can further fine tune the weaving by specifying specific modes for the signature and body of the code element. By default the body and signature modes will be the same as the code element node i.e. *Merge*. [Detailed description of what the signature and body of each code element](xref:RoslynWeaver_Appendix#code-element-mode-specifics).
