using Avalonia.Threading;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IconInTheTaskbar.Models.IPC
{
    public class IPCServer
    {
        private TcpListener? _server = null;
        private Action RestoreMainWindow;

        /// <summary>
        /// The magic wakeup string is sent to an existing app instance
        /// to signal that the existing instances main window should be shown.
        /// This is only used if the new instance was started without any 
        /// command line arguments.
        /// </summary>
        public static readonly string MAGIC_WAKEUP_STR = "wakeup!";

        /// <summary>
        /// Creates a new <c>IPCServer</c> instance and starts the TCP server
        /// to listen for incoming IPC queries from other app instances.
        /// </summary>
        /// <param name="ip">The IP address to listen on, usually 127.0.0.1 (localhost).</param>
        /// <param name="port">The TCP port to listen on. Default is 13000.</param>
        public IPCServer(string ip, int port, Action RestoreMainWindow)
        {
            this.RestoreMainWindow = RestoreMainWindow;
            IPAddress localAddr = IPAddress.Parse(ip);
            _server = new TcpListener(localAddr, port);
            _server.Start();
        }

        /// <summary>
        /// Starts listening for incoming IPC queries from other app instances.
        /// </summary>
        public void StartListening()
        {
            try
            {
                Log.Information("IPC server starts listening...");

                while (true)
                {
                    TcpClient client = _server.AcceptTcpClient();
                    Log.Information("IPC client connected!");

                    Thread handleIncomingIPCThread = new Thread(new ParameterizedThreadStart(HandleIncomingIPC));
                    handleIncomingIPCThread.Start(client);
                }
            }
            catch (SocketException e)
            {
                Log.Error("IPC server SocketException: {0}", e.Message);
                _server.Stop();
            }
        }

        /// <summary>
        /// Processes incoming IPC data.
        /// </summary>
        /// <param name="obj">The <c>TcpClient</c> object representing the incoming connection.</param>
        private void HandleIncomingIPC(object? obj)
        {
            // MainWindow mainWindow = AvaloniaLocator.Current.GetService<MainWindow>();
            TcpClient client = (TcpClient)obj;
            var stream = client.GetStream();
            string imei = string.Empty;
            string data = null;
            byte[] bytes = new byte[256];
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);

                    Log.Information("IPC server received data: {IPCData}", data);

                    Dispatcher.UIThread.Post(() =>
                    {

                        Log.Information("IPC server showing main window!");

                        RestoreMainWindow?.Invoke();
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception in {MethodName}: {IPCServerException}",
                    nameof(HandleIncomingIPC), e.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
