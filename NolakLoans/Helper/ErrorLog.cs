using System;
using System.IO;

namespace NolakLoans.Helper
{
    public class ErrorLog
    {
        private string _errorPath = Environment.CurrentDirectory + @"\Logs\ErrorLog.txt";
        public ErrorLog()
        {
            if (!Directory.Exists(@"\Logs"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\Logs");
            }
            if(!File.Exists(_errorPath))
            {
                File.Create(_errorPath);
            }
        }

        public void LogToErrorFile(Exception e)
        {
            
            File.AppendAllText(_errorPath, "-----------------" + DateTime.Now.ToString() + "-----------------" + Environment.NewLine);
            File.AppendAllText(_errorPath, e.Message.ToString() + Environment.NewLine);
            File.AppendAllText(_errorPath, "----------------------------------" + Environment.NewLine);
            File.AppendAllText(_errorPath, e.Source.ToString() + Environment.NewLine + e.StackTrace.ToString() + Environment.NewLine);
            
        }
    }
}
