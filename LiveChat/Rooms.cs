using System;
using System.Collections.Generic;
using System.Text;

namespace LiveChatServer
{
    class ObiasRoom
    {
        string Name;
        int id;
        List<ObiasClient> UserList;
        public ObiasRoom(string name, int id)
        {
            Name = name;
            this.id = id;
            UserList = new List<ObiasClient>();
        }

        public void join(ObiasClient c)
        {
            c.room = this;
            c.isInRoom = true;
            msg(c.Username + " Joined the Room");
            c.msg("Joined " + Name + " Room");
            String user ="";
            foreach(ObiasClient a in UserList)
            {
                user += a.Username + "; ";
            }
            UserList.Add(c);
        }

        public void msg(ObiasClient SendedUser,String msg)
        {
            foreach(ObiasClient c in UserList)
            {
                c.msg(SendedUser.Username + ": " +msg);
            }
        }

        private void msg( String msg)
        {
            foreach (ObiasClient c in UserList)
            {
                c.msg(msg);
            }
        }

    }
}
