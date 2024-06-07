using ConsoleApp1;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var messageSourceClient = new CustomNetMQ.MessageSourceClient();

            for (int i = 0; i < 10; i++)
            {
                messageSourceClient.SendMessage("Denis" + i, "127.0.0.1");
            }

            Console.ReadLine();

            messageSourceClient.GetUnreadMessages();

            Console.ReadLine();
        }

        public static void SentMessage(string From, int i, string ip = "127.0.0.1")
        {
            UdpClient udpClient = new UdpClient();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);

            while (true)
            {
                string messageText = "";

                Console.Clear();
                Console.WriteLine("Введите сообщение (для выхода введите 'exit'):");
                messageText = Console.ReadLine();

                ICommand command = new ClientSendCommand(udpClient, iPEndPoint, messageText);
                command.Execute();
            }
        }
        public static void GetUnreadMessages(UdpClient udpClient, IPEndPoint iPEndPoint, string ip = "127.0.0.1")
        {
            string messageText = "getUnreadMessages";
            ICommand command = new ClientSendCommand(udpClient, iPEndPoint, messageText);

            command.Execute();

            byte[] buffer = udpClient.Receive(ref iPEndPoint);
            string messageTextResponse = Encoding.UTF8.GetString(buffer);
            MessageList messageList = MessageList.DeserializeFromJson(messageTextResponse);

            messageList.PrintMessages();
        }

    }
}