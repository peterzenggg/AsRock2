using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WpfApp1

{
    internal enum ESCORE_FAN_ID
    {
        ESCORE_FANID_CPU_FAN1,
        ESCORE_FANID_CPU_FAN2,
        ESCORE_FANID_CHASSIS_FAN1,
        ESCORE_FANID_CHASSIS_FAN2,
        ESCORE_FANID_CHASSIS_FAN3,
        ESCORE_FANID_CHASSIS_FAN4,
    }
    class ASRockFansModel
    {
        public double CpuTemp;
        public double MBTemp;
        public double CpuFan1Speed;
        public double CpuFan2Speed;
        public double ChassisFan1Speed;
        public double ChassisFan2Speed;
        public double ChassisFan3Speed;
        public double ChassisFan4Speed;
    }


    internal enum ESCORE_HWM_ITEM
    {
        ESCORE_HWM_CPU_TEMP,
        ESCORE_HWM_MB_TEMP,
        ESCORE_HWM_CPU_FAN1_SPEED,
        ESCORE_HWM_CPU_FAN2_SPEED,
        ESCORE_HWM_CHASSIS_FAN1_SPEED,
        ESCORE_HWM_CHASSIS_FAN2_SPEED,
        ESCORE_HWM_CHASSIS_FAN3_SPEED,
        ESCORE_HWM_CHASSIS_FAN4_SPEED,
    }

    internal enum ESCORE_FAN_CONTROL_TYPE
    {
        ESCORE_FANCTL_MANUAL,
        ESCORE_FANCTL_SMART_FAN_1,
        ESCORE_FANCTL_RESERVED1,
        ESCORE_FANCTL_RESERVED2,
        ESCORE_FANCTL_SMART_FAN_4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SSCORE_FAN_CONFIG
    {
        public ESCORE_FAN_CONTROL_TYPE ControlType;
        public int TargetFanSpeed;
        int TargetTemperature;
        int SMART_FAN4_Temp1;
        int SMART_FAN4_Speed1;
        int SMART_FAN4_Temp2;
        int SMART_FAN4_Speed2;
        int SMART_FAN4_Temp3;
        int SMART_FAN4_Speed3;
        int SMART_FAN4_Temp4;
        int SMART_FAN4_Speed4;
        int SMART_FAN4_Critical_Temp;
        int SMART_FAN4_Temp_Source;
        bool SMART_FAN4_FanStop_Enabled;
    }


    internal class ASRockFanController
    {
        ASRockFansModel model;
        SSCORE_FAN_CONFIG CPUFAN1Config;
        SSCORE_FAN_CONFIG CPUFAN2Config;
        SSCORE_FAN_CONFIG CH1Config;
        SSCORE_FAN_CONFIG CH2Config;
        SSCORE_FAN_CONFIG CH3Config;
        SSCORE_FAN_CONFIG CH4Config;
        #region BoardState
        public double CpuTemp
        {
            get
            {
                return model.CpuTemp;
            }
        }
        public double MBTemp
        {
            get
            {
                return model.MBTemp;
            }
        }
        public double CpuFan1Speed
        {
            get
            {
                return model.CpuFan1Speed;
            }
        }
        public double CpuFan2Speed
        {
            get
            {
                return model.CpuFan2Speed;
            }
        }

        public double ChassisFan1Speed
        {
            get
            {
                return model.ChassisFan1Speed;
            }
        }

        public double ChassisFan2Speed
        {
            get
            {
                return model.ChassisFan2Speed;
            }
        }

        public double ChassisFan3Speed
        {
            get
            {
                return model.ChassisFan3Speed;
            }
        }
        public double ChassisFan4Speed
        {
            get
            {
                return model.ChassisFan4Speed;
            }
        }
        #endregion
        #region FanState
        public double CpuFan1Target
        {
            get
            {
                return Math.Round(CPUFAN1Config.TargetFanSpeed / 255.0 * 100);
            }
        }

        public double CpuFan2Target
        {
            get
            {
                return Math.Round(CPUFAN2Config.TargetFanSpeed / 255.0 * 100);
            }
        }
        public double CH1FanTarget
        {
            get
            {
                return Math.Round(CH1Config.TargetFanSpeed / 255.0 * 100);
            }
        }
        public double CH2FanTarget
        {
            get
            {
                return Math.Round(CH2Config.TargetFanSpeed / 255.0 * 100);
            }
        }
        public double CH3FanTarget
        {
            get
            {
                return Math.Round(CH3Config.TargetFanSpeed / 255.0 * 100);
            }
        }
        public double CH4FanTarget
        {
            get
            {
                return Math.Round(CH4Config.TargetFanSpeed / 255.0 * 100);
            }
        }
        #endregion
        public ASRockFanController()
        {
            model = new ASRockFansModel();
            DLL.AsrLibDllInit();
            CPUFAN1Config = new SSCORE_FAN_CONFIG();
            CPUFAN2Config = new SSCORE_FAN_CONFIG();
            CH1Config = new SSCORE_FAN_CONFIG();
            CH2Config = new SSCORE_FAN_CONFIG();
            CH3Config = new SSCORE_FAN_CONFIG();
            CH4Config = new SSCORE_FAN_CONFIG();
            DLL.GetAsrFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CPU_FAN1, ref CPUFAN1Config);
            DLL.GetAsrFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CPU_FAN2, ref CPUFAN2Config);
            DLL.GetAsrFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN1, ref CH1Config);
            DLL.GetAsrFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN2, ref CH2Config);
            DLL.GetAsrFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN3, ref CH3Config);
            DLL.GetAsrFanConfig(ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN4, ref CH4Config);
            new Thread(TempGood) { IsBackground = true }.Start();
        }

        private void TempGood()
        {
            unsafe
            {
                while (true)
                {
                    DLL.AsrLibGetHardwareMonitor(ESCORE_HWM_ITEM.ESCORE_HWM_CPU_TEMP, ref model.CpuTemp);
                    DLL.AsrLibGetHardwareMonitor(ESCORE_HWM_ITEM.ESCORE_HWM_MB_TEMP, ref model.MBTemp);
                    DLL.AsrLibGetHardwareMonitor(ESCORE_HWM_ITEM.ESCORE_HWM_CPU_FAN1_SPEED, ref model.CpuFan1Speed);
                    DLL.AsrLibGetHardwareMonitor(ESCORE_HWM_ITEM.ESCORE_HWM_CPU_FAN2_SPEED, ref model.CpuFan2Speed);
                    DLL.AsrLibGetHardwareMonitor(ESCORE_HWM_ITEM.ESCORE_HWM_CHASSIS_FAN1_SPEED, ref model.ChassisFan1Speed);
                    DLL.AsrLibGetHardwareMonitor(ESCORE_HWM_ITEM.ESCORE_HWM_CHASSIS_FAN2_SPEED, ref model.ChassisFan2Speed);
                    DLL.AsrLibGetHardwareMonitor(ESCORE_HWM_ITEM.ESCORE_HWM_CHASSIS_FAN3_SPEED, ref model.ChassisFan3Speed);
                    DLL.AsrLibGetHardwareMonitor(ESCORE_HWM_ITEM.ESCORE_HWM_CHASSIS_FAN4_SPEED, ref model.ChassisFan4Speed);
                    Thread.Sleep(100);
                }
            }
        }

        public void Dispose()
        {
            DLL.AsrLibDllUnInit();
        }

        public void SetFanConfig(ESCORE_FAN_ID CH, int TargetSpeed)
        {
            int Target = 50;
            if (TargetSpeed > 0 && TargetSpeed <= 100)
            {
                Target = Convert.ToInt16((TargetSpeed / 100.0) * 255.0);
            }
            SSCORE_FAN_CONFIG config;
            switch (CH)
            {
                case ESCORE_FAN_ID.ESCORE_FANID_CPU_FAN1:
                    config = CPUFAN1Config;
                    break;
                case ESCORE_FAN_ID.ESCORE_FANID_CPU_FAN2:
                    config = CPUFAN2Config;
                    break;
                case ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN1:
                    config = CH1Config;
                    break;
                case ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN2:
                    config = CH2Config;
                    break;
                case ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN3:
                    config = CH3Config;
                    break;
                case ESCORE_FAN_ID.ESCORE_FANID_CHASSIS_FAN4:
                    config = CH4Config;
                    break;
                default:
                    config = new SSCORE_FAN_CONFIG();
                    break;
            }
            config.ControlType = ESCORE_FAN_CONTROL_TYPE.ESCORE_FANCTL_MANUAL;
            config.TargetFanSpeed = Target;
            DLL.SetASRockFanConfig(CH, config);
        }


        #region DLLIMPORT
        class DLL
        {
            private const string DLL_PATH = @"AsrCore.dll";

            [DllImport(DLL_PATH)]
            public static extern bool AsrLibDllInit();

            [DllImport(DLL_PATH)]
            public static extern bool AsrLibDllUnInit();

            [DllImport(DLL_PATH)]
            public static extern int AsrLibDllGetLastError();

            [DllImport(DLL_PATH)]
            public static extern unsafe bool AsrLibGetHardwareMonitor(ESCORE_HWM_ITEM Item, double* temp);

            [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
            public static extern unsafe bool AsrLibGetFanConfig(ESCORE_FAN_ID FanId, SSCORE_FAN_CONFIG* Config);

            [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
            public static extern unsafe bool AsrLibSetFanConfig(ESCORE_FAN_ID FanId, SSCORE_FAN_CONFIG* Config);


            public static bool AsrLibGetHardwareMonitor(ESCORE_HWM_ITEM Item, ref double temp)
            {
                unsafe
                {
                    fixed (double* TempP = &temp)
                    {
                        return AsrLibGetHardwareMonitor(Item, TempP);
                    }
                }
            }


            public static bool GetAsrFanConfig(ESCORE_FAN_ID FanID, ref SSCORE_FAN_CONFIG Config)
            {
                unsafe
                {
                    fixed (SSCORE_FAN_CONFIG* ConfigP = &Config)
                        return AsrLibGetFanConfig(FanID, ConfigP);
                }
            }

            public static bool SetASRockFanConfig(ESCORE_FAN_ID FanID, SSCORE_FAN_CONFIG Config)
            {
                unsafe
                {
                    SSCORE_FAN_CONFIG* ConfigP = &Config;
                    return AsrLibSetFanConfig(FanID, ConfigP);
                }
            }
        }
        #endregion
    }
}
