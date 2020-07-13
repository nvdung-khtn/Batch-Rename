using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Batch_Rename
{
   
    public partial class ReplaceConfigDialog : Window
    {
        ReplaceArgs myArgs;

        public ReplaceConfigDialog(StringArgs args)
        {
            InitializeComponent();

            myArgs = args as ReplaceArgs;
            replacedWithTextBox.Text = myArgs.From;
            replacedWithTextBox.Text = myArgs.To;
            this.DataContext = myArgs;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {          
            DialogResult = true;
            Close();
        }
    }
}
