using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TokenbasedAuthenticationV1.Controllers
{
    public class ProductController : ApiController
    {
        [Authorize]
        [Route("api/products" ,Name = "GetProducts")]
        public IEnumerable<string> GetProductList()
        {
            return new string[] { "Product1", "Product1", "Product1" };
        }
    }
}
