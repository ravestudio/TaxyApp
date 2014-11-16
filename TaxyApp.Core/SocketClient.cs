using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;


namespace TaxyApp.Core
{
    public class SocketClient
    {
        private StreamSocket clientSocket;
        private bool connected = false;

        private String socketIOPatch = "/socket.io/websocket/";

        public SocketClient()
        {
            clientSocket = new StreamSocket();
        }

        public async Task ConnectAsync(string ServerHostname, string ServerPort)
        {
            HostName serverHost = new HostName(ServerHostname);
            // Try to connect to the 
            await clientSocket.ConnectAsync(serverHost, ServerPort);
            connected = true;

            DataWriter writer = new DataWriter(clientSocket.OutputStream);
            writer.WriteString("GET " + socketIOPatch + "?transport=websocket HTTP/1.1\r\n");
            writer.WriteString("Host: http://" + ServerHostname + ":" + ServerPort + "\r\n");
            writer.WriteString("Upgrade: websocket\r\n");
            writer.WriteString("Connection: Upgrade\r\n");
            writer.WriteString("Sec-WebSocket-Key: " + generateSocketKey() + "\r\n");
            writer.WriteString("Sec-WebSocket-Version: 13\r\n");
            writer.WriteString("Sec-WebSocket-Protocol: websocket\r\n");
            writer.WriteString("Origin: *\r\n\r\n");

            await writer.StoreAsync();

            // detach the stream and close it
            writer.DetachStream();
            writer.Dispose();
        }

        //Socket key generation
        private String generateSocketKey()
        {
            return new Guid().ToString();
        }

        public async Task SendAsync(string msg)
        {
            //string sendData = msg + Environment.NewLine;
            DataWriter writer = new DataWriter(clientSocket.OutputStream);

            byte[] data = Hybi10Encoder(msg);

            writer.WriteBytes(data); 

            // Call StoreAsync method to store the data to a backing stream
            await writer.StoreAsync();

            // detach the stream and close it
            writer.DetachStream();
            writer.Dispose();

        }

        public async Task ReceiveDataAsync()
        {
            try
            {
                DataReader reader = new DataReader(clientSocket.InputStream);

                reader.InputStreamOptions = InputStreamOptions.Partial;

                //uint byteCount = reader.UnconsumedBufferLength;

                await reader.LoadAsync(2500);

                //read complete message
                int byteCount = reader.ReadInt32();

                byte[] bytes = new byte[byteCount];
                reader.ReadBytes(bytes);

                //this.handleServerMessage(bytes);  //insert your handler here

                //detach stream so that it won't be closed when the datareader is disposed later
                reader.DetachStream();
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
            }

        }

        private byte[] Hybi10Encoder(String message)
        {
            byte[] rawData = new byte[0];

            rawData = Encoding.UTF8.GetBytes(message);

            int frameCount = 0;
            byte[] frame = new byte[10];
            frame[0] = (byte)129;

            if (rawData.Length <= 125)
            {
                frame[1] = (byte)(rawData.Length);
                frameCount = 2;
            }
            else if (rawData.Length >= 126 && rawData.Length <= 65535)
            {
                frame[1] = (byte)126;
                int len = rawData.Length;
                frame[2] = (byte)((len >> 8) & (byte)255);
                frame[3] = (byte)(len & (byte)255);
                frameCount = 4;

            }

            int bLength = frameCount + rawData.Length;

            byte[] reply = new byte[bLength];

            int bLim = 0;
            for (int i = 0; i < frameCount; i++)
            {
                reply[bLim] = frame[i];
                bLim++;
            }

            for (int i = 0; i < rawData.Length; i++)
            {
                reply[bLim] = (rawData[i]);
                bLim++;
            }
            return reply;

        }
    }
}
