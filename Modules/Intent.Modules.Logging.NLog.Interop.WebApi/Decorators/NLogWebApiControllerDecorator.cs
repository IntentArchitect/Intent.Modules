using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Logging.NLog.Templates.SanitizingJsonSerializer;
using Intent.Templates

namespace Intent.Modules.Logging.NLog.Interop.WebApi.Decorators
{
    public class NLogWebApiControllerDecorator : WebApiControllerDecoratorBase, IHasNugetDependencies, IHasTemplateDependencies
    {
        public const string IDENTIFIER = "Intent.Logging.NLog.Interop.WebApi.Decorator";

        public override IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "NLog",
                "System.Diagnostics"
            };
        }

        public override string DeclarePrivateVariables(IServiceModel service) => @"
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();";


        public override string ConstructorParams(IServiceModel service) => @"";

        public override string ConstructorInit(IServiceModel service) => @"";

        public override IEnumerable<string> PayloadPropertyDecorators(IOperationParameterModel parameter) => new string[] { };

        public override string BeginOperation(IServiceModel service, IOperationModel operation)
        {
            return $@"
            var stopWatch = Stopwatch.StartNew();
            Logger.Trace(""{FormatOperationName(operation.Name)}Parameters: {{0}}"", SanitizingJsonSerializer.Serialize(new {{ " + GetAnonymousObjectInitializer(operation) + $@" }}));";
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
            return $@"
                Logger.Error(""{FormatOperationName(operation.Name)}Parameters: {{0}}"", SanitizingJsonSerializer.Serialize(new {{ " + GetAnonymousObjectInitializer(operation) + $@" }}));
                Logger.Error(e);";
        }

        private static string GetAnonymousObjectInitializer(IOperationModel operation)
        {
            return operation.Parameters
                .Select(x => $"{x.Name}")
                .DefaultIfEmpty()
                .Aggregate((x, y) => $"{x}, {y}");
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

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(SanitizingJsonSerializerTemplate.Identifier),
            };
        }
    }
}