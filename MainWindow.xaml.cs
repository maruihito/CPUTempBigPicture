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
        string monitorOutput1;
        string monitorOutput2;

        public MainWindow()
        {
            InitializeComponent();
            SetupTimer();
        }

        // タイマメソッド
        private void MyTimerMethod(object sender, EventArgs e)
        {
            // ウィンドウサイズからテキストブロックのサイズを調整
            this.TextCPU1.Height = (int)(this.Grid1.ActualHeight / 3 * 2);
            this.TextCPU2.Height = (int)(this.Grid1.ActualHeight / 3 * 1);

            // 情報を取得
            DispCPUGPU();

            // 取得した情報を表示
            this.TextCPU1.Text = monitorOutput1;
            this.TextCPU1.FontSize = ((int)this.TextCPU1.ActualWidth) / 7;
            this.TextCPU2.Text = monitorOutput2;
            this.TextCPU2.FontSize = ((int)this.TextCPU2.ActualWidth) / 16.5;

            //this.TextCPU1.Text = AllMonitor();
            //this.TextCPU1.FontSize = 12;
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

        [GeneratedRegex(".*CPU Core #.*")]
        private static partial Regex CPUCoreNum_Regex();

        [GeneratedRegex(".*GPU.*")]
        private static partial Regex GPU_Regex();

        // CPUGPUだけ表示する版
        private void DispCPUGPU()
        {
            float cpuTemp = 0;
            bool cpuTempFlg = false;
            float gpuTemp = 0;
            bool gpuTempFlg = false;

            float[] cpuClocks = new float[255];
            int cpuCoreCnt = 0;
            float   gpuClock = 0;

            monitorOutput1 = "";
            monitorOutput2 = "";
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
                    if(CPUPackage_Regex().IsMatch(sensor.Name) )
                    {
                        if(!cpuTempFlg && (sensor.SensorType == SensorType.Temperature))
                        {
                            cpuTemp = (float)sensor.Value;
                            cpuTempFlg = true;
                        }

                    }
                    else if(CPUCoreNum_Regex().IsMatch(sensor.Name))
                    {
                        if(sensor.SensorType == SensorType.Clock)
                        {
                            cpuClocks[cpuCoreCnt] = (float)sensor.Value;
                            cpuCoreCnt++;
                        }
                    }
                    else if(GPU_Regex().IsMatch(sensor.Name))
                    {
                        if (!gpuTempFlg && (sensor.SensorType == SensorType.Temperature))
                        {
                            gpuTemp = (float)sensor.Value;
                            gpuTempFlg = true;
                        }
                        else if(sensor.SensorType == SensorType.Clock)
                        {
                            gpuClock = (float)sensor.Value;
                            break;
                        }
                    }
                }
            }

            // CPU、GPUの温度
            monitorOutput1 += "CPU: " + cpuTemp + " ﾟC\n";
            monitorOutput1 += "GPU: " + gpuTemp + " ﾟC\n";

            // CPUのクロック(最大値)
            float cpuMax = 0.0f;
            for (int i=0; i< cpuCoreCnt; i++)
            {
                if(cpuMax < cpuClocks[i])
                {
                    cpuMax = cpuClocks[i];
                }

            }
            monitorOutput2 += "CPU Clock: " + cpuMax.ToString("F1") + " MHz\n";
            monitorOutput2 += "GPU Clock: " + gpuClock.ToString("F1") + " MHz\n";

            computer.Close();

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
