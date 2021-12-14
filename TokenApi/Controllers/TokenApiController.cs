using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
namespace AcsTokenApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [RequiredScope("access_as_user")] //This is the scope we gave the AuthService when registering the application.
    public class TokenController : Controller
    {
        public TokenController(GraphServiceClient graphServiceClient, IOptions<MicrosoftGraphOptions> graphOptions)
        {
            _graphServiceClient = graphServiceClient;
        }

        private readonly GraphServiceClient _graphServiceClient;

        // GET: api/token
        [HttpGet]
        public string Get()
        {
            User user = _graphServiceClient.Me.Request().GetAsync().GetAwaiter().GetResult();
            return user.UserPrincipalName;
        }
    }
}
