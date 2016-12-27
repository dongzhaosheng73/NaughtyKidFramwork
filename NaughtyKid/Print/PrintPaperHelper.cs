using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using NaughtyKid.MyTools;
using System.Drawing.Printing;
using System.Windows.Forms;
using NaughtyKid.DevicesControl;
using NaughtyKid.Interface;

namespace NaughtyKid.Print
{
    /// <summary>
    /// 票据打印帮助类
    /// </summary>
    public  class PrintPaperHelper:IPrint
    {
        private IPrint _printImplementation;

        public List<IPrintPaperData> PrintPaperData { get; set; }

        /// <summary>
        /// 打印机默认
        /// </summary>
        public PrintDocument DocumentPrint { set; get; }

        /// <summary>
        /// 默认起始位置
        /// </summary>
        public Point DocumentPoint { set; get; }

        private PrintStruct.Pdpi _dpi;

        /// <summary>
        /// 打印机DPI
        /// </summary>
        public PrintStruct.Pdpi Dpi
        {
            get
            {
                _dpi.X = DocumentPrint.DefaultPageSettings.PrinterResolution.X;
                _dpi.Y = DocumentPrint.DefaultPageSettings.PrinterResolution.Y;
                return _dpi;
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="printwindowsshow"></param>
        public PrintPaperHelper(bool printwindowsshow = true)
        {
            DocumentPrint = new PrintDocument();
            if (!printwindowsshow) DocumentPrint.PrintController = new StandardPrintController();
            DocumentPrint.PrintPage += Pd_PrintPage;
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="defaultPrinter">默认打印机</param>
        /// <param name="docPoint">默认起始位置</param>
        /// <param name="printwindowsshow">是否显示打印窗体</param>
        public PrintPaperHelper(string defaultPrinter,Point docPoint, bool printwindowsshow = true)
        {
            DocumentPrint = new PrintDocument {PrinterSettings = {PrinterName = defaultPrinter}};
            if (!printwindowsshow) DocumentPrint.PrintController = new StandardPrintController();
            DocumentPoint = docPoint;
            DocumentPrint.PrintPage += Pd_PrintPage;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="defaultPrinter">默认打印机</param>
        /// <param name="docPoint">默认起始位置</param>
        /// <param name="landscape">是否横向打印</param>
        /// <param name="margin">打印边距</param>
        /// <param name="printwindowsshow">是否显示打印窗体</param>
        public PrintPaperHelper(string defaultPrinter, Point docPoint, bool landscape, Margins margin, bool printwindowsshow = true)
        {
       
             DocumentPrint = new PrintDocument
             {
                 PrinterSettings = {PrinterName = defaultPrinter},              
             };
             if (!printwindowsshow) DocumentPrint.PrintController = new StandardPrintController();
             DocumentPoint = docPoint;
             DocumentPrint.PrinterSettings.DefaultPageSettings.Margins = margin;
             DocumentPrint.DefaultPageSettings.Landscape = landscape;
             DocumentPrint.PrintPage += Pd_PrintPage;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="defaultPrinter">默认打印机</param>
        /// <param name="docPoint">默认起始位置</param>
        /// <param name="page">纸张大小</param>
        /// <param name="landscape">纸张横竖</param>
        /// <param name="margin">打印边距</param>
        /// <param name="printwindowsshow">是否显示打印窗体</param>
        public PrintPaperHelper(string defaultPrinter, Point docPoint, string page, bool landscape, Margins margin, bool printwindowsshow = true)
        {
     
            DocumentPrint = new PrintDocument
            {
                PrinterSettings = {PrinterName = defaultPrinter},          
            };
            if (!printwindowsshow) DocumentPrint.PrintController = new StandardPrintController();
            DocumentPoint = docPoint;
            var papers = new List<PaperSize>(PrintDeviceHelper.GetPrintPageType(DocumentPrint));
            DocumentPrint.DefaultPageSettings.PaperSize = papers.Find(x => x.PaperName == page);
            DocumentPrint.DefaultPageSettings.Margins = margin;
            DocumentPrint.DefaultPageSettings.Landscape = landscape;    
            DocumentPrint.PrintPage += Pd_PrintPage;
        }

        public void Print(List<IPrintPaperData> printData)
        {
            PrintPaperData = printData;
            DocumentPrint.Print();
            DocumentPrint.Dispose();
            GC.Collect();
        }

        public void PrintView(List<IPrintPaperData> printData)
        {

            PrintPaperData = printData;
            var pv = new PrintPreviewDialog { Document = DocumentPrint };
            pv.ShowDialog();
            pv.Dispose();
            GC.Collect();
        }

        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.High;
            e.PageSettings.PrinterResolution.X = Dpi.X;
            e.PageSettings.PrinterResolution.Y = Dpi.Y;

            var rect = new Rectangle(DocumentPoint,new Size(e.PageBounds.Width,e.PageBounds.Height));

            foreach (var data in PrintPaperData)
            {
                if (data.GetType() == typeof(PaperText))
                {
                    var datatotype = data as PaperText;

                    rect.Y = (int)(rect.Y + datatotype.Spacing);

                    var rowindex = 0;

                    foreach (var drawdata in datatotype.Segmentation)
                    {
                        if (datatotype.Segmentation.Count > 1 && rowindex>=1)
                        {
                            rect.Y = (int)(rect.Y + e.Graphics.MeasureString(drawdata, datatotype.TextFont).Height + 2);
                        }
 
                        e.Graphics.DrawString(drawdata, datatotype.TextFont, new SolidBrush(Color.Black), rect,
                            datatotype.TextFormat);

                       rowindex =  rowindex + 1;

                    }

                }
                else
                {
                    var datatotype = data as PaperImage;
             

                    if (datatotype.IsFit)
                    {
                        var fittable = ImageHepler.GetAdaptationSize(datatotype.PaperBitmap.Width, datatotype.PaperBitmap.Height, e.PageBounds.Width,
                        e.PageBounds.Height, false);

                        var drawHeight = datatotype.BitmapMaxHeight > 0
                            ? datatotype.BitmapMaxHeight
                            : (int)fittable.Size.Dfith;

                        e.Graphics.DrawImage(datatotype.PaperBitmap, (int) fittable.Point.X,
                            rect.Y + (int) datatotype.Spacing,
                            datatotype.BitmapMaxWidth > 0 ? datatotype.BitmapMaxWidth : (int) fittable.Size.Dfitw,
                            drawHeight
                            );

                        rect.Y = (int)(rect.Y + drawHeight);
                    }
                    else
                    {
                        var x = e.PageBounds.Width/2 - datatotype.PaperBitmap.Width/2;

                        e.Graphics.DrawImage(datatotype.PaperBitmap, x,
                           rect.Y + (int)datatotype.Spacing,
                          datatotype.PaperBitmap.Width,
                          datatotype.PaperBitmap.Height
                           );

                        rect.Y = (int)(rect.Y + datatotype.PaperBitmap.Height);
                    }

                    datatotype.Dispose();
                }
            }
        }

        public void Dispose()
        {
            
        }    

        public event PrintEndDelegate EventPrintend
        {
            add { _printImplementation.EventPrintend += value; }
            remove { _printImplementation.EventPrintend -= value; }
        }
    }

    /// <summary>
    /// 票据文本
    /// </summary>
    public class PaperText : IPrintPaperData
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spacing"></param>
        /// <param name="originalText"></param>
        /// <param name="size"></param>
        /// <param name="textFont"></param>
        public PaperText(decimal spacing, string originalText,PaperSize size,Font textFont)
        {
            Spacing = spacing;

            OriginalText = originalText;

            PagerSize = size;

            TextFormat = new StringFormat() { LineAlignment = System.Drawing.StringAlignment.Near, Alignment = StringAlignment.Center };

            TextFont = textFont;

            var printBitmap = new Bitmap(size.Width * 203 / 100, size.Height * 203 / 100);

            printBitmap.SetResolution(203, 203);

            Segmentation = ImageHepler.GetStringRows(Graphics.FromImage(printBitmap), textFont, originalText);
        }

        public PaperText(decimal spacing, string originalText,StringFormat textFormat,Font textFont,PaperSize size,PrintStruct.Pdpi dpi)
        {
           
            Spacing = spacing;

            OriginalText = originalText;

            TextFormat = textFormat;

            TextFont = textFont;

            PagerSize = size;

            var printBitmap = new Bitmap(size.Width*dpi.X/100, size.Height*dpi.Y/100);

            printBitmap.SetResolution(dpi.X, dpi.Y);

            Segmentation = ImageHepler.GetStringRows(Graphics.FromImage(printBitmap), textFont, originalText);
        }

        /// <summary>
        /// 打印页尺寸
        /// </summary>
        public PaperSize PagerSize { get; set; }

        /// <summary>
        /// 与上一行间距
        /// </summary>
        public decimal Spacing { get; set; }

        /// <summary>
        /// 原始文本
        /// </summary>
        public string OriginalText { get; set; }

        /// <summary>
        /// 文本字体样式
        /// </summary>
        public Font TextFont { get; set; }

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public StringFormat TextFormat { get; set; }

        /// <summary>
        /// 分割后文本
        /// </summary>
        public List<string> Segmentation { get; set; }
    }

    /// <summary>
    /// 票据图像
    /// </summary>
    public class PaperImage : IPrintPaperData,IDisposable
    {
        /// <summary>
        /// 位图
        /// </summary>
        public Bitmap PaperBitmap { get; set; }

        /// <summary>
        /// 与上一行间距
        /// </summary>
        public decimal Spacing { get; set; }

        /// <summary>
        /// 位图最大宽度
        /// </summary>
        public int BitmapMaxWidth { set; get; }

        /// <summary>
        /// 位图最大高度
        /// </summary>
        public int BitmapMaxHeight { set; get; }

        /// <summary>
        /// 是否适配
        /// </summary>
        public bool IsFit { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap">位图</param>
        /// <param name="spacing">与上一行间距</param>
        public PaperImage(Bitmap bitmap, decimal spacing)
        {
            PaperBitmap = bitmap;
            Spacing = spacing;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="spacing">与上一行间距</param>
        public PaperImage(string path, decimal spacing)
        {
            PaperBitmap = ImageHepler.LoadBitmap(path);
            Spacing = spacing;
        }


        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                PaperBitmap.Dispose();
            }         
        }

        ~PaperImage()
        {
            Dispose(false); //释放非托管资源
        }
    }

    public interface IPrintPaperData
    {
        
    }
}
