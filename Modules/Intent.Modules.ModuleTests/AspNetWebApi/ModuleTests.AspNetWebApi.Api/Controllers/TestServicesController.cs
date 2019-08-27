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
    [RoutePrefix("api/testservices")]
    public class TestServicesController : ApiController
    {
        private readonly ITestServices _appService;

        public TestServicesController(ITestServices appService
            )
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("operationwithnosignature")]
        [ResponseType(typeof(void))]
        public IHttpActionResult OperationWithNoSignature()
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };


            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso))
            {

                _appService.OperationWithNoSignature();

                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("operationwithprimitivearguments")]
        [ResponseType(typeof(void))]
        public IHttpActionResult OperationWithPrimitiveArguments(string stringVal, int intVal, TestEnumA enumVal, System.Guid guidVal)
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };


            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso))
            {

                _appService.OperationWithPrimitiveArguments(stringVal, intVal, enumVal, guidVal);

                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("GET")]
        [AllowAnonymous]
        [Route("operationwithprimitiveresponse")]
        [ResponseType(typeof(string))]
        public IHttpActionResult OperationWithPrimitiveResponse()
        {

            string result = default(string);
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };


            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso))
            {
                var appServiceResult = _appService.OperationWithPrimitiveResponse();
                result = appServiceResult;

                ts.Complete();
            }

            return Ok(result);
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("operationwithonedtoargument")]
        [ResponseType(typeof(void))]
        public IHttpActionResult OperationWithOneDTOArgument(TestDTO dto)
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };


            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso))
            {

                _appService.OperationWithOneDTOArgument(dto);

                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("asyncoperationwithonedtoargument")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AsyncOperationWithOneDTOArgument(TestDTO dto)
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };


            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso, TransactionScopeAsyncFlowOption.Enabled))
            {

                await _appService.AsyncOperationWithOneDTOArgument(dto);

                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
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
        [Route("asyncoperationwithprimitivearguments")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AsyncOperationWithPrimitiveArguments(string stringVal, int intVal, TestEnumA enumVal, System.Guid guidVal)
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };


            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso, TransactionScopeAsyncFlowOption.Enabled))
            {

                await _appService.AsyncOperationWithPrimitiveArguments(stringVal, intVal, enumVal, guidVal);

                ts.Complete();
            }

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