using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Intent.RoslynWeaver.Attributes;
using ModuleTests.AspNetWebApi.Application;
using ModuleTests.AspNetWebApi.Application.Enums;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNet.WebApi.Controller", Version = "1.0")]

namespace ModuleTests.AspNetWebApi.Api
{
    [RoutePrefix("api/asynctestservices")]
    public class AsyncTestServicesController : ApiController
    {
        private readonly IAsyncTestServices _appService;

        public AsyncTestServicesController(IAsyncTestServices appService
            )
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("asyncoperationwithoutanything")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AsyncOperationWithoutAnything()
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.AsyncOperationWithoutAnything();
                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("asyncoperationwithprimitivearguments")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AsyncOperationWithPrimitiveArguments(string stringVal, long longVal, System.DateTime dateVal, TestEnumA enumVal)
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.AsyncOperationWithPrimitiveArguments(stringVal, longVal, dateVal, enumVal);
                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("asyncoperationwithdtoargument")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AsyncOperationWithDTOArgument(TestDTO dto)
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.AsyncOperationWithDTOArgument(dto);
                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("GET")]
        [AllowAnonymous]
        [Route("asyncoperationwithdtoresponse")]
        [ResponseType(typeof(TestDTO))]
        public async Task<IHttpActionResult> AsyncOperationWithDTOResponse()
        {
            TestDTO result = default(TestDTO);
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso, TransactionScopeAsyncFlowOption.Enabled))
            {
                var appServiceResult = await _appService.AsyncOperationWithDTOResponse();
                result = appServiceResult;
                ts.Complete();
            }

            return Ok(result);
        }

        [AcceptVerbs("GET")]
        [AllowAnonymous]
        [Route("asyncoperationwithprimitiveresponse")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> AsyncOperationWithPrimitiveResponse()
        {
            string result = default(string);
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso, TransactionScopeAsyncFlowOption.Enabled))
            {
                var appServiceResult = await _appService.AsyncOperationWithPrimitiveResponse();
                result = appServiceResult;
                ts.Complete();
            }

            return Ok(result);
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("asyncoperationwithexplicittransactionscope")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AsyncOperationWithExplicitTransactionScope()
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.AsyncOperationWithExplicitTransactionScope();
                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("asyncoperationwithouttransactionscope")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AsyncOperationWithoutTransactionScope()
        {
            await _appService.AsyncOperationWithoutTransactionScope();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            //dispose all resources
            _appService.Dispose();
        }

    }
}