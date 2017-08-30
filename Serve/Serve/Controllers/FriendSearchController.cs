using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data.Entity;
using Serve.Models;
using System.Web.Script.Serialization;
using System.Device.Location;

namespace Serve.Controllers
{
    public class FriendSearchController : ApiController
    {
        

        [System.Web.Http.HttpPost]
        public FriendSearch[] SearchWithID(dynamic obj)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string id = Convert.ToString(obj.id);
            string key = Convert.ToString(obj.key);

            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "Select studentId, nickname " +
                                   "From Student " +
                                   "Where studentId like '%" + key + "%' AND Student.studentId not in (Select Friend.friendId " +
																	"From Friend " +
                                                                    "WHERE Friend.hostId = '" + id + "') AND Student.studentId != '" + id + "'";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);

            SqlDataReader count = command.ExecuteReader();
            int i = 0;
            while (count.Read())
            {
                i++;
            }
            count.Close();

            SqlDataReader reader = command.ExecuteReader();

            FriendSearch[] friendSearch = new FriendSearch[i];

            i = 0;
            while (reader.Read())
            {
                friendSearch[i] = new FriendSearch();
                friendSearch[i].id = reader[0].ToString();
                friendSearch[i].nickName = reader[1].ToString();
                i++;
            }
            reader.Close();

            for (i = 0; i < friendSearch.Length; i++) {
                strSQLCommand = "Select Unit.unitCode, Unit.unitName " +
                                 "From Student, UnitTaken, Unit " +
                                 "Where Student.studentId = '" + friendSearch[i].id + "' AND Student.studentId = UnitTaken.studentId AND UnitTaken.unitCode = Unit.unitCode";

                command = new SqlCommand(strSQLCommand, conn);

                SqlDataReader count2 = command.ExecuteReader();
                int unitRecord = 0;
                while (count2.Read())
                {
                    unitRecord++;
                }
                count2.Close();

                friendSearch[i].units = new UnitSearchResult[unitRecord];

                SqlDataReader reader2 = command.ExecuteReader();
                unitRecord = 0;
                while (reader2.Read())
                {
                    friendSearch[i].units[unitRecord] = new UnitSearchResult();
                    friendSearch[i].units[unitRecord].unitCode = reader2[0].ToString();
                    friendSearch[i].units[unitRecord].unitName = reader2[1].ToString();
                    
                    unitRecord++;
                }
                reader2.Close();
            }
            conn.Close();

