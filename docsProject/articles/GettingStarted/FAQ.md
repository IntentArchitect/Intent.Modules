# Frequently Asked Questions


## If I use Intent Architect am I locked into your product?

**No**, this is one of our fundamental design principles. Intent Architect is build to be a design time tool, just like your favourite IDE. There is no coupling between it and your application or production environments. This means you can stop using Intent Architect at any point with zero impact to your applications. 

This is different from most similar tools which would have some form of lock-in, examples of such lock-ins may include:-
- A runtime your code must execute in
- Proprietary framework dependencies 
- Unmaintainable code generation, code generation which is unwieldy or poorly implemented such that its is obviously not what a developer would have coded.

## Does this tool replace traditional software development?

**No**, it complements it. Intent Architect is build by developers, for developers, we are in no way trying to diminish the development experience. If anything the tool frees developers up from menial tasks of implementing boilerplate coding to focus on application design and business problems. 

## Does my solution/application need to be Microsoft .Net based?

No, Intent Architect itself is built in .NET Technologies, but the code it generates can be whatever you like.

We have coded several Modules to improve the process for .NET, for example:-
- Visual Studio Integration
- Nuget Integration
- Roslyn Weaver, for advanced code weaving scenarios

These Modules are written outside of Intent Architect as plugins and similar can be produced for other development environments, if you are interested in doing this or need some helping doing so, please contact us. 

## How do I create my own patterns / modules?

**Insert Link to Create Module tutorial**

## Do I need to use T4 templates for my patterns?

There is no requirement to use T4 templates. We typically use T4 templates and our SDK supplies T4 base classes for templates, but you can use  any other technology you like, you simply need to implement the *ITemplate* interface.

## Isn't code generation fundamentally evil?

I must admit I was initially very surprised to find people who had negative views on code generation, I personally have been exposed to various code generation techniques over the past 15 years and have generally had positive experiences. My view is that it a tool, and just like any tool you need to know where it is appropriate to use and how to get the best use out of the tool. 

From the people I have spoken to and my own experience code generation becomes problematic in the following scenarios:-
- **Prescriptive code generated**, in these scenarios you have no control over the code being generated, it's a take it or leave it scenario.
- **The code being generated is bad**, bad code is bad code, whether it is generate or not. Many code generation tools are very generic resulting in poor code, it's obvious to spot the code wasn't hand written.
- **Once off code generation**, this scenario isn't necessarily inherently bad, were it can be problematic is if you generate a lot of code which you later on realise you want to / need to refactor. It's not obvious that code generation is the problem here as you could have hand coded it all to find yourself in the same position, some people will argue manually coding it might have forced you to rethink your implementation.  
- **All or nothing code generation**, here it is difficult to deal with exceptional scenarios, which happen.

Intent Architect we have taken these learnings and tried to create a seamless code generation experience. Some of the factors we believe set us apart include:-
- **You control the content.** The code that is generated is completely within your control.
- **Continuous code generation.** At any point you can run the code generation and your application will update to reflect your design accordingly, even in existing code files. 
- **Managed generated outputs.** Most code generation tools work in a purely additive fashion i.e. only adding to or overwriting files in your code base, Intent Architect tracks the generated outputs and removes generated outputs which are no longer relevant.  
- **Code Weaving.** We support advanced weaving systems which allow for manual code to be introduced into generated code files, and for that code to be maintained on subsequent generations of the code.  
- **Code as you would write it.** This is really up to module implementors but in principle, and from our experience, there is no reason for the generated code to look any different to hand written code.
- **IDE integration.** The generated code is integrated into your IDE, no need to manually add or removed generated code files in your IDE. (We currently have an implementation for Visual Studio, but Intent Architect is extensible and there is no reason it can not be integrated into your IDE of choice)

Code generation is simply a tool, the benefit you get out of it is up to you. In the world of code generation tools, Intent Architect is a power tool, giving you new and better ways to solve your software problems.





