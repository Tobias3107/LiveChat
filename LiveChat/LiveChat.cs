using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace LiveChatServer
{
    class LiveChat
    {
        WebSocketServer wss;
        // True is Free
        public static Dictionary<LiveSocket, bool> Support = new Dictionary<LiveSocket, bool>();
        public static Dictionary<LiveSocket, LiveSocket> SupportPartner = new Dictionary<LiveSocket, LiveSocket>();

        public LiveChat()
        {
            wss = new WebSocketServer(4649);
        }

        public void start()
        {
            wss.AddWebSocketService<LiveSocket>("/LiveSocket");
            wss.Start();
        }

        public void stop()
        {
            wss.Stop();
        }
    }

    class LiveSocket : WebSocketBehavior
    {
        bool isSupport = false;
        String name = null;
        String supportkey = null;
        LiveSocket sup = null;
        protected override void OnOpen()
        {
            Console.WriteLine("Open Something");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (isSupport)
            {
                String[] data = e.Data.Split(';');
                string msg;
                foreach (String s in data)
                {
                    String[] str = s.Split("=");
                    if (str[0].Contains("msg"))
                    {
                        msg = str[1];
                        Send(name + ": " + msg);
                        if (!LiveChat.Support[this])
                        {
                            LiveChat.SupportPartner[this].Send(name + ": " + msg);
                        } else
                        {
                            Send("No Partner - " + msg + " - " + LiveChat.Support[this]);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Data : " + e.Data);
                String[] data = e.Data.Split(';');
                string msg;
                foreach (String s in data)
                {
                    String[] str = s.Split("=");
                    if (str.Length == 2)
                    {
                        if (str[0].Contains("name"))
                        {
                            name = str[1];
                        }
                        else if (str[0].Contains("msg"))
                        {
                            msg = str[1];
                            if (sup != null)
                            {
                                sup.Send(name + ": " + msg);
                                Send(name + ": " + msg);
                            }
                            else
                            {
                                Send("No Support there");
                                this.Close();
                            }
                        }
                        else if (str[0].Contains("key"))
                        {
                            supportkey = str[1];
                            CheckKey();
                        }
                    }
                    else if (str.Length == 1)
                    {
                        if (str[0].Contains("openchat"))
                        {
                            OpenChatWithSupport();
                        }
                    } 
                }
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("Closed");
            if(isSupport)
            {
                LiveChat.Support.Remove(this);
                LiveChat.SupportPartner.Remove(this);
            }
        }

        private void OpenChatWithSupport()
        {
            if(LiveChat.Support.Count > 0)
            {
                LiveSocket sup;
                int i = 0;
                do
                {
                    Random rnd = new Random();
                    
                    List<LiveSocket> f = Enumerable.ToList<LiveSocket>(LiveChat.Support.Keys);
                    sup = f[rnd.Next(0, LiveChat.Support.Count)];
                } while (!LiveChat.Support[sup]);
                sup.Send("OPEN CHAT WITH " + name);
                LiveChat.Support[sup] = false;
                LiveChat.SupportPartner.Add(sup, this);
                this.sup = sup;
                Console.WriteLine(sup.name + " => " + name);
            } else
            {
                Send("No Support Online");
                this.Close();
            }
        }

        private void CheckKey()
        {
            if(supportkey == "TestKey")
            {
                LiveChat.Support.Add(this, true);
                isSupport = true;
            }
        }
    }
}
