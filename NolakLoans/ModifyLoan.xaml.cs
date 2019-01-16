using NolakLoans.Helper;
using NolakLoans.Types;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows;

namespace NolakLoans
{
    /// <summary>
    /// Interaction logic for ModifyLoan.xaml
    /// </summary>
    public partial class ModifyLoan : Window
    {
        public Loan loan;
        public SQLiteConnection connection;
        public ErrorLog _errorLog = new ErrorLog();
        
        public ModifyLoan(Loan currentLoan)
        {
            InitializeComponent();
            loan = currentLoan;
            loadGui(loan);


        }

        private void populateLoanView(List<Payment> payments)
        {
            try
            {
                this.paymentListView.Items.Clear();

                foreach (Payment p in payments)
                {
                    this.paymentListView.Items.Add(p);
                }
            } catch(Exception e)
            {
                _errorLog.LogToErrorFile(e);
            }

        }

        private void loadGui(Loan loan)
        {
            try
            {
                borrowerName.Text = loan.BName;
                borrowedAmt.Text = loan.BAmt.ToString();
                interestRate.Text = loan.IntRate.ToString();
                loanID.Text = loan.LoanID.ToString();
                collateralHeld.Text = loan.Collat;
                loanStart.SelectedDate = loan.LoanStart;
                loanDurationMonths.Text = loan.LoanDuration.ToString();
                loanLink.Text = loan.LoanLink;
                daysLate.Text = (DateTime.Today - loan.LoanStart.AddDays(loan.LoanDuration * 30)).TotalDays.ToString();
                balRemaining.Text = String.Format("{0:c0}", loan.BalanceRemaining);

                if (loan.PaidOff == 1)
                {
                    paidOff.IsChecked = true;
                }
                else
                {
                    paidOff.IsChecked = false;
                }

                totalPayoffAmt.Text = String.Format("{0:c0}", loan.getTotalLoan());
            }
            catch(Exception e)
            {
                _errorLog.LogToErrorFile(e);
            }
            try
            {
                List<Payment> payments = getPayments(loan.LoanID);
                this.paymentListView.Items.Clear();
                foreach (Payment p in payments)
                {
                    this.paymentListView.Items.Add(p);
                }
            }
            catch(Exception e)
            {
                _errorLog.LogToErrorFile(e);
            }

        }

        private void OpenLink_Click(object sender, RoutedEventArgs eV)
        {
            try
            {
            Uri url = new Uri(loan.LoanLink);
            Process.Start(url.AbsoluteUri);
            } catch(Exception e)
            {
                _errorLog.LogToErrorFile(e);
            }

        }
        private void updateLoan(Loan loanToUpdate)
        {
            connection = new SQLiteConnection("Data Source=Loans.sqlite;Version=3;");
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
        private List<Payment> getPayments(int loanID)
        {
            List<Payment> allPayments = new List<Payment>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Loans.sqlite;Version=3;"))
            {
                con.Open();

                string stm = "SELECT * FROM Payments WHERE loanID=" + loanID.ToString() + " order by paymentID asc";

                using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Payment p = new Payment()
                            {
                                PaymentAmt = Convert.ToDouble(rdr["paymentAmt"].ToString()),
                                PaymentFrom = rdr["paymentFrom"].ToString(),
                                PaymentID = Convert.ToInt32(rdr["paymentID"].ToString())
                            };
                            allPayments.Add(p);
                        }
                    }
                }
                con.Close();
            }
            return allPayments;
        }
        private void ModLoan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Loan modifiedLoan = new Loan()
                {
                    BName = borrowerName.Text,
                    BAmt = Convert.ToDouble(borrowedAmt.Text),
                    IntRate = Convert.ToDecimal(interestRate.Text),
                    LoanID = Convert.ToInt32(loanID.Text),
                    Collat = collateralHeld.Text,
                    LoanStart = (DateTime)loanStart.SelectedDate,
                    LoanDuration = Convert.ToInt32(loanDurationMonths.Text),
                    LoanLink = loanLink.Text,
                    DaysLate = Convert.ToInt32(daysLate.Text),
                    BalanceRemaining = Convert.ToDecimal(balRemaining.Text.Remove(0, 1)),
                    TotalAmt = Convert.ToDecimal(totalPayoffAmt.Text.Remove(0, 1))

                };

                if (paidOff.IsChecked == true)
                {
                    modifiedLoan.PaidOff = 1;
                }
                else
                {
                    modifiedLoan.PaidOff = 0;
                }
                updateLoan(modifiedLoan);
                loadGui(modifiedLoan);
            }catch(Exception ef)
            {
                MessageBox.Show(ef.Message);
            }

            this.Close();

            
        }

        private void processPayment(object sender, RoutedEventArgs e)
        {
            Loan l = loan;
            Payment p = new Payment()
            {
                PaymentAmt = Convert.ToDouble(pmtAmt.Text),
                PaymentFrom = paidFrom.Text,
                LoanID = loan.LoanID
            };

            insertPayment(p);

            l.BalanceRemaining = l.BalanceRemaining - Convert.ToDecimal(pmtAmt.Text);
            loadGui(l);
            updateLoan(l);
            
            paidFrom.Text = "";
            pmtAmt.Text = "";
        }

        private void insertPayment(Payment p)
        {
            connection = new SQLiteConnection("Data Source=Loans.sqlite;Version=3;");
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
            }catch(Exception e)
            {

            }
            connection.Close();
        }



        private void deletePaymentClick(object sender, RoutedEventArgs e)
        {
            if(paymentListView.SelectedItem != null)
            {
                Loan l = loan;
                Payment p = paymentListView.SelectedItem as Payment;
                l.BalanceRemaining = l.BalanceRemaining + (decimal)p.PaymentAmt;

                deletePaymentFromScreen(p);
                loadGui(l);
                updateLoan(l);
            }
            else
            {
                MessageBox.Show("Please select a payment");
            }
            
        }

        private void deletePaymentFromScreen(Payment p)
        {
            connection = new SQLiteConnection("Data Source=Loans.sqlite;Version=3");
            connection.Open();
            SQLiteCommand pmtDltCommand = new SQLiteCommand()
            {
                CommandText = "DELETE FROM Payments WHERE paymentID=@param1"
            };
            pmtDltCommand.Parameters.AddWithValue("@param1", p.PaymentID);
            pmtDltCommand.Connection = connection;
            
            try
            {
                pmtDltCommand.ExecuteNonQuery();
            }
            catch(Exception e)
            {
              
            }
            connection.Close();
        }
    }
}
