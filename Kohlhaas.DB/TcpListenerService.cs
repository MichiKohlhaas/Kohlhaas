using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Kohlhaas.Common.Interfaces;
using Kohlhaas.Common.Models;

namespace Kohlhaas.DB;

public class TcpListenerService : BackgroundService
{
    private const int MaxConnections = 10;
    private const int Kilobyte = 1024;
    private readonly ILogger<TcpListenerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private TcpListener _server;

    public TcpListenerService(ILogger<TcpListenerService> logger, IConfiguration config, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        var port = config.GetValue("Server:Port", 9898);
        var ipEndPoint = new IPEndPoint(IPAddress.Any, port);
        _server = new TcpListener(ipEndPoint);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _server.Start(MaxConnections);
            while (!stoppingToken.IsCancellationRequested)
            {
                using TcpClient tcpClient = await _server.AcceptTcpClientAsync(stoppingToken);
                using var scope =  _serviceProvider.CreateScope();
                var queryExecutor = scope.ServiceProvider.GetRequiredService<IQueryExecutor>();

                string receivedQuery;
                
                await using NetworkStream networkStream = tcpClient.GetStream();
                var buffer = new byte[Kilobyte];
                while (tcpClient.Connected && await networkStream.ReadAsync(buffer, stoppingToken) != 0)
                {
                    var incomingQuery = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
#if DEBUG  
                    Console.WriteLine(incomingQuery);
                    var response = Encoding.UTF8.GetBytes("Connection established");
                    await networkStream.WriteAsync(response, stoppingToken);
#endif
                    if (incomingQuery.Length == 0)
                    {
                        var noData = Encoding.UTF8.GetBytes("No data");
                        await networkStream.WriteAsync(noData, stoppingToken);
                    }
                    var clientQuery = new QueryRequest
                    {
                        Query = incomingQuery
                    };
                    await queryExecutor.ExecuteQueryAsync(clientQuery, stoppingToken);
                }
                
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        finally
        {
            _server.Stop();
        }
        
    }
    
    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            $"{nameof(TcpListenerService)} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}