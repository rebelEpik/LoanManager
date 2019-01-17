using NolakLoans.Types;
using System;
using System.Data.SQLite;

namespace NolakLoans.Helper
{
    public class InsertLoansHandler
    {
        HelperClass _helper = new HelperClass();
        ErrorLog _errorLog = new ErrorLog();
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

        public void updateLoan(Loan loanToUpdate)
        {
            SQLiteConnection connection = new SQLiteConnection(_helper.ConnectionString);
            connection.Open();
            SQLiteCommand updateCommand = new SQLiteCommand();
            updateCommand.CommandText = @"UPDATE Loans SET name=@param1, amt=@param2, interest=@param3, collateral=@param4, start=@param5, duration=@param6, dayslate=@param7, link=@param8, totalLoanAmt=@param9, paidOff=@param10, balanceRemaining=@param11 WHERE id=@param12";
            updateCommand.Parameters.AddWithValue("@param1", loanToUpdate.BName);
            updateCommand.Parameters.AddWithValue("@param2", loanToUpdate.BAmt);
            updateCommand.Parameters.AddWithValue("@param3", loanToUpdate.IntRate);
            updateCommand.Parameters.AddWithValue("@param4", loanToUpdate.Collat);
            updateCommand.Parameters.AddWithValue("@param5", loanToUpdate.LoanStart);
            updateCommand.Parameters.AddWithValue("@param6", loanToUpdate.LoanDuration);
            updateCommand.Parameters.AddWithValue("@param7", loanToUpdate.DaysLate);
            updateCommand.Parameters.AddWithValue("@param8", loanToUpdate.LoanLink);
            updateCommand.Parameters.AddWithValue("@param9", loanToUpdate.TotalAmt);
            updateCommand.Parameters.AddWithValue("@param10", loanToUpdate.PaidOff);
            updateCommand.Parameters.AddWithValue("@param11", loanToUpdate.BalanceRemaining);
            updateCommand.Parameters.AddWithValue("@param12", loanToUpdate.LoanID);

            updateCommand.Connection = connection;
            updateCommand.ExecuteNonQuery();
            connection.Close();
        }

        public void insertPayment(Payment p)
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection(_helper.ConnectionString);
                connection.Open();
                SQLiteCommand pmtInsertCommand = new SQLiteCommand();
                pmtInsertCommand.CommandText = "INSERT INTO Payments (loanID, paymentAmt, paymentFrom) VALUES (@param1, @param2, @param3)";
                pmtInsertCommand.Connection = connection;
                pmtInsertCommand.Parameters.AddWithValue("@param1", p.LoanID);
                pmtInsertCommand.Parameters.AddWithValue("@param2", p.PaymentAmt);
                pmtInsertCommand.Parameters.AddWithValue("@param3", p.PaymentFrom);

                try
                {
                    pmtInsertCommand.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception e)
                {
                    _errorLog.LogToErrorFile(e);
                }
            }
            catch (Exception ef)
            {
                _errorLog.LogToErrorFile(ef);
            }

        }
    }
}
