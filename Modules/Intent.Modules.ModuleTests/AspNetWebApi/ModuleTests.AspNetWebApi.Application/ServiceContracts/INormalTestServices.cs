using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ModuleTests.AspNetWebApi.Application.Enums;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace ModuleTests.AspNetWebApi.Application
{

    public interface INormalTestServices : IDisposable
    {

        void OperationWithoutAnything();

        void OperationWithPrimitiveArguments(string stringVal, long longVal, System.DateTime dateVal, TestEnumA enumVal);

        string OperationWithPrimitiveResponse();

        void OperationWithDTOArgument(TestDTO dto);

        TestDTO OperationWithDTOResponse();

        void OperationWithExplicitTransactionScope();

        void OperationWithoutTransactionScope();
    }
}