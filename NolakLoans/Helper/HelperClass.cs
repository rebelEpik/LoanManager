using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NolakLoans.Helper
{
    public class HelperClass
    {

        public string ConnectionString = "Data Source=Loans.sqlite;Version=3;";
        public SQLiteConnection _connection;
        public void createDB()
        {
            SQLiteConnection.CreateFile("Loans.sqlite");
            _connection = new SQLiteConnection("Data Source=Loans.sqlite;Version=3;");
            _connection.Open();

            string createTable = @"CREATE TABLE Loans (id INTEGER PRIMARY KEY AUTOINCREMENT, name varchar(50), amt int, interest REAL, collateral varchar(50), start varchar(50), duration int, dayslate int, link varchar(150), totalLoanAmt LONG INTEGER, paidOff int, balanceRemaining LONG INTEGER);";

            SQLiteCommand create = new SQLiteCommand(createTable, _connection);
            create.ExecuteNonQuery();

            string createPmtTable = @"CREATE TABLE `Payments` ( `paymentID` INTEGER PRIMARY KEY AUTOINCREMENT, `loanID` INTEGER, `paymentAmt` LONG INTEGER, `paymentFrom` TEXT )";
            SQLiteCommand com = new SQLiteCommand(createPmtTable, _connection);
            com.ExecuteNonQuery();
            _connection.Close();
        }
    }

    
}
