using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Serve.Models;
using System.Data.SqlClient;
using System.Data.Entity;

namespace Serve.Models
{
    public static class UnitTableProcess
    {
        public static StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
        private static object tableLock = new object();

        public static void UpdateUnitTaken(string id, UnitTaken[] unitTaken) { 
            lock(tableLock){
                SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
                conn.Open();
                string strSQLCommand = "DELETE " +
                                       "FROM UnitTaken " +
                                       "WHERE studentId = '" + id + "'";
                SqlCommand command = new SqlCommand(strSQLCommand, conn);
                command.ExecuteNonQuery();
                conn.Close();

                if (unitTaken.Length > 0)
                {
                    for (int i = 0; i < unitTaken.Length; i++)
                    {
                        unitTaken[i].studentId = id;
                        db.UnitTaken.Add(unitTaken[i]);
                    }
                }

                db.SaveChanges();
            }
        }
    }
}