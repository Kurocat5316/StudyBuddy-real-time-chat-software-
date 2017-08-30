using Serve.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using System.Net.Mail;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using Serve.Controllers;


namespace Serve.Controllers
{
    public class RegisterController : ApiController
    {
        

        [System.Web.Http.HttpPost]
        public bool createAccount(dynamic obj)
        {
            string id = Convert.ToString(obj.id);
            string password = Convert.ToString(obj.password);

            bool falg = StudentTableProcess.Register(id, password);

            EmailSend(id);

            return falg;
        }


        [System.Web.Http.HttpPost]
        public void ResetPassword(dynamic obj)
        {
            string email = Convert.ToString(obj.studentId) + "@uni.canberra.edu.au";
            string studentId = Convert.ToString(obj.studentId);

            var fromAddress = new MailAddress("studybuddy.uc@gmail.com", "STUDY BUDDY");
            var toAddress = new MailAddress(email, studentId);

            const string fromPassword = "UCSTUDYBUDDY2017";
            const string subject = "StudyBuddy";
            string content = "Dear {0}<BR/>We receive your reset password require, please click the <a href=\'{1}\' title=\"User Email Confirm\">link</a> to reset your password.";
            string body = string.Format(content, studentId, Url.Link("Default", new { Controller = "Home", Action = "EmailApply", Email = email }));

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            smtp.Send(message);
        }


        public void EmailSend(string studentId) {
            string email = studentId +"@uni.canberra.edu.au";
            //string email = studentId;
            var fromAddress = new MailAddress("studybuddy.uc@gmail.com", "STUDY BUDDY");
            var toAddress = new MailAddress(email, studentId);
            

            const string fromPassword = "UCSTUDYBUDDY2017";
            const string subject = "StudyBuddy";
            string content = "Dear {0}<BR/>Thank you for your registration, please click on the <a href=\'{1}\' title=\"User Email Confirm\">link</a> to complete your registration.";
            string body = string.Format(content, studentId, Url.Link("Default", new { Controller = "Home", Action = "EmailConfirm", Email = email }));


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            smtp.Send(message);
        }
    }
}
