using Kohlhaas.DB;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<TcpListenerService>();
builder.Services.AddSingleton<Kohlhaas.Common.Interfaces.IQueryExecutor, QueryExecutorService>();

var host = builder.Build();

MonitorLoop monitorLoop = host.Services.GetRequiredService<MonitorLoop>();
monitorLoop.StartMonitorLoop();

host.Run();