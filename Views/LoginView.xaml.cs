using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;
using Dapper;
using System.Data;
using System.Data.SqlClient; //added
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;
using log4net;
using log4net.Config;
//using CE_Tracker.Controllers;
using CE_Tracker.Model;
using CE_Tracker.ViewModel;
using CE_Tracker.Components;
using System.Security;

//TODO trim name, check for illegal characters

namespace CE_Tracker
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    /// 
    public partial class Login : MetroWindow
    {
        //log4net
        private static readonly ILog log = Helper.GetLoggerRollingFileAppender("TestAppender", "D:/log.txt");

        public Login()
        {
            InitializeComponent();
             


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void LoginWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            log.Info("unloaded through login window");
        }
    }
}
