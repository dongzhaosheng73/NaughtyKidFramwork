using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace NaughtyKid.Extension
{
    public static class UIElementExtend
    {
        /// <summary>
        /// 拷贝一个新的副本
        /// </summary>
        /// <param name="uielement"></param>
        /// <returns></returns>
        public static UIElement Clone(this UIElement uielement)
        {
            string saveduielement = XamlWriter.Save(uielement);
            StringReader stringReader = new StringReader(saveduielement);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            return (UIElement)XamlReader.Load(xmlReader);
        }
    }
}
