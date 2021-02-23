// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema.Teams;
using System;

namespace Microsoft.BotBuilderSamples
{
    public class ProactiveBot : ActivityHandler
    {
        // Message to send to users when the bot receives a Conversation Update event
        private const string WelcomeMessage = "Welcome to your Notification Bot";

        // Dependency injected dictionary for storing ConversationReference objects used in NotifyController to proactively message users
        public readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

        public ProactiveBot(ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            _conversationReferences = conversationReferences;
        }

        private void AddConversationReference(Activity activity, string email, string orgId)
        {
            var conversationReference = activity.GetConversationReference();
            conversationReference.User.Properties.Add("Email", email);
            conversationReference.User.Properties.Add("OrgID", orgId);
            _conversationReferences.AddOrUpdate(conversationReference.User.Id, conversationReference, (key, newValue) => conversationReference);
        }

        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            string email = "emulator@test.com";
            string orgId = null;
            if (! turnContext.Activity.ServiceUrl.ToLower().Contains("localhost"))
            {
                TeamsChannelAccount member = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken);
                email = member.Email;
                orgId = member.AadObjectId;
            }
            AddConversationReference(turnContext.Activity as Activity, email, orgId);
            await base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(WelcomeMessage), cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string email = "emulator@test.com";
            string orgId = null;
            if (!turnContext.Activity.ServiceUrl.ToLower().Contains("localhost"))
            {
                TeamsChannelAccount member = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken);
                orgId = member.AadObjectId;
                email = member.Email;
            }
            AddConversationReference(turnContext.Activity as Activity, email, orgId);
            // Echo back what the user said
            await turnContext.SendActivityAsync(MessageFactory.Text($"You sent '{turnContext.Activity.Text}'"), cancellationToken);
        }
    }
}
