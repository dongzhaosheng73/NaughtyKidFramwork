using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using NaughtyKid.DevicesControl;
using NaughtyKid.MyEnum;

namespace NaughtyKid.WinAPI
{
    public class WinApiHepler
    {
        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        [DllImport("gdi32")]
        public static extern int DeleteObject(IntPtr o);

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinter", SetLastError = true,
         CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurity()]
         internal static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPTStr)] 
         string printerName,
         out IntPtr phPrinter,
         ref PrintDeviceStruct.StructPrinterDefaults pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true,
          CharSet = CharSet.Unicode, ExactSpelling = false,
          CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurity()]
         internal static extern bool ClosePrinter(IntPtr phPrinter);

        [DllImport("winspool.Drv", EntryPoint = "AddFormW", SetLastError = true,
         CharSet = CharSet.Unicode, ExactSpelling = true,
         CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurity()]
         internal static extern bool AddForm(
         IntPtr phPrinter,
         [MarshalAs(UnmanagedType.I4)] int level,
         ref PrintDeviceStruct.FormInfo1 form);

        [DllImport("winspool.Drv", EntryPoint = "DeleteForm", SetLastError = true,
          CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurity()]
         internal static extern bool DeleteForm(
         IntPtr phPrinter,
         [MarshalAs(UnmanagedType.LPTStr)] string pName);

        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = false,
          ExactSpelling = true, CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurity()]
         internal static extern Int32 GetLastError();

        [DllImport("GDI32.dll", EntryPoint = "CreateDC", SetLastError = true,
          CharSet = CharSet.Unicode, ExactSpelling = false,
          CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurity()]
         internal static extern IntPtr CreateDC([MarshalAs(UnmanagedType.LPTStr)] 
         string pDrive,
         [MarshalAs(UnmanagedType.LPTStr)] string pName,
         [MarshalAs(UnmanagedType.LPTStr)] string pOutput,
         ref PrintDeviceStruct.StructDevMode pDevMode);

        [DllImport("GDI32.dll", EntryPoint = "ResetDC", SetLastError = true,
          CharSet = CharSet.Unicode, ExactSpelling = false,
          CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurity()]
         internal static extern IntPtr ResetDC(
         IntPtr hDc,
         ref PrintDeviceStruct.StructDevMode
         pDevMode);

        [DllImport("GDI32.dll", EntryPoint = "DeleteDC", SetLastError = true,
          CharSet = CharSet.Unicode, ExactSpelling = false,
          CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurity()]
         internal static extern bool DeleteDC(IntPtr hDc);

        [DllImport("winspool.Drv", EntryPoint = "SetPrinterA", SetLastError = true,
          CharSet = CharSet.Auto, ExactSpelling = true,
          CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurity()]
         internal static extern bool SetPrinter(
         IntPtr hPrinter,
         [MarshalAs(UnmanagedType.I4)] int level,
         IntPtr pPrinter,
         [MarshalAs(UnmanagedType.I4)] int command);

        [DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesA", SetLastError = true,
          ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
         internal static extern int DocumentProperties(
         IntPtr hwnd,
         IntPtr hPrinter,
         [MarshalAs(UnmanagedType.LPStr)] string pDeviceName,
         IntPtr pDevModeOutput,
         IntPtr pDevModeInput,
         int fMode
         );

        [DllImport("winspool.Drv", EntryPoint = "GetPrinterA", SetLastError = true,
          ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
         internal static extern bool GetPrinter(
         IntPtr hPrinter,
         int dwLevel,
         IntPtr pPrinter,
         int dwBuf,
         out int dwNeeded
         );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
         internal static extern IntPtr SendMessageTimeout(
         IntPtr windowHandle,
         uint msg,
         IntPtr wParam,
         IntPtr lParam,
         SendMessageTimeoutFlags flags,
         uint timeout,
         out IntPtr result
         );

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumPrinters(PrinterEnumFlags flags, string name, uint level,
         IntPtr pPrinterEnum, uint cbBuf,
         ref uint pcbNeeded, ref uint pcReturned);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int size);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool EnumPrintersA(PrinterEnumFlags flags, string name, uint level, IntPtr pPrintEnum, uint cbBuf, ref uint pcbNeeded, ref uint pcReturned);

        [DllImport("winspool.drv", EntryPoint = "EnumForms")]
        internal static extern int EnumFormsA(IntPtr hPrinter, int level, ref byte pForm, int cbBuf, ref int pcbNeeded, ref int pcReturned);

        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(string Name);

        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);
        /// <summary>
        /// 获取短路径
        /// </summary>
        /// <param name="lpszLongPath">指定欲获取短路径名的那个文件的名字。可以是个完整路径，或者由当前目录决定</param>
        /// <param name="lpszShortPath">指定一个缓冲区，用于装载文件的短路径和文件名</param>
        /// <param name="cchBuffer">缓冲区长度</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "GetShortPathName")]
        public static extern int GetShortPathName(
            string lpszLongPath,
            StringBuilder lpszShortPath,
            int cchBuffer
        );

        [DllImport("winmm.dll", EntryPoint = "mciSendString")]
        public static extern int mciSendString(
            string lpstrCommand,
            StringBuilder lpstrReturnString,
            int uReturnLength,
            IntPtr hwndCallback
        );

        [DllImport("winmm.dll", EntryPoint = "mciGetErrorString")]
        public static extern int mciGetErrorString(
            int dwError,
            StringBuilder lpstrBuffer,
            int uLength
        );

        // GDI plus functions
        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GdipGetPropertyItem(IntPtr image, int propid, int size, IntPtr buffer);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GdipGetPropertyItemSize(IntPtr image, int propid, out int size);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GdipLoadImageFromFile(string filename, out IntPtr image);

        [DllImport("gdiplus.dll", EntryPoint = "GdipDisposeImage", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GdipDisposeImage(IntPtr image);

        // EXIT tag value for thumbnail data. Value specified by EXIF standard
        public static int THUMBNAIL_DATA = 0x501B;
    }
}
