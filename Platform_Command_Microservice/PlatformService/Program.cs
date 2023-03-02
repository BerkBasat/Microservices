using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if(builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SqlServer Db!");
    builder.Services.AddDbContext<AppDbContext>(o => 
    o.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMem Db!");
    builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();

app.UseEndpoints(endpoints =>
    {   
        endpoints.MapControllers();
        endpoints.MapGrpcService<GrpcPlatformService>();
    }
);

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();
