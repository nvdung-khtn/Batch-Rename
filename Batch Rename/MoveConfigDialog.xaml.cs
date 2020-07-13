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
    /// <summary>
    /// Interaction logic for MoveConfigDialog.xaml
    /// </summary>
    public partial class MoveConfigDialog : Window
    {
        MoveArgs myArgs;
        public MoveConfigDialog(StringArgs args)
        {
            InitializeComponent();
            myArgs = args as MoveArgs;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileName_ISBN_RadioButton.IsChecked == true)
            {
                myArgs.Type = 1;
            }
            else if (ISBN_FileName_RadioButton.IsChecked == true)
            {
                myArgs.Type = 0;
            }          

            this.DataContext = myArgs;
            DialogResult = true;
            Close();
        }
    }
}
