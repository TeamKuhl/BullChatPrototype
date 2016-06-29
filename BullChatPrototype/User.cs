using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using CommunicationLibrary;

namespace BullChatPrototype
{
    public enum Status
    {
        Offline = 0,
        Connecting = 1,
        Failed = 2,
        Connected = 3
    }

    public class User
    {
        public String ConnectionId;
        public String Ip;
        public String Name;
        public Status Status;

        private bool incoming;
        private Client outgoing;

        public User(String ConnectionId, String ip)
        {
            // set status
            this.Status = Status.Connecting;

            // save id, server, ip
            this.ConnectionId = ConnectionId;
            this.Ip = ip;

            Console.WriteLine("Connecting to " + this.Ip + "...");

            // create client
            this.outgoing = new Client();
            bool connected = this.outgoing.connect(ip, 2345); // TODO: dynamic port
            if (!connected)
            {
                this.Status = Status.Failed;
                Console.WriteLine("Connection to " + this.Ip + " failed");
            }
            else
            {
                Console.WriteLine("Connected! Initializing connection...");
                this.outgoing.send("init", this.ConnectionId);
            }

            // check connection
            CheckConnection();
        }

        public void ClientConnected()
        {
            this.incoming = true;
            CheckConnection();
        }

        public void ClientDisconnected()
        {
            this.incoming = false;
        }

        public void CheckConnection()
        {
            if (incoming && outgoing != null)
            {
                Connected();
            }
        }

        public void Connected()
        {
            this.Status = Status.Connected;
            this.outgoing.send("hello", Chat.Name);
            Console.WriteLine("Done! Connection to " + this.Ip + " initialized!");
        }

        public void Received(String type, String message)
        {
            switch (type)
            {
                case "hello":
                    this.Name = message;
                    Console.WriteLine(this.Ip + " is " + this.Name);
                    break;
                case "message":
                    Console.WriteLine(this.Name + " => " + Chat.Name + ": " + message);
                    break;
            }
        }

        public void Send(String message)
        {
            this.outgoing.send("message", message);
        }
    }
}
