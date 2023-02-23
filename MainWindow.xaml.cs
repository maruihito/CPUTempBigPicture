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
using System.Timers;
using System.ComponentModel;
using System.Windows.Threading;
using LibreHardwareMonitor.Hardware;
using System.Text.RegularExpressions;
using LibreHardwareMonitor.Hardware.CPU;
using System.IO;

namespace CPUTempBigPicture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.SetupTimer();

            // 動画読み込み
            this.SetupMedia();
        }

        // タイマメソッド
        private void MyTimerMethod(object sender, EventArgs e)
        {
            // 画面情報の更新
            UpdateScreen();
            if ( this.timerCnt != 7 )
            {
                this.timerCnt++;
            }
            else
            {
                this.timerCnt = 0;
            }

        }

        // タイマのインスタンス
        private DispatcherTimer _timer;

        // タイマイベントの回数カウント
        private uint timerCnt = 0;

        // カレントパス
        string startPath = AppDomain.CurrentDomain.BaseDirectory;


        // タイマを設定する
        private void SetupTimer()
        {
            // タイマのインスタンスを生成
            _timer = new DispatcherTimer(); // 優先度はDispatcherPriority.Background
                                            // インターバルを設定
            _timer.Interval = new TimeSpan(0, 0, 3);
            // タイマメソッドを設定
            _timer.Tick += new EventHandler(MyTimerMethod);
            // タイマを開始
            _timer.Start();

            // 画面が閉じられるときに、タイマを停止
            this.Closing += new CancelEventHandler(StopTimer);
        }

        // タイマを停止
        private void StopTimer(object sender, CancelEventArgs e)
        {
            _timer.Stop();
        }

        // メディアの設定
        private void SetupMedia()
        {
            try
            {
                // カレントフォルダの動画を探す
                string[] names = Directory.GetFiles(@".", "*.mp4");
                foreach (string name in names)
                {
                    // LoadBehaviorをマニュアルに設定(こうしないと連続再生できない)
                    this.MediaElement.LoadedBehavior = MediaState.Manual;

                    // 見つけた動画をUriに変換
                    Uri uri = new Uri(startPath + name);
                    this.MediaElement.Source = uri;

                    // 再生速度(負荷軽減のため1/4に)
                    this.MediaElement.SpeedRatio = 0.25;

                    // 再生開始
                    this.MediaElement.Play();

                    this.Closing += new CancelEventHandler(StopMedia);
                    break;
                }
            }
            catch (Exception e)
            {
                // 動画取得エラー、特に何もしない
            }
        }

        // 連続再生
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.RePlayMedia();
        }

        private void RePlayMedia()
        {
            this.MediaElement.Position = TimeSpan.FromMilliseconds(1);
            this.MediaElement.Play();
        }

        private void StopMedia(object sender, CancelEventArgs e)
        {
            this.MediaElement.Stop();
            this.MediaElement.Close();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.WindowStyle != WindowStyle.None)
            {
                // タイトルバーと境界線を表示しない
                this.WindowStyle = WindowStyle.None;

                // 最大化表示
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                // タイトルバーと境界線を表示
                this.WindowStyle = WindowStyle.SingleBorderWindow;

                // 最大化解除
                this.WindowState = WindowState.Normal;
            }


        }

        /// 画面情報の更新
        private void UpdateScreen()
        {
            // 表示文字列
            string monitorOutput1 = "";
            string monitorOutput2 = "";
            string monitorOutput3 = "";

            // クラスインスタンス化
            GetCPUGPUInfo GetCGI = new GetCPUGPUInfo();
            // 情報を取得
            Task t1 = Task.Run(GetCGI.DispCPUGPU);

            // ウィンドウサイズからテキストブロックの高さを調整
            this.TextCPU1.Height = (int)(this.Grid1.ActualHeight / 3 * 2);
            this.TextCPU2.Height = (int)(this.Grid1.ActualHeight / 3 * 1);

            // フォントサイズの調整
            this.TextCPU1.FontSize = ((int)this.TextCPU1.ActualWidth) / 7;
            this.TextCPU2.FontSize = ((int)this.TextCPU2.ActualWidth) / 16.5;

            // 情報取得が完了するのを待つ
            t1.Wait();

            // CPU、GPUの温度
            monitorOutput1 += "CPU: " + GetCGI.cpuTemp.ToString() + " ﾟC\n";
            monitorOutput1 += "GPU: " + GetCGI.gpuTemp.ToString() + " ﾟC\n";

            // クロック
            monitorOutput2 += "CPU Clock: " + GetCGI.cpuMax.ToString("F1") + " MHz\n";
            monitorOutput2 += "GPU Clock: " + GetCGI.gpuClock.ToString("F1") + " MHz\n";

            // 消費電力
            monitorOutput3 += "CPU Power: " + GetCGI.cpuPow.ToString("F3") + " W\n";
            monitorOutput3 += "GPU Power: " + GetCGI.gpuPow.ToString("F3") + " W\n";

            // 取得した情報を表示
            this.TextCPU1.Text = monitorOutput1;

            if (GetCGI.cpuTemp >= 60 || GetCGI.gpuTemp >= 60)
            {
                this.TextCPU1.Foreground = Brushes.OrangeRed;
            }
            else
            {
                this.TextCPU1.Foreground = Brushes.SpringGreen;
            }

            if (timerCnt < 4)
            {
                // 周波数を表示
                this.TextCPU2.Text = monitorOutput2;
                this.TextCPU2.Foreground = Brushes.SteelBlue;
            }
            else
            {
                // 電力を表示
                this.TextCPU2.Text = monitorOutput3;
                this.TextCPU2.Foreground = Brushes.Goldenrod;
            }

            //this.TextCPU1.Text = AllMonitor();
            //this.TextCPU1.FontSize = 12;

            GetCGI.Dispose();
        }

    }
}