            return friendSearch;
        }


        [System.Web.Http.HttpPost]
        public FriendSearch[] GetFriendListWithName(dynamic obj) {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string id = Convert.ToString(obj.id);
            double x, y;

            string key = Convert.ToString(obj.key);
            string rangeTemp = Convert.ToString(obj.range);
            double range = Convert.ToDouble(rangeTemp);

            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = Convert.ToString(obj.units);
            UnitSearchResult[] unit = js.Deserialize<UnitSearchResult[]>(json);


            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "Select Student.LocationX, Student.LocationY " +
                                   "From Student " +
                                   "Where Student.studentId = '" + id + "'";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);
            SqlDataReader locationReader = command.ExecuteReader();
            locationReader.Read();
            
            x = Convert.ToDouble(locationReader[0].ToString());
            y = Convert.ToDouble(locationReader[1].ToString());
            
            locationReader.Close();
            
            strSQLCommand = "Select Student.studentId, Student.nickname, Student.LocationX, Student.LocationY " +
                            "From Student, UnitTaken " +
                            "Where UnitTaken.studentId in (Select Student.studentId "+
                            "From Student " +
                            "Where Student.nickname like '%" + key + "%' AND Student.studentId not in (Select Friend.friendId " +
															                                           "From Friend " +
															                                           "WHERE Friend.hostId = '" + id + "') AND Student.studentId != '" + id + "') AND UnitTaken.studentId = Student.studentId ";

            if (unit.Length > 0) {
                strSQLCommand += "AND (";
                for (int i = 0; i < unit.Length; i++) {
                    if (i < 1)
                    {
                        strSQLCommand += "UnitTaken.unitCode = '" + unit[i].unitCode + "' ";
                    }
                    else {
                        strSQLCommand += "OR UnitTaken.unitCode = '" + unit[i].unitCode + "' ";
                    }
                }
                strSQLCommand += ")";
            }

            command = new SqlCommand(strSQLCommand, conn);
            SqlDataReader reader = command.ExecuteReader();
            Queue<FriendSearch> tempQueue = new Queue<FriendSearch>();
            Queue<double> distanceRecord = new Queue<double>();
            int a = 0;

            while (reader.Read()) {
                double locationX = Convert.ToDouble(reader[2].ToString());
                double locationY = Convert.ToDouble(reader[3].ToString());

                double distance = DistanceCalculate(x, y, locationX, locationY);
                distance = Math.Round(distance, 2);
                if (distance < range) {
                    FriendSearch temp = new FriendSearch();
                    temp.id = reader[0].ToString();
                    temp.nickName = reader[1].ToString();
                    temp.distance = Convert.ToString(distance);
                    tempQueue.Enqueue(temp);
                    distanceRecord.Enqueue(distance);
                    a++;
                }
            }
            reader.Close();

            double[] distanceArray = new double[a];
            FriendSearch[] friendSearch = new FriendSearch[a];

            for (int i = 0; i < a; i++) {
                distanceArray[i] = distanceRecord.Dequeue();
                friendSearch[i] = tempQueue.Dequeue();
            }

            for (int i = 0; i < a - 1; i++) {
                double min = distanceArray[i];
                int position = i;
                for (int b = i + 1; b < a; b++) {
                    if (min > distanceArray[b]) {
                        min = distanceArray[b];
                        position = b;
                    }
                }
                double tempDouble = distanceArray[i];
                distanceArray[i] = distanceArray[position];
                distanceArray[position] = tempDouble;

                FriendSearch tempSearch = friendSearch[i];
                friendSearch[i] = friendSearch[position];
                friendSearch[position] = tempSearch;
            }


            for (int i = 0; i < friendSearch.Length; i++)
            {
                strSQLCommand = "Select Unit.unitCode, Unit.unitName " +
                                 "From Student, UnitTaken, Unit " +
                                 "Where Student.studentId = '" + friendSearch[i].id + "' AND Student.studentId = UnitTaken.studentId AND UnitTaken.unitCode = Unit.unitCode";

                command = new SqlCommand(strSQLCommand, conn);

                SqlDataReader count = command.ExecuteReader();
                int unitRecord = 0;
                while (count.Read())
                {
                    unitRecord++;
                }
                count.Close();

                friendSearch[i].units = new UnitSearchResult[unitRecord];

                SqlDataReader reader2 = command.ExecuteReader();
                unitRecord = 0;
                while (reader2.Read())
                {
                    friendSearch[i].units[unitRecord] = new UnitSearchResult();
                    friendSearch[i].units[unitRecord].unitCode = reader2[0].ToString();
                    friendSearch[i].units[unitRecord].unitName = reader2[1].ToString();

                    unitRecord++;
                }
                reader2.Close();
            }
            conn.Close();

            return friendSearch;
        }
        
        private double DistanceCalculate(double x, double y, double locationX, double locationY){
            var locA = new GeoCoordinate(x, y);
            var locB = new GeoCoordinate(locationX, locationY);

            return locA.GetDistanceTo(locB);
        }

        [System.Web.Http.HttpPost]
        public void SendFriendApply(dynamic obj){
            string senderId = Convert.ToString(obj.senderId);
            string receiverId = Convert.ToString(obj.receiverId);
            string content = Convert.ToString(obj.content);

            RequireTableProcess.ApplyFriend(senderId, receiverId, content);
        }
    }
}