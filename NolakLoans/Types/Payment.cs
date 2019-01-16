using System;

namespace NolakLoans.Types
{
    public class Payment
    {
        private int _paymentID;
        private int _loanID;
        private double _paymentAmt;
        private string _paymentFrom;

        public int PaymentID { get => _paymentID; set => _paymentID = value; }
        public int LoanID { get => _loanID; set => _loanID = value; }
        public double PaymentAmt { get => _paymentAmt; set => _paymentAmt = value; }
        public string PaymentFrom { get => _paymentFrom; set => _paymentFrom = value; }

        public string PmtAmt
        {
            get
            {
                return String.Format("{0:c0}", PaymentAmt);
            }
        }
    }
}
