// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace ProactiveBot.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;


        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            _adapter = adapter;
            _conversationReferences = conversationReferences;
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;
        }

        public async Task<IActionResult> Get()
        {
            foreach (var conversationReference in _conversationReferences.Values)
            {
                if (Request.Query["userid"].ToString().Length > 0)
                {
                    if (Request.Query["userid"].ToString() == conversationReference.User.Id)
                    {
                        await ((BotAdapter)_adapter)
                            .ContinueConversationAsync(_appId, conversationReference,
                            BotCallback, default(CancellationToken));
                    }
                }
                else
                {
                    return new ContentResult()
                    {
                        Content = "{ \"body\": \"No user specified to send message to.\"}",
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
            }
            
            if(_conversationReferences.Values.Count == 0)
            {
                return new ContentResult()
                {
                    Content = "{ \"body\": \"No conversations found to send message to.\"}",
                    ContentType = "application/json",
                    StatusCode = (int) HttpStatusCode.OK
                };
            }
            // Let the caller know proactive messages have been sent
            return new ContentResult()
            {
                Content = "{ \"body\": \"Message has been sent.\"}",
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            // If you encounter permission-related errors when sending this message, see
            // https://aka.ms/BotTrustServiceUrl
            if (Request.Query["message"].ToString() != string.Empty)
            {
                await turnContext.SendActivityAsync(Request.Query["message"]);
            }
            else
            {
                await turnContext.SendActivityAsync("No message provided");
            }
        }
    }
}
