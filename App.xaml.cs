using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RemoveComVK
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void Help()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/i9a4/RemoveComVK/blob/main/README.md/",
                UseShellExecute = true
            });

        }
    }
}
