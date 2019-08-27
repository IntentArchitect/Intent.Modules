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
    public class NormalTestServices : INormalTestServices
    {
        public NormalTestServices()
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public void OperationWithoutAnything()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public void OperationWithPrimitiveArguments(string stringVal, long longVal, System.DateTime dateVal, TestEnumA enumVal)
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public string OperationWithPrimitiveResponse()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public void OperationWithDTOArgument(TestDTO dto)
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public TestDTO OperationWithDTOResponse()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public void OperationWithExplicitTransactionScope()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public void OperationWithoutTransactionScope()
        {
            throw new NotImplementedException("Write your implementation for this services here...");
        }

        public void Dispose()
        {
        }
    }
}