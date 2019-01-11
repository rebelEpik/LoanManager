using NolakLoans.Types;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NolakLoans.Helper
{
    public class GrabLoans
    {
    }

    public class GrabAllLoans
    {
        public HelperClass _helper = new HelperClass();
        public ErrorLog _errorLog = new ErrorLog();
        public GrabAllLoans()
        {

        }

        public List<Loan> returnAllLoans()
        {
            List<Loan> allLoans = new List<Loan>();
            try
            {


                using (SQLiteConnection con = new SQLiteConnection(_helper.ConnectionString))
                {
                    con.Open();

                    string stm = "SELECT * FROM Loans";

                    using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                Loan l = new Loan()
                                {
                                    
                                    LoanID = Convert.ToInt32(rdr["id"]),
                                    BName = rdr["name"].ToString(),
                                    BAmt = Convert.ToDouble(rdr["amt"]),
                                    IntRate = Convert.ToDecimal(rdr["interest"]),
                                    Collat = rdr["collateral"].ToString(),
                                    LoanStart = Convert.ToDateTime(rdr["start"]),
                                    LoanDuration = Convert.ToInt32(rdr["duration"]),
                                    DaysLate = Convert.ToInt32((DateTime.Today - Convert.ToDateTime(rdr["start"]).AddDays(Convert.ToInt32(rdr["duration"]) * 30)).TotalDays),
                                    LoanLink = rdr["link"].ToString(),
                                    PaidOff = Convert.ToInt32(rdr["paidOff"]),
                                    BalanceRemaining = Convert.ToDecimal(rdr["balanceRemaining"])
                                };
                                l.IntRate = Convert.ToDecimal(rdr["interest"].ToString());
                                l.TotalAmt = Convert.ToDecimal(rdr["totalLoanAmt"].ToString());
                                allLoans.Add(l);
                            }
                        }
                    }
                    con.Close();
                }
            }catch(Exception e)
            {
                _errorLog.LogToErrorFile(e);
            }

            return allLoans;
        }

        public List<Loan> returnSearchLoans(string name)
        {
            List<Loan> foundLoans = new List<Loan>();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(_helper.ConnectionString))
                {
                    con.Open();

                    string stm = "SELECT id, name, amt, interest, collateral, start, duration, dayslate, link, totalLoanAmt, paidOff, balanceRemaining FROM Loans WHERE name='" + name + "'";

                    using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                Loan l = new Loan()
                                {
                                    LoanID = Convert.ToInt32(rdr["id"]),
                                    BName = rdr["name"].ToString(),
                                    BAmt = Convert.ToDouble(rdr["amt"]),
                                    IntRate = Convert.ToDecimal(rdr["interest"]),
                                    Collat = rdr["collateral"].ToString(),
                                    LoanStart = Convert.ToDateTime(rdr["start"]),
                                    LoanDuration = Convert.ToInt32(rdr["duration"]),
                                    DaysLate = Convert.ToInt32((DateTime.Today - Convert.ToDateTime(rdr["start"]).AddDays(Convert.ToInt32(rdr["duration"]) * 30)).TotalDays),
                                    LoanLink = rdr["link"].ToString(),
                                    PaidOff = Convert.ToInt32(rdr["paidOff"]),
                                    BalanceRemaining = Convert.ToDecimal(rdr["balanceRemaining"])
                                };
                                l.TotalAmt = Convert.ToDecimal(rdr["totalLoanAmt"].ToString());
                                foundLoans.Add(l);
                            }
                        }
                    }
                    con.Close();
                }
            }catch(Exception e)
            {
                _errorLog.LogToErrorFile(e);
            }
            return foundLoans;
        }
    }
}
