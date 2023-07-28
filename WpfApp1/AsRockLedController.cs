using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace WpfApp1

{
    [StructLayout(LayoutKind.Sequential)]
    public struct ASRLIB_LedColor
    {
        public byte ColorR;
        public byte ColorG;
        public byte ColorB;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ASRLIB_ControllerInfo
    {
        uint ControllerId;
        ASRLIB_ControllerType Type;
        uint FirmwareVersion;
        uint FirmwareDate;
        uint MaxLedChannels;
        public uint ActiveChannel;
        public ASRLIB_ChannelConfig ch0;
        public ASRLIB_ChannelConfig ch1;
        public ASRLIB_ChannelConfig ch2;
        public ASRLIB_ChannelConfig ch3;
        public ASRLIB_ChannelConfig ch4;
        public ASRLIB_ChannelConfig ch5;
        public ASRLIB_ChannelConfig ch6;
        public ASRLIB_ChannelConfig ch7;
    }
    public enum ASRLIB_ControllerType
    {
        RGB_CONTROLLER_MB = 0,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ASRLIB_ChannelConfig
    {
        public int MaxLeds;
        //[MarshalAs(UnmanagedType.U1)]
        public bool RGSwap;
    }

    enum RockMode
    {
        Red,
        Green,
        Blue,
        Rainbow,
    }

    internal class AsRockLedController
    {
        List<AsrockRGBChannel> ChannelController;
        public AsRockLedController()
        {
            uint result = DLL.Polychrome_SDKInit();
            ChannelController = new List<AsrockRGBChannel>();
            if (result == 0)
            {
                ASRLIB_ControllerInfo Info = new ASRLIB_ControllerInfo();
                DLL.Polychrome_GetLedControllerInfo(ref Info);
                List<ASRLIB_ChannelConfig> ChConfig = new List<ASRLIB_ChannelConfig>();
                ChConfig.Add(Info.ch0);
                ChConfig.Add(Info.ch1);
                ChConfig.Add(Info.ch2);
                ChConfig.Add(Info.ch3);
                ChConfig.Add(Info.ch4);
                ChConfig.Add(Info.ch5);
                ChConfig.Add(Info.ch6);
                ChConfig.Add(Info.ch7);
                for (int i = 0; i < 8; i++)
                {
                    bool enable = ((Info.ActiveChannel >> i) & 0x01) == 1;
                    AsrockRGBChannel Ch = new AsrockRGBChannel(i, ChConfig[i], enable);
                    ChannelController.Add(Ch);
                }
                new Thread(Streaming) { IsBackground = true }.Start();
            }
        }


        public string GetMaxLed(int ch)
        {
            if (ChannelController.Count > ch)
            {
                return ChannelController[ch].GetMaxLed().ToString();
            }
            else
            {
                return "";
            }
        }

        void Streaming()
        {
            while (true)
            {
                AsrockRGBChannel.ChangeRainbow();
                foreach (AsrockRGBChannel CHC in ChannelController)
                {
                    CHC.Apply();
                }

                //ChannelController[2].Apply();
                DLL.Polychrome_SetLedColors();
            }
        }

        public void ChnageChMode(int ch, RockMode mode)
        {
            if (ChannelController.Count > ch)
            {
                ChannelController[ch].ChangeMode(mode);
            }
        }

        public void ChangeLight(int ch, int Count)
        {
            if (ChannelController.Count > ch)
            {
                ChannelController[ch].ChangeLight(Count);
            }
        }

        public void Dispose()
        {
            DLL.Polychrome_BackToDefault();
            DLL.Polychrome_SDKRelease();
        }


        #region DLL
        class DLL
        {
            private const string DLL_PATH = @"AsrPolychromeSDK64.dll";

            [DllImport(DLL_PATH)]
            public static extern uint Polychrome_SDKInit();
            [DllImport(DLL_PATH)]
            public static extern uint Polychrome_SDKRelease();

            [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
            public static extern unsafe uint Polychrome_GetLedControllerCount(uint* Count);

            [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
            public static extern unsafe uint Polychrome_GetLedControllerInfo(ASRLIB_ControllerInfo* Info);

            [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
            public static extern unsafe uint Polychrome_SetLedColorConfig();

            [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
            public static extern uint Polychrome_SetLedColors();

            [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
            public static extern uint Polychrome_BackToDefault();

            [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
            public static extern uint Polychrome_SaveUserData();

            [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
            public static extern uint Polychrome_SetLedColorConfig(uint ChannelId, ASRLIB_LedColor[] LedColor, uint LedSize, uint Brightness);

            public static uint Polychrome_GetLedControllerInfo(ref ASRLIB_ControllerInfo Info)
            {
                unsafe
                {
                    fixed (ASRLIB_ControllerInfo* InfoP = &Info)
                    {
                        return Polychrome_GetLedControllerInfo(InfoP);
                    }
                }

            }
        }
        #endregion

        class AsrockRGBChannel
        {
            static List<ASRLIB_LedColor> RainbowColor = new List<ASRLIB_LedColor>()
        {
            new ASRLIB_LedColor(){ ColorR = 0xff, ColorG = 0x00, ColorB = 0x88},
            new ASRLIB_LedColor(){ ColorR = 0xff, ColorG = 0x00, ColorB = 0x00},
            new ASRLIB_LedColor(){ ColorR = 0xff, ColorG = 0x55, ColorB = 0x11},
            new ASRLIB_LedColor(){ ColorR = 0xff, ColorG = 0x88, ColorB = 0x00},
            new ASRLIB_LedColor(){ ColorR = 0xff, ColorG = 0xBB, ColorB = 0x00},
            new ASRLIB_LedColor(){ ColorR = 0xBB, ColorG = 0xff, ColorB = 0x00},
            new ASRLIB_LedColor(){ ColorR = 0x77, ColorG = 0xff, ColorB = 0x00},
            new ASRLIB_LedColor(){ ColorR = 0x00, ColorG = 0xff, ColorB = 0x00},
            new ASRLIB_LedColor(){ ColorR = 0x00, ColorG = 0xff, ColorB = 0x99},
            new ASRLIB_LedColor(){ ColorR = 0x00, ColorG = 0xff, ColorB = 0xcc},

            new ASRLIB_LedColor(){ ColorR = 0x00, ColorG = 0xBB, ColorB = 0xFF},
            new ASRLIB_LedColor(){ ColorR = 0x00, ColorG = 0x66, ColorB = 0xFF},
            new ASRLIB_LedColor(){ ColorR = 0x00, ColorG = 0x00, ColorB = 0xFF},
            new ASRLIB_LedColor(){ ColorR = 0x55, ColorG = 0x00, ColorB = 0xFF},
            new ASRLIB_LedColor(){ ColorR = 0x77, ColorG = 0x00, ColorB = 0xFF},
            new ASRLIB_LedColor(){ ColorR = 0x99, ColorG = 0x00, ColorB = 0xFF},
            new ASRLIB_LedColor(){ ColorR = 0xCC, ColorG = 0x00, ColorB = 0xFF},
            new ASRLIB_LedColor(){ ColorR = 0xFF, ColorG = 0x00, ColorB = 0xFF},
            new ASRLIB_LedColor(){ ColorR = 0xC1, ColorG = 0x00, ColorB = 0x66},
            new ASRLIB_LedColor(){ ColorR = 0xCC, ColorG = 0x00, ColorB = 0x00},
        };
            RockMode Mode;
            int Channel;
            bool Enable;
            int MaxLed;
            int SettingLED;
            public AsrockRGBChannel(int Channel, ASRLIB_ChannelConfig Config, bool Enable)
            {
                this.Channel = Channel;
                this.Enable = Enable;
                MaxLed = Config.MaxLeds;
                SettingLED = MaxLed;
                Mode = RockMode.Red;
            }
            public void Apply()
            {
                if (Enable)
                {
                    List<ASRLIB_LedColor> ResultColor = new List<ASRLIB_LedColor>();
                    switch (Mode)
                    {
                        case RockMode.Rainbow:
                            for (int i = 0; i < SettingLED; i++)
                            {
                                int GetColorCount = i % (RainbowColor.Count - 1);
                                ResultColor.Add(RainbowColor[GetColorCount]);
                            }
                            break;
                        case RockMode.Red:
                            for (int i = 0; i < SettingLED; i++)
                            {
                                ResultColor.Add(new ASRLIB_LedColor() { ColorR = 0xff, ColorG = 0x00, ColorB = 0x00 });
                            }
                            break;
                        case RockMode.Green:
                            for (int i = 0; i < SettingLED; i++)
                            {
                                ResultColor.Add(new ASRLIB_LedColor() { ColorR = 0x00, ColorG = 0xff, ColorB = 0x00 });
                            }
                            break;
                        case RockMode.Blue:
                            for (int i = 0; i < SettingLED; i++)
                            {
                                ResultColor.Add(new ASRLIB_LedColor() { ColorR = 0x00, ColorG = 0x00, ColorB = 0xff });
                            }
                            break;
                    }
                    while (ResultColor.Count != MaxLed)
                    {
                        ResultColor.Add(new ASRLIB_LedColor() { ColorR = 0x00, ColorG = 0x00, ColorB = 0x00 });
                    }
                    DLL.Polychrome_SetLedColorConfig((uint)Channel, ResultColor.ToArray(), (uint)MaxLed, 100);
                }
            }
            public void ChangeMode(RockMode mode)
            {
                this.Mode = mode;
            }

            public void ChangeLight(int SettingLED)
            {
                this.SettingLED = SettingLED;
            }

            public int GetMaxLed()
            {
                return MaxLed;
            }
            public static void ChangeRainbow()
            {
                ASRLIB_LedColor Color = RainbowColor[RainbowColor.Count - 1];
                RainbowColor.RemoveAt(RainbowColor.Count - 1);
                RainbowColor.Insert(0, Color);
            }
        }
    }
}