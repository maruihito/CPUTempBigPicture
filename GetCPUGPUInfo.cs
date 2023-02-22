using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CPUTempBigPicture
{
    public partial class GetCPUGPUInfo
    {
        public string monitorOutput1 = "";
        public string monitorOutput2 = "";


        [GeneratedRegex(".*UHD.*")]
        private static partial Regex UHD_Regex();

        [GeneratedRegex(".*CPU Package.*")]
        private static partial Regex CPUPackage_Regex();

        [GeneratedRegex(".*CPU Core #.*")]
        private static partial Regex CPUCoreNum_Regex();

        [GeneratedRegex(".*GPU.*")]
        private static partial Regex GPU_Regex();

        // CPUGPUだけ表示する版
        public void DispCPUGPU()
        {
            float cpuTemp = 0;
            bool cpuTempFlg = false;
            float gpuTemp = 0;
            bool gpuTempFlg = false;

            float[] cpuClocks = new float[255];
            int cpuCoreCnt = 0;
            float gpuClock = 0;

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

                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (CPUPackage_Regex().IsMatch(sensor.Name))
                    {
                        if (!cpuTempFlg && (sensor.SensorType == SensorType.Temperature))
                        {
                            cpuTemp = (float)sensor.Value;
                            cpuTempFlg = true;
                        }

                    }
                    else if (CPUCoreNum_Regex().IsMatch(sensor.Name))
                    {
                        if (sensor.SensorType == SensorType.Clock)
                        {
                            cpuClocks[cpuCoreCnt] = (float)sensor.Value;
                            cpuCoreCnt++;
                        }
                    }
                    else if (GPU_Regex().IsMatch(sensor.Name))
                    {
                        if (!gpuTempFlg && (sensor.SensorType == SensorType.Temperature))
                        {
                            gpuTemp = (float)sensor.Value;
                            gpuTempFlg = true;
                        }
                        else if (sensor.SensorType == SensorType.Clock)
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
            for (int i = 0; i < cpuCoreCnt; i++)
            {
                if (cpuMax < cpuClocks[i])
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

        //Disposeメソッドを実装
        public void Dispose()
        {
            monitorOutput1 = null;
            monitorOutput2 = null;
        }
    }
}
