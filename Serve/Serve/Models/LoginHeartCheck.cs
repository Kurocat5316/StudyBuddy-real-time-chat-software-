using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Serve.Models;

namespace Serve.Models
{
    public class LoginHeartCheck
    {
        private List<UserLoginInfor> checkList = new List<UserLoginInfor>();
        private List<UserLoginInfor> expiredList = new List<UserLoginInfor>();

        public void GetList(List<UserLoginInfor> userList) {
            if (userList.Count != 0) { 
                foreach(UserLoginInfor user in userList)
                    checkList.Add(user);
            }
        }

        public void CheckExpiredUser() {
            if (checkList.Count != 0) {
                foreach (UserLoginInfor user in checkList) {
                    if (user.time.AddMinutes(3) < DateTime.UtcNow)
                        expiredList.Add(user);
                }
            }
        }

        public void DeliveryExpiredUser() {
            if (expiredList.Count > 0)
            {
                UserQueue.RemoveExpiredUser(expiredList);
            }
        }
    }
}