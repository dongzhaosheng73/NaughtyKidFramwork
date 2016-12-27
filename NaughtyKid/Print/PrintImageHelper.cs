using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using NaughtyKid.DevicesControl;
using NaughtyKid.Error;
using NaughtyKid.Interface;
using NaughtyKid.MyTools;

namespace NaughtyKid.Print
{
    /// <summary>
    /// 打印帮助类
    /// </summary>
    public class PrintImageHelper:IPrint
    {
       
        /// <summary>
        /// 打印完成事件
        /// </summary>
        public event PrintEndDelegate EventPrintend;

        /// <summary>
        /// 打印机默认
        /// </summary>
        public PrintDocument DocumentPrint { set; get; }

        private PrintStruct.Pdpi _dpi;

        /// <summary>
        /// 打印机DPI
        /// </summary>
        public PrintStruct.Pdpi Dpi
        {
            get
            {
                var returnDpi = new PrintStruct.Pdpi
                {
                    X =
                        DocumentPrint.DefaultPageSettings.PrinterResolution.X < 0
                            ? _dpi.X
                            : DocumentPrint.DefaultPageSettings.PrinterResolution.X,
                    Y=       
                    DocumentPrint.DefaultPageSettings.PrinterResolution.Y < 0 ? 
                    _dpi.Y : DocumentPrint.DefaultPageSettings.PrinterResolution.Y
                };

                return returnDpi;
            }
            set { _dpi = value; }
            
        }
        /// <summary>
        /// 是否置换打印区域长宽
        /// </summary>
        public bool IsTp { set; get; }

        /// <summary>
        /// 是否裁切
        /// </summary>
        public bool ImageCut { set; get; }

        /// <summary>
        /// 打印的位图
        /// </summary>
        private Bitmap PrintBitmap { set; get; }

