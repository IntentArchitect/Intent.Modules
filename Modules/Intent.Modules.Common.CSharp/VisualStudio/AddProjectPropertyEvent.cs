using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
	/// <summary>
	/// Adds a CSProject Property if it is not already there
	/// </summary>
	public class AddProjectPropertyEvent
	{
		public IOutputTarget Target { get; }
		public string PropertyName { get; }
		public string PropertyValue { get; }

		public AddProjectPropertyEvent(IOutputTarget target, string propertyName, string propertyValue)
		{
			Target = target;
			PropertyName = propertyName;
			PropertyValue = propertyValue;
		}
	}
}
