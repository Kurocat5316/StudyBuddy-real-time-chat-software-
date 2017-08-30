using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Serve.Models;
using System.Data.Entity;
using System.Data.SqlClient;

namespace Serve.Models
{
    public static class RequireTableProcess
    {
        private static StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
        private static object tableLock = new object();

        public static void ApplyFriend(string senderId, string receiverId, string content) {
            lock (tableLock) {
                Request require = new Request();
                require.senderId = senderId;
                require.receiverId = receiverId;
                require.content = content;
                require.time = DateTime.Now;
                require.inforClass = "0";
                db.Request.Add(require);
                db.SaveChanges();
            }
        }
        public static void RejectInvest(string id, string senderId) {
            lock (tableLock) {
                SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
                conn.Open();
                string strSQLCommand = "DELETE " +
                                       "From Request " +
                                       "Where Request.receiverId = '" + id + "' AND Request.senderId = '" + senderId + "' AND Request.inforClass = '0'";
                SqlCommand command = new SqlCommand(strSQLCommand, conn);
                command.ExecuteNonQuery();

                strSQLCommand = "Select nickName " +
                                "From Student " +
                                "Where studentId = '" + id + "'";
                command = new SqlCommand(strSQLCommand, conn);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                string nickName = reader[0].ToString();
                conn.Close();

                Request rejectInfor = new Request();
                rejectInfor.senderId = id;
                rejectInfor.receiverId = senderId;
                rejectInfor.content = "Your friend add requirment to (" + id + " " + nickName + ") have been rejected";
                rejectInfor.time = DateTime.Now;
                rejectInfor.inforClass = "1";
                db.Request.Add(rejectInfor);
                db.SaveChanges();
            }
        }

        public static void DeleteFriend(string id, string friendId) {
            lock (tableLock) {
                SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
                conn.Open();
                string strSQLCommand = "DELETE " +
                                       "From Friend " +
                                       "Where (Friend.hostId = '" + id + "' AND Friend.friendId = '" + friendId + "') OR (Friend.hostId = '" + friendId + "' AND Friend.friendId = '" + id + "')";
                SqlCommand command = new SqlCommand(strSQLCommand, conn);
                command.ExecuteNonQuery();

                strSQLCommand = "Select nickName " +
                                "From Student " +
                                "Where studentId = '" + id + "'";
                command = new SqlCommand(strSQLCommand, conn);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                string nickName = reader[0].ToString();
                conn.Close();

                Request rejectInfor = new Request();
                rejectInfor.senderId = "Admin";
                rejectInfor.receiverId = friendId;
                rejectInfor.content = "You have been deleted by " + nickName + " from hie/her friend list";
                rejectInfor.time = DateTime.Now;
                rejectInfor.inforClass = "1";
                db.Request.Add(rejectInfor);
                db.SaveChanges();
            }
        }

        public static void ReadedSystem(string id, string senderId) {
            lock (tableLock) {
                SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
                conn.Open();
                string strSQLCommand = "DELETE " +
                                       "From Request " +
                                       "Where Request.receiverId = '" + id + "' AND Request.senderId = '" + senderId + "' AND Request.inforClass = '1'";
                SqlCommand command = new SqlCommand(strSQLCommand, conn);
                command.ExecuteNonQuery();
            }
        }

        public static void CreateNewFirend(string id, string senderId) {
            lock (tableLock) {
                SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
                conn.Open();
                string strSQLCommand = "DELETE " +
                                       "From Request " +
                                       "Where Request.receiverId = '" + id + "' AND Request.senderId = '" + senderId + "' AND Request.inforClass = '0'";
                SqlCommand command = new SqlCommand(strSQLCommand, conn);
                command.ExecuteNonQuery();
                conn.Close();

                Friend relation = new Friend();
                relation.hostId = id;
                relation.friendId = senderId;
                relation.chatId = id + senderId;
                relation.BlockOpetion = "0";
                db.Friend.Add(relation);

                Friend relation2 = new Friend();
                relation2.hostId = senderId;
                relation2.friendId = id;
                relation2.chatId = id + senderId;
                relation2.BlockOpetion = "0";
                db.Friend.Add(relation2);
                db.SaveChanges();
            }
        }
    }
}