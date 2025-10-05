using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Intent.Modules.Common.AI.Settings;
using Intent.Modules.Common.AI.Tests.Helpers;
using Intent.Modules.Common.AI.Tasks;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Xunit;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

[assembly: CollectionBehavior(DisableTestParallelization = false, MaxParallelThreads = 4)]

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
     [MemberData(nameof(GetProviderModelTestData))]
     public async Task AiConnectionWithCalculatorToolTest(AISettings.ProviderOptionsEnum provider, string model)
     {
          // ARRANGE
          var factory = new IntentSemanticKernelFactory(SettingsMock.GetUserSettingsProvider(provider, model));
          var kernel = factory.BuildSemanticKernel(model, builder =>
          {
               builder.Plugins.AddFromType<CalculatorPlugin>();
          });
          
          // ACT
          var function = kernel.CreateFunctionFromPrompt(
               "What is 847,293,156 × 923,847,521? Use the calculator tool and return only the answer (in the form 0,000.0)!",
               kernel.GetRequiredService<IAiProviderService>().GetPromptExecutionSettings(null));
          var result = await function.InvokeAsync(kernel);
          
          // ASSERT
          Assert.NotNull(result);
          Assert.Equal((847_293_156L * 923_847_521L).ToString("N1", CultureInfo.InvariantCulture), result.ToString().Trim());
     }

     public static IEnumerable<object[]> GetProviderModelTestData()
     {
          var userSettingsProvider = SettingsMock.GetUserSettingsProvider(AISettings.ProviderOptionsEnum.OpenAi, "gpt-4o");
          var task = new ProviderModelsTask(userSettingsProvider);
          var json = task.Execute();
          
          var models = JsonSerializer.Deserialize<List<ModelRecord>>(json, new JsonSerializerOptions
          {
               PropertyNameCaseInsensitive = true,
               PropertyNamingPolicy = JsonNamingPolicy.CamelCase
          });

          return models.Select(m => new object[]
          {
               MapProviderIdToEnum(m.ProviderId),
               m.ModelName
          });
     }

     private static AISettings.ProviderOptionsEnum MapProviderIdToEnum(string providerId)
     {
          return new AISettings.ProviderOptions(providerId).AsEnum();
     }

     private record ModelRecord(string ProviderId, string ModelName, string ProviderName, string ThinkingType);

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