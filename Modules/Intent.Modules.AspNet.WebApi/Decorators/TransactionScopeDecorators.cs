using Intent.Modules.Application.Contracts;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common;
using System;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.AspNet.WebApi.Decorators
{
    public class BeginTransactionScopeDecorator : WebApiControllerDecoratorBase
    {
        public const string Identifier = "Intent.AspNet.WebApi.BeginTransactionScope.Decorator";

        private readonly IApplication _application;

        public BeginTransactionScopeDecorator(IApplication application)
        {
            _application = application;
        }

        public override int Priority => 1000;

        public override string BeforeCallToAppLayer(ServiceModel service, OperationModel operation)
        {
            if (!operation.GetStereotypeProperty<bool>("Transactional Settings", "Explicit Scope", true))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            sb.AppendIndentation(3)
                .AppendLine("var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };");

            sb.AppendIndentation(3)
                .AppendLine($"using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso{(operation.IsAsync() ? ", TransactionScopeAsyncFlowOption.Enabled" : string.Empty)}))")
                .AppendIndentation(3)
                .AppendLine("{");

            return sb.ToString();
        }
    }

    public class EndTransactionScopeDecorator : WebApiControllerDecoratorBase
    {
        public const string Identifier = "Intent.AspNet.WebApi.EndTransactionScope.Decorator";

        private readonly IApplication _application;

        public EndTransactionScopeDecorator(IApplication application)
        {
            _application = application;
        }

        public override int Priority => -1000;

        public override string AfterCallToAppLayer(ServiceModel service, OperationModel operation)
        {
            if (!operation.GetStereotypeProperty<bool>("Transactional Settings", "Explicit Scope", true))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            sb.AppendIndentation(4)
                .AppendLine("ts.Complete();")
                .AppendIndentation(3)
                .AppendLine("}");

            return sb.ToString();
        }
    }

    static class StringBuilderExtensions
    {
        public static StringBuilder AppendIndentation(this StringBuilder sb, int length = 1)
        {
            for (var i = 0; i < length; i++)
            {
                sb.Append("    ");
            }

            return sb;
        }
    }
}
