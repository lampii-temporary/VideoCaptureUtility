using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OculusFPV.Launcher
{
    public static class VIWrapper
    {
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

    }
}
