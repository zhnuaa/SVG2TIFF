using Microsoft.WindowsAPICodePack.Taskbar;
using Ookii.Dialogs.Wpf;
using StyxFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private string selectFolder;
        private DataClass data;
        
        public MainWindow()
        {
            InitializeComponent();
            data = new DataClass();
            _GridRoot.DataContext = data;
            _List.DataContext = data;
        }

        private void _BTNOpen_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog()
            {
                Description = @"请选择图片目录",
                ShowNewFolderButton=false
            };
            if (folderDialog.ShowDialog() == true) 
            {
                selectFolder = folderDialog.SelectedPath;                
                List<ImagePath> imageList = new List<ImagePath>();
                Console.WriteLine(data.FilterList[_CBFormatIn.SelectedIndex]);
                foreach (var image in System.IO.Directory.GetFiles(selectFolder, data.FilterList[_CBFormatIn.SelectedIndex], System.IO.SearchOption.AllDirectories))
                {
                    ImagePath imagePath = new ImagePath();
                    imagePath.Path = image;
                    imageList.Add(imagePath);
                }
                data.ImageList = imageList;
                Console.WriteLine(imageList.Count);
            }
        }

        private void _BTNConvert_Click(object sender, RoutedEventArgs e)
        {
            if (data.ImageNum != 0)
            {                
                CmdBuilder cmdBuilder = new CmdBuilder()
                {
                    Command = @"magick"
                };
                cmdBuilder.UpdateInputArgs("convert", "", "convert");
                if (_DBDPI.Value != 0)
                {
                    cmdBuilder.UpdateInputArgs("density", "-density", _DBDPI.Value.ToString());
                    cmdBuilder.UpdateInputArgs("unit", "-units", "PixelsPerInch");
                }
                if (_DBWidth.Value != 0) { cmdBuilder.UpdateInputArgs("width", "-resize", _DBWidth.Value.ToString()); }
                cmdBuilder.UpdateInputArgs("compress", "-compress", data.Compress[_CBCompress.SelectedIndex].ToLower());
                cmdBuilder.UpdateInputArgs("type", "-type", "TrueColor");
                cmdBuilder.UpdateInputArgs("colorspace", "-colorspace", "RGB");
                cmdBuilder.UpdateInputArgs("depth", "-depth", "8");
                string extName = data.FormatOut[_CBFormat.SelectedIndex].ToLower();
                CommandExcutor excutor = new CommandExcutor(cmdBuilder);
                int count = 1;
                foreach(var image in data.ImageList)
                {
                    TaskbarManager.Instance.SetProgressValue(count, data.ImageNum);
                    string outputName = System.IO.Path.ChangeExtension(image.Path,extName);
                    cmdBuilder.UpdateInput("input", "", image.Path, true);
                    cmdBuilder.UpdateOutput("output", "", outputName, true);
                    //MessageBox.Show(excutor.CommandCB.Command);
                    Debug.WriteLine(excutor.CommandCB.GetFullCmdString());
                    excutor.Excute();
                    count++;
                }
                data.ImageList = new List<ImagePath>();
                TaskbarManager.Instance.SetProgressValue(0, 1);
                MessageBox.Show(this,"转换完成!");
            }
        }
    }

    public class DataClass : System.ComponentModel.INotifyPropertyChanged
    {

        private List<ImagePath> imageList;
        public List<ImagePath> ImageList
        {
            get { return imageList; }
            set
            {
                imageList = value;
                NotifyPropertyChanged("ImageList");
                NotifyPropertyChanged("ImageNum");
            }
        }
        public int ImageNum { get { return imageList.Count; } }

        public List<string> Compress { get { return new List<string> { @"LZW", @"None", @"RLE", @"Zip", @"Fax", @"Group4", @"JPEG", @"JPEG2000", @"RunlengthEncoded" }; } }
        public List<string> FormatOut { get { return new List<string>() { @"TIFF", @"PNG", @"JPG" }; } }
        public List<string> FormatIn { get { return new List<string>() { @"SVG", @"EMF", @"TIFF", @"TIF", @"PNG", @"JPG" }; } }
        public List<string> FilterList { get { return new List<string>() { @"*.svg", @"*.emf", @"*.tiff", @"*.tif", @"*.png", @"*.jpg" }; } }
        //构造函数
        public DataClass()
        {
            ImageList = new List<ImagePath>();
        }
        //注册属性改变事件，便于通告属性改变
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
        }
    }
    public class ImagePath
    {
        public string Path { get; set; }
        public ImagePath()
        {
            Path = string.Empty;
        }
    }

}
