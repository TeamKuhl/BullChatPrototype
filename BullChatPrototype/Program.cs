using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullChatPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            Chat chat = new Chat();
            String lastName = "";

            while (true)
            {
                String text = Console.ReadLine();
                string[] parts = text.Split(' ');

                switch (parts[0])
                {
                    case "/connect":
                        chat.Connect(parts[1]);
                        break;
                    case "/name":
                        chat.SetName(parts[1]);
                        Console.WriteLine("Name set to " + Chat.Name);
                        break;
                    case "/list":
                        chat.List();
                        break;
                    case "/msg":
                        IEnumerable<string> newParts = parts.Skip(2);
                        string msg = string.Join(" ", newParts);
                        chat.Message(parts[1], msg);
                        lastName = parts[1];
                        break;
                    default:
                        chat.Message(lastName, text);
                        break;
                }
            }
        }
    }
}
