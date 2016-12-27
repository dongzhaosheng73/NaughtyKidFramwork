using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace NaughtyKid.WPFUI.Command
{
    public static  class NaughtyKidWindowsCommand
    {
        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static RoutedCommand SettingWindowCommand { get; } = new RoutedCommand("SettingWindow", typeof(NaughtyKidWindowsCommand));
    }
}
