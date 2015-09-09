using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OculusFPV.Launcher
{
    public static class VIWrapper
    {

        public enum MSubType : int
        {
         //   AUTO = 99,
            RGB24 = 0,
            RGB32 = 1,
            RGB555 = 2,
            RGB565 = 3,
            YUY2 = 4,
            YVYU = 5,
            YUYV = 6,
            IYUV = 7,
            UYVY = 8,
            YV12 = 9,
            YVU9 = 10,
            Y411 = 11,
            Y41P = 12,
            Y211 = 13,
            AYUV = 14,
            Y800 = 15,
            Y8 = 16,
            GREY = 17,
            MJPG = 18
        };
        public enum PhysicalType : int
        {
          //  AUTO = 99,
            COMPOSITE = 0,
            S_VIDEO = 1,
            TV_TUNER = 2,
            USB = 3,
            FW1394 = 4
        }
        public enum FormatType : int
        {
         //   AUTO = 99,
            NTSC_M = 0,
            PAL_B = 1,
            PAL_D = 2,
            PAL_G = 3,
            PAL_H = 4,
            PAL_I = 5,
            PAL_M = 6,
            PAL_N = 7,
            PAL_NC = 8,
            SECAM_B = 9,
            SECAM_D = 10,
            SECAM_G = 11,
            SECAM_H = 12,
            SECAM_K = 13,
            SECAM_K1 = 14,
            SECAM_L = 15,
            NTSC_M_J = 16,
            NTSC_433 = 17,
        };
        // [DllImport(@"videoInputDLL.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DoMath()")]
        [DllImport(@"videoInputDLL.dll")]
        public static extern int ListDevices();

        [DllImport(@"videoInputDLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]

        public static extern string GetDeviceName(int devId);
        [DllImport(@"videoInputDLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]

        public static extern void SetupDevice(int devId);
        [DllImport(@"videoInputDLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]

        public static extern bool IsFrameReady(int devId);
        [DllImport(@"videoInputDLL.dll", EntryPoint = "GetImage", CallingConvention = CallingConvention.Cdecl)]

        public static extern void GetImage(int devId, ref IntPtr data, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)] bool color, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)] bool flip);
        [DllImport(@"videoInputDLL.dll", EntryPoint = "GetBufferSize", CallingConvention = CallingConvention.Cdecl)]

        public static extern int GetBufferSize(int devId);
        [DllImport(@"videoInputDLL.dll", EntryPoint = "GetHeight", CallingConvention = CallingConvention.Cdecl)]

        public static extern int GetHeight(int devId);

        [DllImport(@"videoInputDLL.dll", EntryPoint = "GetWidth", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetWidth(int devId);
        [DllImport(@"videoInputDLL.dll", EntryPoint = "SetFormat", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetFormat(int devId, FormatType format);
        [DllImport(@"videoInputDLL.dll", EntryPoint = "ShowSettingsWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ShowSettingsWindow(int devId);

        [DllImport(@"videoInputDLL.dll", EntryPoint = "StopDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopDevice(int devId);

        [DllImport(@"videoInputDLL.dll", EntryPoint = "SetRequestedMediaSubType", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetRequestedMediaSubType(MSubType mediaType);


        [DllImport(@"videoInputDLL.dll", EntryPoint = "SetIdealFramerate", CallingConvention = CallingConvention.Cdecl)]

        public static extern void SetIdealFramerate(int devId, int fps);
        [DllImport(@"videoInputDLL.dll", EntryPoint = "SetUseCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetUseCallback(bool useCallback);

         [DllImport(@"videoInputDLL.dll", EntryPoint = "SetupDevice1", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetupDevice1(int devId, PhysicalType connType);
        
    }
}
