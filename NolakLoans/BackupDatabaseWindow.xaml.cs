using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NolakLoans
{
    /// <summary>
    /// Interaction logic for BackupDatabaseWindow.xaml
    /// </summary>
    public partial class BackupDatabaseWindow : Window
    {
        public DialogResult result;
        public BackupDatabaseWindow()
        {
            InitializeComponent();
        }

        private void getBackupLocation(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                result = dialog.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    backupLocation.Text = dialog.SelectedPath;
                }
            }
        }

        private void backupDatabase(object sender, RoutedEventArgs e)
        {
            System.IO.File.Copy("Loans.sqlite", backupLocation.Text + @"\Loans" + DateTime.Now.ToString("MM.dd.yyyy") + ".sqlite");
        }
    }
}
