using AIStreaming;
using AIStreaming.Hubs;

var builder = WebApplication.CreateBuilder(args);

var azureSignalRConnectionString = builder.Configuration["Azure:SignalR:ConnectionString"];

// Configure SignalR with Azure SignalR Service
if (!string.IsNullOrEmpty(azureSignalRConnectionString))
{
    // Using Azure SignalR Service
    builder.Services.AddSignalR(configure =>
    {
        configure.EnableDetailedErrors = true;
        configure.HandshakeTimeout = TimeSpan.FromSeconds(5);
    })
    .AddAzureSignalR(options =>
    {
        options.ConnectionString = azureSignalRConnectionString;
    });
}
else
{
    // Using local SignalR local (development)
    builder.Services.AddSignalR(configure =>
    {
        configure.EnableDetailedErrors = true;
        configure.HandshakeTimeout = TimeSpan.FromSeconds(5);
    });
}

builder.Services
    .AddSingleton<GroupAccessor>()
    .AddSingleton<GroupHistoryStore>();

// If you want to user OpenAI, uncomment below line and comment the line after that
//builder.Services.AddOpenAI(builder.Configuration);

// Using Azure OpenAI
builder.Services.AddAzureOpenAI(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

// Using Azure OpenAI
app.MapHub<GroupChatHubAzureOpenAI>("/groupChat");

// If you want to user OpenAI, uncomment here and comment the line above
//app.MapHub<GroupChatHubOpenAI>("/groupChat");

await app.RunAsync();
