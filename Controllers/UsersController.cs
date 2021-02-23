using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
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

            var confidentialClient = ConfidentialClientApplicationBuilder
                .Create("1aafdd77-ada0-47ad-bc7a-ce6e59cdb7b9")
                .WithAuthority($"https://login.microsoftonline.com/"+ "c31a1761-7d00-4b39-91a5-cc79c01bc9f6" + "/v2.0")
                .WithClientSecret("4-L6d8x.RYACAq2X.6xwdgzIu_NQm709uU")
                .Build();
            var scopes = new string[] { "https://graph.microsoft.com/.default" };
            GraphServiceClient graphServiceClient = null;
            Microsoft.Graph.User me = null;
            graphServiceClient =
                new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) => {
                    // Retrieve an access token for Microsoft Graph (gets a fresh token if needed).
                    var authResult = await confidentialClient
                        .AcquireTokenForClient(scopes)
                        .ExecuteAsync();

                    // Add the access token in the Authorization header of the API request.
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                     me = await graphServiceClient.Me
                    .Request()
                    .GetAsync();
                })
            );

            System.Console.WriteLine(me);
            foreach (var reference in _conversationReferences)
            {
                users.Add(new User()
                {
                    Name = reference.Value.User.Name,
                    ID = reference.Value.User.Id,
                    Email = reference.Value.User.Properties.Value<string>("Email"),
                    OrgID = reference.Value.User.Properties.Value<string>("OrgID")
                });
                //testcommit
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
        public string OrgID { get; set; }
    }
}
