using Netboot.Utility.Logging.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Docker;

var builder = WebApplication.CreateBuilder(args);

// Configuring Ocelot.json for the right environment
builder.Host.ConfigureAppConfiguration((hostingContext, config)
    => config.AddJsonFile("ocelot.json", true, false));

// Configuring log operations
builder.Host.UseCustomSerilog();

// Configuring Ocelot
builder.Services
    .AddOcelot(builder.Configuration)
    .AddDocker();

var app = builder.Build();

app.UseRouting();

await app.UseOcelot();

app.Run();