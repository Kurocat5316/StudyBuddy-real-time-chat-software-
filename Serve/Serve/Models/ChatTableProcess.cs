using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Serve.Models;
using System.Data.Entity;
using System.Data.SqlClient;

namespace Serve.Models
{
    public static class ChatTableProcess
    {
        private static StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
        private static object tableLock = new object();

        public static void SetNewMessage(string chatId, string senderId, string content) {
            lock (tableLock)
            {
                Chat chat = new Chat();
                chat.chatId = chatId;
                chat.senderId = senderId;
                chat.sendTime = DateTime.Now;
                chat.content = content;

                db.Chat.Add(chat);
                db.SaveChanges();
            }
        }
    }
}