using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace NaughtyKid.WPF
{
    public class ElementInquiry
    {
        /// <summary>
        /// 获取指定元素的父元素
        /// </summary>
        /// <param name="obj">要查询的元素</param>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns></returns>
        public static T GetParentObject<T>(DependencyObject obj) where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                var o = parent as T;
                if (o != null)
                {
                    return o;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
        /// <summary>
        /// 获取指定元素的所有元素
        /// </summary>
        /// <typeparam name="T">获取元素类型</typeparam>
        /// <param name="obj">要查询的元素</param>
        /// <returns></returns>
        public List<T> GetChildObjects<T>(DependencyObject obj) where T : FrameworkElement
        {
            var childList = new List<T>();

            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                if (child is T)
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child));
            }
            return childList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="obj">容器元素</param>
        /// <param name="name">元素名称</param>
        /// <returns></returns>
        public T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);


                var o = child as T;
                if (o != null && (o.Name == name | string.IsNullOrEmpty(name)))
                {
                    return o;
                }
                var grandChild = GetChildObject<T>(child, name);
                if (grandChild != null)
                    return grandChild;
            }
            return null;
        }
    }
}
