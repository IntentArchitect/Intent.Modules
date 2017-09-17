using Intent.Modules.AspNet.WebApi.Legacy.Controller;
using Intent.Modules.Logging.NLog.Templates.SanitizingJsonSerializer;
using Intent.SoftwareFactory.MetaModels.Class;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Logging.NLog.Interop.WebApi.Legacy
{
    public class NLogDistributionDecorator : IDistributionDecorator, IHasNugetDependencies, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Logging.NLog.Interop.WebApi.Decorator.Legacy";

        public NLogDistributionDecorator()
        {
        }

        public IEnumerable<string> DeclareUsings(ServiceModel service)
        {
            return new[]
                {
                "using NLog;",
                "using System.Diagnostics;"
                };
        }

        public string DeclarePrivateVariables(ServiceModel service) => @"
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();";


        public string ConstructorParams(ServiceModel service) => @"";

        public string ConstructorInit(ServiceModel service) => @"";

        public IEnumerable<string> PayloadPropertyDecorators(ParameterModel parameter) => new string[] { };

        public string BeginOperation(ServiceModel service, ServiceOperationModel operation)
        {
            switch (service.DistributionMode)
            {
                case ServiceDistributionMode.WebApi:
                    return !operation.UsesRawSignature
                        ? BeginOperation(new[] { "payload" }, operation.Name)
                        : BeginOperation(operation.Parameters.Select(x => x.Name), operation.Name);
                case ServiceDistributionMode.Wcf:
                case ServiceDistributionMode.WcfRestful:
                    return BeginOperation(operation.Parameters.Select(x => x.Name), operation.Name);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        public string BeforeTransaction(ServiceModel service, ServiceOperationModel operation) => @"";

        public string BeforeCallToAppLayer(ServiceModel service, ServiceOperationModel operation) => @"";

        public string AfterCallToAppLayer(ServiceModel service, ServiceOperationModel operation) => @"";

        public string AfterTransaction(ServiceModel service, ServiceOperationModel operation)
        {
            var returnString = @"
";
            if (operation.HasReturnType())
            {
                returnString += $@"
                Logger.Trace(""{FormatOperationName(operation.Name)}Returning: {{0}}"", SanitizingJsonSerializer.Serialize(result));";
            }

            returnString += $@"
                Logger.Info(""{FormatOperationName(operation.Name)}Duration (ms): {{0:#,0}}"", stopWatch.ElapsedMilliseconds);";

            return returnString;
        }

        public string OnExceptionCaught(ServiceModel service, ServiceOperationModel operation)
        {
            switch (service.DistributionMode)
            {
                case ServiceDistributionMode.WebApi:
                    return !operation.UsesRawSignature
                        ? OnExceptionCaught(new[] { "payload" }, operation.Name)
                        : OnExceptionCaught(operation.Parameters.Select(x => x.Name), operation.Name);
                case ServiceDistributionMode.Wcf:
                case ServiceDistributionMode.WcfRestful:
                    return OnExceptionCaught(operation.Parameters.Select(x => x.Name), operation.Name);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string OnExceptionCaught(IEnumerable<string> parameterNames, string operationName)
        {
            var anonymousObjectInitializer = parameterNames.Select(x => $"{x}")
                .DefaultIfEmpty()
                .Aggregate((x, y) => $"{x}, {y}");

            return $@"
                Logger.Error(""{FormatOperationName(operationName)}Parameters: {{0}}"", SanitizingJsonSerializer.Serialize(new {{ " + anonymousObjectInitializer + $@" }}));
                Logger.Error(e);";
        }

        public string ClassMethods(ServiceModel service) => @"";

        public int Priority { get; set; } = int.MaxValue;

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
    }
}