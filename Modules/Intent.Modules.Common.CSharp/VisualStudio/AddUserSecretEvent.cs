using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
	public class AddUserSecretEvent
	{
		public AddUserSecretEvent(IOutputTarget target, string secretKey, string secretValue)
		{
			Target = target;
			SecretKey = secretKey;
			SecretValue = secretValue;
		}

		public IOutputTarget Target { get; }
		public string SecretKey { get; }
		public string SecretValue { get; }

	}
}
