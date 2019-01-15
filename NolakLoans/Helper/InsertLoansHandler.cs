using NolakLoans.Types;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NolakLoans.Helper
{
    public class InsertLoansHandler
    {
        HelperClass _helper = new HelperClass();
        public void insertLoan(Loan currentLoan)
        {
            SQLiteConnection connection = new SQLiteConnection(_helper.ConnectionString);
            connection.Open();
            SQLiteCommand insertCommand = new SQLiteCommand
            {
                CommandText = @"Insert Into Loans (name, amt, interest, collateral, start, duration, dayslate, link, totalLoanAmt, paidOff, balanceRemaining) values(@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9, @param10, @param11)"
                
            };
            insertCommand.Parameters.AddWithValue("@param1", currentLoan.BName);
            insertCommand.Parameters.AddWithValue("@param2", currentLoan.BAmt);
            insertCommand.Parameters.AddWithValue("@param3", currentLoan.IntRate);
            insertCommand.Parameters.AddWithValue("@param4", currentLoan.Collat);
            insertCommand.Parameters.AddWithValue("@param5", currentLoan.LoanStart);
            insertCommand.Parameters.AddWithValue("@param6", currentLoan.LoanDuration);
            insertCommand.Parameters.AddWithValue("@param7", currentLoan.DaysLate);
            insertCommand.Parameters.AddWithValue("@param8", currentLoan.LoanLink);
            insertCommand.Parameters.AddWithValue("@param9", currentLoan.TotalAmt);
            insertCommand.Parameters.AddWithValue("@param10", currentLoan.PaidOff);
            insertCommand.Parameters.AddWithValue("@param11", currentLoan.BalanceRemaining);

            insertCommand.Connection = connection;
            insertCommand.ExecuteNonQuery();
            connection.Close();

        }
    }
}
