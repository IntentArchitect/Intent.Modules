using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ModuleTests.AspNetWebApi.Application;
using ModuleTests.AspNetWebApi.Application.Enums;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations", Version = "1.0")]

namespace ModuleTests.AspNetWebApi.Application.ServiceImplementation
{
    public class AsyncTestServices : IAsyncTestServices
    {
        public AsyncTestServices()
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task AsyncOperationWithoutAnything()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task AsyncOperationWithPrimitiveArguments(string stringVal, long longVal, System.DateTime dateVal, TestEnumA enumVal)
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task AsyncOperationWithDTOArgument(TestDTO dto)
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task<TestDTO> AsyncOperationWithDTOResponse()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task<string> AsyncOperationWithPrimitiveResponse()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task AsyncOperationWithExplicitTransactionScope()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task AsyncOperationWithoutTransactionScope()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        public void Dispose()
        {
        }
    }
}