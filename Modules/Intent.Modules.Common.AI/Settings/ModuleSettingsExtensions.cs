using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Common.AI.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static AISettings GetAISettings(this IApplicationSettingsProvider settings)
        {
            return new AISettings(settings.GetGroup("62594c9b-21fe-4c65-ac5c-b32a836a2ca5"));
        }
    }

    public class AISettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public AISettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public string Id => _groupSettings.Id;

        public string Title
        {
            get => _groupSettings.Title;
            set => _groupSettings.Title = value;
        }

        public ISetting GetSetting(string settingId)
        {
            return _groupSettings.GetSetting(settingId);
        }
        public ProviderOptions Provider() => new ProviderOptions(_groupSettings.GetSetting("41ac1bce-6362-4b77-b588-ba3df55a2afb")?.Value);

        public class ProviderOptions
        {
            public readonly string Value;

            public ProviderOptions(string value)
            {
                Value = value;
            }

            public ProviderOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "open-ai" => ProviderOptionsEnum.OpenAi,
                    "azure-open-ai" => ProviderOptionsEnum.AzureOpenAi,
                    "ollama" => ProviderOptionsEnum.Ollama,
                    "anthropic" => ProviderOptionsEnum.Anthropic,
                    "open-router" => ProviderOptionsEnum.OpenRouter,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsOpenAi()
            {
                return Value == "open-ai";
            }

            public bool IsAzureOpenAi()
            {
                return Value == "azure-open-ai";
            }

            public bool IsOllama()
            {
                return Value == "ollama";
            }

            public bool IsAnthropic()
            {
                return Value == "anthropic";
            }

            public bool IsOpenRouter()
            {
                return Value == "open-router";
            }
        }

        public enum ProviderOptionsEnum
        {
            OpenAi,
            AzureOpenAi,
            Ollama,
            Anthropic,
            OpenRouter,
        }

        public string OpenAIAPIKey() => _groupSettings.GetSetting("9e9a32b4-194e-4d53-b62c-c9c28fb7b6f8")?.Value;

        public string AzureOpenAIAPIKey() => _groupSettings.GetSetting("d76d0a4d-60c5-42b4-aa3b-de4f11b44ac1")?.Value;

        public string AnthropicAPIKey() => _groupSettings.GetSetting("715e2ce8-677c-467d-a876-8dc84b99ae05")?.Value;

        public string OpenRouterAPIKey() => _groupSettings.GetSetting("d615e7c5-e3d6-4ee0-a1b2-b671e03b5330")?.Value;

        public string Model() => _groupSettings.GetSetting("effcfd41-aaed-4278-a7e7-818009584139")?.Value;

        public string APIUrl() => _groupSettings.GetSetting("5d2a1254-9f21-4cbd-818e-497bf09c87ea")?.Value;

        public string DeploymentName() => _groupSettings.GetSetting("07a3313b-0602-4f83-a1c1-820e746bde48")?.Value;

        public string MaxTokens() => _groupSettings.GetSetting("4cb52ca6-af4f-4dbf-9f75-5a74438cd281")?.Value;
    }
}