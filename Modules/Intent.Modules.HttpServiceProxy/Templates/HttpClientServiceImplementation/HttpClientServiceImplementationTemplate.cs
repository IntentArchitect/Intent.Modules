﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceImplementation
{
    using Intent.Modules.Common.Templates;
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.HttpServiceProxy\Templates\HttpClientServiceImplementation\HttpClientServiceImplementationTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class HttpClientServiceImplementationTemplate : IntentRoslynProjectItemTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@" 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace ");
            
            #line 23 "C:\Dev\Intent.Modules\Modules\Intent.Modules.HttpServiceProxy\Templates\HttpClientServiceImplementation\HttpClientServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.HttpServiceProxy\Templates\HttpClientServiceImplementation\HttpClientServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.HttpServiceProxy\Templates\HttpClientServiceImplementation\HttpClientServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetHttpClientServiceInterfaceName()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        private static readonly object LockObject = new object();\r\n     " +
                    "   private static readonly int TimeoutInSeconds;\r\n\r\n        private static bool " +
                    "_instantiated;\r\n\r\n        private readonly Func<");
            
            #line 32 "C:\Dev\Intent.Modules\Modules\Intent.Modules.HttpServiceProxy\Templates\HttpClientServiceImplementation\HttpClientServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetInterceptorInterfaceName()));
            
            #line default
            #line hidden
            this.Write(@"[]> _proxyInterceptorsProvider;
        private readonly IDictionary<string, HttpClient> _httpClients = new Dictionary<string, HttpClient>();

        static HttpClientService()
        {
            TimeoutInSeconds = 60;
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[""HttpClient.Timeout.Seconds""]) &&
                !int.TryParse(ConfigurationManager.AppSettings[""HttpClient.Timeout.Seconds""], out TimeoutInSeconds))
            {
                throw new Exception(""Could not parse 'HttpClient.Timeout.Seconds' setting in config file as int."");
            }
        }

        public HttpClientService(Func<");
            
            #line 45 "C:\Dev\Intent.Modules\Modules\Intent.Modules.HttpServiceProxy\Templates\HttpClientServiceImplementation\HttpClientServiceImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetInterceptorInterfaceName()));
            
            #line default
            #line hidden
            this.Write("[]> proxyInterceptorsProvider)\r\n        {\r\n            lock (LockObject)\r\n       " +
                    "     {\r\n                if (_instantiated)\r\n                {\r\n                 " +
                    "   throw new Exception(\"This service is designed to run as a singleton and has a" +
                    "lready been instantiated.\");\r\n                }\r\n\r\n                _instantiated" +
                    " = true;\r\n            }\r\n\r\n            _proxyInterceptorsProvider = proxyInterce" +
                    "ptorsProvider;\r\n        }\r\n\r\n        /// <summary>\r\n        /// Retrieve an exis" +
                    "ting, or otherwise lazy instantiate a new, HttpClient for a particular host/port" +
                    " combination\r\n        /// in a thread safe manner.\r\n        /// </summary>\r\n    " +
                    "    private HttpClient GetHttpClient(string url)\r\n        {\r\n            var uri" +
                    " = new Uri(url);\r\n            var key = $\"{uri.Host}.{uri.Port}\";\r\n\r\n           " +
                    " if (_httpClients.ContainsKey(key)) // First check of double checked lock\r\n     " +
                    "       {\r\n                return _httpClients[key];\r\n            }\r\n\r\n          " +
                    "  lock (LockObject)\r\n            {\r\n                if (_httpClients.ContainsKey" +
                    "(key)) // Second check of double checked lock\r\n                {\r\n              " +
                    "      return _httpClients[key];\r\n                }\r\n\r\n                var httpCl" +
                    "ient = _httpClients[key] = new HttpClient\r\n                {\r\n                  " +
                    "  BaseAddress = new Uri(uri.GetLeftPart(UriPartial.Authority)),\r\n               " +
                    "     Timeout = TimeSpan.FromSeconds(TimeoutInSeconds)\r\n                };\r\n     " +
                    "           httpClient.DefaultRequestHeaders.Accept.Clear();\r\n                htt" +
                    "pClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(\"ap" +
                    "plication/json\"));\r\n\r\n                return httpClient;\r\n            }\r\n       " +
                    " }\r\n\r\n        public string GetBaseAddress(string targetApplicationName)\r\n      " +
                    "  {\r\n            var baseUrl = ConfigurationManager.AppSettings[$\"WebApiEndpoint" +
                    "BaseUrl.{targetApplicationName}\"];\r\n            if (string.IsNullOrWhiteSpace(ba" +
                    "seUrl))\r\n            {\r\n                throw new Exception($\"No configuration e" +
                    "ntry defined. Please add the following to the config file\'s appSetting section: " +
                    "<add key=\\\"WebApiEndpointBaseUrl.{targetApplicationName}\\\" value=\\\"INSERT_URL_HE" +
                    "RE\\\" />\");\r\n            }\r\n\r\n            return baseUrl;\r\n        }\r\n\r\n        p" +
                    "ublic Task<HttpResponseMessage> PostAsJsonAsync<T>(string targetApplicationName," +
                    " string requestUri, T value)\r\n        {\r\n            return PostAsJsonAsync(targ" +
                    "etApplicationName, requestUri, value, CancellationToken.None);\r\n        }\r\n\r\n   " +
                    "     public Task<HttpResponseMessage> PostAsJsonAsync<T>(string targetApplicatio" +
                    "nName, string requestUri, T value, CancellationToken cancellationToken)\r\n       " +
                    " {\r\n            var baseUri = new Uri(GetBaseAddress(targetApplicationName));\r\n " +
                    "           var fullRequestUri = new Uri(baseUri, requestUri);\r\n\r\n            ret" +
                    "urn PostAsJsonAsync(fullRequestUri.ToString(), value, cancellationToken);\r\n     " +
                    "   }\r\n\r\n        public Task<HttpResponseMessage> PostAsJsonAsync<T>(string fullR" +
                    "equestUri, T value)\r\n        {\r\n            return PostAsJsonAsync(fullRequestUr" +
                    "i, value, CancellationToken.None);\r\n        }\r\n\r\n        public Task<HttpRespons" +
                    "eMessage> PostAsJsonAsync<T>(string fullRequestUri, T value, CancellationToken c" +
                    "ancellationToken)\r\n        {\r\n            // This method is an adaptation of Pos" +
                    "tAsJsonAsync(...) from the following two source files:\r\n            // https://g" +
                    "ithub.com/aspnet/AspNetWebStack/blob/master/src/System.Net.Http.Formatting/HttpC" +
                    "lientExtensions.cs\r\n            // https://github.com/dotnet/corefx/blob/master/" +
                    "src/System.Net.Http/src/System/Net/Http/HttpClient.cs\r\n\r\n            // A custom" +
                    " method was required so that interceptors can change the headers per request, pa" +
                    "rticularly the\r\n            // authorization one, rather than just having defaul" +
                    "ts headers per HttpClient instance, by extension allowing\r\n            // us to " +
                    "keep a single instance of an HttpClient per application, as per Microsoft guidel" +
                    "ines.\r\n\r\n            // This Github issue explains the requirement perfectly: ht" +
                    "tps://github.com/dotnet/corefx/issues/23544\r\n\r\n            var httpClient = GetH" +
                    "ttpClient(fullRequestUri);\r\n\r\n            var request = new HttpRequestMessage(\r" +
                    "\n                method: HttpMethod.Post,\r\n                requestUri: new Uri(f" +
                    "ullRequestUri).PathAndQuery)\r\n            {\r\n                Content = new Objec" +
                    "tContent<T>(value, new JsonMediaTypeFormatter(), (MediaTypeHeaderValue)null)\r\n  " +
                    "          };\r\n\r\n            foreach (var interceptor in _proxyInterceptorsProvid" +
                    "er())\r\n            {\r\n                interceptor.BeforeRequest(request);\r\n     " +
                    "       }\r\n\r\n            return httpClient.SendAsync(request, cancellationToken);" +
                    "\r\n        }\r\n    }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
