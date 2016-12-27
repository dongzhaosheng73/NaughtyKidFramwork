using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NaughtyKid.BaseClass;


namespace NaughtyKid.MyTools
{
    /// <summary>
    /// 分页帮助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageHepler<T> : PropertyChangedBase
    {

        public PageHepler()
        {


        }

        private ObservableCollection<T> _currentDate;

        /// <summary>
        /// 当前页数据
        /// </summary>
        public ObservableCollection<T> CurrentDate
        {
             get { return _currentDate; }
            set { _currentDate = value; OnPropertyChanged(() => CurrentDate); }
        }
        

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="span">每页数据数量</param>
        /// <param name="data">数据</param>
        public PageHepler(int span, List<T> data)
        {
            if (data == null)
                throw new ArgumentNullException();

            Pageindex = 0;

            Pagecount = (int)Math.Ceiling((double)data.Count / span);

            PageSpan = span;

            PageData = data;

            CurrentDate = new ObservableCollection<T>(PageGo(Pageindex));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="span"></param>
        /// <param name="totalPageCount"></param>
        public PageHepler(int span, int totalPageCount)
        {
           
            Pageindex = 0;

            Pagecount = totalPageCount;

            PageSpan = span;

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="span">每页数据数量</param>
        /// <param name="data">数据</param>
        /// <param name="pageindex">起始页</param>
        public PageHepler(int span, List<T> data, int pageindex)
        {
            if (data == null)
                throw new ArgumentNullException();

            Pageindex = pageindex;

            Pagecount = (int)Math.Ceiling((double)data.Count / span);

            PageSpan = span;

            PageData = data;

            CurrentDate = new ObservableCollection<T>(PageGo(Pageindex));
        }

        private int _pageindex;

        /// <summary>
        /// 当前页
        /// </summary>
        public int Pageindex
        {
            get { return _pageindex; }
            set { _pageindex = value; OnPropertyChanged(() => Pageindex); }
        }

        private int _pagecount;

        /// <summary>
        /// 总页数
        /// </summary>
        public int Pagecount
        {
            get { return _pagecount; }
            set { _pagecount = value; OnPropertyChanged(() => Pagecount); }
        }
        /// <summary>
        /// 每页的个数
        /// </summary>
        public int PageSpan { set; get; }
        /// <summary>
        /// 数据
        /// </summary>
        public List<T> PageData { set; get; }
        /// <summary>
        /// 跳转指定页数
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<T> PageGo(int index)
        {
            if (Pagecount <= 0 || Pageindex >= Pagecount || Pageindex < 0 || !PageData.Any())
                return new List<T>();

            var l2 = PageData.Skip(index * PageSpan).Take(PageSpan).ToList();

            Pageindex = index;

            CurrentDate = new ObservableCollection<T>(l2);

            return l2;
        }
        /// <summary>
        /// 后翻页
        /// </summary>
        /// <returns></returns>
        public List<T> NextPage()
        {
            if (Pagecount <= 0 || Pageindex + 1 >= Pagecount) return new List<T>();

            return PageGo(Pageindex + 1);
        }

        /// <summary>
        /// 前翻页
        /// </summary>
        /// <returns></returns>
        public List<T> AgoPage()
        {
            if (Pagecount <= 0 || Pageindex - 1 < 0) return new List<T>();

            return PageGo(Pageindex - 1);
        }
    }
}
