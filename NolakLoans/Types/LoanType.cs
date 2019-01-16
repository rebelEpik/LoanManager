using System;
using System.Globalization;

namespace NolakLoans.Types
{
    class LoanType
    {
    }

    public class Loan
    {

        public Loan()
        {
            DaysLate = Convert.ToInt32((LoanStart.AddDays(LoanDuration * 30) - DateTime.Today).TotalDays);
            PaidOff = 0; //1 for true, 0 for false
        }
        private int _loanID;
        private string _bName;
        private double _bAmt;
        private decimal _intRate;
        private string _collat;
        private DateTime _loanStart;
        private int _loanDuration;
        private string _loanLink;
        private int _daysLate;
        private int _paidOff;
        private decimal _totalAmt;
        private decimal _balanceRemaining;

        public string BName { get => _bName; set => _bName = value; }
        public double BAmt { get => _bAmt; set => _bAmt = value; }
        public decimal IntRate { get => _intRate; set => _intRate = value; }
        public string Collat { get => _collat; set => _collat = value; }
        public DateTime LoanStart { get => _loanStart; set => _loanStart = value; }
        public int LoanDuration { get => _loanDuration; set => _loanDuration = value; }
        public string LoanLink { get => _loanLink; set => _loanLink = value; }        
        public int DaysLate { get => _daysLate; set => _daysLate = value; }
        public int LoanID { get => _loanID; set => _loanID = value; }
        public int PaidOff { get => _paidOff; set => _paidOff = value; }
        public decimal TotalAmt { get => _totalAmt; set => _totalAmt = value; }
        public decimal BalanceRemaining { get => _balanceRemaining; set => _balanceRemaining = value; }


        public string BalAmt
        {
            get
            {
                return BalanceRemaining.ToString("c0", CultureInfo.CurrentCulture);
            }
        }

        public string BorAmt
        {
            get
            {
                return (BAmt * 1000000000).ToString("c0", CultureInfo.CurrentCulture);
            
            }
        }

        public string PaidOffDisplay
        {
            get
            {
                if(PaidOff == 0)
                {
                    return "No";
                }
                else
                {
                    return "Yes";
                }
            }
        }

        public string Late
        {
            get
            {
                if (DaysLate.ToString().Contains("-"))
                {
                    return "0";
                }
                return "1";
            }
        }


        

        public decimal getTotalLoan()
        {

            return (((decimal)BAmt * 1000000000) * (IntRate / 100) * LoanDuration) + ((decimal)BAmt * 1000000000);
        }

        public bool Current
        {
            get
            {
                if (DaysLate.ToString().Contains("-"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        

    }
}
