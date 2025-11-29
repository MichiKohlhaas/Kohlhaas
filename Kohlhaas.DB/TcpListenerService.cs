using System.Net;
using System.Net.Sockets;
using System.Text;
using Kohlhaas.Common.Interfaces;

namespace Kohlhaas.DB;

public class TcpListenerService : BackgroundService
{
    private const int MaxConnections = 10;
    private const int KB = 1024;
    private readonly ILogger<TcpListenerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _port;
    private TcpListener _tcpListener;
    private TcpListener _listener;
    private IPEndPoint _ipEndPoint;

    public TcpListenerService(ILogger<TcpListenerService> logger, IConfiguration config, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _port = config.GetValue<int>("TcpServer:Port", 9898);
        _ipEndPoint = new IPEndPoint(IPAddress.Any, _port);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _listener = new TcpListener(_ipEndPoint);
        
        try
        {
            _listener.Start(MaxConnections);
            while (!stoppingToken.IsCancellationRequested)
            {
                using TcpClient tcpClient = await _listener.AcceptTcpClientAsync(stoppingToken);
                using var scope =  _serviceProvider.CreateScope();
                var queryExecutor = scope.ServiceProvider.GetRequiredService<IQueryExecutor>();
                
                await using NetworkStream networkStream = tcpClient.GetStream();
                var buffer = new byte[KB];

                while (tcpClient.Connected)
                {
                    var bytesRead = await networkStream.ReadAsync(buffer, stoppingToken);
                    if (bytesRead == 0) break;
                    
                    
                }
                
            }

        }
        catch (SocketException)
        {

        }
        finally
        {
            _listener.Stop();
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