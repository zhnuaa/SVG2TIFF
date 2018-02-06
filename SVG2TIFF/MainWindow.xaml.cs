using Ookii.Dialogs.Wpf;
using StyxFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SVG2TIFF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string selectFolder;
        public List<string> svgList;
        public static readonly string[] extNameList = { @".tiff", @".png", @".jpg" };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void _BTNOpen_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog()
            {
                Description = @"请选择SVG目录",
                ShowNewFolderButton=false
            };
            if (folderDialog.ShowDialog() == true) 
            {
                selectFolder = folderDialog.SelectedPath;
                svgList = System.IO.Directory.GetFiles(selectFolder, @"*.svg",System.IO.SearchOption.AllDirectories).ToList();
                int svgNum = svgList.Count;
                _TBTotal.Text = svgNum.ToString();
                _List.ItemsSource = svgList;
            }
        }

        private void _BTNConvert_Click(object sender, RoutedEventArgs e)
        {
            if (svgList.Count != 0)
            {                
                CmdBuilder cmdBuilder = new CmdBuilder()
                {
                    Command = @"magick"
                };
                cmdBuilder.UpdateArgs("convert", "", "convert");
                cmdBuilder.UpdateArgs("density", "-density", _DBDPI.Value.ToString());                
                string extName = extNameList[_CBFormat.SelectedIndex];
                CommandExcutor excutor = new CommandExcutor(cmdBuilder);
                int count = 1;
                foreach(var svg in svgList)
                {
                    _TBProgress.Text = count.ToString();
                    string outputName = System.IO.Path.ChangeExtension(svg,extName);
                    cmdBuilder.UpdateArgs("input", "", svg, true);
                    cmdBuilder.UpdateArgs("output", "", outputName, true);                    
                    //MessageBox.Show(excutor.CommandCB.Command);
                    //MessageBox.Show(excutor.CommandCB.GetString());
                    excutor.Excute();
                    count++;
                }
                _TBProgress.Text = @"0";
                _TBProgress.Text = "0";
                MessageBox.Show("转换完成!");
            }
        }
    }
}
