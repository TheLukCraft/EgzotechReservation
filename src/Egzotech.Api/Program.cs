using Egzotech.Api.Middlewares;
using Egzotech.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddInfrastructureLayer(builder.Configuration);

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();