using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.SignalR;

namespace CMS
{
    [NotMapped]
    public class FeedHub : Hub
    {
        //private readonly FeedUpdate _feedUpdate;

        //public FeedHub() : this(FeedUpdate.Instance)
        //{

        //}

        //public FeedHub(FeedUpdate feedUpdate)
        //{
        //    _feedUpdate = feedUpdate;
        //}

        //public void BroadcastFeed()
        //{
        //    object sender = new object();

        //    _feedUpdate.BroadcastFeed(sender);
        //}

        public void BroadcastMessage(string text)
        {
            Clients.All.showMessage(text);
        }
    }
}