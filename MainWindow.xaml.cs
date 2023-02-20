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
            this.TextCPU.Text = DispCPUGPU();
            this.TextCPU.FontSize = ((int)this.TextCPU.ActualWidth) / 7;
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


        [GeneratedRegex(".*UHD.*")]
        private static partial Regex UHD_Regex();

        [GeneratedRegex(".*CPU Package.*")]
        private static partial Regex CPUPackage_Regex();

        [GeneratedRegex(".*GPU.*")]
        private static partial Regex GPU_Regex();

        // CPUGPUだけ表示する版
        private string DispCPUGPU()
        {
            string monitorOutput = "";
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            foreach (IHardware hardware in computer.Hardware)
            {
                // UHD Graphicsは表示しない
                if (UHD_Regex().IsMatch(hardware.Name)) break;

                // HW名は省略
                // monitorOutput += "Hardware: " + hardware.Name + "\n";
/*
                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    monitorOutput += "\tSubhardware: " + subhardware.Name + "\n";

                    foreach (ISensor sensor in subhardware.Sensors)
                    {
                        monitorOutput += "\t\tSensor: " + sensor.Name + ", value: " + sensor.Value + "\n";
                    }
                }
*/
                foreach (ISensor sensor in hardware.Sensors)
                {
                    if(CPUPackage_Regex().IsMatch(sensor.Name))
                    {
                        monitorOutput += "CPU: " + sensor.Value + " ﾟC\n";
                        break;
                    }
                    else if(GPU_Regex().IsMatch(sensor.Name))
                    {
                        monitorOutput += "GPU: " + sensor.Value + " ﾟC\n";
                        break;
                    }
                }
            }

            computer.Close();
            return monitorOutput;
        }
            // 全部表示する版
            private string AllMonitor()
        {
            string monitorOutput = "";

            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            foreach (IHardware hardware in computer.Hardware)
            {
                monitorOutput += "Hardware: " + hardware.Name + "\n";

                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    monitorOutput += "\tSubhardware: " + subhardware.Name + "\n";

                    foreach (ISensor sensor in subhardware.Sensors)
                    {
                        monitorOutput += "\t\tSensor: " + sensor.Name + ", value: " + sensor.Value + "\n";
                    }
                }

                foreach (ISensor sensor in hardware.Sensors)
                {
                    monitorOutput += "\tSensor: " + sensor.Name + ", value: " + sensor.Value + "\n";
                }
            }

            computer.Close();
            return monitorOutput;
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
    }
}
