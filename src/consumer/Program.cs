using Consumer.Handlers;
using Database.Extensions;
using Microsoft.Extensions.Hosting;
using Server.ServiceBus;
using Server.ServiceBus.Events;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSqlLiteDatabase(builder.Configuration);
builder.Services.AddEventBusConsumer<FileUploadEvent, FileUploadEventConsumer>(TimeSpan.FromSeconds(5));

var host = builder.Build();
await host.RunAsync();
