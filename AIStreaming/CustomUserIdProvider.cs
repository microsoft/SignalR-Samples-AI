using Microsoft.AspNetCore.SignalR;

namespace AIStreaming
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext()?.Request.Query["userId"] ?? connection.ConnectionId;
        }
    }
}
