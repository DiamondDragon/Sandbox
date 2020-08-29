// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace EchoBot1.Bots
{
    public class UserProfile
    {
        public string Name { get; set; }
    }

    // Defines a state property used to track conversation data.
    public class ConversationData
    {
        // The time-stamp of the most recent incoming message.
        public string Timestamp { get; set; }

        // The ID of the user's channel.
        public string ChannelId { get; set; }

        // Track whether we have already asked the user's name
        public bool PromptedUserForName { get; set; } = false;
    }


    public class EchoBot : ActivityHandler
    {
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

        public EchoBot(ConversationState conversationState, UserState userState, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            _conversationState = conversationState;
            _userState = userState;
            _conversationReferences = conversationReferences;
        }

        private void AddConversationReference(Activity activity)
        {
            var conversationReference = activity.GetConversationReference();
            _conversationReferences.AddOrUpdate(conversationReference.User.Id, conversationReference, (key, newValue) => conversationReference);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        protected override Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            AddConversationReference(turnContext.Activity as Activity);

            return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        }

        //protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    // Get the state properties from the turn context.

        //    var conversationStateAccessors = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
        //    var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData(), cancellationToken);

        //    var userStateAccessors = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
        //    var userProfile = await userStateAccessors.GetAsync(turnContext, () => new UserProfile(), cancellationToken);

        //    if (string.IsNullOrEmpty(userProfile.Name))
        //    {
        //        // First time around this is set to false, so we will prompt user for name.
        //        if (conversationData.PromptedUserForName)
        //        {
        //            // Set the name to what the user provided.
        //            userProfile.Name = turnContext.Activity.Text?.Trim();

        //            // Acknowledge that we got their name.
        //            await turnContext.SendActivityAsync($"Thanks {userProfile.Name}. To see conversation data, type anything.");

        //            // Reset the flag to allow the bot to go through the cycle again.
        //            conversationData.PromptedUserForName = false;
        //        }
        //        else
        //        {
        //            // Prompt the user for their name.
        //            await turnContext.SendActivityAsync($"What is your name?");

        //            // Set the flag to true, so we don't prompt in the next turn.
        //            conversationData.PromptedUserForName = true;
        //        }
        //    }
        //    else
        //    {
        //        // Add message details to the conversation data.
        //        // Convert saved Timestamp to local DateTimeOffset, then to string for display.
        //        var messageTimeOffset = (DateTimeOffset)turnContext.Activity.Timestamp;
        //        var localMessageTime = messageTimeOffset.ToLocalTime();
        //        conversationData.Timestamp = localMessageTime.ToString();
        //        conversationData.ChannelId = turnContext.Activity.ChannelId.ToString();

        //        // Display state data.
        //        await turnContext.SendActivityAsync($"{userProfile.Name} sent: {turnContext.Activity.Text}");
        //        await turnContext.SendActivityAsync($"Message received at: {conversationData.Timestamp}");
        //        await turnContext.SendActivityAsync($"Message received from: {conversationData.ChannelId}");
        //    }

        //}

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = new CancellationToken())
        {
            await base.OnTurnAsync(turnContext, cancellationToken);


            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            AddConversationReference(turnContext.Activity as Activity);

            if (string.Equals(turnContext.Activity.Text, "wait", System.StringComparison.InvariantCultureIgnoreCase))
            {
                await turnContext.SendActivitiesAsync(
                    new Activity[] {
                        new Activity { Type = ActivityTypes.Typing },
                        new Activity { Type = "delay", Value= 3000 },
                        MessageFactory.Text("Finished typing", "Finished typing"),
                    },
                    cancellationToken);
                return;
            }

            if (turnContext.Activity.Text.Equals("image", StringComparison.OrdinalIgnoreCase))
            {
                var reply = MessageFactory.Text("This is an inline attachment.");
                reply.Attachments = new List<Attachment>() { GetInlineAttachment() };

                await turnContext.SendActivityAsync(reply, cancellationToken);

                return;
            }

            if (turnContext.Activity.Text.Equals("upload", StringComparison.OrdinalIgnoreCase))
            {
                var reply = MessageFactory.Text("This is an uploaded attachment.");
                reply.Attachments = new List<Attachment>() { await GetUploadedAttachmentAsync(turnContext, turnContext.Activity.ServiceUrl, turnContext.Activity.Conversation.Id, cancellationToken) };

                await turnContext.SendActivityAsync(reply, cancellationToken);

                return;
            }

            if (turnContext.Activity.Text.Equals("internet", StringComparison.OrdinalIgnoreCase))
            {
                var reply = MessageFactory.Text("This is an internet attachment.");
                reply.Attachments = new List<Attachment>() { GetInternetAttachment() };

                await turnContext.SendActivityAsync(reply, cancellationToken);

                return;
            }

            if (turnContext.Activity.Text.Equals("page", StringComparison.OrdinalIgnoreCase))
            {
                var reply = MessageFactory.Text("This is an internet page.");
                reply.Attachments = new List<Attachment>() { GetPageAttachment() };

                await turnContext.SendActivityAsync(reply, cancellationToken);

                return;
            }

            if (turnContext.Activity.Text.Equals("action", StringComparison.OrdinalIgnoreCase))
            {
                var reply = MessageFactory.Text("What is your favorite color?");

                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                    {
                        new CardAction() { Title = "Red", Type = ActionTypes.ImBack, Value = "Red" },
                        new CardAction() { Title = "Yellow", Type = ActionTypes.ImBack, Value = "Yellow" },
                        new CardAction() { Title = "Blue", Type = ActionTypes.ImBack, Value = "Blue" },
                    },
                };

                await turnContext.SendActivityAsync(reply, cancellationToken);

                return;
            }


            if (turnContext.Activity.Text.Equals("options", StringComparison.OrdinalIgnoreCase))
            {
                // Cards are sent as Attachments in the Bot Framework.
                // So we need to create a list of attachments for the reply activity.
                var attachments = new List<Attachment>();

                // Create a HeroCard with options for the user to interact with the bot.
                var heroCard = new HeroCard
                {
                    Text = "You can upload an image or select one of the following choices",
                    Buttons = new List<CardAction>
                    {
                        // Note that some channels require different values to be used in order to get buttons to display text.
                        // In this code the emulator is accounted for with the 'title' parameter, but in other channels you may
                        // need to provide a value for other parameters like 'text' or 'displayText'.
                        new CardAction(ActionTypes.ImBack, title: "1. Inline Attachment", value: "1"),
                        new CardAction(ActionTypes.ImBack, title: "2. Internet Attachment", value: "2"),
                        new CardAction(ActionTypes.ImBack, title: "3. Uploaded Attachment", value: "3"),
                    },
                };

                attachments.Add(heroCard.ToAttachment());

                var heroCard2 = new HeroCard
                {
                    Title = "BotFramework Hero Card",
                    Subtitle = "Microsoft Bot Framework",
                    Text = "Build and connect intelligent bots to interact with your users naturally wherever they are," +
                           " from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                    Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") },
                };

                attachments.Add(heroCard2.ToAttachment());


                var card = new SigninCard
                {
                    Text = "BotFramework Sign-in Card",
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign-in", value: "https://login.microsoftonline.com/") },
                };

                attachments.Add(card.ToAttachment());

                // Reply to the activity we received with an activity.
                var reply = MessageFactory.Attachment(attachments);

                reply.AttachmentLayout = AttachmentLayoutTypes.List;

                await turnContext.SendActivityAsync(reply, cancellationToken);

                return;
            }


            var replyText = $"Echo: {turnContext.Activity.Text}.";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        private static Attachment GetInternetAttachment()
        {
            // ContentUrl must be HTTPS.
            return new Attachment
            {
                Name = @"Resources\architecture-resize.png",
                ContentType = "image/png",
                ContentUrl = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png",
            };
        }

        private static Attachment GetPageAttachment()
        {
            // ContentUrl must be HTTPS.
            return new Attachment
            {
                Name = @"Internet Page",
                ContentType = "text/html",
                ContentUrl = "https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-add-media-attachments?view=azure-bot-service-4.0&tabs=csharp",
            };
        }


        private static async Task<Attachment> GetUploadedAttachmentAsync(ITurnContext turnContext, string serviceUrl, string conversationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
            {
                throw new ArgumentNullException(nameof(serviceUrl));
            }

            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            var connector = turnContext.TurnState.Get<IConnectorClient>() as ConnectorClient;
            var attachments = new Attachments(connector);
            var response = await attachments.Client.Conversations.UploadAttachmentAsync(
                conversationId,
                new AttachmentData
                {
                    Name = @"Resources\architecture-resize.png",
                    OriginalBase64 = GetImageData(),
                    Type = "image/png",
                },
                cancellationToken);

            var attachmentUri = attachments.GetAttachmentUri(response.Id);

            return new Attachment
            {
                Name = @"Resources\architecture-resize.png",
                ContentType = "image/png",
                ContentUrl = attachmentUri,
            };
        }

        private static Attachment GetInlineAttachment()
        {
            var imageData = GetImageData();

            return new Attachment
            {
                Name = @"Resources\architecture-resize.png",
                ContentType = "image/png",
                ContentUrl = $"data:image/png;base64,{Convert.ToBase64String(imageData)}"
            };
        }

        private static byte[] GetImageData()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EchoBot1.Resources.avatar-new.png"))
            {
                return ReadAllBytes(stream);
            }
        }

        public static byte[] ReadAllBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


    }
}
