using Fleck;

var server = new WebSocketServer("ws://0.0.0.0:8181");

var wsConnection = new List<IWebSocketConnection>();

server.Start(ws =>
{
    ws.OnOpen = () =>
    {
        Console.WriteLine("New connection established.");
        wsConnection.Add(ws);
    };

    ws.OnMessage = message =>
    {
        //boardcast the message to all other connected clients
        foreach (var connection in wsConnection)
        {
            if (connection != ws)
            {
                connection.Send(message);
            }
        }
    };
});

WebApplication.CreateBuilder(args).Build().Run();

