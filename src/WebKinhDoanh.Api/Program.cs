kết using WebKinhDoanh.Api.Infrastructure;
using WebKinhDoanh.Application;
using WebKinhDoanh.Infrastructure;
using WebKinhDoanh.Infrastructure.Sql;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    if (allowedOrigins.Contains("*"))
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    }
    else
    {
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    }
}));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

var enableSwagger = app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("EnableSwagger");
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebKinhDoanh API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors();

if (enableSwagger)
{
    app.MapGet("/", () => Results.Redirect("/swagger"));
}
else
{
    app.MapGet("/", () => Results.Ok(new { status = "online", service = "WebKinhDoanh API" }));
}

app.MapControllers();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    await DatabaseInitializer.InitializeAsync(dbContext);
}

app.Run();

public partial class Program { }
