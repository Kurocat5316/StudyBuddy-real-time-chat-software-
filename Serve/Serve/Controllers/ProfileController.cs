using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Data.SqlClient;
using Serve.Models;
using System.Web.Script.Serialization;

namespace Serve.Controllers
{
    public class ProfileController : ApiController
    {
        
        // POST api/<controller>
        [System.Web.Http.HttpPost]
        public string GetNickName(dynamic obj)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string studentId = Convert.ToString(obj.id);
            string nickName = null;

            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "SELECT nickName " +
                                    "FROM Student " +
                                    "WHERE studentId = '" + studentId + "'";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                nickName = reader[0].ToString();
            reader.Close();
            conn.Close();

            return nickName;
        }

        [System.Web.Http.HttpPost]
        public ProfileUnit[] GetUnit(dynamic obj)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string studentId = Convert.ToString(obj.id);

            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "SELECT Unit.unitCode, Unit.unitName " +
                                   "FROM  UnitTaken, Unit " +
                                   "WHERE UnitTaken.studentId = '" + studentId + "' AND Unit.unitCode = UnitTaken.unitCode Order By Unit.unitCode";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);
            SqlDataReader count = command.ExecuteReader();
            int i = 0;
            while (count.Read()) {
                i++;
            }
            count.Close();

            ProfileUnit[] profileUnit = new ProfileUnit[i];

            if (i > 0)
            {
                SqlDataReader reader = command.ExecuteReader();
                i = 0;
                while (reader.Read())
                {
                    profileUnit[i] = new ProfileUnit();
                    profileUnit[i].unitCode = reader[0].ToString();
                    profileUnit[i].unitName = reader[1].ToString();
                    i++;
                }
                reader.Close();
            }
            conn.Close();

            return profileUnit;
        }

        [System.Web.Http.HttpPost]
        public void ChangPassWord(dynamic obj){
            string id = Convert.ToString(obj.id);
            string password = Convert.ToString(obj.password);

            StudentTableProcess.ChangePassWord(id, password);
        }

        [System.Web.Http.HttpPost]
        public void ChangeNickName(dynamic obj) {
            string id = Convert.ToString(obj.id);
            string nickName = Convert.ToString(obj.nickName);

            StudentTableProcess.ChangNickName(id, nickName);
        }

        [System.Web.Http.HttpPost]
        public UnitSearchResult[] SearchUnit(dynamic obj)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string unitCode = Convert.ToString(obj.unitCode);

            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "SELECT Unit.unitCode, Unit.unitName "+
                                   "FROM Unit " +
                                   "WHERE Unit.unitCode like '%" + unitCode + "%'";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);
            SqlDataReader count = command.ExecuteReader();
            int i = 0;
            while (count.Read())
            {
                i++;
            }
            count.Close();

            UnitSearchResult[] unitResult = new UnitSearchResult[i];
            i = 0;
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                unitResult[i] = new UnitSearchResult();
                unitResult[i].unitCode = reader[0].ToString();
                unitResult[i].unitName = reader[1].ToString();
                i++;
            }
            reader.Close();
            conn.Close();

            return unitResult;
        }
        
        [System.Web.Http.HttpPost]
        public void ChangeUnit(dynamic obj) {
            string id = Convert.ToString(obj.id);

            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = Convert.ToString(obj.unit);
            UnitTaken[] unitTaken = js.Deserialize<UnitTaken[]>(json);

            UnitTableProcess.UpdateUnitTaken(id, unitTaken);
        }
    }


}