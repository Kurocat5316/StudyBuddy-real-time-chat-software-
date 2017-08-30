using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Serve.Models;

namespace Serve.Models
{
    public static class UserQueue
    {
        private static List<UserLoginInfor> onlineQueue = new List<UserLoginInfor>();
        private static object userLock = new object();

        public static void UserLogin(string userId){
            lock(userLock){
                onlineQueue.Add(new UserLoginInfor() { id = userId, time = DateTime.UtcNow });
                StudentTableProcess.Login(userId);
            }
        }

        public static void UserLogOff(string userId) {
            lock (userLock) {
                onlineQueue.Remove(new UserLoginInfor { id = userId });
                StudentTableProcess.Logout(userId);
            }
        }

        public static List<UserLoginInfor> CopyOnlineList()
        {
            lock (userLock)
            {
                return onlineQueue;
            }
        }

        public static void RemoveExpiredUser(List<UserLoginInfor> expiredList) {
            lock (userLock) {
                foreach (UserLoginInfor user in expiredList) {
                    onlineQueue.Remove(user);
                    StudentTableProcess.Logout(user.id);
                }
            }
        }
        public static void update(string id, DateTime time){
            lock (userLock) {
                UserLoginInfor updateInfor = new UserLoginInfor();
                updateInfor.id = id;
                updateInfor.time = time;
                int index = -1;
                foreach (UserLoginInfor user in onlineQueue) {
                    index++;
                    if (user.id.Equals(id))
                        break;
                    
                }

                if (index == onlineQueue.Count - 1 )
                {
                    if(onlineQueue[index].id != id)
                     onlineQueue.Add(updateInfor);
                }
                else
                    onlineQueue[index] = updateInfor;
            }
        }
    }
}