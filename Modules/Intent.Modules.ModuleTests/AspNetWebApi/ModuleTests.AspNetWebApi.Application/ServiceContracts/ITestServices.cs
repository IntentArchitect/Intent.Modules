using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ModuleTests.AspNetWebApi.Application.Enums;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace ModuleTests.AspNetWebApi.Application
{

    public interface ITestServices : IDisposable
    {

        void OperationWithNoSignature();

        void OperationWithPrimitiveArguments(string stringVal, int intVal, TestEnumA enumVal, System.Guid guidVal);

        string OperationWithPrimitiveResponse();

        void OperationWithOneDTOArgument(TestDTO dto);

        Task AsyncOperationWithOneDTOArgument(TestDTO dto);

        Task<string> AsyncOperationWithPrimitiveResponse();

        Task AsyncOperationWithPrimitiveArguments(string stringVal, int intVal, TestEnumA enumVal, System.Guid guidVal);
    }
}