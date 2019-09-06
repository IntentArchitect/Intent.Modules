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
    [RoutePrefix("api/normaltestservices")]
    public class NormalTestServicesController : ApiController
    {
        private readonly INormalTestServices _appService;

        public NormalTestServicesController(INormalTestServices appService
            )
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("operationwithoutanything")]
        [ResponseType(typeof(void))]
        public IHttpActionResult OperationWithoutAnything()
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso))
            {
                _appService.OperationWithoutAnything();
                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("operationwithprimitivearguments")]
        [ResponseType(typeof(void))]
        public IHttpActionResult OperationWithPrimitiveArguments(string stringVal, long longVal, System.DateTime dateVal, TestEnumA enumVal)
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso))
            {
                _appService.OperationWithPrimitiveArguments(stringVal, longVal, dateVal, enumVal);
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
        [Route("operationwithdtoargument")]
        [ResponseType(typeof(void))]
        public IHttpActionResult OperationWithDTOArgument(TestDTO dto)
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso))
            {
                _appService.OperationWithDTOArgument(dto);
                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("GET")]
        [AllowAnonymous]
        [Route("operationwithdtoresponse")]
        [ResponseType(typeof(TestDTO))]
        public IHttpActionResult OperationWithDTOResponse()
        {
            TestDTO result = default(TestDTO);
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso))
            {
                var appServiceResult = _appService.OperationWithDTOResponse();
                result = appServiceResult;
                ts.Complete();
            }

            return Ok(result);
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("operationwithexplicittransactionscope")]
        [ResponseType(typeof(void))]
        public IHttpActionResult OperationWithExplicitTransactionScope()
        {
            var tso = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tso))
            {
                _appService.OperationWithExplicitTransactionScope();
                ts.Complete();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("POST")]
        [AllowAnonymous]
        [Route("operationwithouttransactionscope")]
        [ResponseType(typeof(void))]
        public IHttpActionResult OperationWithoutTransactionScope()
        {
            _appService.OperationWithoutTransactionScope();

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