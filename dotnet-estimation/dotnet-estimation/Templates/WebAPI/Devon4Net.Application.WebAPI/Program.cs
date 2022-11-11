using Devon4Net.Application.WebAPI.Configuration;
using Devon4Net.Application.WebAPI.Configuration.Application;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Configuration;
using Devon4Net.Domain.UnitOfWork;
using Devon4Net.Infrastructure.CircuitBreaker;
using Devon4Net.Infrastructure.Grpc;
using Devon4Net.Infrastructure.Kafka;
using Devon4Net.Infrastructure.Logger;
using Devon4Net.Infrastructure.Middleware.Middleware;
using Devon4Net.Infrastructure.Swagger;
using WebSocketServer.CustomMiddleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.WebHost.InitializeDevonFw(builder.Host);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region devon services
var devonfwOptions =  builder.Services.SetupDevonfw(builder.Configuration);
builder.Services.SetupMiddleware(builder.Configuration);
builder.Services.SetupLog(builder.Configuration);
builder.Services.SetupSwagger(builder.Configuration);
builder.Services.SetupCircuitBreaker(builder.Configuration);
builder.Services.SetupCors(builder.Configuration);
builder.Services.SetupJwt(builder.Configuration);
builder.Services.SetupUnitOfWork(typeof(DevonConfiguration));
builder.Services.SetupLiteDb(builder.Configuration);
builder.Services.SetupRabbitMq(builder.Configuration);
builder.Services.SetupMediatR(builder.Configuration);
builder.Services.SetupKafka(builder.Configuration);
builder.Services.SetupGrpc(builder.Configuration);
builder.Services.SetupDevonDependencyInjection(builder.Configuration);
#endregion

var app = builder.Build();

#region devon app
app.ConfigureSwaggerEndPoint();
app.SetupMiddleware(builder.Services);
app.SetupCors();
if (devonfwOptions.ForceUseHttpsRedirection || (!devonfwOptions.UseIIS && devonfwOptions.Kestrel.UseHttps))
{
    app.UseHttpsRedirection();
}
#endregion

app.UseStaticFiles();
app.UseAuthorization();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);

/*
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            Console.WriteLine("Entered Test");
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next(context);
    }

});*/
//app.UseMiddleware<WebSocketMiddleware>();
app.MapControllers();

app.Run();
