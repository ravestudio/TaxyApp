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

            try
            {
                await clientSocket.ConnectAsync(serverHost, ServerPort);
                connected = true;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
            }

            if (connected)
            {
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

            Task readHead = this.ReadHeader();

            await readHead;
        }

        public async Task ReadHeader()
        {
            Windows.Storage.Streams.IInputStream stream = clientSocket.InputStream;

            DataReader reader = new DataReader(stream);

            System.Text.StringBuilder header = new StringBuilder();

            uint bufferLenght = 0;

            await reader.LoadAsync(100);

            bufferLenght = reader.UnconsumedBufferLength;

            header.Append(reader.ReadString(bufferLenght));

            //reader.DetachStream();
            //reader.Dispose();
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

            String message = string.Format("42[\"message\", \"{0}\"]", msg);

            byte[] data = Hybi10Encoder(message);

            writer.WriteBytes(data); 

            // Call StoreAsync method to store the data to a backing stream
            await writer.StoreAsync();

            // detach the stream and close it
            writer.DetachStream();
            writer.Dispose();

        }

        public void StrartListen()
        {
            Task t = Task.Run(InputProcess().Result);
        }

        private async Task<System.Action> InputProcess()
        {
            var th = Environment.CurrentManagedThreadId;
            Windows.Storage.Streams.IInputStream stream = clientSocket.InputStream;
            DataReader reader = new DataReader(stream);

            reader.InputStreamOptions = InputStreamOptions.Partial;

            byte b = 0;
            byte[] frameHeader = new byte[10]; // Header processing 
            int byteposition = 0;
            int framebytes = 0;
            int startreadbytes = 0; //Package to be reckoned with frames
            byte framelength = 0; //Payload length
            byte[] payload = null; //Payload data

            byte[] lenghtlen = null;

            do
            {
                await reader.LoadAsync(1);

                b = reader.ReadByte();

                if (framebytes == 0) { frameHeader[byteposition] = b; }

                if (byteposition==0)
                {
                    if (b == 129)
                    {
                        //"Frame is text"
                        /* TODO */
                        //Ping-Pong, binary
                    }
                }
                else
                    if (byteposition==1) { //Process second byte
                        if (b<126){framebytes=0x00 << 24 | b & 0xff; payload=new byte[framebytes];byteposition++;continue;} //small package (<125 bytes)
                        if (b==126){framelength=4;} //Middle package, >= 126 <= 65535 bytes
                        if (b==127){framelength=10;} //Big package > 65535 bytes
                    }

                if (framebytes == 0)
                { //Process frame length

                    if (byteposition == framelength)
                    {


                        lenghtlen = new byte[10];

                        for (int i = 2; i < byteposition; i++)
                        {
                            framebytes = (framebytes << 8) + (int)frameHeader[i];
                            lenghtlen[i - 2] = frameHeader[i];
                        }
                        payload = new byte[framebytes];

                    }
                }

                if (framebytes > 0)
                { //Write package at row

                    payload[startreadbytes] = b;
                    startreadbytes++;
                    if (startreadbytes == framebytes)
                    {// Package complete
                        String alltext = Encoding.UTF8.GetString(payload, 0, payload.Length);

                        
                        //Pattern pt = Pattern.compile("\\[\"message\",\"(.*)\"\\]");
                        //Matcher mt = pt.matcher(alltext);
                        //if (mt.find())
                        //{ //Message finding
                        //    if (mt.group().length() > 1)
                        //    {

                        //        onMessage(mt.group(1).replaceAll("\\\\\"", "\""));


                        //    }
                        //}

                        byteposition = 0;
                        framebytes = 0;
                        startreadbytes = 0;
                        framelength = 0;
                        payload = null;
                        frameHeader = new byte[10];
                        continue;

                    }
                }



                byteposition++;

            } while (true);
        }

        public async Task ReceiveDataAsync()
        {

            try
            {

                Windows.Storage.Streams.IInputStream stream = clientSocket.InputStream;

                DataReader reader = new DataReader(stream);

                reader.InputStreamOptions = InputStreamOptions.Partial;

                //uint bufferLength = reader.UnconsumedBufferLength;

                uint bufferLength = 0;

                do
                {
                    await reader.LoadAsync(50);
                    bufferLength = reader.UnconsumedBufferLength;

                    //read complete message
                    //int byteCount = reader.();

                    byte[] bytes = new byte[bufferLength];
                    reader.ReadBytes(bytes);

                } while (bufferLength == 50);

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
