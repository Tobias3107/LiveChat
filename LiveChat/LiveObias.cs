using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
namespace LiveChatServer
{
    class LiveObias
    {
        public Dictionary<int, ObiasRoom> ChatRooms;
        public Dictionary<String, ObiasClient> User;
        public List<ObiasClient> Support; 
        bool isListen;
        TcpListener server;
        public LiveObias(String name, int port)
        {
            server = new TcpListener(port);
            User = new Dictionary<string, ObiasClient>();
            Support = new List<ObiasClient>();
        }

        public void start()
        {
            server.Start();


            isListen = true;
            listen();
        }

        private void init()
        {
            
        }

        public void stop()
        {
            isListen = false;
        }

        private void listen()
        {
            while(isListen)
            {
                Console.Write("Waiting for a connection... ");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected! ");
                new Thread(new ThreadStart(new ObiasClient(client, this).start)).Start();
            }
        }

        public ObiasRoom OpenSupportRoom(ObiasClient c)
        {
            Random rnd = new Random();
            int RoomNumber = 3;
            ObiasRoom r = new ObiasRoom("Support", RoomNumber);
            r.join(c);
            r.join(Support[rnd.Next(0, Support.Count)]);
            return r;
        }
    }

    class ObiasClient
    {

        // Buffer for reading data
        Byte[] bytes = new Byte[256];
        String data = null;
        private TcpClient client;
        private LiveObias live;
        NetworkStream stream;

        // User data
        public bool loggedIn = false;
        public String Username;
        public bool isInRoom = false;
        public ObiasRoom room;
        public ObiasClient(TcpClient client, LiveObias l)
        {
            this.client = client;
            this.live = l;
            stream = client.GetStream();
        }

        public void start()
        {
            data = null;

            // Get a stream object for reading and writing

            int i;
            stream.Write(Encoding.Default.GetBytes("Welcome to The LiveObias Server" + Environment.NewLine));
            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                // Process the data sent by the client.
                command(data);
                
            }

        }

        private void command(String cmd)
        {
            String[] args = cmd.Split(" ");
            switch(args[0].ToLower())
            {
                case "join":
                    if(loggedIn)
                    {
                        if (args.Length >= 2)
                        {
                            msg("Joined room " + args[1]);
                        }
                        else
                        {
                            msg("No room given");
                        }
                    } else
                    {
                        msg("you musst logged in");
                    }
                    break;
                case "login":
                     if(args.Length == 2)
                    {
                        Username = args[1];
                        live.User.Add(Username, this);
                        msg("Logged as " + Username);
                        loggedIn = true;
                    }
                    break;
                case "loginassupport":
                    if (args.Length == 2)
                    {
                        Username = args[1];
                        live.Support.Add(this);
                        msg("Logged as " + Username);
                        loggedIn = true;
                    }
                    break;
                case "send":
                    if(!isInRoom)
                    {
                        if (args.Length >= 3)
                        {
                            if (live.User.ContainsKey(args[1]))
                            {
                                String msg = "- ";
                                for (int i = 2; i < args.Length; i++)
                                {
                                    msg = msg + args[i] + " ";
                                }
                                live.User[args[1]].msg(Username + ": " + msg);
                            }
                            else
                            {
                                this.msg("User Existiert nicht");
                            }

                        }
                    } else if (args.Length >= 2 && isInRoom)
                    {
                        String msg = " ";
                        for (int i = 1; i < args.Length; i++)
                        {
                            msg = msg + args[i] + " ";
                        }
                        room.msg(this, msg);
                    }
                    break;
                case "opensupport":
                    if(args.Length == 1)
                    {
                        if(loggedIn)
                        {
                            ObiasRoom room = live.OpenSupportRoom(this);
                            isInRoom = true;
                            this.room = room;
                        }
                    }
                    break;

            }
        }

        private void joinRoom(int Room)
        {

        }

        public void msg(String Message)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(Message +Environment.NewLine);
            stream.Write(msg,0, msg.Length);
        }
    }
}
