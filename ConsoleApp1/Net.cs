using NetMQ;
using NetMQ.Sockets;
using System;
using System.Text;
using System.Threading;

namespace CustomNetMQ
{
    public interface IMessageSource
    {
        void Start(string name, CancellationToken cancellationToken);
    }

    public interface IMessageSourceClient
    {
        void SendMessage(string message, string ip);
        void GetUnreadMessages();
    }

    public class MessageSource : IMessageSource
    {
        public void Start(string name, CancellationToken cancellationToken)
        {
            using (var server = new PairSocket())
            {
                server.Bind("tcp://127.0.0.1:5555");

                Console.WriteLine($"Server {name} is waiting for messages...");

                while (!cancellationToken.IsCancellationRequested)
                {
                    string message = server.ReceiveFrameString();
                    Console.WriteLine($"Received message: {message}");
                }
            }
        }
    }

    public class MessageSourceClient : IMessageSourceClient
    {
        public void SendMessage(string message, string ip)
        {
            string serverEndpoint = $"tcp://{ip}:5555";

            using (var client = new PairSocket())
            {
                client.Connect(serverEndpoint);
                client.SendFrame(message);
                Console.WriteLine($"Sent message '{message}' to server at {serverEndpoint}");
            }
        }

        public void GetUnreadMessages()
        {
            string serverEndpoint = $"tcp://127.0.0.1:5555";

            using (var client = new PairSocket())
            {
                client.Connect(serverEndpoint);
                client.SendFrame("getUnreadMessages");

                string response = client.ReceiveFrameString();
                Console.WriteLine($"Received response: {response}");
            }
        }
    }
}