﻿using NolakLoans.Properties;
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
using System.Windows.Media;
using System.Collections;
using System.Data;
using System.Windows.Input;

namespace NolakLoans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SQLiteConnection connection;

        public DispatcherTimer updateLV = new DispatcherTimer();
        public GrabAllLoans _loanHelper = new GrabAllLoans();
        public InsertLoansHandler _insertHelper = new InsertLoansHandler();
        public HelperClass _helper;
        public List<Loan> allLoans;

        public ErrorLog _error = new ErrorLog();


        public MainWindow()
        {
            InitializeComponent();
            allLoans = _loanHelper.returnAllLoans();

            currentLoansDG.ItemsSource = _loanHelper.returnAllLoans();
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



        


        private void closeMain(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void selectLoan(object sender, MouseButtonEventArgs e)
        {
            if (currentLoansDG.SelectedItem != null)
            {
                Loan l = (Loan)currentLoansDG.SelectedItem;
                ModifyLoan modify = new ModifyLoan(l);
                modify.Show();
            }
        }
        private void selectSearchLoan(object sender, MouseButtonEventArgs e)
        {
            if (searchLoansDG.SelectedItem != null)
            {
                Loan l = (Loan)searchLoansDG.SelectedItem;
                ModifyLoan modify = new ModifyLoan(l);
                modify.Show();
            }
        }



        private void addLoan(object sender, RoutedEventArgs e)
        {
            Loan currentLoan = new Loan()
            {
                BName = borrowerName.Text.ToLower(),
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
                _insertHelper.insertLoan(currentLoan);
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

        private void populateLoanView(List<Loan> loans)
        {
            currentLoansDG.ItemsSource = null;
            currentLoansDG.ItemsSource = loans;
            currentLoansDG.Items.Refresh();
            currentLoansDG.InvalidateVisual();

            var totalAmtOut = 0;
            BigInteger returnIsk = 0;
            foreach (Loan l in loans)
            {
                totalAmtOut = totalAmtOut + (Int32)l.BAmt;
                returnIsk = returnIsk + (BigInteger)l.TotalAmt;
            }
            totalBorrowedAmt.Content = String.Format("Isk Sent Out: {0:c0}", ((BigInteger)totalAmtOut * 1000000000));
            expectedReturn.Content = String.Format("Expected Return: {0:c0}", returnIsk);
        }
        private void populateSearchView(List<Loan> loans)
        {
            searchLoansDG.ItemsSource = null;
            searchLoansDG.ItemsSource = loans;
            searchLoansDG.Items.Refresh();
            searchLoansDG.InvalidateVisual();

        }

 

        //Button event for search loan
        private void searchLoan(object sender, RoutedEventArgs e)
        {
            if (searchLoans.SelectedValue != null)
            {
                _loanHelper.SearchByName = searchLoans.SelectedValue.ToString();
                populateSearchView(_loanHelper.returnSearchLoans());
            }
            else
            {
                MessageBox.Show("Please enter borrower name before clicking Search Loan");
            }
        }

        //Loads ComboBox on Loan Look Up Tab with a listing of all names that have had loans in the DB in the past
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
