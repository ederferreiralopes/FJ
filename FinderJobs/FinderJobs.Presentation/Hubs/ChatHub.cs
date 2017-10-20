using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinderJobs.Site.Hubs
{    
    [Authorize]
    public class ChatHub : Hub
    {
        public static List<string> Users = new List<string>();

        public void SendMessage(string to, string message)
        {
            List<string> toUsers = new List<string>();
            toUsers.Add(to);
            toUsers.Add(Context.User.Identity.Name);

            Clients.Users(toUsers).showMessage(Context.User.Identity.Name, message);
        }

        public void UpdateUsersOnline()
        {
            var userList = JsonConvert.SerializeObject(Users);
            Clients.All.updateUserOnline(userList);
        }

        public override Task OnConnected()
        {            
            string user = Context.User.Identity.Name;
            if (Users.IndexOf(user) == -1)
                Users.Add(user);
            UpdateUsersOnline();

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string user = Context.User.Identity.Name;
            if (Users.IndexOf(user) > -1)
                Users.Remove(user);
            UpdateUsersOnline();

            return base.OnDisconnected(stopCalled);
        }
    }
}