using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ModuleTests.AspNetWebApi.Application.Enums;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace ModuleTests.AspNetWebApi.Application
{

    public interface IAsyncTestServices : IDisposable
    {

        Task AsyncOperationWithoutAnything();

        Task AsyncOperationWithPrimitiveArguments(string stringVal, long longVal, System.DateTime dateVal, TestEnumA enumVal);

        Task AsyncOperationWithDTOArgument(TestDTO dto);

        Task<TestDTO> AsyncOperationWithDTOResponse();

        Task<string> AsyncOperationWithPrimitiveResponse();

        Task AsyncOperationWithExplicitTransactionScope();

        Task AsyncOperationWithoutTransactionScope();
    }
}