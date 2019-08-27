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
    public class TestServices : ITestServices
    {
        public TestServices()
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public void OperationWithNoSignature()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public void OperationWithPrimitiveArguments(string stringVal, int intVal, TestEnumA enumVal, System.Guid guidVal)
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public string OperationWithPrimitiveResponse()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public void OperationWithOneDTOArgument(TestDTO dto)
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task AsyncOperationWithOneDTOArgument(TestDTO dto)
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task<string> AsyncOperationWithPrimitiveResponse()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task AsyncOperationWithPrimitiveArguments(string stringVal, int intVal, TestEnumA enumVal, System.Guid guidVal)
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        public void Dispose()
        {
        }
    }
}