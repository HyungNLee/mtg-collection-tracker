using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using DesktopApp.Utils;

using Serilog;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            FileLogger.CreateFileLogger();

            Log.Debug($"{nameof(App)}: Constructor");
        }
    }
}
