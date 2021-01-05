using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
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
        public UsersController(ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            _conversationReferences = conversationReferences;
        }
        public async Task<IActionResult> Get()
        {
            StringBuilder content = new StringBuilder();
            content.Append("<html><body>");
            foreach (var conversationReference in _conversationReferences)
            {
                content.Append("<h1>" + JsonSerializer.Serialize(conversationReference) + "</h1>");
            }
            if(_conversationReferences.Count == 0)
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
