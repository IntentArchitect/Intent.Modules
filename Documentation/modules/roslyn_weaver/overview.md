# Roslyn Weaver 

## Overview
Roslyn Weaver allows developers to change or extend generated code files with their own custom code. Templates designed to work with this extension are far more powerful than traditional code generation, they allow *hand written* code to be introduced into the generated code. These template still maintaining the ability to continue changing and executing without losing the *hand written* code. In summary it allows generated code and custom written code to live symbiotically in a single file.

The primary driver behind Roslyn Weaver is to overcome one of the biggest limitations of traditional code generation systems which is that they are typically '*all or nothing*'. That is to to say you have to use the generated code as is for all scenarios and there is a very limit scope for extensibility. Any extensibility is typically done through inheritance or partial classes. While these traditional methods are available and may be great to use for some scenarios Roslyn Weaver introduces a wide new range of possibilities.

Due to the Roslyn Weaver's ability to integrate *non-generated* with generated code there are many new possibilities for extensibility beyond those of traditional code generation techniques. Some extensibility techniques may include, (but not limited to)
- Creating custom code extension points in your templates
- Roslyn Weaver allows explicit deviation from the template, by the developer, for exceptional scenarios
- Template outputs are more open to change by the developer

Some of the design goals behind Roslyn Weaver
- Feel and look like *normal* code, generated code is often easy to spot and terrible to read or maintain, Roslyn Weaver wants to be part of your dev team.
- Respect the developers code, try to maintain formatting and white space as much as possible. 
- Try to be unintrusive, code should look and fell like code you would traditionally hand write. 

[Terminology](terminology.md)

[How Roslyn Weaver works](how_it_works.md)

[Roslyn Weaver for Template Designers](how_to_implement.md)

[Roslyn Weaver for Template Consumers](how_to_work_with.md)

[Roslyn Weaver Attributes](attributes.md)

[Code Migrations](code_migrations.md)

[Gotchas](gotchas.md)

[Appendix](appendix.md)




 
