using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
	/// <summary>
	/// Adds the secrets to the User Secrets file, if the keys don't exist
	/// If the dictionary is empty , will create the secrets file
	/// </summary>
	public class AddUserSecretsEvent
	{
		public AddUserSecretsEvent(IOutputTarget target, Dictionary<string, string> secretsToAdd)
		{
			Target = target;
			SecretsToAdd = secretsToAdd;
		}

		public IOutputTarget Target { get; }
		public Dictionary<string, string> SecretsToAdd { get; }

	}
}
