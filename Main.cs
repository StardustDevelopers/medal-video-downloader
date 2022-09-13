using System;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using AltoHttp;
using System.Diagnostics;

namespace MedalVideoDownloaderFinal
{
    public partial class Main : Form
    {
        private HttpDownloader httpDownloader = null;
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }
        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            string dir = @"C:\MVD";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string dir2 = @"C:\MVD\ffmpeg";
            if (!Directory.Exists(dir2))
            {
                Directory.CreateDirectory(dir2);
            }
            label4.Visible = true;
            httpDownloader = new HttpDownloader("https://download1349.mediafire.com/fkj4h1se3uvg/gmsyid0fqbtbma2/MVD.zip", @"C:\MVD\ffmpeg.zip");
            httpDownloader.DownloadCompleted += HttpDownloader_DownloadCompletedZip;
            httpDownloader.ProgressChanged += HttpDownloader_ProgressChanged;
            httpDownloader.Start();

        }

        private void guna2ControlBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void HttpDownloader_ProgressChanged(object sender, AltoHttp.ProgressChangedEventArgs e)
        {
            progressBar1.Value = (int)e.Progress;
        }
        Timer t1 = new Timer();
        void fadeIn(object sender, EventArgs e)
        {
            if (Opacity >= 1)
                t1.Stop();
            else
                Opacity += 0.05;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //fade in event on load
            Opacity = 0;

            t1.Interval = 10;
            t1.Tick += new EventHandler(fadeIn);
            t1.Start();
            label4.Visible = false;
        }
        private void downloadMedal()
        {
            httpDownloader = new HttpDownloader(textBox1.Text, @"C:\MVD\ffmpeg\bin\clip.m3u8");
            httpDownloader.DownloadCompleted += HttpDownloader_DownloadCompletedClip;
            httpDownloader.ProgressChanged += HttpDownloader_ProgressChanged;
            httpDownloader.Start();
        }
        private void HttpDownloader_DownloadCompletedZip(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                string pathzip = @"C:\MVD\ffmpeg.zip";
                string extractionPath = @"C:\MVD\ffmpeg\";
                ZipFile.ExtractToDirectory(pathzip, extractionPath);
                label4.Text = "Downloading your clip...";
                downloadMedal();
            });
        }
        private async void HttpDownloader_DownloadCompletedClip(object sender, EventArgs e)
        {
            label4.Text = "Converting your clip...";
            Process process = new Process();
            process.StartInfo.FileName = @"C:\MVD\ffmpeg\bin\ffmpeg.exe";
            process.EnableRaisingEvents = false;
            process.StartInfo.WorkingDirectory = @"C:\MVD\ffmpeg\bin\";
            process.StartInfo.Arguments = @"-protocol_whitelist file,http,https,tcp,tls,crypto -i clip.m3u8 -acodec copy -vcodec copy -f mp4 clip.mp4";
            process.Start();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
        }

        private void onenter(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        void fadeOut(object sender, EventArgs e)
        {
            //fade out event on closing
            if (Opacity <= 0)
            {
                t1.Stop();
                Close();
            }
            else
                Opacity -= 0.05;
        }

        private void main_onClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            t1.Tick += new EventHandler(fadeOut);
            t1.Start();

            if (Opacity == 0)
                e.Cancel = false;
        }
    }
}
