using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using CommunicationLibrary;

namespace BullChatPrototype
{
    public class Chat
    {
        public static String Name = "Noname";
        private Server server;
        private Dictionary<int, User> users = new Dictionary<int,User>();
        private Dictionary<String, User> pending = new Dictionary<string,User>(); 

        public Chat()
        {
            // start a server
            server = new Server();
            server.start(2345); // TODO: dynamic port
            Console.WriteLine("BullChatServer started. Listening on " + 2345);

            server.onReceive += new ServerReceiveHandler(Received);
        }

        public void SetName(String name)
        {
            Chat.Name = name;
        }

        public void Connect(String ip)
        {
            String connectionId = System.Guid.NewGuid().ToString();
            pending.Add(connectionId, new User(connectionId, ip));
        }

        public void List()
        {
            foreach (var item in this.users)
            {
                Console.WriteLine(item.Value.Ip + " | " + item.Value.Name);
            }
        }

        public void Message(String to, String message)
        {
            foreach (var item in users)
            {
                User user = item.Value;
                if (user.Ip == to || user.Name == to)
                {
                    user.Send(message);
                    Console.WriteLine(Chat.Name + " => " + user.Name + ": " + message);
                    break;
                }
            }
        }

        public void Received(TcpClient tcpClient, String type, String message)
        {
            switch (type)
            {
                case "init":
                    if (pending.ContainsKey(message))
                    {
                        User user = pending[message];
                        user.ClientConnected();
                        pending.Remove(message);
                        users.Add(tcpClient.Client.GetHashCode(), user);
                    }
                    else
                    {
                        String ip = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                        User user = new User(message, ip);
                        user.ClientConnected();
                        users.Add(tcpClient.Client.GetHashCode(), user);
                    }
                    break;
                default:
                    if (users.ContainsKey(tcpClient.Client.GetHashCode()))
                    {
                        users[tcpClient.Client.GetHashCode()].Received(type, message);
                    }
                    break;
            }
        }


    }
}
