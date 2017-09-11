using System;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Service;
using Intent.Package.Logging.NLog;
using Intent.Packages.Logging.NLog.Templates.SanitizingJsonSerializer;
using Intent.Modules.WebApi.Templates.Controller;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Logging.NLog.Interop.WebApi.Decorators
{
    public class NLogDistributionDecorator : DistributionDecoratorBase, IHasNugetDependencies, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Logging.NLog.Interop.WebApi.Decorator";

        public NLogDistributionDecorator()
        {
        }

        public override IEnumerable<string> DeclareUsings(IServiceModel service)
        {
            return new[]
                {
                "using NLog;",
                "using System.Diagnostics;"
                };
        }

        public override string DeclarePrivateVariables(IServiceModel service) => @"
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();";


        public override string ConstructorParams(IServiceModel service) => @"";

        public override string ConstructorInit(IServiceModel service) => @"";

        public override IEnumerable<string> PayloadPropertyDecorators(IOperationParameterModel parameter) => new string[] { };

        public override string BeginOperation(IServiceModel service, IOperationModel operation)
        {
            return BeginOperation(new[] { "payload" }, operation.Name);
        }

        private static string BeginOperation(IEnumerable<string> parameterNames, string operationName)
        {
            var anonymousObjectInitializer = parameterNames.Select(x => $"{x}")
                .DefaultIfEmpty()
                .Aggregate((x, y) => $"{x}, {y}");

            return $@"
            var stopWatch = Stopwatch.StartNew();
            Logger.Trace(""{FormatOperationName(operationName)}Parameters: {{0}}"", SanitizingJsonSerializer.Serialize(new {{ " + anonymousObjectInitializer + $@" }}));";
        }

        public override string BeforeTransaction(IServiceModel service, IOperationModel operation) => @"";

        public override string BeforeCallToAppLayer(IServiceModel service, IOperationModel operation) => @"";

        public override string AfterCallToAppLayer(IServiceModel service, IOperationModel operation) => @"";

        public override string AfterTransaction(IServiceModel service, IOperationModel operation)
        {
            var returnString = @"
";
            if (operation.ReturnType != null)
            {
                returnString += $@"
                Logger.Trace(""{FormatOperationName(operation.Name)}Returning: {{0}}"", SanitizingJsonSerializer.Serialize(result));";
            }

            returnString += $@"
                Logger.Info(""{FormatOperationName(operation.Name)}Duration (ms): {{0:#,0}}"", stopWatch.ElapsedMilliseconds);";

            return returnString;
        }

        public override string OnExceptionCaught(IServiceModel service, IOperationModel operation)
        {
            return OnExceptionCaught(new[] { "payload" }, operation.Name);
        }

        private static string OnExceptionCaught(IEnumerable<string> parameterNames, string operationName)
        {
            var anonymousObjectInitializer = parameterNames.Select(x => $"{x}")
                .DefaultIfEmpty()
                .Aggregate((x, y) => $"{x}, {y}");

            return $@"
                Logger.Error(""{FormatOperationName(operationName)}Parameters: {{0}}"", SanitizingJsonSerializer.Serialize(new {{ " + anonymousObjectInitializer + $@" }}));
                Logger.Error(e);";
        }

        public override string ClassMethods(IServiceModel service) => @"";

        public override int Priority { get; set; } = int.MaxValue;

        private static string FormatOperationName(string operationName)
        {
            if (string.IsNullOrWhiteSpace(operationName))
            {
                return string.Empty;
            }

            return $"({operationName}) ";
        }

        public override bool HandlesCaughtException() => false;


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
    }
}