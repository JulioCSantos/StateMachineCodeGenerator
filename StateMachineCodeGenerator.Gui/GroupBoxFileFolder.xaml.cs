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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StateMachineCodeGenerator.Gui
{
    /// <summary>
    /// Interaction logic for GroupBoxFileFolder.xaml
    /// </summary>
    public partial class GroupBoxFileFolder : GroupBox
    {
        public GroupBoxFileFolder() {
            InitializeComponent();
            //DefaultStyleKey=
        }

        #region TextProperty
        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string)
            , typeof(GroupBoxFileFolder), new PropertyMetadata(null, TextCallBack)); 
        public string Text {
            get { return (string)this.GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = d as GroupBoxFileFolder;
        }
        #endregion TextProperty

        #region LocateFileFolderCommandProperty
        public static DependencyProperty LocateFileFolderCommandProperty = DependencyProperty.Register("LocateFileFolderCommand"
            , typeof(ICommand), typeof(GroupBoxFileFolder), new PropertyMetadata(null, LocateFileFolderCommandCallBack)); 
        public ICommand LocateFileFolderCommand {
            get { return (ICommand)this.GetValue(LocateFileFolderCommandProperty); }
            set { SetValue(LocateFileFolderCommandProperty, value); }
        }

        private static void LocateFileFolderCommandCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = d as GroupBoxFileFolder;
        }
        #endregion LocateFileFolderCommandProperty

        #region CommandParameterProperty
        public static DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter"
            , typeof(object), typeof(GroupBoxFileFolder),
            new PropertyMetadata(null, CommandParameterCallBack)); 
        public object CommandParameter {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        private static void CommandParameterCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = d as GroupBoxFileFolder;
        }
        #endregion CommandParameterProperty
    }
}
