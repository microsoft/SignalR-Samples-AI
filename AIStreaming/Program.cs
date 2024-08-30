using AIStreaming;
using AIStreaming.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSingleton<GroupAccessor>()
    .AddSingleton<GroupHistoryStore>()
    .AddAzureOpenAI(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapHub<GroupChatHub>("/groupChat");
app.Run();
