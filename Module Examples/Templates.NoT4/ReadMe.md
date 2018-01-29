# Templates : No T4 Template

Creating templates without using T4 as your templating engine.

## Concepts shown
- Intent Architect is not coupled to T4, any technology can be used to create / manage your templates.

## StringBuilder Template
This example show a bare bones template, not using any T4, it simply generates the output using a StringBuilder class. This template is a project item based template as this is what you would want to do 99% of the time.

## Xml template
This example uses _'System.Xml.Linq.XDocument'_ class to produce and XML file output. This template introduces some code weaving techniques i.e. generate code and manual code living together in a single file.

To see the weaving in action, add or remove settings in the _'Registration.cs'_ or make changes to the out xml file _'Templates\NoT4Template\Xml\Someoutput.xml'_. (You will to generate first)


