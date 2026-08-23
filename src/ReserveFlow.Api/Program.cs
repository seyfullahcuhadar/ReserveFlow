using ReserveFlow.Api.Options;
using ReserveFlow.Application;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;
using ReserveFlow.Infrastructure;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<HttpsOptions>()
    .Bind(builder.Configuration.GetSection(HttpsOptions.SectionName));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var httpsOptions = builder.Configuration
    .GetSection(HttpsOptions.SectionName)
    .Get<HttpsOptions>() ?? new HttpsOptions();

if (httpsOptions.Port is int httpsPort)
{
    builder.Services.AddHttpsRedirection(options => options.HttpsPort = httpsPort);
}
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(ReserveFlow.Application.DependencyInjection).Assembly);

    // EF Core DbContext / lambda kayıtları Wolverine codegen için "opaque";
    // bu tipler service location ile çözülür (Wolverine 6 varsayılanı NotAllowed).
    opts.CodeGeneration.AlwaysUseServiceLocationFor<IEventRepository>();
    opts.CodeGeneration.AlwaysUseServiceLocationFor<IUnitOfWork>();
});

var app = builder.Build();
app.UseMiddleware<ReserveFlow.Api.Middleware.ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ReserveFlow API v1");
    });
}

if (httpsOptions.Port.HasValue)
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
