using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.AI.Settings;
using Intent.Utils;
using Microsoft.SemanticKernel;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Intent.Modules.Common.AI.Tests;

public class IntentSemanticKernelFactoryTests
{
     public IntentSemanticKernelFactoryTests(Xunit.Abstractions.ITestOutputHelper output)
     {
          Logging.SetTracing(new XUnitTraceProvider(output));
     }
     
     [Theory(
          //Skip = "Requires API Token environment variables to be set AND access to AI providers."
          )]
     [InlineData(AISettings.ProviderOptionsEnum.OpenAi, "gpt-4o")]
     [InlineData(AISettings.ProviderOptionsEnum.Anthropic, "claude-3-5-haiku-20241022")]
     [InlineData(AISettings.ProviderOptionsEnum.OpenRouter, "qwen/qwen3-coder:free")] // Requires "Enable free endpoints that may train on inputs" ENABLED
     public async Task AiConnectionWithCalculatorToolTest(AISettings.ProviderOptionsEnum provider, string model)
     {
          // ARRANGE
          var settings = GetUserSettingsProvider(provider, model);
          var factory = new IntentSemanticKernelFactory(settings);
          var kernel = factory.BuildSemanticKernel(builder =>
          {
               builder.Plugins.AddFromType<CalculatorPlugin>();
          });
          
          // ACT
          var function = kernel.CreateFunctionFromPrompt(
               "What is 847,293,156 × 923,847,521? Use the calculator tool and return only the answer (in the form 0,000.0)!",
               new PromptExecutionSettings
               {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
               });
          var result = await function.InvokeAsync(kernel);
          
          // ASSERT
          Assert.NotNull(result);
          Assert.Equal((847_293_156L * 923_847_521L).ToString("N1", CultureInfo.InvariantCulture), result.ToString());
     }

     private class CalculatorPlugin
     {
          [KernelFunction]
          [Description("Multiplies two long numbers together.")]
          public string Multiply([Description("First number to multiply with")] long a, [Description("Second number to multiply with")] long b)
          {
               return (a * b).ToString("N1", CultureInfo.InvariantCulture);
          }
     }
     
     private static IUserSettingsProvider GetUserSettingsProvider(AISettings.ProviderOptionsEnum providerOptionsEnum, string model)
     {
          switch (providerOptionsEnum)
          {
               case AISettings.ProviderOptionsEnum.OpenAi:
                    return GetProviderStubConfig("OPENAI_API_KEY", "open-ai", model, "9e9a32b4-194e-4d53-b62c-c9c28fb7b6f8");
               case AISettings.ProviderOptionsEnum.AzureOpenAi:
                    return GetProviderStubConfig("AZURE_OPENAI_API_KEY", "azure-open-ai", model, "d76d0a4d-60c5-42b4-aa3b-de4f11b44ac1");
               case AISettings.ProviderOptionsEnum.Ollama:
                    return GetProviderStubConfig(null, "ollama", model, null);
               case AISettings.ProviderOptionsEnum.Anthropic:
                    return GetProviderStubConfig("ANTHROPIC_API_KEY", "anthropic", model, "715e2ce8-677c-467d-a876-8dc84b99ae05");
               case AISettings.ProviderOptionsEnum.OpenRouter:
                    return GetProviderStubConfig("OPENROUTER_API_KEY", "open-router", model, "d615e7c5-e3d6-4ee0-a1b2-b671e03b5330");
               default:
                    throw new ArgumentOutOfRangeException(nameof(providerOptionsEnum), providerOptionsEnum, null);
          }
     }

     private static IUserSettingsProvider GetProviderStubConfig(string? apiKeyEnvName, string providerId, string model, string? apiKeySettingId)
     {
          string? apiKey = null;
          if (apiKeyEnvName is not null)
          {
               apiKey = Environment.GetEnvironmentVariable(variable: apiKeyEnvName);
               if (apiKey is null)
               {
                    throw new InvalidOperationException($"{apiKeyEnvName} environment variable is not set.");
               }
          }

          return new TestSettingsProvider(apiKey, model, providerId, apiKeySettingId);
     }

     private class TestSettingsProvider : IUserSettingsProvider
     {
          private readonly Dictionary<string, IGroupSettings> _groups = new();
          public TestSettingsProvider(string? apiKey, string model, string providerId, string? apiKeySettingId)
          {
               var gs = Substitute.For<IGroupSettings>();
               gs.GetSetting("effcfd41-aaed-4278-a7e7-818009584139").Value.Returns(model);
               gs.GetSetting("41ac1bce-6362-4b77-b588-ba3df55a2afb").Value.Returns(providerId);
               gs.GetSetting("4cb52ca6-af4f-4dbf-9f75-5a74438cd281").Value.Returns("8192");
               if (apiKeySettingId is not null)
               {
                    gs.GetSetting(apiKeySettingId).Value.Returns(apiKey);
               }

               _groups["62594c9b-21fe-4c65-ac5c-b32a836a2ca5"] = gs;
          }

          IGroupSettings IApplicationSettingsProvider.GetGroup(string groupId)
          {
               return _groups[groupId];
          }

          ISetting IUserSettingsProvider.GetSetting(string groupId, string settingId)
          {
               return _groups[groupId].GetSetting(settingId);
          }

          IGroupSettings IUserSettingsProvider.GetGroup(string groupId)
          {
               return _groups[groupId];
          }

          ISetting IApplicationSettingsProvider.GetSetting(string groupId, string settingId)
          {
               return _groups[groupId].GetSetting(settingId);
          }
     }
     
     private class XUnitTraceProvider : ITracing
     {
          private readonly ITestOutputHelper _output;

          public XUnitTraceProvider(ITestOutputHelper output)
          {
               _output = output;
          }

          public void Debug(string message)
          {
               _output.WriteLine($"DEBUG: {message}");
          }

          public void Failure(Exception exception)
          {
               _output.WriteLine($"FAILURE: {exception.Message}");
          }

          public void Failure(string exceptionMessage)
          {
               _output.WriteLine($"FAILURE: {exceptionMessage}");
          }

          public void Info(string message)
          {
               _output.WriteLine($"INFO: {message}");
          }

          public void Warning(string message)
          {
               _output.WriteLine($"WARNING: {message}");
          }
     }
}