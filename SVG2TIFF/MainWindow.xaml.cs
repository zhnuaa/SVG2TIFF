using Microsoft.WindowsAPICodePack.Taskbar;
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
        public DataClass data;
        public MainWindow()
        {
            InitializeComponent();
            data = new DataClass();
            _GridRoot.DataContext = data;
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
                data.SvgList = System.IO.Directory.GetFiles(selectFolder, @"*.svg",System.IO.SearchOption.AllDirectories).ToList();
            }
        }

        private void _BTNConvert_Click(object sender, RoutedEventArgs e)
        {
            if (data.SvgNum != 0)
            {                
                CmdBuilder cmdBuilder = new CmdBuilder()
                {
                    Command = @"magick"
                };
                cmdBuilder.UpdateArgs("convert", "", "convert");
                cmdBuilder.UpdateArgs("density", "-density", _DBDPI.Value.ToString());
                cmdBuilder.UpdateArgs("compress", "-compress", data.Compress[_CBCompress.SelectedIndex].ToLower());
                string extName = data.Format[_CBFormat.SelectedIndex].ToLower();
                CommandExcutor excutor = new CommandExcutor(cmdBuilder);
                int count = 1;
                foreach(var svg in data.SvgList)
                {
                    TaskbarManager.Instance.SetProgressValue(count, data.SvgNum);
                    string outputName = System.IO.Path.ChangeExtension(svg,extName);
                    cmdBuilder.UpdateArgs("input", "", svg, true);
                    cmdBuilder.UpdateArgs("output", "", outputName, true);                    
                    //MessageBox.Show(excutor.CommandCB.Command);
                    //MessageBox.Show(excutor.CommandCB.GetString());
                    excutor.Excute();
                    count++;
                }
                data.SvgList = new List<string>();
                TaskbarManager.Instance.SetProgressValue(0, 1);
                MessageBox.Show("转换完成!");
            }
        }
    }

    public class DataClass : System.ComponentModel.INotifyPropertyChanged
    {

        private List<string> svgList;
        public List<string> SvgList
        {
            get { return svgList; }
            set
            {
                svgList = value;
                NotifyPropertyChanged("SvgList");
                NotifyPropertyChanged("SvgNum");
            }
        }
        public int SvgNum { get { return svgList.Count; } }

        public List<string> Compress { get { return new List<string> { @"LZW", @"None", @"RLE", @"Zip", @"B44", @"LZMA", @"B44A", @"BZip", @"DXT1", @"DXT3", @"DXT5", @"Fax", @"Group4", @"JBIG1", @"JBIG2", @"JPEG", @"JPEG2000", @"Lossless", @"LosslessJPEG", @"Piz", @"Pxr24", @"RunlengthEncoded", @"ZipS" }; } }
        public List<string> Format { get { return new List<string>() { @"TIFF", @"PNG", @"JPG" }; } }

        //构造函数
        public DataClass()
        {
            SvgList = new List<string>();
        }
        //注册属性改变事件，便于通告属性改变
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
        }
    }


}
