using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
namespace LiveChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            LiveChat lc = new LiveChat();
            lc.start();

            Console.ReadKey();
            lc.stop();
        }
    }
}
