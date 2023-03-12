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
        private static readonly ILog log = Helper.GetLoggerRollingFileAppender("TestAppender", "D:/log.txt");

        protected override void OnStartup(StartupEventArgs e)
        {
           
            
            log.Info("LocalPath= " + Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "../../../App_Data"));
            Debug.WriteLine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "../../../App_Data"));
            var appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cetrack\\App_Data");
            var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"); 
            Debug.WriteLine($"localPath = {localPath}");
            Debug.WriteLine("appPath = " + appPath);
            log.Info("appPath = " + appPath);
            
            AppDomain.CurrentDomain.SetData("DataDirectory", localPath); // appPath
        
            Debug.WriteLine("Data Directory set to: " + AppDomain.CurrentDomain.GetData("DataDirectory"));

            //Debug.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        }


    }

 
}
