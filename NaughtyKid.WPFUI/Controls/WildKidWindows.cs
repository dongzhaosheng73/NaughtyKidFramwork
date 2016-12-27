
using System;
using System.Windows;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NaughtyKid.WPFUI.Command;

using System.Drawing.Printing;
using System.Windows.Controls;
using NaughtyKidMVVM.Threading;

namespace NaughtyKid.WPFUI.Controls
{
  
    public partial class NaughtyKidWindows:DpiAwareWindow
    {
        /// <summary>
        /// 标题字体颜色
        /// </summary>
        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(NaughtyKidWindows), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255,0,0,0))));


        /// <summary>
        /// 标题字体大小
        /// </summary>
        public int TitleSize
        {
            get { return (int)GetValue(TitleSizeProperty); }
            set { SetValue(TitleSizeProperty, value); }
        }


        public static readonly DependencyProperty TitleSizeProperty =
            DependencyProperty.Register("TitleSize", typeof(int), typeof(NaughtyKidWindows), new PropertyMetadata(10));


        /// <summary>
        /// 标题颜色
        /// </summary>
        public Brush TitleColor
        {
            get { return (Brush)GetValue(TitleColorProperty); }
            set { SetValue(TitleColorProperty, value); }
        }

        public static readonly DependencyProperty TitleColorProperty =
            DependencyProperty.Register("TitleColor", typeof(Brush), typeof(NaughtyKidWindows), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255,169,169,169))));


        /// <summary>
        /// 系统按键宽度
        /// </summary>
        public double SystemButtonWidth
        {
            get { return (double)GetValue(SystemButtonWidthProperty); }
            set { SetValue(SystemButtonWidthProperty, value); }
        }

        public static readonly DependencyProperty SystemButtonWidthProperty =
            DependencyProperty.Register("SystemButtonWidth", typeof(double), typeof(NaughtyKidWindows), new PropertyMetadata((double)20));

        /// <summary>
        /// 系统按键高度
        /// </summary>
        public double SystemButtonHeight
        {
            get { return (double)GetValue(SystemButtonHeightProperty); }
            set { SetValue(SystemButtonHeightProperty, value); }
        }

        public static readonly DependencyProperty SystemButtonHeightProperty =
            DependencyProperty.Register("SystemButtonHeight", typeof(double), typeof(NaughtyKidWindows), new PropertyMetadata((double)20));


        /// <summary>
        /// 系统按键间隔
        /// </summary>
        public Thickness SystemButtonMargin
        {
            get { return (Thickness) GetValue(SystemButtonMarginProperty); }
            set { SetValue(SystemButtonMarginProperty, value); }
        }

        public static readonly DependencyProperty SystemButtonMarginProperty =
            DependencyProperty.Register("SystemButtonMargin", typeof(Thickness), typeof(NaughtyKidWindows), new PropertyMetadata(new Thickness(2)));



        /// <summary>
        /// 设置菜单调用事件
        /// </summary>
       public static readonly RoutedEvent SettingButtonClickEvent =
           EventManager.RegisterRoutedEvent("SettingButtonClick", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(NaughtyKidWindows));


        public event RoutedEventHandler SettingButtonClick
        {
            add
            {
                base.AddHandler(SettingButtonClickEvent,value);
            }
            remove
            {
                base.RemoveHandler(SettingButtonClickEvent, value);
            }
        }


        public NaughtyKidWindows()
        { 
            DispatcherHelper.Initialize();

            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("/NaughtyKid.WPFUI;component/Themes/NaughtyKidWindows.xaml", UriKind.Relative)) as ResourceDictionary);

            Style = (Style)Application.Current.Resources["WkWindowsBaseStyle"];

            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(NaughtyKidWindowsCommand.SettingWindowCommand, OnSettingWindow, OnCanSettingWindow));
           
        }

        private void OnSettingWindow(object sender, ExecutedRoutedEventArgs e)
        {         
            var settingButtonEventArgs = new RoutedEventArgs(SettingButtonClickEvent);
            RaiseEvent(settingButtonEventArgs);
        }

        private void OnCanSettingWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            (e.OriginalSource as Button).Visibility = Visibility.Collapsed;
        }

        private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
        }        

        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(this);
        }  

        private void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Windows.Shell.SystemCommands.CloseWindow(this);
        }

    }
}
