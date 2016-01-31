using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.SignalR;

namespace CMS.Infrastructure.Entities
{
    [NotMapped]
    public class FeedHub : Hub
    {
        private readonly FeedUpdate _feedUpdate;

        public FeedHub() : this(FeedUpdate.Instance)
        {

        }

        public void Send(string message)
        {
            // Call the addMessage method on all clients            
            Clients.All.showMessage(message);
        }
    }
}