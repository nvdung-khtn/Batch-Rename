using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Batch_Rename
{
    class StringOperationContract
    {
    }

    public class StringArgs
    {
    }

    public class ReplaceArgs : StringArgs
    {
        public string From { get; set; }
        public string To { get; set; }    
    }

    public class NewCaseArgs : StringArgs
    {
        public string Origin { get; set; }
        // 0: lower 1:upper 2:upper first letter
        public int Type { get; set; }
    }

    public class FullnameNormalizeArgs : StringArgs
    {
        public string Final { get; set; }
    }

    public class MoveArgs:StringArgs
    {
        public int Type { get; set; }
    }

    public abstract class StringOperation : INotifyPropertyChanged
    {
        public StringArgs Args { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract string Operate(string origin);

        public abstract string Name { get; }
        public abstract string Description { get; }

        public abstract StringOperation Clone();

        public abstract void Config();
    }

    /* Replace*/
    public class ReplaceOperation : StringOperation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Operate(string origin)
        {
            var args = Args as ReplaceArgs;
            var from = args.From;
            var to = args.To;

            return origin.Replace(from, to); // Goi ham doi ten
        }

        public override StringOperation Clone()
        {
            var oldArgs = Args as ReplaceArgs;
            return new ReplaceOperation()
            {
                Args = new ReplaceArgs()
                {
                    From = oldArgs.From,
                    To = oldArgs.To
                }
            };
        }

        public override void Config()
        {
            var screen = new ReplaceConfigDialog(Args);
            if (screen.ShowDialog() == true)
            {              
                // Cap nhap lai du lieu
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs("Description"));
            }
            
        }

        public override string Name => "Replace";
        public override string Description
        {
            get
            {
                var args = Args as ReplaceArgs;
                //return $"Replace from {args.From} to {args.To}";
                return args.To;
            }
        }
    }

    /* NEW CASE*/
    public class NewCaseOperation : StringOperation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Operate(string final)
        {
            var args = Args as NewCaseArgs;
            var origin = args.Origin;
            var type = args.Type;

            if (type == 0)
            {
                return final.ToLower();
            }
            else if (type == 1)
            {
                return final.ToUpper();
            }
            else
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                return textInfo.ToTitleCase(final);
            }
        }
        
        public override StringOperation Clone()
        {
            var oldArgs = Args as NewCaseArgs;
            return new NewCaseOperation()
            {
                Args = new NewCaseArgs()
                {
                    Origin = oldArgs.Origin,
                    Type = oldArgs.Type
                }
            };
        }

        public override void Config()
        {
            var screen = new NewCaseConfigDialog(Args);
            if (screen.ShowDialog() == true)
            {
                // Cap nhap lai du lieu
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs("Description"));
            }

        }

        public override string Name => "New Case";
        public override string Description
        {
            get
            {
                var args = Args as NewCaseArgs;
                //return $"Replace from {args.From} to {args.To}";
                return args.Origin;
            }
        }
    }



    /* Fullname Normalize*/
    public class FullnameNormalizeOperation : StringOperation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Operate(string origin)
        {
            var args = Args as FullnameNormalizeArgs;
            var final = args.Final;          

            return NormalizeFunction(origin);
        }

        public override StringOperation Clone()
        {
            var oldArgs = Args as FullnameNormalizeArgs;
            return new FullnameNormalizeOperation()
            {
                Args = new FullnameNormalizeArgs()
                {
                    Final = oldArgs.Final,
                }
            };
        }

        public override void Config()
        {
            /*var screen = new FullnameNormalizeConfigDialog(Args);
            if (screen.ShowDialog() == true)
            {
                // Cap nhap lai du lieu
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs("Description"));
            }*/
            MessageBox.Show("Method have not setting.");

        }

        public override string Name => "FullnameNormalize";
        public override string Description
        {
            get
            {
                var args = Args as FullnameNormalizeArgs;
                //return $"Replace from {args.From} to {args.To}";
                return args.Final;
            }
        }

        public static string NormalizeFunction(string origin)
        {
            const string Slash = " ";
            string[] token = origin.Split(new string[] { Slash },
                StringSplitOptions.RemoveEmptyEntries);

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            string final = textInfo.ToTitleCase(token[0]);
            for (int i = 1; i < token.Length; i++)
            {
                final += " " + textInfo.ToTitleCase(token[i]);
            }
            return final;
        }
    }

    /* Move*/
    public class MoveOperation : StringOperation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Operate(string origin)
        {
            var args = Args as MoveArgs;
            var type = args.Type;

            if (origin.Length > 13)
            {
                string isbn = origin.Substring(0, 13);
                string temp = origin.Remove(0, 13);
                const string Slash = ".";
                string[] token = temp.Split(new string[] { Slash },
                    StringSplitOptions.None);
                string filename = token[0];
                string extension = "." + token[1];
                // 0: Số ISBN - Tên File
                // 1: Tên File - Số ISBN
                
               if(type == 1 && extension==".")
                {
                    return filename + " " + isbn;
                }
               return filename + " " + isbn + extension;
            }
            return origin;
        }

        public override StringOperation Clone()
        {
            var oldArgs = Args as MoveArgs;
            return new MoveOperation()
            {
                Args = new MoveArgs()
                {                
                    Type = oldArgs.Type
                }
            };
        }

        public override void Config()
        {
            var screen = new MoveConfigDialog(Args);
            if (screen.ShowDialog() == true)
            {
                // Cap nhap lai du lieu
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs("Description"));
            }

        }

        public override string Name => "Move";
        public override string Description // Vi trí trả về giá trị của màn hình trc
        {
            get
            {
                var args = Args as MoveArgs;                  
                //return args.Type;
                return "";
            }
        }
    }
}
