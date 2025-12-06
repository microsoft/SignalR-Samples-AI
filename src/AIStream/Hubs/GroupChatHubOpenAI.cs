using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using OpenAI;
using System.Text;

namespace AIStreaming.Hubs;

public sealed class GroupChatHubOpenAI : Hub
{
    private readonly GroupAccessor _groupAccessor;
    private readonly GroupHistoryStore _history;

    private readonly OpenAIClient _openAI;
    private readonly OpenAIOptions _openAIOptions;

    public GroupChatHubOpenAI(
        GroupAccessor groupAccessor,
        GroupHistoryStore history,
        OpenAIClient openAI,
        IOptions<OpenAIOptions> openAIOptions)
    {
        _groupAccessor = groupAccessor ?? throw new ArgumentNullException(nameof(groupAccessor));
        _history = history ?? throw new ArgumentNullException(nameof(history));
        _openAI = openAI ?? throw new ArgumentNullException(nameof(openAI));
        _openAIOptions = openAIOptions?.Value ?? throw new ArgumentNullException(nameof(openAIOptions));
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _groupAccessor.Join(Context.ConnectionId, groupName);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _groupAccessor.Leave(Context.ConnectionId);

        return Task.CompletedTask;
    }

    public async Task Chat(string userName, string message)
    {
        if (!_groupAccessor.TryGetGroup(Context.ConnectionId, out var groupName))
        {
            throw new InvalidOperationException("Not in a group.");
        }

        if (message.Contains("@gpt"))
        {
            var id = Guid.NewGuid().ToString();
            var actualMessage = message.Substring(4).Trim();
            var messagesIncludeHistory = _history.GetOrAddGroupHistory(groupName!, userName, actualMessage);

            await Clients.OthersInGroup(groupName!).SendAsync("NewMessage", userName, message);

            var chatClient = _openAI.GetChatClient(_openAIOptions.Model);

            var totalCompletion = new StringBuilder();
            var lastSentTokenLength = 0;

            var streamingResult = chatClient.CompleteChatStreamingAsync(messagesIncludeHistory);

            await foreach (var update in streamingResult)
            {
                if (update.ContentUpdate.Count > 0)
                {
                    foreach (var contentPart in update.ContentUpdate)
                    {
                        totalCompletion.Append(contentPart.Text);
                    }

                    if (totalCompletion.Length - lastSentTokenLength > 20)
                    {
                        await Clients.Group(groupName!).SendAsync("newMessageWithId", "AI Assistant", id, totalCompletion.ToString());
                        lastSentTokenLength = totalCompletion.Length;
                    }
                }
            }

            _history.UpdateGroupHistoryForAssistant(groupName!, totalCompletion.ToString());

            await Clients.Group(groupName!).SendAsync("newMessageWithId", "AI Assistant", id, totalCompletion.ToString());
        }
        else
        {
            _history.GetOrAddGroupHistory(groupName!, userName, message);

            await Clients.OthersInGroup(groupName!).SendAsync("NewMessage", userName, message);
        }
    }
}
