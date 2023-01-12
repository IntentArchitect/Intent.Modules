using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    public static class DynamoDBTableModelStereotypeExtensions
    {
        public static DynamoDBTableSettings GetDynamoDBTableSettings(this DynamoDBTableModel model)
        {
            var stereotype = model.GetStereotype("DynamoDB Table Settings");
            return stereotype != null ? new DynamoDBTableSettings(stereotype) : null;
        }


        public static bool HasDynamoDBTableSettings(this DynamoDBTableModel model)
        {
            return model.HasStereotype("DynamoDB Table Settings");
        }

        public static bool TryGetDynamoDBTableSettings(this DynamoDBTableModel model, out DynamoDBTableSettings stereotype)
        {
            if (!HasDynamoDBTableSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DynamoDBTableSettings(model.GetStereotype("DynamoDB Table Settings"));
            return true;
        }

        public class DynamoDBTableSettings
        {
            private IStereotype _stereotype;

            public DynamoDBTableSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? ProvisionedThroughputReadCapacityUnits()
            {
                return _stereotype.GetProperty<int?>("ProvisionedThroughput - ReadCapacityUnits");
            }

            public int? ProvisionedThroughputWriteCapacityUnits()
            {
                return _stereotype.GetProperty<int?>("ProvisionedThroughput - WriteCapacityUnits");
            }

            public RemovalPolicyOptions RemovalPolicy()
            {
                return new RemovalPolicyOptions(_stereotype.GetProperty<string>("Removal Policy"));
            }

            public class RemovalPolicyOptions
            {
                public readonly string Value;

                public RemovalPolicyOptions(string value)
                {
                    Value = value;
                }

                public RemovalPolicyOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Destroy":
                            return RemovalPolicyOptionsEnum.Destroy;
                        case "Retain":
                            return RemovalPolicyOptionsEnum.Retain;
                        case "Snapshot":
                            return RemovalPolicyOptionsEnum.Snapshot;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDestroy()
                {
                    return Value == "Destroy";
                }
                public bool IsRetain()
                {
                    return Value == "Retain";
                }
                public bool IsSnapshot()
                {
                    return Value == "Snapshot";
                }
            }

            public enum RemovalPolicyOptionsEnum
            {
                Destroy,
                Retain,
                Snapshot
            }

        }

    }
}