        public PrintImageHelper(bool printwindowsshow = true)
        {         
            DocumentPrint = new PrintDocument();
            if(!printwindowsshow)DocumentPrint.PrintController = new StandardPrintController();
            DocumentPrint.PrintPage += Pd_PrintPage;  
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="defaultPrinter">默认打印机</param>
        /// <param name="printwindowsshow">是否显示打印窗体</param>
        public PrintImageHelper(string defaultPrinter, bool printwindowsshow = true)
        {
            DocumentPrint = new PrintDocument {PrinterSettings = {PrinterName = defaultPrinter}};
            if (!printwindowsshow) DocumentPrint.PrintController = new StandardPrintController();
            DocumentPrint.PrintPage += Pd_PrintPage;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="defaultPrinter">默认打印机</param>
        /// <param name="landscape">是否横向打印</param>
        /// <param name="margin">打印边距</param>
        /// <param name="printwindowsshow">是否显示打印窗体</param>
        public PrintImageHelper(string defaultPrinter, bool landscape, Margins margin, bool printwindowsshow = true)
        {
       
             DocumentPrint = new PrintDocument
             {
                 PrinterSettings = {PrinterName = defaultPrinter},              
             };
             if (!printwindowsshow) DocumentPrint.PrintController = new StandardPrintController();
             DocumentPrint.PrinterSettings.DefaultPageSettings.Margins = margin;
             DocumentPrint.DefaultPageSettings.Landscape = landscape;
             DocumentPrint.PrintPage += Pd_PrintPage;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="defaultPrinter">默认打印机</param>
        /// <param name="page">纸张大小</param>
        /// <param name="landscape">纸张横竖</param>
        /// <param name="margin">打印边距</param>
        /// <param name="printwindowsshow">是否显示打印窗体</param>
        public PrintImageHelper(string defaultPrinter, string page, bool landscape, Margins margin, bool printwindowsshow = true)
        {
     
            DocumentPrint = new PrintDocument
            {
                PrinterSettings = {PrinterName = defaultPrinter},          
            };
            if (!printwindowsshow) DocumentPrint.PrintController = new StandardPrintController();
            var papers = new List<PaperSize>(PrintDeviceHelper.GetPrintPageType(DocumentPrint));
            DocumentPrint.DefaultPageSettings.PaperSize = papers.Find(x => x.PaperName == page);
            DocumentPrint.DefaultPageSettings.Margins = margin;
            DocumentPrint.DefaultPageSettings.Landscape = landscape;    
            DocumentPrint.PrintPage += Pd_PrintPage;
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="printfile">照片路径</param>
        public void Print(string printfile)
        {
            PrintBitmap = ImageHepler.LoadBitmap(printfile);
            DocumentPrint.Print();
            DocumentPrint.Dispose();
            GC.Collect();
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="bmp">位图</param>
        /// <param name="isCut">是否剪裁</param>
        public void Print(Bitmap bmp, bool isCut)
        {
            if (bmp == null)
                return;
            ImageCut = isCut;

            PrintBitmap = new Bitmap(bmp.Width, bmp.Height);

            var g = Graphics.FromImage(PrintBitmap);

            g.DrawImage(bmp, 0, 0, PrintBitmap.Width, PrintBitmap.Height);

            g.Dispose();

            bmp.Dispose();

            DocumentPrint.Print();
            DocumentPrint.Dispose();
            GC.Collect();
        }

       /// <summary>
       /// 打印
       /// </summary>
       /// <param name="bmp">位图</param>
       /// <param name="isCut">是否剪裁</param>
       /// <param name="tp">是否置换打印区域</param>
        public void Print(Bitmap bmp, bool isCut, bool tp)
        {
            if (bmp == null)
                return;
            ImageCut = isCut;
            IsTp = tp;

            PrintBitmap = new Bitmap(bmp.Width, bmp.Height);

            var g = Graphics.FromImage(PrintBitmap);

            g.DrawImage(bmp, 0, 0, PrintBitmap.Width, PrintBitmap.Height);

            g.Dispose();

            DocumentPrint.Print();
            DocumentPrint.Dispose();
            GC.Collect();
        }
        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="printfile">打印文件路径</param>
        public void PrintView(string printfile)
        {
            PrintBitmap = ImageHepler.LoadBitmap(printfile);
            var pv = new PrintPreviewDialog { Document = DocumentPrint };
            pv.ShowDialog();
            pv.Dispose();
            GC.Collect();
        }
        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="bmp">位图</param>
        /// <param name="isCut">是否剪裁</param>
        public void PrintView(Bitmap bmp, bool isCut)
        {
            if (bmp == null)
                return;
            ImageCut = isCut;

            PrintBitmap = new Bitmap(bmp.Width, bmp.Height);

            var g = Graphics.FromImage(PrintBitmap);

            g.DrawImage(bmp, 0, 0, PrintBitmap.Width, PrintBitmap.Height);

            g.Dispose();

            var pv = new PrintPreviewDialog { Document = DocumentPrint };
            pv.ShowDialog();
            pv.Dispose();
            GC.Collect();
        }
        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="isCut"></param>
        /// <param name="tp"></param>
        public void PrintView(Bitmap bmp, bool isCut, bool tp)
        {
            if (bmp == null)
                return;
            ImageCut = isCut;
            IsTp = tp;

            PrintBitmap = new Bitmap(bmp.Width, bmp.Height);

            var g = Graphics.FromImage(PrintBitmap);

            g.DrawImage(bmp, 0, 0, PrintBitmap.Width, PrintBitmap.Height);

            g.Dispose();

            var pv = new PrintPreviewDialog { Document = DocumentPrint };
            pv.ShowDialog();
            pv.Dispose();
            GC.Collect();
        }
        /// <summary>
        /// 打印事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.High;
                e.PageSettings.PrinterResolution.X = Dpi.X;
                e.PageSettings.PrinterResolution.Y = Dpi.Y;

                Bitmap bitmap;

                if (IsTp)
                {
                    var tbh = (PrintBitmap.Height * (e.PageBounds.Width * Dpi.X / 100)) / PrintBitmap.Width;
                    bitmap = new Bitmap(e.PageBounds.Width * Dpi.X / 100, tbh);
                }
                else
                {
                    bitmap = new Bitmap(e.PageBounds.Width * Dpi.X / 100, e.PageBounds.Size.Height * Dpi.Y / 100);
                }


                if (ImageCut == false)
                {
                    bitmap.SetResolution(Dpi.X, Dpi.Y);
                    var picsize =  ImageHepler.FitSize(PrintBitmap.Width, PrintBitmap.Height, bitmap.Width, bitmap.Height);
                    var tempXy = ImageHepler.PointXy(picsize, bitmap.Width, bitmap.Height);

                    var g = Graphics.FromImage(bitmap);

                    g.DrawImage(PrintBitmap,
                        new Rectangle(tempXy.X, tempXy.Y, (int)picsize.Fitw, (int)picsize.Fith));

                    g.Dispose();
                    e.Graphics.Clear(Color.White);
                    e.HasMorePages = false;
                    e.Graphics.DrawImage(bitmap, 0, 0);
                    if (EventPrintend != null) EventPrintend(bitmap.Clone());
                    PrintBitmap.Dispose();
                    bitmap.Dispose();
                }
                else
                {
                    bitmap.SetResolution(Dpi.X, Dpi.Y);
                    var picsize = ImageHepler.FitSizeOutSide(PrintBitmap.Width, PrintBitmap.Height, bitmap.Width, bitmap.Height);
                    var tempXy = ImageHepler.PointXy(picsize, bitmap.Width, bitmap.Height);

                    var g = Graphics.FromImage(bitmap);
                    g.DrawImage(PrintBitmap,
                        new Rectangle(tempXy.X, tempXy.Y, (int)picsize.Fitw, (int)picsize.Fith));

                    g.Dispose();
                    e.Graphics.Clear(Color.White);
                    e.HasMorePages = false;
                    e.Graphics.DrawImage(bitmap, 0, 0);
                    if (EventPrintend != null)
                    {
                        EventPrintend(bitmap.Clone());
                    }
                    PrintBitmap.Dispose();
                    bitmap.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (EventPrintend != null) EventPrintend(null);
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }
      

        public void Close()
        {
            Dispose();
        }

        protected bool IsDisplsed { get; set; }

        ~PrintImageHelper()
        {
            Dispose();
        }
 
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Dispose(IsDisplsed);
        }
        public virtual void Dispose(bool isDisplsed)
        {
            if (isDisplsed)
                return;

            if (DocumentPrint != null) DocumentPrint.Dispose();

            if (PrintBitmap != null) PrintBitmap.Dispose();

            MemoryHepler.FlushMemory();
        }
        
    }
}
