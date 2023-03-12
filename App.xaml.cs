using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using log4net.Config;


namespace CE_Tracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // relative path to App_Data from debug folder
        private static readonly string appDataLoc = "../../../App_Data";
        //log4net
        private static readonly ILog log = Helper.GetLoggerRollingFileAppender("TestAppender", "log.txt");
        

        protected override void OnStartup(StartupEventArgs e)
        {

            log.Info("LocalPath = " + Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + appDataLoc));
            Debug.WriteLine("LocalPath = " + Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + appDataLoc));
            Debug.WriteLine("Base: " + AppDomain.CurrentDomain.BaseDirectory);

            // not using
            //var appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cetrack\\App_Data");
#if DEBUG
            var localPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + appDataLoc);
#else // release version will have different path to App_Data (will not have to go up multiple dirs from debug folder)
            var localPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "App_Data"); 
#endif

                      
            // set DataDirectory location
            AppDomain.CurrentDomain.SetData("DataDirectory", localPath); // appPath
            
            log.Info("Data Directory set to: " + AppDomain.CurrentDomain.GetData("DataDirectory"));
            Debug.WriteLine("Data Directory set to: " + AppDomain.CurrentDomain.GetData("DataDirectory"));

            // Debug.WriteLine($"localPath = {localPath}");
            // Debug.WriteLine("appPath = " + appPath);
            // log.Info("appPath = " + appPath);


        }


    }

 
}
