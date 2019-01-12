using NolakLoans.Properties;
using NolakLoans.Types;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Numerics;
using System.ComponentModel;
using System.Linq;
using NolakLoans.Helper;

namespace NolakLoans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SQLiteConnection connection;

        public DispatcherTimer updateLV = new DispatcherTimer();
        public string searchByName;
        public BindingListCollectionView blcv;
        public GridViewColumnHeader _lastHeaderClicked;
        public ListSortDirection _lastDirection = ListSortDirection.Ascending;
        public GrabAllLoans _loanHelper = new GrabAllLoans();
        public HelperClass _helper;

        public ErrorLog _error = new ErrorLog();


        public MainWindow()
        {
            InitializeComponent();
            _lastHeaderClicked = null;


            updateLV.Tick += updateListView;
            updateLV.Interval = new TimeSpan(0, 0, 30);
            updateLV.Start();
            if (!File.Exists("Loans.sqlite"))
            {
                _helper.createDB();
            }
            else
            {
                populateLoanView(_loanHelper.returnAllLoans());
            }

            loadComboBox();


        }

        private void updateListView(object sender, EventArgs e)
        {
            populateLoanView(_loanHelper.returnAllLoans());
        }

        private void populateLoanView(List<Loan> loans)
        {
            this.loanListView.Items.Clear();
            var totalAmtOut = 0;
            BigInteger returnIsk = 0;
            foreach(Loan l in loans)
            {
                totalAmtOut = totalAmtOut + (Int32)l.BAmt;
                returnIsk = returnIsk + (BigInteger)l.TotalAmt;
                this.loanListView.Items.Add(l);
            }
           foreach(ListViewItem lvi in loanListView.Items)
            {
                loanListView.Items
            }
            totalBorrowedAmt.Content = String.Format("Isk Sent Out: {0:c0}", ((BigInteger)totalAmtOut * 1000000000));
            expectedReturn.Content = String.Format("Expected Return: {0:c0}", returnIsk);
            //
        }

        
        private void insertLoan(Loan currentLoan)
        {
            connection = new SQLiteConnection("Data Source=Loans.sqlite;Version=3;");
            connection.Open();
            SQLiteCommand insertCommand = new SQLiteCommand();
            insertCommand.CommandText = @"Insert Into Loans (name, amt, interest, collateral, start, duration, dayslate, link, totalLoanAmt, paidOff, balanceRemaining) values(@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9, @param10, @param11)";
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

        private void closeMain(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void selectLoan(object sender, RoutedEventArgs e)
        {

            Loan t = ((ListView)sender).SelectedItem as Loan;
            ModifyLoan modify = new ModifyLoan(t);
            modify.Show();
        }
        private void sortColumn(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            string header = headerClicked.Content.ToString();
            List<Loan> sortedLoans = new List<Loan>();
            if(header == "Borrower Name")
            {
                if (_lastDirection == ListSortDirection.Descending)
                {
                    sortedLoans = _loanHelper.returnAllLoans().OrderByDescending(o => o.BName).ToList();
                    populateLoanView(sortedLoans);
                    _lastDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortedLoans = _loanHelper.returnAllLoans().OrderBy(o => o.BName).ToList();
                    populateLoanView(sortedLoans);
                    _lastDirection = ListSortDirection.Descending;
                }
            }if(header == "Initial Borrowed Amount"){
                if (_lastDirection == ListSortDirection.Descending)
                {
                    sortedLoans = _loanHelper.returnAllLoans().OrderByDescending(o => o.BAmt).ToList();
                    populateLoanView(sortedLoans);
                    _lastDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortedLoans = _loanHelper.returnAllLoans().OrderBy(o => o.BAmt).ToList();
                    populateLoanView(sortedLoans);
                    _lastDirection = ListSortDirection.Descending;
                }
            }
            if(header == "Borrowed Amount Remaining")
            {
                if (_lastDirection == ListSortDirection.Descending)
                {
                    sortedLoans = _loanHelper.returnAllLoans().OrderByDescending(o => o.BalAmt).ToList();
                    populateLoanView(sortedLoans);
                    _lastDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortedLoans = _loanHelper.returnAllLoans().OrderBy(o => o.BalAmt).ToList();
                    populateLoanView(sortedLoans);
                    _lastDirection = ListSortDirection.Descending;
                }
            }
            if(header == "Days Late")
            {
                if (_lastDirection == ListSortDirection.Descending)
                {
                    sortedLoans = _loanHelper.returnAllLoans().OrderByDescending(o => o.DaysLate).ToList();
                    populateLoanView(sortedLoans);
                    _lastDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortedLoans = _loanHelper.returnAllLoans().OrderBy(o => o.DaysLate).ToList();
                    populateLoanView(sortedLoans);
                    _lastDirection = ListSortDirection.Descending;
                }
            }
            if(header == "Paid Off")
            {
                if (_lastDirection == ListSortDirection.Descending)
                {
                    sortedLoans = _loanHelper.returnAllLoans().OrderByDescending(o => o.PaidOffDisplay).ToList();
                    populateLoanView(sortedLoans);
                    _lastDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortedLoans = _loanHelper.returnAllLoans().OrderBy(o => o.PaidOffDisplay).ToList();
                    populateLoanView(sortedLoans);
                    _lastDirection = ListSortDirection.Descending;
                }
            }
            if(header == "Collateral")
            {

            }

          
        }

        private void sortSearchColumn(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            string header = headerClicked.Content.ToString();
            List<Loan> sortedLoans = new List<Loan>();
            if (header == "Borrower Name")
            {
                if (_lastDirection == ListSortDirection.Descending)
                {
                    sortedLoans = returnSearchLoans().OrderByDescending(o => o.BName).ToList();
                    populateSearchView(sortedLoans);
                    _lastDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortedLoans = returnSearchLoans().OrderBy(o => o.BName).ToList();
                    populateSearchView(sortedLoans);
                    _lastDirection = ListSortDirection.Descending;
                }
            }
            if (header == "Initial Borrowed Amount")
            {
                if (_lastDirection == ListSortDirection.Descending)
                {
                    sortedLoans = returnSearchLoans().OrderByDescending(o => o.BAmt).ToList();
                    populateSearchView(sortedLoans);
                    _lastDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortedLoans = returnSearchLoans().OrderBy(o => o.BAmt).ToList();
                    populateSearchView(sortedLoans);
                    _lastDirection = ListSortDirection.Descending;
                }
            }
            if (header == "Borrowed Amount Remaining")
            {
                if (_lastDirection == ListSortDirection.Descending)
                {
                    sortedLoans = returnSearchLoans().OrderByDescending(o => o.BalAmt).ToList();
                    populateSearchView(sortedLoans);
                    _lastDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortedLoans = returnSearchLoans().OrderBy(o => o.BalAmt).ToList();
                    populateSearchView(sortedLoans);
                    _lastDirection = ListSortDirection.Descending;
                }
            }
            if (header == "Days Late")
            {
                if (_lastDirection == ListSortDirection.Descending)
                {
                    sortedLoans = returnSearchLoans().OrderByDescending(o => o.DaysLate).ToList();
                    populateSearchView(sortedLoans);
                    _lastDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortedLoans = returnSearchLoans().OrderBy(o => o.DaysLate).ToList();
                    populateSearchView(sortedLoans);
                    _lastDirection = ListSortDirection.Descending;
                }
            }
            if (header == "Paid Off")
            {
                if (_lastDirection == ListSortDirection.Descending)
                {
                    sortedLoans = returnSearchLoans().OrderByDescending(o => o.PaidOffDisplay).ToList();
                    populateSearchView(sortedLoans);
                    _lastDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortedLoans = returnSearchLoans().OrderBy(o => o.PaidOffDisplay).ToList();
                    populateSearchView(sortedLoans);
                    _lastDirection = ListSortDirection.Descending;
                }
            }
            if (header == "Collateral")
            {

            }


        }
        private void addLoan(object sender, RoutedEventArgs e)
        {
            Loan currentLoan = new Loan()
            {
                BName = borrowerName.Text,
                BAmt = Convert.ToDouble(borrowedAmt.Text),
                IntRate = Convert.ToDecimal(interestRate.Text),
                LoanStart = (DateTime)loanStart.SelectedDate,
                Collat = collateralHeld.Text,
                LoanDuration = Convert.ToInt32(loanDurationMonths.Text),
                LoanLink = loanLink.Text,
                DaysLate = (int)(DateTime.Today - ((DateTime)loanStart.SelectedDate).AddDays(Convert.ToInt32(loanDurationMonths.Text) * 30)).TotalDays


            };
            currentLoan.TotalAmt = currentLoan.getTotalLoan();
            currentLoan.BalanceRemaining = currentLoan.getTotalLoan();

            try
            {
                insertLoan(currentLoan);
            }catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            populateLoanView(_loanHelper.returnAllLoans());
            clearLoanData(this, null);
        }
        //DB Form Functions
        private void backupDB(object sender, RoutedEventArgs e)
        {
            BackupDatabaseWindow backup = new BackupDatabaseWindow();
            backup.Show();
        }
        private void restoreDB(object sender, RoutedEventArgs e)
        {

        }
        //End DB Form Functions


        private void clearLoanData(object sender, RoutedEventArgs e)
        {
            borrowerName.Text = "";
            borrowedAmt.Text = "";
            collateralHeld.Text = "";
            loanLink.Text = "";
            loanDurationMonths.Text = "";
            interestRate.Text = "";
            loanStart.Text = "";
            
        }

        private void populateSearchView(List<Loan> loans)
        {
            this.searchListView.Items.Clear();
            foreach (Loan l in loans)
            {
                this.searchListView.Items.Add(l);
            }

        }

        private List<Loan> returnSearchLoans()
        {
            List<Loan> foundLoans = new List<Loan>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Loans.sqlite;Version=3;"))
            {
                con.Open();

                string stm = "SELECT id, name, amt, interest, collateral, start, duration, dayslate, link, totalLoanAmt, paidOff, balanceRemaining FROM Loans WHERE name='" + searchByName + "'";

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
            populateSearchView(foundLoans);
            return foundLoans;

        }

        private void searchLoan(object sender, RoutedEventArgs e)
        {
            searchByName = searchLoans.SelectedValue.ToString();
            populateSearchView(returnSearchLoans());

        }

        private void loadComboBox()
        {
            List<Loan> loans = _loanHelper.returnAllLoans().OrderBy(o => o.BName).ToList();
            searchLoans.Items.Clear();
            foreach(Loan l in loans)
            {
                if (!searchLoans.Items.Contains(l.BName))
                {
                    searchLoans.Items.Add(l.BName);
                }
            }
        }
    }


}
