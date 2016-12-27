using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace NaughtyKidMVVM.Threading
{
    public static class DispatcherHelper
    {
        /// <summary>
        /// Gets a reference to the UI thread's dispatcher, after the
        /// <see cref="Initialize" /> method has been called on the UI thread.
        /// </summary>
        // ReSharper disable InconsistentNaming
#if NETFX_CORE
        public static CoreDispatcher UIDispatcher
#else
        public static Dispatcher UIDispatcher
#endif
        // ReSharper restore InconsistentNaming
        {
            get;
            private set;
        }

        /// <summary>
        /// Executes an action on the UI thread. If this method is called
        /// from the UI thread, the action is executed immendiately. If the
        /// method is called from another thread, the action will be enqueued
        /// on the UI thread's dispatcher and executed asynchronously.
        /// <para>For additional operations on the UI thread, you can get a
        /// reference to the UI thread's dispatcher thanks to the property
        /// <see cref="UIDispatcher" /></para>.
        /// </summary>
        /// <param name="action">The action that will be executed on the UI
        /// thread.</param>
        // ReSharper disable InconsistentNaming
        public static void CheckBeginInvokeOnUI(Action action)
        // ReSharper restore InconsistentNaming
        {
            if (action == null)
            {
                return;
            }

            CheckDispatcher();

#if NETFX_CORE
            if (UIDispatcher.HasThreadAccess)
#else
            if (UIDispatcher.CheckAccess())
#endif
            {
                action();
            }
            else
            {
#if NETFX_CORE
                UIDispatcher.RunAsync(CoreDispatcherPriority.Normal,  () => action());
#else
                UIDispatcher.BeginInvoke(action);
#endif
            }
        }

        private static void CheckDispatcher()
        {
            if (UIDispatcher == null)
            {
                var error = new StringBuilder("The DispatcherHelper is not initialized.");
                error.AppendLine();

#if SILVERLIGHT
#if WINDOWS_PHONE
                error.Append("Call DispatcherHelper.Initialize() at the end of App.InitializePhoneApplication.");
#else
                error.Append("Call DispatcherHelper.Initialize() in Application_Startup (App.xaml.cs).");
#endif
#elif NETFX_CORE
                error.Append("Call DispatcherHelper.Initialize() at the end of App.OnLaunched.");
#else
                error.Append("Call DispatcherHelper.Initialize() in the static App constructor.");
#endif

                throw new InvalidOperationException(error.ToString());
            }
        }

#if NETFX_CORE
        /// <summary>
        /// Invokes an action asynchronously on the UI thread.
        /// </summary>
        /// <param name="action">The action that must be executed.</param>
        /// <returns>The object that provides handlers for the completed async event dispatch.</returns>
        public static IAsyncAction RunAsync(Action action)
#else
        /// <summary>
        /// Invokes an action asynchronously on the UI thread.
        /// </summary>
        /// <param name="action">The action that must be executed.</param>
        /// <returns>An object, which is returned immediately after BeginInvoke is called, that can be used to interact
        ///  with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation RunAsync(Action action)
#endif
        {
            CheckDispatcher();

#if NETFX_CORE
            return UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
#else
            return UIDispatcher.BeginInvoke(action);
#endif
        }

        /// <summary>
        /// This method should be called once on the UI thread to ensure that
        /// the <see cref="UIDispatcher" /> property is initialized.
        /// <para>In a Silverlight application, call this method in the
        /// Application_Startup event handler, after the MainPage is constructed.</para>
        /// <para>In WPF, call this method on the static App() constructor.</para>
        /// </summary>
        public static void Initialize()
        {
#if SILVERLIGHT
            if (UIDispatcher != null)
#else
#if NETFX_CORE
            if (UIDispatcher != null)
#else
            if (UIDispatcher != null
                && UIDispatcher.Thread.IsAlive)
#endif
#endif
            {
                return;
            }

#if NETFX_CORE
            UIDispatcher = Window.Current.Dispatcher;
#else
#if SILVERLIGHT
            UIDispatcher = Deployment.Current.Dispatcher;
#else
            UIDispatcher = Dispatcher.CurrentDispatcher;
#endif
#endif
        }

        /// <summary>
        /// Resets the class by deleting the <see cref="UIDispatcher"/>
        /// </summary>
        public static void Reset()
        {
            UIDispatcher = null;
        }
    }
}
