using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            List<User> users = new List<User>();

            foreach(var reference in _conversationReferences)
            {
                users.Add(new User()
                {
                    Name = reference.Value.User.Name,
                    ID = reference.Value.User.Id,
                    Email = reference.Value.User.Properties.Value<string>("email")
                });
            }
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(users),
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
    }

    public class User
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string Email { get; set; }
    }
}
