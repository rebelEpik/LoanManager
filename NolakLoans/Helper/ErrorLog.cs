using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NolakLoans.Helper
{
    public class ErrorLog
    {
        private string _errorPath;
        public ErrorLog()
        {
            if (!Directory.Exists(@"\Logs"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\Logs");
            }
            if(!File.Exists(@"\Logs\ErrorLog" + DateTime.Today.ToString("MM.dd.yyyy")))
            {
                _errorPath = Environment.CurrentDirectory + @"\Logs\ErrorLog" + DateTime.Today.ToString("MM.dd.yyyy") + ".txt";
                //File.Create(_errorPath);
            }
        }

        public void LogToErrorFile(Exception e)
        {
            File.AppendAllText(_errorPath, e.Message.ToString());
        }
    }
}
