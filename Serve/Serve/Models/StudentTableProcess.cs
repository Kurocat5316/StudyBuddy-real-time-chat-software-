using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Serve.Models;
using System.Data.Entity;

namespace Serve.Models
{
    public static class StudentTableProcess
    {
        public static StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
        private static object tableLock = new object();

        public static void Login(string id) {
            lock (tableLock) {
                Student student = db.Student.Find(id);
                if (student.logonStatus.Equals("0"))
                {
                    student.logonStatus = "1";
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public static void Logout(string id) {
            lock (tableLock)
            {
                Student student = db.Student.Find(id);
                if (student.logonStatus.Equals("1"))
                {
                    student.logonStatus = "0";
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public static void ChangNickName(string id, string newNickName) {
            lock (tableLock)
            {
                Student student = db.Student.Find(id);
                student.nickname = newNickName;

                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void ChangePassWord(string id, string newPassWord) {
            lock (tableLock)
            {
                Student student = db.Student.Find(id);
                student.password = newPassWord;

                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void UpdateLocation(string id, double x, double y) {
            lock (tableLock) {
                Student student = db.Student.Find(id);
                student.LocationX = x;
                student.LocationY = y;
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static bool Register(string id, string password) {
            lock (tableLock) {
                Student student = db.Student.Find(id);
                if (student == null)
                {
                    student = new Student();
                    student.studentId = id;
                    student.password = password;
                    student.nickname = id;
                    student.logonStatus = "0";
                    student.vertificationCheck = "0";
                    student.LocationX = 0;
                    student.LocationY = 0;
                    student.managerForbid = "0";
                    db.Student.Add(student);
                    db.SaveChanges();
                }
                else
                {
                    if (student.vertificationCheck.Equals("1"))
                        return false;
                    else
                    {
                        student.password = password;
                        db.Entry(student).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return true;
            }
        }

        public static void EmailConfirm(string id) {
            lock (tableLock) {
                Student student = db.Student.Find(id);
                student.vertificationCheck = "1";
                db.SaveChanges();
            }
        }

        public static string ResetPassWord(string id){
            lock (tableLock)
            {
                Student student = db.Student.Find(id);
                string password = System.Web.Security.Membership.GeneratePassword(10, 5);
                student.password = password;
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return password;
            }
        }
    }
}