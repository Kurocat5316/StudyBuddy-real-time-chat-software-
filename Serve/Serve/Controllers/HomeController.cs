using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Serve.Models;
using System.Data.Entity;
using System.Net;
using System.IO;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using System.Threading;

namespace Serve.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            return View(db.Student.ToList());
        }

        public ActionResult EmailConfirm(string email)
        {
            string id = email.Substring(0, 8);
            StudentTableProcess.EmailConfirm(id);
            return View("EmailConfirm");
        }

        public ActionResult EmailApply(string Email)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            string id = Email.Substring(0, 8);
            Student student = db.Student.Find(id);
            
            if (student != null && student.vertificationCheck.Equals("1"))
            {
                ViewBag.password = StudentTableProcess.ResetPassWord(id);
                return View();
            }
            return View("InvalideRequire");
        }

        public ActionResult InvalideRequire() {
            return View();
        }

        public ActionResult SwichStatus(string id)
        {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student.managerForbid.Equals("0"))
            {
                student.managerForbid = "1";
            }
            else {
                student.managerForbid = "0";
            }
            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Update()
        {
            string year = Request["year"].ToString();
            int num = -1;
            if (year == "")
            {
                year = DateTime.Now.Year.ToString();
            }
            else
            {
                if (!int.TryParse(year, out num))
                {
                    return RedirectToAction("UnitManage");
                }
            }
            Thread unitUpdateThread = new Thread(UpdateUnits);
            unitUpdateThread.Start(year);
            return RedirectToAction("UnitManage");
        }


        public ActionResult UnitManage() {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "SELECT Unit.unitCode, Unit.unitName, Semester.semester, Convener.convenerName, Convener.phoneNumber, Convener.email, Faculty.facultyName " +
                                    "FROM UnitArrangement, Unit, Convener, Semester, Faculty " +
                                    "WHERE UnitArrangement.unitCode = Unit.unitCode AND UnitArrangement.convenerId = Convener.convenerId AND Semester.semesterId = UnitArrangement.semesterId AND Unit.facultyId = Faculty.facultyId";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);

            SqlDataReader count = command.ExecuteReader();
            int i = 0;
            while (count.Read())
            {
                i++;
            }
            count.Close();

            SqlDataReader reader = command.ExecuteReader();


            UnitInfor[] unitInfor = new UnitInfor[i];
            i = 0;
            while (reader.Read())
            {
                unitInfor[i] = new UnitInfor();
                unitInfor[i].unitCode = reader[0].ToString();
                unitInfor[i].unitName = reader[1].ToString();
                unitInfor[i].semeter = reader[2].ToString();
                unitInfor[i].convenerName = reader[3].ToString();
                unitInfor[i].phoneNumber = reader[4].ToString();
                unitInfor[i].email = reader[5].ToString();
                unitInfor[i].facultyName = reader[6].ToString();
                i++;
            }
            conn.Close();


            return View(unitInfor.ToList());
        }


        private void unitCheck(string unitCode, string unitName, string facultyName){
            //try
            //{
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
                Unit myunit = db.Unit.Find(Convert.ToInt32(unitCode));
                if (myunit == null)
                {
                    try
                    {
                        int facultyId = FacultyCheck(facultyName);
                        Unit newUnit = new Unit();
                        newUnit.unitCode = Convert.ToInt32(unitCode);
                        newUnit.unitName = unitName;
                        newUnit.facultyId = facultyId;
                        db.Unit.Add(newUnit);
                        db.SaveChanges();
                    }
                    catch (Exception e) {
                        throw e;
                    }
                }
            //}
            //catch (Exception e) {
            //    throw e;
            //}
        }

         private int FacultyCheck(string facultyName){
             int facultyId = 1;
             //try
             //{
             StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
             if (facultyName.IndexOf("&amp;") > 0)
             {
                 string tempFacultyName = facultyName.Substring(facultyName.IndexOf("&amp;") + 5);
                 facultyName = facultyName.Substring(0, facultyName.IndexOf("&amp;"));
                 facultyName = facultyName + tempFacultyName;
             }
                 SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
                 conn.Open();
                 string strSQLCommand = "SELECT * " +
                                        "FROM Faculty " +
                                        "WHERE facultyName = '" + facultyName + "'";
                 SqlCommand command = new SqlCommand(strSQLCommand, conn);
                 SqlDataReader reader = command.ExecuteReader();
                 if (reader.Read())
                 {
                     facultyId = Convert.ToInt32(reader[0].ToString());
                     reader.Close();
                 }
                 else
                 {
                     reader.Close();
                     Faculty facultyTemp = new Faculty();
                     facultyTemp.facultyName = facultyName;
                     db.Faculty.Add(facultyTemp);
                     db.SaveChanges();

                     command = new SqlCommand(strSQLCommand, conn);
                     SqlDataReader reader2 = command.ExecuteReader();
                     reader2.Read();
                     facultyId = Convert.ToInt32(reader2[0].ToString());
                     reader2.Close();
                 }

                 conn.Close();
             //}
             //catch (Exception e) {
             //    throw e;
             //}

             return facultyId;
         }


         private int GetSemesterId(string semester) {
             int semesterId = 1;
             //try
             //{
             StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
                 SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
                 conn.Open();
                 string strSQLCommand = "SELECT * " +
                                        "FROM Semester " +
                                        "WHERE semester = '" + semester + "'";
                 SqlCommand command = new SqlCommand(strSQLCommand, conn);
                 SqlDataReader reader = command.ExecuteReader();
                 if (reader.Read())
                 {
                     semesterId = Convert.ToInt32(reader[0].ToString());
                     reader.Close();
                 }
                 else
                 {
                     reader.Close();
                     Semester semesterTemp2 = new Semester();
                     semesterTemp2.semester1 = semester;
                     db.Semester.Add(semesterTemp2);
                     db.SaveChanges();

                     command = new SqlCommand(strSQLCommand, conn);
                     SqlDataReader reader2 = command.ExecuteReader();
                     reader2.Read();
                     semesterId = Convert.ToInt32(reader2[0].ToString());
                     reader2.Close();
                 }
                 conn.Close();
             //}
             //catch (Exception e) {
             //    throw e;
             //}
             return semesterId;
         }

         private int ConvenerCheck(string name, string phoneNumber, string email) {
             
             int convenerId = 1;
             //try
             //{
             StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
                 SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
                 conn.Open();
                 string strSQLCommand = "SELECT * " +
                                        "FROM Convener " +
                                        "WHERE phoneNumber = '" + phoneNumber + "' AND convenerName = '" + name + "'";
                 SqlCommand command = new SqlCommand(strSQLCommand, conn);
                 SqlDataReader reader = command.ExecuteReader();
                 if (reader.Read())
                 {
                     convenerId = Convert.ToInt32(reader[0].ToString());
                     reader.Close();
                 }
                 else
                 {
                     reader.Close();
                     Convener convenerTemp = new Convener();
                     convenerTemp.convenerName = name;
                     convenerTemp.phoneNumber = phoneNumber;
                     convenerTemp.email = email;
                     db.Convener.Add(convenerTemp);
                     db.SaveChanges();

                     command = new SqlCommand(strSQLCommand, conn);
                     SqlDataReader reader2 = command.ExecuteReader();
                     reader2.Read();
                     convenerId = Convert.ToInt32(reader2[0].ToString());
                     reader2.Close();
                 }
                 conn.Close();
             //}
             //catch (Exception e) {
             //    throw e;
             //}
             return convenerId;
         }

         private void UnitAssignmentCheck(int unitCode, int semesterId, int convenerId) {
             //try
             //{
             StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
                 SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
                 conn.Open();
                 string strSQLCommand = "SELECT * " +
                                        "FROM UnitArrangement " +
                                        "WHERE unitCode = '" + unitCode + "' AND semesterId = '" + semesterId + "' AND convenerId = '" + convenerId + "'";
                 SqlCommand command = new SqlCommand(strSQLCommand, conn);
                 SqlDataReader reader = command.ExecuteReader();
                 if (reader.Read()) { }
                 else
                 {
                     UnitArrangement newAssignment = new UnitArrangement();
                     newAssignment.unitCode = unitCode;
                     newAssignment.semesterId = semesterId;
                     newAssignment.convenerId = convenerId;
                     db.UnitArrangement.Add(newAssignment);
                     db.SaveChanges();
                 }
                 reader.Close();
                 conn.Close();
             //}
             //catch (Exception e) {
             //    throw e;
             //}
         }

        private void DeleteUnitAssignment(){
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "DELETE " +
                                   "FROM UnitArrangement";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        private void DeleteUnit() {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "DELETE " +
                                   "FROM Unit";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        private void DeleteUnitTaken() {
            StudyBuddyDaEntities1 db = new StudyBuddyDaEntities1();
            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            string strSQLCommand = "DELETE " +
                                   "FROM UnitTaken";
            SqlCommand command = new SqlCommand(strSQLCommand, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        private void UpdateUnits(object year) {
            string getYear = Convert.ToString(year);

            UnitInfor unitInfor = new UnitInfor();
            string url = null;
            string menuCode = null;

            int unitId = -1;
            int semesterId = -1;
            int convenerId = -1;

            WebClient webClient;

            int totalNumber = 0;
            int units = 1;
            while (true)
            {
                url = String.Format("http://search.canberra.edu.au/s/search.html?collection=CAPS&form=unit-search&profile=_default&meta_I_and=UNIT&sort=metat&query=&course-search__submit=&meta_D_and=&course-search-type=units&f.Year|B={0}&start_rank={1}", getYear, units);
                webClient = new WebClient();
                webClient.Encoding = System.Text.Encoding.UTF8;
                menuCode = webClient.DownloadString(url);
                if (menuCode.IndexOf("/s/redirect") < 0)
                    break;

                while (menuCode.IndexOf("/s/redirect") >= 0)
                {
                    menuCode = menuCode.Substring(menuCode.IndexOf("/s/redirect") + 1);
                    menuCode = menuCode.Substring(menuCode.IndexOf("/s/redirect") + 1);
                    totalNumber++;
                }
                units = units + 10;
                Thread.Sleep(300);
            }

            DeleteUnitAssignment();
            DeleteUnitTaken();
            DeleteUnit();


            for (int item = 1; item < units; item += 10)
            {
                //try { 
                url = String.Format("http://search.canberra.edu.au/s/search.html?collection=CAPS&form=unit-search&profile=_default&meta_I_and=UNIT&sort=metat&query=&course-search__submit=&meta_D_and=&course-search-type=units&f.Year|B={0}&start_rank={1}", year, item);
                webClient = new WebClient();
                webClient.Encoding = System.Text.Encoding.UTF8;
                menuCode = webClient.DownloadString(url);
                totalNumber -= 10;
                int c = 0;
                if (totalNumber < 10)
                    c = 10 - totalNumber;
                for (; c < 10; c++)
                {
                    menuCode = menuCode.Substring(menuCode.IndexOf("/s/redirect") + 1);
                    menuCode = menuCode.Substring(menuCode.IndexOf("/s/redirect"));
                    menuCode = menuCode.Substring(menuCode.IndexOf("title=") + 7);
                    string suburl = menuCode.Substring(0, menuCode.IndexOf('>') - 2);


                    string childCode = webClient.DownloadString(suburl);
                    childCode = childCode.Substring(childCode.IndexOf("</script><h1>") + 13);

                    if (childCode.IndexOf("Syllabus") >= 0)
                        childCode = childCode.Substring(0, childCode.IndexOf("Syllabus") - 1);

                    if (childCode.IndexOf("Unit Outlines") >= 0)
                        childCode = childCode.Substring(0, childCode.IndexOf("Unit Outlines") + 1);

                    unitInfor = new UnitInfor();
                    unitInfor.unitName = childCode.Substring(0, childCode.IndexOf("(") - 1);
                    if (childCode.IndexOf("(Internships)") >= 0)
                    {
                        unitInfor.unitName = unitInfor.unitName + " (Internships)";
                        childCode = childCode.Substring(childCode.IndexOf("(Internships)") + 2);
                    }

                    if (childCode.IndexOf("(Health)") >= 0)
                    {
                        unitInfor.unitName = unitInfor.unitName + " (Health)";
                        childCode = childCode.Substring(childCode.IndexOf("(Health)") + 2);
                    }

                    if (childCode.IndexOf("(Extended)") >= 0)
                    {
                        unitInfor.unitName = unitInfor.unitName + " (Extended)";
                        childCode = childCode.Substring(childCode.IndexOf("(Extended)") + 2);
                    }




                    if (childCode.IndexOf("(Part") >= 0)
                        childCode = childCode.Substring(childCode.IndexOf("(Part") + 1);

                    if (childCode.IndexOf("0cp") >= 0)
                        childCode = childCode.Substring(childCode.IndexOf("0cp") + 1);

                    if (childCode.IndexOf("6cp") >= 0)
                        childCode = childCode.Substring(childCode.IndexOf("6cp") + 1);

                    if (childCode.IndexOf("3cp") >= 0)
                        childCode = childCode.Substring(childCode.IndexOf("3cp") + 1);

                    if (childCode.IndexOf("(A)") >= 0)
                        childCode = childCode.Substring(childCode.IndexOf("(A)") + 1);

                    if (childCode.IndexOf("(B)") >= 0)
                        childCode = childCode.Substring(childCode.IndexOf("(B)") + 1);

                    if (childCode.IndexOf("Inf. Sc.") >= 0)
                        childCode = childCode.Substring(childCode.IndexOf("Inf. Sc.") + 9);

                    unitInfor.unitCode = childCode.Substring(childCode.IndexOf("(") + 1, childCode.IndexOf(".") - childCode.IndexOf("(") - 1);

                    childCode = childCode.Substring(childCode.IndexOf("Faculty:") + 17);
                    unitInfor.facultyName = childCode.Substring(1, childCode.IndexOf("</td>") - 1);

                    unitCheck(unitInfor.unitCode, unitInfor.unitName, unitInfor.facultyName);
                    unitId = Convert.ToInt32(unitInfor.unitCode);

                    if (childCode.IndexOf("UC Canberra - Bruce Campus") > 0)
                    {
                        childCode = childCode.Substring(childCode.IndexOf("UC Canberra - Bruce Campus") + 13);
                        childCode = childCode.Substring(childCode.IndexOf("<tbody>") + 7, childCode.IndexOf("</tbody>") - childCode.IndexOf("<tbody>") - 7);

                        string temp = childCode;

                        int i = 0;
                        while (true)
                        {
                            temp = temp.Substring(temp.IndexOf("</tr>") + 1);
                            i++;
                            if (temp.IndexOf("<tr>") < 0)
                                break;
                        }

                        temp = null;
                        string[] temp2 = new string[i];
                        for (int a = 0; a < i; a++)
                        {
                            temp2[a] = childCode.Substring(childCode.IndexOf("<tr>") + 4, childCode.IndexOf("</tr>") - childCode.IndexOf("<tr>") - 5);
                            childCode = childCode.Substring(childCode.IndexOf("</tr>") + 5);
                        }


                        for (int a = 0; a < i; a++)
                        {
                            temp2[a] = temp2[a].Substring(temp2[a].IndexOf("<td>") + 4);
                            temp2[a] = temp2[a].Substring(temp2[a].IndexOf("<td>") + 4);
                            unitInfor.semeter = temp2[a].Substring(0, temp2[a].IndexOf("</td>"));

                            semesterId = GetSemesterId(unitInfor.semeter);

                            temp2[a] = temp2[a].Substring(temp2[a].IndexOf("<a target"));
                            temp2[a] = temp2[a].Substring(temp2[a].IndexOf(">") + 1);
                            unitInfor.convenerName = temp2[a].Substring(0, temp2[a].IndexOf("</a>"));
                            string[] tempName = unitInfor.convenerName.Split('\n');
                            unitInfor.convenerName = null;
                            for (int b = 0; b < tempName.Length; b++)
                                unitInfor.convenerName = unitInfor.convenerName + tempName[b];
                            string[] tempName2 = unitInfor.convenerName.Split(' ');
                            unitInfor.convenerName = null;
                            for (int b = 0; b < tempName2.Length; b++)
                                unitInfor.convenerName = unitInfor.convenerName + tempName2[b];

                            unitInfor.email = tempName2[40] + "." + tempName2[tempName2.Length - 1] + "@canberra.edu.au";

                            if (temp2[a].IndexOf("+") > 0)
                            {
                                temp2[a] = temp2[a].Substring(temp2[a].IndexOf("+") + 1);
                                temp2[a] = temp2[a].Substring(temp2[a].IndexOf("+") + 1);
                                unitInfor.phoneNumber = temp2[a].Substring(0, temp2[a].IndexOf("</a>"));
                            }
                            else
                                unitInfor.phoneNumber = "null";

                            if (unitInfor.email.Equals(".@canberra.edu.au"))
                            {
                                UnitAssignmentCheck(unitId, semesterId, 1);
                                break;
                            }
                            else
                            {
                                string tempSearchName;
                                if (unitInfor.convenerName.Contains("'"))
                                {
                                    int tempRecord = unitInfor.convenerName.IndexOf("'");
                                    tempSearchName = unitInfor.convenerName.Remove(tempRecord, 1);
                                }
                                else
                                {
                                    tempSearchName = unitInfor.convenerName;
                                }



                                convenerId = ConvenerCheck(tempSearchName, unitInfor.phoneNumber, unitInfor.email);

                                UnitAssignmentCheck(unitId, semesterId, convenerId);
                            }
                        }
                    }
                    else
                        UnitAssignmentCheck(unitId, 1, 1);
                    Thread.Sleep(300);
                }
                //}
                //catch (Exception e)
                //{
                //    continue;
                //    throw e;
                //}
            }
        }
    }

   
}
