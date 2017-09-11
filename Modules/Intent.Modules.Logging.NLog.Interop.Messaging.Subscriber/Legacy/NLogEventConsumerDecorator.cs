using System.Collections.Generic;
using System.Linq;
using Intent.Package.Logging.NLog;
using Intent.Packages.Logging.NLog.Templates.SanitizingJsonSerializer;
using Intent.Packages.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Logging.NLog.Interop.Messaging.Subscriber.Legacy
{
    public class NLogEventConsumerDecorator : IEventConsumerDecorator, IHasNugetDependencies, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Logging.NLog.Interop.EventConsumerDecorator.Legacy";

        public NLogEventConsumerDecorator()
        {
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "using NLog;",
                "using System.Diagnostics;"
            };
        }

        string IEventConsumerDecorator.DeclarePrivateVariables() => @"
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();";


        string IEventConsumerDecorator.ConstructorParams() => @"";

        string IEventConsumerDecorator.ConstructorInit() => @"";

        string IEventConsumerDecorator.BeginOperation() => BeginOperation(new[] { "payload" }, "ConsumeMessage");
      
        private static string BeginOperation(IEnumerable<string> parameterNames, string operationName)
        {
            var anonymousObjectInitializer = parameterNames.Select(x => $"{x}")
                .DefaultIfEmpty()
                .Aggregate((x, y) => $"{x}, {y}");

            return $@"
            var stopWatch = Stopwatch.StartNew();
            Logger.Trace(""{FormatOperationName(operationName)}Parameters: {{0}}"", SanitizingJsonSerializer.Serialize(new {{ " + anonymousObjectInitializer + $@" }}));";
        }

        string IEventConsumerDecorator.BeforeTransaction() => @"";

        string IEventConsumerDecorator.BeforeCallToAppLayer() => @"";

        string IEventConsumerDecorator.AfterCallToAppLayer() => @"";

        string IEventConsumerDecorator.AfterTransaction() => AfterTransaction(false, "ConsumeMessage");
        private static string AfterTransaction(bool operationHasReturnType, string operationName)
        {
            var returnString = @"
";
            if (operationHasReturnType)
            {
                returnString += $@"
                Logger.Trace(""{FormatOperationName(operationName)}Returning: {{0}}"", SanitizingJsonSerializer.Serialize(result));";
            }

            returnString += $@"
                Logger.Info(""{FormatOperationName(operationName)}Duration (ms): {{0:#,0}}"", stopWatch.ElapsedMilliseconds);";

            return returnString;
        }

        string IEventConsumerDecorator.OnExceptionCaught() => OnExceptionCaught(new[] { "payload" }, "ConsumeMessage");
       public string OnExceptionCaught(IEnumerable<string> parameterNames, string operationName)
        {
            var anonymousObjectInitializer = parameterNames.Select(x => $"{x}")
                .DefaultIfEmpty()
                .Aggregate((x, y) => $"{x}, {y}");

            return $@"
                Logger.Error(""{FormatOperationName(operationName)}Parameters: {{0}}"", SanitizingJsonSerializer.Serialize(new {{ " + anonymousObjectInitializer + $@" }}));
                Logger.Error(e);";
        }

        string IEventConsumerDecorator.ClassMethods() => @"";

        public int Priority() => int.MinValue;

        private static string FormatOperationName(string operationName)
        {
            if (string.IsNullOrWhiteSpace(operationName))
            {
                return string.Empty;
            }

            return $"({operationName}) ";
        }

        public bool HandlesCaughtException() => false;


        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.NLog,
                NugetPackages.NewtonsoftJson,
            };
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(SanitizingJsonSerializerTemplate.Identifier),
            };
        }

        int IPriorityDecorator.Priority { get; set; } = 0;
    }
}