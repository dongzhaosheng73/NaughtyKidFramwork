using System;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using NaughtyKid.Error;

namespace NaughtyKid.MyTools
{
    /// <summary>
    /// 二维码生成帮助类
    /// </summary>
    public  class QrCodeHelper
    {
        /// <summary>
        /// 二维码生成
        /// </summary>
        /// <param name="qRstring">字符串</param>
        /// <returns></returns>
        public static Image PhotoQr(string qRstring)
        {
            try
            {
                var qrCodeEncoder = new QRCodeEncoder
                {
                    QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                    QRCodeScale = 4,
                    QRCodeVersion = 5,
                    QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M
                };
                String data = qRstring;
                Image image = qrCodeEncoder.Encode(data);
                return image;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
           
        }

        /// <summary>
        /// 二维码生成
        /// </summary>
        /// <param name="qRstring">字符串</param>
        /// <param name="qrEncoding">三种模式：Byte（任意类型） ，AlphaNumeric（包括文字与数字的），Numeric（数字的）</param>
        /// <param name="level">错误更正级别：L M Q H</param>
        /// <param name="version">版本4如果遇到超出数组应设置为0</param>
        /// <param name="scale">大小</param>
        /// <returns></returns>
        public static Image PhotoQr(string qRstring, string qrEncoding, string level, int version, int scale)
        {
            try
            {
                var qrCodeEncoder = new QRCodeEncoder();
                string encoding = qrEncoding;
                switch (encoding)
                {
                    case "Byte":
                        qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                        break;
                    case "AlphaNumeric":
                        qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                        break;
                    case "Numeric":
                        qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                        break;
                    default:
                        qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                        break;
                }

                qrCodeEncoder.QRCodeScale = scale;
                qrCodeEncoder.QRCodeVersion = version;
                switch (level)
                {
                    case "L":
                        qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                        break;
                    case "M":
                        qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                        break;
                    case "Q":
                        qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                        break;
                    default:
                        qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                        break;
                }

                return qrCodeEncoder.Encode(qRstring);
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
           
        }
    }
}
