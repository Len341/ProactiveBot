using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProactiveBot.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;
        private ClientCredentialProvider _authProvider;
        public UsersController(ConcurrentDictionary<string, ConversationReference> conversationReferences,
            IConfiguration configuration)
        {
            _conversationReferences = conversationReferences;

            //IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
            //.Create(configuration["MicrosoftAppId"])
            //.WithTenantId(configuration["TenantID"])
            //.WithClientSecret(configuration["MicrosoftAppPassword"])
            //.Build();

            //_authProvider = new ClientCredentialProvider(confidentialClientApplication);
        }


            public async Task<IActionResult> Get()
        {
            StringBuilder content = new StringBuilder();
            content.Append("<html><body><h1>USERS</h1>");
            foreach (var conversationReference in _conversationReferences)
            {
                content.Append("<h3>USERID: " + conversationReference.Value.User.Id.Substring(0,7)+"</h3>");
                content.Append("<h3>USERNAME: "+conversationReference.Value.User.Name == null ||
                    conversationReference.Value.User.Name  == string.Empty? "NoName":
                    conversationReference.Value.User.Name + "</h3>");
                content.Append("<hr><br>");
            }
            if (_conversationReferences.Count == 0)
            {
                content.Append("<h1>No Conversation refs</h1>");
            }
            content.Append("</body></html>");

            // Let the caller know proactive messages have been sent
            return new ContentResult()
            {
                Content = content.ToString(),
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
    }
}
