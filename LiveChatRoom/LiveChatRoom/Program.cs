using Fleck;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatRoom API", Version = "v1" });
});

var app = builder.Build();

// setup app
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatRoom API V1");
        c.RoutePrefix = "swagger";
    });
}

// read static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();




var server = new WebSocketServer("ws://0.0.0.0:8181");
var wsConnections = new List<IWebSocketConnection>();

server.Start(ws =>
{
    ws.OnOpen = () =>
    {
        Console.WriteLine("New connection established.");
        wsConnections.Add(ws);
    };

    ws.OnMessage = message =>
    {
        Console.WriteLine($"Received: {message}");
        foreach (var conn in wsConnections.ToList())
        {
            if (conn != ws && conn.IsAvailable)
                conn.Send(message);
        }
    };

    ws.OnClose = () =>
    {
        Console.WriteLine("Connection closed.");
        wsConnections.Remove(ws);
    };
});


app.Run();
