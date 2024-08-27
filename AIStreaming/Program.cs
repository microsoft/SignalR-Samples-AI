using AIStreaming.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AIStreaming
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            builder.Services.AddSingleton<GroupAccessor>()
                .AddSingleton<GroupHistoryStore>()
                .AddAzureOpenAI(builder.Configuration);

            var app = builder.Build();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapHub<GroupChatHub>("/groupChat");
            app.Run();
        }
    }
}
