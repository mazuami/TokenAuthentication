using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace TokenbasedAuthenticationV1.Controllers
{
    public class TestController : ApiController
    {
        private TestService _ts;

        public TestController(TestService testService)
        {
            _ts = testService;
        }

        [Route("api/ping",Name ="GetPingResponse")]
        public async Task<HttpResponseMessage> GetPing()
        {
            var ping = await _ts.Getping();
            return Request.CreateResponse(HttpStatusCode.OK, ping);
        }

    }
}
