using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Intent.Framework.Core.Context;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNet.WebApi.ServiceCallContext", Version = "1.0")]

namespace ModuleTests.AspNetWebApi.Api.Context
{
    public class WebApiServiceCallContext : IContextBackingStore
    {
        private static readonly object ModuleKey = new object();

        T IContextBackingStore.Get<T>()
        {
            var dict = GetDictionary(HttpContext.Current);

            if (dict != null)
            {
                object obj;

                if (dict.TryGetValue(typeof(T), out obj))
                {
                    return (T)obj;
                }
            }

            return default(T);
        }

        void IContextBackingStore.Set<T>(T value)
        {
            var dict = GetDictionary(HttpContext.Current);

            if (dict == null)
            {
                dict = new Dictionary<Type, object>();

                HttpContext.Current.Items[ModuleKey] = dict;
            }

            dict[typeof(T)] = value;
        }

        private static Dictionary<Type, object> GetDictionary(HttpContext context)
        {
            if (context == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(WebApiServiceCallContext)} can only be used in the context of an HTTP request. Possible causes for this error are using {nameof(WebApiServiceCallContext)} on a non-ASP.NET application, or using it in a thread that is not associated with the appropriate synchronization context.");
            }

            var dict = (Dictionary<Type, object>)context.Items[ModuleKey];

            return dict;
        }
    }
}