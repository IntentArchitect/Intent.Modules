using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.S3.Api
{
    public static class S3BucketModelStereotypeExtensions
    {
        public static S3BucketSettings GetS3BucketSettings(this S3BucketModel model)
        {
            var stereotype = model.GetStereotype("S3 Bucket Settings");
            return stereotype != null ? new S3BucketSettings(stereotype) : null;
        }


        public static bool HasS3BucketSettings(this S3BucketModel model)
        {
            return model.HasStereotype("S3 Bucket Settings");
        }

        public static bool TryGetS3BucketSettings(this S3BucketModel model, out S3BucketSettings stereotype)
        {
            if (!HasS3BucketSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new S3BucketSettings(model.GetStereotype("S3 Bucket Settings"));
            return true;
        }

        public class S3BucketSettings
        {
            private IStereotype _stereotype;

            public S3BucketSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public RemovalPolicyOptions RemovalPolicy()
            {
                return new RemovalPolicyOptions(_stereotype.GetProperty<string>("Removal Policy"));
            }

            public bool Versioned()
            {
                return _stereotype.GetProperty<bool>("Versioned");
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