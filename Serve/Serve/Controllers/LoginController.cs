using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Data.SqlClient;
using Serve.Models;

namespace Serve.Controllers
{
    
    public class LoginController : ApiController
    {
        [System.Web.Http.HttpPost]
        public string Test() {
            object a = "abc";
            int b = (int)a;
            return "process";
        }
        
        [System.Web.Http.HttpPost]
        public bool Login(dynamic obj)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string id = Convert.ToString(obj.id);
            string password = Convert.ToString(obj.password);

            Student student = db.Student.Find(id);

            if(student == null || student.vertificationCheck.Equals("0") || !student.password.Equals(password) || student.managerForbid.Equals("1"))
                return false;
            else {
                StudentTableProcess.Login(id);
                UserQueue.UserLogin(id);
                return true;
            }
        }

        [System.Web.Http.HttpPost]
        public void Logout(dynamic obj) {
            string id = Convert.ToString(obj.id);

            StudentTableProcess.Logout(id);
            UserQueue.UserLogOff(id);
        }

        [System.Web.Http.HttpPost]
        public FriendListData[] GetFriendList(dynamic obj)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string studentId = Convert.ToString(obj.id);


            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "Select d.friendId, d.nickname, d.chatId, c.sendTime, c.content, d.logonStatus " +
                                    "From (SELECT Friend.friendId, Friend.chatId, Student.nickname, Student.logonStatus " +
                                    "FROM Friend, Student "+
                                    "WHERE Friend.hostId = '" + studentId + "' AND Student.studentId = Friend.friendId) d " +
                                    "LEFT JOIN ( Select Chat.Id, Chat.chatId, Chat.sendTime, Chat.content "+
	                                    "From Chat "+
	                                    "INNER JOIN ( "+
		                                    "SELECT Chat.chatId, sendTime = max( Chat.sendTime ) "+
		                                    "FROM  Chat "+
		                                    "GROUP BY Chat.chatId "+
	                                    ") b ON Chat.chatId = b.chatId AND Chat.sendTime = b.sendTime ) c "+
                                   "ON c.chatId = d.chatId "+
                                    "ORDER BY d.logonStatus desc ,c.sendTime desc, d.nickname";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);

            SqlDataReader count = command.ExecuteReader();
            int i = 0;
            while (count.Read())
            {
                i++;
            }
            count.Close();

            SqlDataReader reader = command.ExecuteReader();


            FriendListData[] friendListData = new FriendListData[i];
            i = 0;
            while (reader.Read())
            {
                friendListData[i] = new FriendListData();
                friendListData[i].friendId = reader[0].ToString();
                friendListData[i].friendNickName = reader[1].ToString();
                friendListData[i].chatId = reader[2].ToString();
                friendListData[i].sendTime = reader[3].ToString();
                friendListData[i].lastMessage = reader[4].ToString();
                friendListData[i].loginStatus = reader[5].ToString();
                i++;
            }
            reader.Close();
            conn.Close();

            return friendListData;
        }

        [System.Web.Http.HttpPost]
        public void UpdateLocation(dynamic obj) {
            string id = Convert.ToString(obj.id);
            double locationX = Convert.ToDouble(obj.locationX);
            double locationY = Convert.ToDouble(obj.locationY);
            UserQueue.update(id, DateTime.UtcNow);
            StudentTableProcess.UpdateLocation(id, locationX, locationY);
        }


        [System.Web.Http.HttpPost]
        public SystemInforList[] GetSystemList(dynamic obj)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string id = Convert.ToString(obj.id);

            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "Select Request.senderId, Student.nickname, Request.content, Request.time, Request.inforClass " +
                                   "From Request, Student " +
                                   "Where Request.receiverId = '" + id + "' AND Request.senderId = Student.studentId";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);

            int i = 0;
            SqlDataReader count = command.ExecuteReader();
            while (count.Read())
            {
                i++;
            }
            count.Close();

            SystemInforList[] applyList = new SystemInforList[i];
            i = 0;

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                applyList[i] = new SystemInforList();
                applyList[i].senderId = reader[0].ToString();
                applyList[i].nickName = reader[1].ToString();
                applyList[i].content = reader[2].ToString();
                applyList[i].time = reader[3].ToString();
                applyList[i].inforClass = reader[4].ToString();
                i++;
            }
            reader.Close();
            conn.Close();

            return applyList;
        }


        [System.Web.Http.HttpPost]
        public void CreateNewRelation(dynamic obj) {
            string id = Convert.ToString(obj.id);
            string senderId = Convert.ToString(obj.senderId);

            RequireTableProcess.CreateNewFirend(id, senderId);
        }

        [System.Web.Http.HttpPost]
        public void RejectRequire(dynamic obj) {
            string id = Convert.ToString(obj.id);
            string senderId = Convert.ToString(obj.senderId);

            RequireTableProcess.RejectInvest(id, senderId);
        }

        [System.Web.Http.HttpPost]
        public void ReadedInfor(dynamic obj){
            string id = Convert.ToString(obj.id);
            string senderId = Convert.ToString(obj.senderId);

            RequireTableProcess.ReadedSystem(id, senderId);
        }
        
        [System.Web.Http.HttpPost]
        public void DeleteRelationship(dynamic obj)
        {
            string id = Convert.ToString(obj.id);
            string friendId = Convert.ToString(obj.friendId);

            RequireTableProcess.DeleteFriend(id, friendId);
        }
    }
}