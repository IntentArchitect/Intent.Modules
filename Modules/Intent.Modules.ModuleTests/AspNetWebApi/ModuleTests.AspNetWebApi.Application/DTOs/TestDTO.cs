using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.DTO", Version = "1.0")]

namespace ModuleTests.AspNetWebApi.Application
{
    [DataContract]
    public class TestDTO
    {
        public TestDTO()
        {
        }

        public static TestDTO Create(
            string fieldA)
        {
            return new TestDTO
            {
                FieldA = fieldA,
            };
        }

        [DataMember]
        public string FieldA { get; set; }
    }
}