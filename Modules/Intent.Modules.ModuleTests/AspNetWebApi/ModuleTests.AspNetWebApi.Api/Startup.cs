using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using Intent.Framework.Core.Context;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Owin;
using ModuleTests.AspNetWebApi.Api.Context;
using Owin;

[assembly: OwinStartup(typeof(ModuleTests.AspNetWebApi.Api.Startup))]
[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Owin.OwinStartup", Version = "1.0")]

namespace ModuleTests.AspNetWebApi.Api
{
    [IntentManaged(Mode.Merge)]
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            InitializeServiceCallContext();
            WebApiConfig.Configure(app);
            CustomConfiguration(app);
        }

        [IntentManaged(Mode.Ignore)]
        public void CustomConfiguration(IAppBuilder app)
        {
            // Put your own custom configuration here
        }

        private void InitializeServiceCallContext()
        {
            ServiceCallContext.SetBackingStore(new WebApiServiceCallContext());
        }
    }
}
