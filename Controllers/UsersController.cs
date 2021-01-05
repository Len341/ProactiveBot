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
        public UsersController(ConcurrentDictionary<string, ConversationReference> conversationReferences,
            IConfiguration configuration)
        {
            _conversationReferences = conversationReferences;
        }


            public async Task<IActionResult> Get()
        {
            // Let the caller know proactive messages have been sent
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(_conversationReferences),
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
    }
}
