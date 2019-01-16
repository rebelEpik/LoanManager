using System;
using System.Data.SQLite;

namespace NolakLoans.Helper
{
    public class HelperClass
    {
        public ErrorLog errorLog = new ErrorLog();
        public string DBName = "Loans.sqlite";
        public string ConnectionString = "Data Source=Loans.sqlite;Version=3;";
        public SQLiteConnection _connection;
        public void createDB()
        {
            try
            {
                SQLiteConnection.CreateFile("Loans.sqlite");
            }
            catch(Exception e)
            {
                errorLog.LogToErrorFile(e);
            }
            try
            {
                _connection = new SQLiteConnection(ConnectionString);
                _connection.Open();
            }
            catch (Exception e)
            {
                errorLog.LogToErrorFile(e);
            }
                

            string createTable = @"CREATE TABLE Loans (id INTEGER PRIMARY KEY AUTOINCREMENT, name varchar(50), amt int, interest REAL, collateral varchar(50), start varchar(50), duration int, dayslate int, link varchar(150), totalLoanAmt LONG INTEGER, paidOff int, balanceRemaining LONG INTEGER);";
            string createPmtTable = @"CREATE TABLE `Payments` ( `paymentID` INTEGER PRIMARY KEY AUTOINCREMENT, `loanID` INTEGER, `paymentAmt` LONG INTEGER, `paymentFrom` TEXT )";


            try
            {
                SQLiteCommand create = new SQLiteCommand(createTable, _connection);
                create.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                errorLog.LogToErrorFile(e);
            }


            try
            {
                SQLiteCommand com = new SQLiteCommand(createPmtTable, _connection);
                com.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                errorLog.LogToErrorFile(e);
            }

            _connection.Close();
        }
    }

    
}
