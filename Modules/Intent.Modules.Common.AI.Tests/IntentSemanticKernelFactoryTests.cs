using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Intent.Modules.Common.AI.Settings;
using Intent.Modules.Common.AI.Tests.Helpers;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Xunit;

namespace Intent.Modules.Common.AI.Tests;

public class IntentSemanticKernelFactoryTests
{
     public IntentSemanticKernelFactoryTests(Xunit.Abstractions.ITestOutputHelper output)
     {
          Logging.SetTracing(new XUnitTraceProvider(output));
     }
     
     [Theory(
          Skip = "Requires API Token environment variables to be set AND access to AI providers."
          )]
     [InlineData(AISettings.ProviderOptionsEnum.OpenAi, "gpt-4o")]
     [InlineData(AISettings.ProviderOptionsEnum.Anthropic, "claude-3-5-haiku-20241022")]
     [InlineData(AISettings.ProviderOptionsEnum.OpenRouter, "qwen/qwen3-coder:free")] // Requires "Enable free endpoints that may train on inputs" ENABLED
     public async Task AiConnectionWithCalculatorToolTest(AISettings.ProviderOptionsEnum provider, string model)
     {
          // ARRANGE
          var factory = new IntentSemanticKernelFactory(SettingsMock.GetUserSettingsProvider(provider, model));
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
}