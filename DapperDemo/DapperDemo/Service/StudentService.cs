using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperDemo
{
    public class StudentService
    {
        /// <summary>
        /// 使用無參數預存程序
        /// </summary>
        /// <returns></returns>
        public List<StudentInfo> GetPassStudent()
        {
            using(IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                List<StudentInfo> students = db.Query<StudentInfo>("dbo.sp_get_stumarkinfo_notpass", // 預存程序名稱
                     null, //預存程序參數
                     null, //交易物件
                     true, //是否緩存
                     null, //獲取或設置在中止執行命令的嘗試並生成錯誤之間的等待時間
                     CommandType.StoredProcedure //指定SQL語句唯存儲過程的類型
                ).ToList();

                return students;
            }
        }

        /// <summary>
        /// 使用參數預存程序
        /// </summary>
        /// <param name="writtenExam"></param>
        /// <param name="labExam"></param>
        /// <returns></returns>
        public int GetPassStudentNumber(int writtenExam, int labExam)
        {
            var p = new DynamicParameters(); //動態參數類
            p.Add("@w", writtenExam); // 筆試及格線輸入參數
            p.Add("@l", labExam);  // 測試及格線輸入參數
            p.Add("@countNum", 0, DbType.Int32, ParameterDirection.Output); //標註為輸出參數

            using (IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                // 執行預存程序
                db.Query("dbo.sp_get_markinfo", p, null, true, null, CommandType.StoredProcedure);
                return p.Get<int>("@countNum");
            }
        }

        /// <summary>
        /// 使用交易依序刪除從表及主表資料
        /// </summary>
        /// <returns></returns>
        public bool DeleteStudent(int id)
        {
            using (IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                db.Open();
                // 建立交易物件
                IDbTransaction trans = db.BeginTransaction(); //開始資料庫交易
                try {
                    string query1 = "delete from StuInfo where StuNo = @StuNo";
                    string query2 = "delete from StuMark where StuNo = @StuNo";

                    // 當刪除資料存在外來鍵限制，需要先刪除從表才可刪除主表
                    db.Execute(query2, new { StuNo = id }, trans, null, null); //從表
                    db.Execute(query1, new { StuNo = id }, trans, null, null); //主表

                    trans.Commit(); // 提交事務
                    return true;
                } catch(Exception ex) {
                    trans.Rollback();
                    return false;
                }
            }
        }

        public StudentInfo GetStudentInfo(int id)
        {
            using (IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                string query = "select * from StuInfo where StuNo = @StuNo";
                return db.QueryFirstOrDefault<StudentInfo>(query, new { StuNo = id });
            }
        }

        public StuMark GetStudentMark(int id)
        {
            using (IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                string query = "select * from StuMark where StuNo = @StuNo";
                return db.QueryFirstOrDefault<StuMark>(query, new { StuNo = id });
            }
        }

        public List<StudentInfo> GetStdentAndMark(int id)
        {
            using(IDbConnection db = new SqlConnection(DBHelper.ConnStrings))
            {
                var sql = "select * from stuinfo inner join stumark on stuinfo.stuNo=stumark.stuNo";

                // 執行多表查詢 (類型一, 類型二, 返回值)
                var list = db.Query<StudentInfo, StuMark, StudentInfo>(
                    sql,
                    (students, scores)=> { return students; }, //變數students對應的StuInfo類型 scores對應StuMark類型 
                    null,    // 預存程序參數
                    null,    // 交易
                    true,    // 緩存
                    splitOn: "stuNo"  // 用來劃分查詢中的欄位是屬於哪個表的 splitOn可省略
                );

                /*splitOn:stuNo 劃分查詢中的欄位是屬於哪個表(查詢結果映射到哪個實體)，
                 SQL運行時: 會將查詢結果的最後一個欄位到splitOn欄位，進行匹配，
                 EX: 最右邊欄位向左依序至 splitOn欄位匹配給 scores；
                     splitOn欄位向左依序至最左欄位匹配給 students   */

                return list.ToList();

            }
        }

    }
}
