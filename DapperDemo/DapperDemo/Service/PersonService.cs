using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperDemo
{
    public class PersonService
    {
        public List<Person> FindListByLastName(string lastName)
        {
            // Dapper ORM的操作實際上是對IDbConneciton類的擴展，所有方法都是該類的擴展方法。
            // 所以在使用前先實例化一個IDBConnection物件。
            using (IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                // 字串插補，存在SQL INJECTION風險
                string sql = $"select * from Person where LastName = '{lastName}'";
                IEnumerable<Person> lst = db.Query<Person>(sql);
                return lst.ToList();
            }
        }

        public List<Person> FindListById(int id)
        {
            using (IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                string sql = $"select * from Person where Id=@Id";
                // 提交參數化查詢，解決SQL Injection 問題
                IEnumerable<Person> lst = db.Query<Person>(sql, new { Id = id });
                return lst.ToList();
            }
        }

        public bool InsertData(Person person)
        {
            using (IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                // 準備插入SQL語句
                string sql = "insert into Person(FirstName, LastName, EmailAddress) " +
                    "values(@FirstName, @LastName, @EmailAddress)";

                // 調用Dapper中的IDbConnection擴展方法 Execute() 來執行插入操作
                int result = db.Execute(sql, person);
                return result > 0;
            }
        }

        public bool UpdateData(Person person)
        {
            using (IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                string sql = "update Person set FirstName=@FirstName, LastName=@LastName, EmailAddress=@EmailAddress Where Id=@Id";
                int result = db.Execute(sql, person);
                return result > 0;
            }
        }

        public bool DeleteDataById(int id)
        {
            using (IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                string sql = "DELETE Person WHERE Id=@Id";
                var parameters = new { Id = id };
                int result = db.Execute(sql, parameters);
                return result > 0;
            }
        }
    }
}
