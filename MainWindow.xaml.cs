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
            SetupTimer();
        }

        // タイマメソッド
        private void MyTimerMethod(object sender, EventArgs e)
        {
            UpdateScreen();
        }

        // タイマのインスタンス
        private DispatcherTimer _timer;

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
            // クラスインスタンス化
            GetCPUGPUInfo GetCGI = new GetCPUGPUInfo();

            // ウィンドウサイズからテキストブロックのサイズを調整
            this.TextCPU1.Height = (int)(this.Grid1.ActualHeight / 3 * 2);
            this.TextCPU2.Height = (int)(this.Grid1.ActualHeight / 3 * 1);

            // 情報を取得
            GetCGI.DispCPUGPU();

            // 取得した情報を表示
            this.TextCPU1.Text = GetCGI.monitorOutput1;
            this.TextCPU1.FontSize = ((int)this.TextCPU1.ActualWidth) / 7;
            this.TextCPU2.Text = GetCGI.monitorOutput2;
            this.TextCPU2.FontSize = ((int)this.TextCPU2.ActualWidth) / 16.5;

            //this.TextCPU1.Text = AllMonitor();
            //this.TextCPU1.FontSize = 12;

            GetCGI.Dispose();
        }
    }
}
