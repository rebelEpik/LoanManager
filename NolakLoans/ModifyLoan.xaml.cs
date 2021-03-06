﻿using NolakLoans.Helper;
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
        public InsertLoansHandler _insertHelper = new InsertLoansHandler();
        public GrabAllLoans _loansHelper = new GrabAllLoans();
        
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
                List<Payment> payments = _loansHelper.getPayments(loan.LoanID);
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
                _insertHelper.updateLoan(modifiedLoan);
                loadGui(modifiedLoan);
            }catch(Exception ef)
            {
                _errorLog.LogToErrorFile(ef);
            }

            this.Close();

            
        }

        private void processPayment(object sender, RoutedEventArgs e)
        {
            Loan l = loan;
            try
            {

                Payment p = new Payment()
                {
                    PaymentAmt = Convert.ToDouble(pmtAmt.Text),
                    PaymentFrom = paidFrom.Text,
                    LoanID = loan.LoanID
                };
                _insertHelper.insertPayment(p);
                l.BalanceRemaining = l.BalanceRemaining - Convert.ToDecimal(pmtAmt.Text);
                loadGui(l);
                _insertHelper.updateLoan(l);

                paidFrom.Text = "";
                pmtAmt.Text = "";
            }
            catch (Exception ef)
            {
                _errorLog.LogToErrorFile(ef);
            }
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
                _insertHelper.updateLoan(l);
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
