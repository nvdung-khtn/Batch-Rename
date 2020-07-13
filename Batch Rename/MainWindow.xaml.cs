using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace Batch_Rename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<StringOperation> _prototypes = new List<StringOperation>();

        BindingList<StringOperation> _actions = new BindingList<StringOperation>();
        List<FileInformation> Files = new List<FileInformation>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Nạp CSDL để biết những khả năng đổi tên mình có thể
            var prototype1 = new ReplaceOperation()
            {
                Args = new ReplaceArgs()
                {
                    From = "from",
                    To = "to"
                }
            };

            var prototype2 = new NewCaseOperation()
            {
                Args = new NewCaseArgs()
                {
                    Origin = "",
                    Type = 0
                }
            };

            var prototype3 = new FullnameNormalizeOperation()
            {
                Args = new FullnameNormalizeArgs()
                {
                    Final = "",
                }
            };

            var prototype4 = new MoveOperation()
            {
                Args = new MoveArgs()
                {
                    Type = 0
                }
            };

            _prototypes.Add(prototype1);
            _prototypes.Add(prototype2);
            _prototypes.Add(prototype3);
            _prototypes.Add(prototype4);

            methodComboBox.ItemsSource = _prototypes;
            operationsListBox.ItemsSource = _actions;
        }

        private void PrototypesComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var action =
                methodComboBox.SelectedItem as StringOperation;
            _actions.Add(action.Clone());
            Preview();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = operationsListBox.SelectedItem as StringOperation;
            item.Config();
            Preview();
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var index = operationsListBox.SelectedIndex;
            _actions.RemoveAt(index);
        }





        private void AddMethod_Click(object sender, RoutedEventArgs e)
        {

        }


        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            filesSelectedListView.ItemsSource = null;
            var screen = new OpenFileDialog();
            screen.Multiselect = true;
            FileInformation tempFile = new FileInformation();
            if (screen.ShowDialog() == true)
            {
                // Lấy ra thông tin tất cả file được chọn
                foreach (string fileName in screen.FileNames)
                {
                    tempFile.FileName = System.IO.Path.GetFileName(fileName);
                    tempFile.Path = fileName;
                    tempFile.Error = "ok";
                    tempFile.NewFileName = tempFile.FileName;

                    const string Slash = ".";
                    string[] token = tempFile.FileName.Split(new string[] { Slash },
                        StringSplitOptions.None);
                    tempFile.Name = token[0];
                    tempFile.Extension = "." + token[1];

                    bool containsItem = Files.Any(item => item.FileName == tempFile.FileName);
                    if (containsItem == false)
                    {
                        Files.Add(new FileInformation(tempFile));
                    }
                    else
                    {
                        MessageBox.Show("The items are already in the list");
                    }
                }
            }
            filesSelectedListView.ItemsSource = Files;
        }
        public class FileInformation : INotifyPropertyChanged
        {
            private string _fileName;
            private string _path;
            private string _newFileName;

            public string FileName
            {
                get { return _fileName; }
                set
                {
                    _fileName = value;
                    Notify("FileName");
                }

            }
            public string NewFileName
            {
                get { return _newFileName; }
                set
                {
                    _newFileName = value;
                    Notify("NewFileName");
                }

            }
            public string Path
            {
                get { return _path; }
                set
                {
                    _path = value;
                    Notify("Path");
                }

            }
            public string Name { get; set; }
            public string Extension { get; set; }

            public string Error { get; set; }

            public FileInformation(FileInformation tempFile)
            {
                this.FileName = tempFile.FileName;
                this.Path = tempFile.Path;
                this.Error = tempFile.Error;
                this.NewFileName = tempFile.NewFileName;
                this.Name = tempFile.Name;
                this.Extension = tempFile.Extension;
            }

            public FileInformation()
            {
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void Notify(string propertyName)
            {
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        private void StartBatchButton_Click(object sender, RoutedEventArgs e)
        {
            string oldNameFile = "";
            string newNameFile = "";
            string oldName = "";
            string option = optionApplyMethodComboBox.SelectedValue.ToString();
            if (option == "Name") // Làm việc với tên file
            {
                for (int index = 0; index < Files.Count; index++)
                {
                    oldName = Files[index].Name;
                    for (int i = 0; i < _actions.Count; i++)
                    {
                        newNameFile = _actions[i].Operate(oldName);
                    }
                    Debug.WriteLine(newNameFile);

                    // Tiến hành đổi tên file tại vị trí lưu trên ổ đĩa
                    oldNameFile = Files[index].FileName;
                    newNameFile = newNameFile + Files[index].Extension;
                    string newPath = Files[index].Path.Replace(oldNameFile, newNameFile);
                    File.Move(Files[index].Path, newPath);

                    // Tiến hành cập nhập trên ListView.
                    Files[index].FileName = newNameFile;
                    Files[index].Path = newPath;
                }
            }
            else if (option == "Extension") //Làm việc với phần đuôi mở rộng
            {
                for (int index = 0; index < Files.Count; index++)
                {
                    oldName = Files[index].Extension;
                    for (int i = 0; i < _actions.Count; i++)
                    {
                        newNameFile = _actions[i].Operate(oldName);
                    }
                    Debug.WriteLine(newNameFile);

                    // Tiến hành đổi tên file tại vị trí lưu trên ổ đĩa
                    oldNameFile = Files[index].FileName;
                    newNameFile = Files[index].Name + newNameFile;
                    string newPath = Files[index].Path.Replace(oldNameFile, newNameFile);
                    File.Move(Files[index].Path, newPath);

                    // Tiến hành cập nhập trên ListView.
                    Files[index].FileName = newNameFile;
                    Files[index].Path = newPath;
                }
            }
            else
            {
                for (int index = 0; index < Files.Count; index++)
                {
                    oldNameFile = Files[index].FileName;
                    for (int i = 0; i < _actions.Count; i++)
                    {
                        newNameFile = _actions[i].Operate(oldNameFile);
                    }
                    Debug.WriteLine(newNameFile);

                    // Tiến hành đổi tên file tại vị trí lưu trên ổ đĩa
                    string newPath = Files[index].Path.Replace(oldNameFile, newNameFile);
                    File.Move(Files[index].Path, newPath);

                    // Tiến hành cập nhập trên ListView.
                    Files[index].FileName = newNameFile;
                    Files[index].Path = newPath;
                }
            }
            MessageBox.Show("Rename file successfully.");
        }

        // Hàm cho xem trước và kiểm tra lỗi.
        private void Preview()
        {
            string oldNameFile = "";
            string newNameFile = "";
            string oldName = "";
            string option = optionApplyMethodComboBox.SelectedValue.ToString();

            if (option == "Name") // Làm việc với tên file
            {
                for (int index = 0; index < Files.Count; index++)
                {
                    oldName = Files[index].Name;
                    for (int i = 0; i < _actions.Count; i++)
                    {
                        newNameFile = _actions[i].Operate(oldName);
                    }

                    newNameFile = newNameFile + Files[index].Extension;

                    // Tiến hành cập nhập giá trị NewFileName.
                    Files[index].NewFileName = newNameFile;
                }
            }
            else if (option == "Extension") //Làm việc với phần đuôi mở rộng
            {
                for (int index = 0; index < Files.Count; index++)
                {
                    oldName = Files[index].Extension;
                    for (int i = 0; i < _actions.Count; i++)
                    {
                        newNameFile = _actions[i].Operate(oldName);
                    }

                    newNameFile = Files[index].Name + newNameFile;

                    // Tiến hành cập nhập giá trị NewFileName.
                    Files[index].NewFileName = newNameFile;
                }
            }
            else
            {
                for (int index = 0; index < Files.Count; index++)
                {
                    oldNameFile = Files[index].FileName;
                    for (int i = 0; i < _actions.Count; i++)
                    {
                        newNameFile = _actions[i].Operate(oldNameFile);
                    }

                    // Tiến hành cập nhập giá trị NewFileName.
                    Files[index].NewFileName = newNameFile;
                }
            }
        }


        private void nameOption_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void operationsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
