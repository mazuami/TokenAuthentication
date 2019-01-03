		
		1) Create an empty Web project with "Web API" as template and "No Authentication" as authentication mechanism
		2) Install the below dlls
			
		Microsoft.AspNet.WebApi.Owin
		Microsoft.Owin.Host.SystemWeb
		Microsoft ASP.NET Identity Owin
		Microsoft.Owin.Security.Cookies
		Microsoft.AspNet.Identity.Owin
		Microsoft.Owin.Security.OAuth
		Update Newtonsoft.Json" version="12.0.1"
		
		3) Package Json should be as below
		
		Package.config set up will be as below
		
		<package id="Microsoft.AspNet.Identity.Core" version="2.2.2" targetFramework="net461" />
		<package id="Microsoft.AspNet.Identity.Owin" version="2.2.2" targetFramework="net461" />
		<package id="Microsoft.AspNet.WebApi" version="5.2.3" targetFramework="net461" />
		<package id="Microsoft.AspNet.WebApi.Client" version="5.2.7" targetFramework="net461" />
		<package id="Microsoft.AspNet.WebApi.Core" version="5.2.7" targetFramework="net461" />
		<package id="Microsoft.AspNet.WebApi.Owin" version="5.2.7" targetFramework="net461" />
		<package id="Microsoft.AspNet.WebApi.WebHost" version="5.2.3" targetFramework="net461" />
		<package id="Microsoft.CodeDom.Providers.DotNetCompilerPlatform" version="1.0.5" targetFramework="net461" />
		<package id="Microsoft.Net.Compilers" version="2.10.0" targetFramework="net461" developmentDependency="true" />
		<package id="Microsoft.Owin" version="4.0.0" targetFramework="net461" />
		<package id="Microsoft.Owin.Host.SystemWeb" version="4.0.0" targetFramework="net461" />
		<package id="Microsoft.Owin.Security" version="4.0.0" targetFramework="net461" />
		<package id="Microsoft.Owin.Security.Cookies" version="4.0.0" targetFramework="net461" />
		<package id="Microsoft.Owin.Security.OAuth" version="4.0.0" targetFramework="net461" />
		<package id="Newtonsoft.Json" version="12.0.1" targetFramework="net461" />
		<package id="Owin" version="1.0" targetFramework="net461" />
		
		
		4) Under App_Start , create a file "Startup.Auth.cs"
		
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TokenbasedAuthenticationV1.Provider;

namespace TokenbasedAuthenticationV1
{
    public partial class Startup
    {

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        static Startup()
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new OAuthAppProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(2),
                AllowInsecureHttp = true
            };
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}
		5) Create a folder Extentions and under Extentions create OwinContextExtentions.cs

using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TokenbasedAuthenticationV1.Extentions
{
    public static class OwinContextExtensions
    {
        public static string GetUserId(this IOwinContext ctx)
        {
            var result = "-1";
            var claim = ctx.Authentication.User.Claims.FirstOrDefault(c => c.Type == "UserID");
            if (claim != null)
            {
                result = claim.Value;
            }
            return result;
        }
    }
}
		
		6) Under Models create User.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TokenbasedAuthenticationV1.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

		7) Create a folder Service and UserServices.cs
		
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TokenbasedAuthenticationV1.Models;

namespace TokenbasedAuthenticationV1.Service
{
    public class UserServices
    {
        public User GetUserByCredentials(string email, string password)
        {
            User user = new User() { Id = "1", Email = "email@domain.com", Password = "password", Name = "Ole Petter Dahlmann" };
            if (user != null)
            {
                user.Password = string.Empty;
            }
            return user;
        }
    }
}		

		8) Create a file Startup.cs
		
		using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TokenbasedAuthenticationV1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}


		9) Create a folder Provider and file OAuthAppProvider.cs
	
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using TokenbasedAuthenticationV1.Models;
using TokenbasedAuthenticationV1.Service;

namespace TokenbasedAuthenticationV1.Provider
{
    public class OAuthAppProvider : OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                var username = context.UserName;
                var password = context.Password;
                var userService = new UserServices();
                User user = userService.GetUserByCredentials(username, password);
                if (user != null)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("UserID", user.Id)
                    };

                    ClaimsIdentity oAutIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                    context.Validated(new AuthenticationTicket(oAutIdentity, new AuthenticationProperties() { }));
                }
                else
                {
                    context.SetError("invalid_grant", "Error");
                }
            });
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }
            return Task.FromResult<object>(null);
        }
    }
}	
		