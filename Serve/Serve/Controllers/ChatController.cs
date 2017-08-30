using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Data.SqlClient;
using Serve.Models;
using System.Threading;

namespace Serve.Controllers
{
    public class ChatController : ApiController
    {
        

        // POST api/<controller>
        [System.Web.Http.HttpPost, ActionName("review")]
        public ChatMessage[] review(dynamic obj)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string chatId = Convert.ToString(obj.chatId);
            

            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "SELECT nickname, sendTime, content " +
                                    "FROM Chat, Student " +
                                    "WHERE chatId = '" + chatId + "' AND senderId = studentId ORDER BY sendTime";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);

            SqlDataReader count = command.ExecuteReader();
            int i = 0;
            while (count.Read())
            {
                i++;
            }
            count.Close();

            SqlDataReader reader = command.ExecuteReader();

            ChatMessage[] chatMessage = new ChatMessage[i];
            i = 0;
            while (reader.Read())
            {
                chatMessage[i] = new ChatMessage();
                chatMessage[i].nickName = reader[0].ToString();
                chatMessage[i].time = reader[1].ToString();
                chatMessage[i].content = reader[2].ToString();
                i++;
            }
            conn.Close();

            return chatMessage;
        }

        [System.Web.Http.HttpPost, ActionName("sendMessage")]
        public bool sendMessage(dynamic obj)
        {
            string chatId = Convert.ToString(obj.chatId);
            string senderId = Convert.ToString(obj.id);
            string content = Convert.ToString(obj.content);

            ChatTableProcess.SetNewMessage(chatId, senderId, content);

            return true;
        }

        [System.Web.Http.HttpPost, ActionName("newMessage")]
        public ChatMessage[] newMessage(dynamic obj)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string chatId = Convert.ToString(obj.chatId);
            DateTime time = Convert.ToDateTime(obj.time);

            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "SELECT nickName, sendTime, content " +
                                    "FROM Chat, Student " +
                                    "WHERE chatId = '" + chatId + "'AND sendTime > '" + time + "' AND studentId = senderId ORDER BY sendTime";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);
            int i = 0;
            for (int a = 0; a < 50; a++) {
                SqlDataReader count = command.ExecuteReader();
                while (count.Read())
                {
                    i++;
                }
                count.Close();
                if (i != 0)
                    break;
                else
                    Thread.Sleep(300);
            }
            

            SqlDataReader reader = command.ExecuteReader();

            ChatMessage[] chatMessage = new ChatMessage[i];
            if (i > 0)
            {
                
                i = 0;
                while (reader.Read())
                {
                    chatMessage[i] = new ChatMessage();
                    chatMessage[i].nickName = reader[0].ToString();
                    chatMessage[i].time = reader[1].ToString();
                    chatMessage[i].content = reader[2].ToString();
                    i++;
                }
            }
            conn.Close();

            return chatMessage;
        }

    }
}