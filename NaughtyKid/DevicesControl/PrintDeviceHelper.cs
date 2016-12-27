using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using NaughtyKid.Error;
using NaughtyKid.Interface;
using NaughtyKid.MyEnum;
using NaughtyKid.WinAPI;

namespace NaughtyKid.DevicesControl
{
    public class PrintDeviceHelper
    {

        /// <summary>
        /// 获取打印机驱动名称
        /// </summary>
        /// <param name="printname"></param>
        /// <returns></returns>
        public static string GetPrintDirName(string printname)
        {
            uint pcbNeeded = 0;
            uint pcReturne = 0;
            var pAddr = Marshal.AllocHGlobal((int) pcbNeeded);
            var ret = WinApiHepler.EnumPrintersA(PrinterEnumFlags.PrinterEnumLocal, null, 2, pAddr, pcbNeeded,
                ref pcbNeeded, ref pcReturne);
            if (!ret) return "";
            var info2 = new PrintDeviceStruct.PrinterInfo2[pcReturne];
            var offset = pAddr.ToInt32();
            var temp = string.Empty;
            for (var i = 0; i < pcReturne; i++)
            {
                info2[i] =
                    (PrintDeviceStruct.PrinterInfo2)
                        Marshal.PtrToStructure(new IntPtr(offset), typeof(PrintDeviceStruct.PrinterInfo2));
                offset += Marshal.SizeOf(typeof(PrintDeviceStruct.PrinterInfo2));
                if (info2[i].pPrinterName == printname)
                {
                    temp = info2[i].pDriverName;
                }

            }
            return temp;
        }

        /// <summary>   
        /// 获取本机的打印机列表。   
        /// </summary>   
        public static List<string> GetLocalPrinters()
        {
            var fPrinters = new List<string>();

            foreach (
                var fPrinterName in
                    PrinterSettings.InstalledPrinters.Cast<string>()
                        .Where(fPrinterName => !fPrinters.Contains(fPrinterName)))
            {
                fPrinters.Add(fPrinterName);
            }
            return fPrinters;
        }

        /// <summary>
        /// 返回所有可用纸张类型
        /// </summary>
        /// <returns>PaperSizes</returns>
        public static PaperSize[] GetPrintPageType(PrintDocument pd)
        {
            var pageType = pd.DefaultPageSettings.PrinterSettings.PaperSizes;
            var type = new PaperSize[pageType.Count];

            for (var i = 0; i < pageType.Count; i++)
            {
                type[i] = pageType[i];
            }

            return type;
        }

        /// <summary>
        /// 返回所有可用纸张类型
        /// </summary>
        /// <returns>PaperSizes</returns>
        public static PaperSize[] GetPrintPageType(string pdName)
        {
            var pd = new PrintDocument {PrinterSettings = {PrinterName = pdName}};
            var pageType = pd.DefaultPageSettings.PrinterSettings.PaperSizes;
            var type = new PaperSize[pageType.Count];

            for (var i = 0; i < pageType.Count; i++)
            {
                type[i] = pageType[i];
            }

            return type;
        }

        /// <summary>
        /// 返回可用纸张名称
        /// </summary>
        /// <param name="pdName"></param>
        /// <returns></returns>
        public static List<string> GetPrintPaperName(string pdName)
        {
            var pd = new PrintDocument {PrinterSettings = {PrinterName = pdName}};
            var pageType = pd.DefaultPageSettings.PrinterSettings.PaperSizes;
            var paperList = new List<string>();

            for (var i = 0; i < pageType.Count; i++)
            {
                paperList.Add(pageType[i].PaperName);
            }

            return paperList;
        }

        /// <summary>
        /// 获取打印机是否脱机
        /// </summary>
        /// <param name="printerDevice">打印机名称</param>
        /// <returns>状态</returns>
        public static bool GetPrinterState(string printerDevice)
        {
            try
            {
                var path = @"win32_printer.DeviceId='" + printerDevice + "'";
                var printer = new ManagementObject(path);
                printer.Get();
                return !printer["WorkOffline"].ToString().ToLower().Equals("true");
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }

        }

        /// <summary>
        /// 获取默认打印机纸张
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultPage()
        {
            var fPrintDocument = new PrintDocument();
            return fPrintDocument.PrinterSettings.DefaultPageSettings.PaperSize.PaperName;
        }

        /// <summary>
        /// 获取默认打印机
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultPrint()
        {
            var fPrintDocument = new PrintDocument();
            return fPrintDocument.PrinterSettings.PrinterName;
        }

        /// <summary>
        /// 设置默认打印机
        /// </summary>
        /// <param name="name">打印机名称</param>
        /// <returns></returns>
        public static bool SettingDefaultPrinter(string name)
        {
            return WinApiHepler.SetDefaultPrinter(name);
        }

        /// <summary>
        /// 获取指定纸张尺寸的像素大小
        /// </summary>
        /// <param name="printName"></param>
        /// <param name="papaerName"></param>
        public static int[] GetPaperSize(string printName, string paperName, int defaultdpi = 300)
        {

            var pd = new PrintDocument { PrinterSettings = { PrinterName = printName } };
            var pageType = pd.DefaultPageSettings.PrinterSettings.PaperSizes;

            var type = new PaperSize[pageType.Count];

            pageType.CopyTo(type, 0);

            var paper = type.FirstOrDefault(x => x.PaperName == paperName);

            if (paper == null) return null;
            var w = pd.DefaultPageSettings.PrinterResolution.X < 0
                ? defaultdpi * paper.Width / 100
                : pd.DefaultPageSettings.PrinterResolution.X * paper.Width /100;
            var h = pd.DefaultPageSettings.PrinterResolution.Y < 0
                ? defaultdpi * paper.Height / 100
                : pd.DefaultPageSettings.PrinterResolution.Y * paper.Height / 100;
            return new[] { w, h };
        }


    }
}


