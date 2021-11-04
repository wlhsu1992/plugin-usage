using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperDemo
{
    public class DBHelper
    {
        public static string ConnStrings
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
            }
        }

    }
}
