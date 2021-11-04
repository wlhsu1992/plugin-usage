using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            GetStudentAndMark(1001);
        }

        /// <summary>
        /// 一般字串查詢 (存在SQL Injection風險)
        /// </summary>
        static void SearchDataByLastNameTest(string lastName)
        {
            PersonService ps = new PersonService();
            //var list = ps.FindListByLastName("Manes' or '1'='1");  //sql injection test
            var list = ps.FindListByLastName(lastName);
            foreach (var l in list)
            {
                Console.Write($"Id: {l.Id}\tFirstName: {l.FirstName}\tLastName: {l.LastName}\tEmailAddrss: {l.EmailAddress}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 參數化查詢 (解決SQL Injection問題)
        /// </summary>
        static void SearchDataByIdTest(int id)
        {
            PersonService ps = new PersonService();
            var list = ps.FindListById(id);
            foreach (var l in list)
            {
                Console.Write($"Id: {l.Id}\tFirstName: {l.FirstName}\tLastName: {l.LastName}\tEmailAddrss: {l.EmailAddress}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 新增操作
        /// </summary>
        static void InsertDataTest(Person newPerson)
        {
            PersonService ps = new PersonService();

            Person person = new Person();
            person.FirstName = newPerson.FirstName;
            person.LastName = newPerson.LastName;
            person.EmailAddress = newPerson.EmailAddress;

            var success = ps.InsertData(person);
            if (success) Console.WriteLine("成功加入資料");
            else Console.WriteLine("加入資料發生錯誤");
        }

        /// <summary>
        /// 更新操作
        /// </summary>
        static void UpdateDataTest(Person p)
        {
            PersonService ps = new PersonService();

            Person person = new Person(p.Id, p.FirstName, p.LastName, p.EmailAddress);
            var success = ps.UpdateData(person);
            if (success) Console.WriteLine("成功修改資料");
            else Console.WriteLine("修改資料發生錯誤");
        }

        /// <summary>
        /// 刪除操作
        /// </summary>
        static void DeleteDataTest(int id)
        {
            PersonService ps = new PersonService();

            var success = ps.DeleteDataById(id);
            if (success) Console.WriteLine("成功刪除資料");
            else Console.WriteLine("刪除資料發生錯誤");
        }

        /// <summary>
        /// 使用預存程序進行查詢
        /// </summary>
        static void GetPassStudent()
        {
            StudentService ss = new StudentService();
            var students = ss.GetPassStudent();
            if(students.Count > 0)
            {
                Console.WriteLine("姓名\t學號\t性別\t年齡");
                students.ForEach((stu) =>
                {
                    Console.WriteLine($"{stu.StuName} {stu.StuNo} {stu.StuSex} {stu.StuAge}");
                });
            }
        }

        /// <summary>
        /// 傳入參數使用預存程序進行查詢
        /// </summary>
        static void GetPassStudentNumber(int writtenExam, int labExam)
        {
            StudentService ss = new StudentService();
            Console.WriteLine($"筆試分數>{writtenExam} 且 上機分數> {labExam} 的人數 {ss.GetPassStudentNumber(writtenExam, labExam)}");
        }

        /// <summary>
        /// 交易實例: 刪除包含外來鍵限制條件的資料組
        /// </summary>
        static void DeleteStudentTest(int id)
        {
            StudentService ss = new StudentService();
            bool success = ss.DeleteStudent(id);
            if (success) Console.WriteLine($"刪除學號:{id}的學生及成績資料");
            else Console.WriteLine("刪除資料失敗");
        }

        /// <summary>
        /// join 資料查詢 
        /// </summary>
        /// <param name="id"></param>
        static void GetStudentAndMark(int id)
        {
            StudentService ss = new StudentService();
            var list = ss.GetStdentAndMark(id);
            foreach(var item in list) {
                Console.WriteLine($"學號:{item.StuNo} 姓名:{item.StuName} 年齡:{item.StuAge}");
            }
        }

        static void GetStudentInfoTest(int id)
        {
            StudentService ss = new StudentService();
            var info = ss.GetStudentInfo(id);
            if (info is null) Console.WriteLine($"沒有學號 {id} 的資料");
            else Console.WriteLine($"姓名:{info.StuName} 學號:{info.StuNo} 性別:{info.StuSex} 年齡:{info.StuAge} 地址:{info.StuAddress}");
        }

        static void GetStudentMarkTest(int id)
        {
            StudentService ss = new StudentService();
            var mark = ss.GetStudentMark(id);
            if (mark is null) Console.WriteLine($"沒有學號 {id} 的資料");
            else Console.WriteLine($"學號:{mark.StuNo} 筆試成績:{mark.WrittenExam} 上機成績:{mark.LabExam}");
        }
    }
}
