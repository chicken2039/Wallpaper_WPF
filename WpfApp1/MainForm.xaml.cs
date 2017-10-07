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
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WpfApp1
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainForm : Window
    {

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        #region Variables
        private GIFForm m_GifForm = null;
        private VideoForm m_VideoForm = null;
        private YoutubeForm m_YoutubeForm = null;
        private MorphForm m_MorphForm = null;
        private int m_Volume = 0;
        private string VideoPath = "";
        private string GIFPath = "";
        private string ImagePath = "";
        public int Volume { get { return m_Volume; } set { waveOutSetVolume(IntPtr.Zero, (uint)value); } }
        private const string VideoExt = "Video|*.mp4;*.avi;*.wmv;*.mpeg";
        private const string ImageExt = "Image|*.jpg;*.jpeg;*.bmp;*.png";
        #endregion




        public MainForm()
        {
            InitializeComponent();
            SettingManager.Initialize();
            UI_init();
        }

        public void UI_Init()
        {
            txtID.Text = Setting.YoutubeID;
            lblVideoPath.Content = Setting.VideoPath;
            lblGIFPath.Content = Setting.GIFPath;
            pictureBox1.ImageLocation = Setting.MorphID;
            txtFirst.Text = Setting.First_Ment;
            txtSecond.Text = Setting.Second_Ment;
            if (Setting.IsStartUp)
                checkBox1.IsChecked = true;
            trkVolume.Value = Setting.Volume;
        }

        private void StopWallpaper()
        {
            if (m_GifForm != null)
                m_GifForm.Close();
            if (m_VideoForm != null)
                m_VideoForm.Close();
            if (m_YoutubeForm != null)
                m_YoutubeForm.Close();
            if (m_MorphForm != null)
                m_MorphForm.Close();
        }

        void btnYoutube_Click(object sender, RoutedEventArgs e) 
        {
            StopWallpaper();
            StringBuilder URi = new StringBuilder("https://youtube.com/embed/");
            URi.Append(txtID.Text);
            URi.Append("?autoplay=1&loop=1&controls=0&showinfo=0&vq=hd1080&playlist=");
            URi.Append(txtID.Text);
            Setting.YoutubeID = txtID.Text;
            m_YoutubeForm = new YoutubeForm()
            {
                Videopath = URi.ToString()
            };
            m_YoutubeForm.Show();
        }
        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            StopWallpaper();
            if (VideoPath != "")
            {
                Setting.VideoPath = VideoPath;
                m_VideoForm = new VideoForm()
                {
                    Videopath = VideoPath
                };
                m_VideoForm.Show();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBgwr13tNn2BL5TbKLvT6e_kCR8egeQ5og", // 키 지정
                ApplicationName = "wallpaperengine"
            });
            listView1.Items.Clear();
            // Search용 Request 생성
            var request = youtube.Search.List("snippet");
            request.Q = txtSearch.Text;  //ex: "양희은"
            request.MaxResults = 25;

            // Search용 Request 실행
            var result = await request.ExecuteAsync();

            // Search 결과를 리스트뷰에 담기
            foreach (var item in result.Items)
            {
                if (item.Id.Kind == "youtube#video")
                {
                    listView1.Items.Add(item.Id.VideoId.ToString(), item.Snippet.Title, 0);
                }
            }

        }

        private void btnOpenVideo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpenGIF_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Filter = "*.gif|*.gif";
            openFileDialog1.Title = "GIF Open";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                GIFPath = openFileDialog1.FileName;
                lblGIFPath.Content = GIFPath;
            }

        }

        private void btnGIF_Click(object sender, RoutedEventArgs e)
        {
            StopWallpaper();
            if (GIFPath != "")
            {
                Setting.GIFPath = GIFPath;
                m_GifForm = new GIFForm()
                {
                    Gifpath = GIFPath
                };
                m_GifForm.Show();
            }

        }

        private void btnOpenText_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Title = "Open Image";
            openFileDialog1.Filter = ImageExt;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Setting.MorphID = openFileDialog1.FileName;
                pictureBox1.ImageLocation = Setting.MorphID;
            }
        }

        private void btnText_Click(object sender, RoutedEventArgs e)
        {
            StopWallpaper();
            if (Setting.MorphID != "" && txtFirst.Text != "" && txtSecond.Text != "")
            {
                Setting.First_Ment = txtFirst.Text;
                Setting.Second_Ment = txtSecond.Text;
                m_MorphForm = new MorphForm()
                {
                    Text_First = txtFirst.Text,
                    Text_Second = txtSecond.Text,
                    ImagePath = Setting.MorphID
                };
                m_MorphForm.Show();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopWallpaper();
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void button9_Click(object sender, RoutedEventArgs e)// Update Check Button
        {

        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double NewVolume = ((ushort.MaxValue / 10) * trkVolume.Value);
            // Set the same volume for both the left and the right channels
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            // Set the volume
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
            //Save The Setting
            Setting.Volume = (int)trkVolume.Value;
        }
    }
}
