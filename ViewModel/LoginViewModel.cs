using CE_Tracker.Model;
using Dapper;
using FirstFloor.ModernUI.Presentation;
using log4net;
using log4net.Config;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;


namespace CE_Tracker.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {

        private readonly string connString = Properties.Settings.Default.connString;
        private readonly List<UsersModel> lUsers = new List<UsersModel>();
        public static int CurrentId { get; set; } = 0;
        public static string CurrentUser { get; set; } = "";
        // for log4net
        private static readonly ILog log = Helper.GetLoggerRollingFileAppender("TestAppender", "D:/log.txt");

        // All properties associated with textboxes end with Box.
        private string _userNameBox;
        public string UserNameBox
        {
            get { return _userNameBox.Trim(); } // stops any whitespace in username textbox. // value.Trim() ?? string.Empty; }
            set
            {
                _userNameBox = value; 
                OnPropertyChanged();
            }
        }
        // {Binding Path=PwdBox, UpdateSourceTrigger=PropertyChanged}
        // using workaround with custom component in order to bind passwordbox, but not safe for commercial application.
        private string _pwdBox; 
        public string PwdBox
        {
            get { return _pwdBox; }  
            set
            {
                _pwdBox = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _btnLogin_Click;
        public ICommand BtnLogin_Click => _btnLogin_Click ??= new RelayCommand(PerformBtnLogin_Click);

        private RelayCommand _btnRegister_Click;
        public ICommand BtnRegister_Click => _btnRegister_Click ??= new RelayCommand(PerformBtnRegister_Click);


        public LoginViewModel()
        {
            UserNameBox = "";
            PwdBox = "";
            // log4net
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("log4net.config"));
            Debug.WriteLine(Environment.SpecialFolder.ApplicationData);
            log.Info("LoginViewModel instantiated");
        }

        private void PerformBtnLogin_Click(object commandParameter)
        {
            if (RegexCheck())
            {

                Debug.WriteLine("Passed regex check");
                log.Info("Regex check passed [Login_Click]");

                try
                {

                    // TODO: more checks - regex, duplicates, etc
                    using (IDbConnection conn = new SqlConnection(connString))
                    {
  
                        // Find corresponding password in database to username from textbox 
                        var sql = @"SELECT UserName, Pwd, Id from Users WHERE UserName = @UserName";
                        var result = conn.Query(sql, new { UserName = UserNameBox });

                        Debug.WriteLine(result.Count());

                        if (Hashing.ValidatePassword(PwdBox, result.FirstOrDefault().Pwd)) //changed from First()
                        {
                            Debug.WriteLine("Passed password check");
                            CurrentId = result.First().Id;
                            CurrentUser = result.First().UserName;
                            MainWindow mw = new MainWindow();
                            mw.Show();
                            Debug.WriteLine($"{result.First().UserName}, {result.First().Pwd}");

                            // get a reference to this window then close it.
                            Login login = Application.Current.Windows.OfType<Login>().FirstOrDefault();
                            login.Close();

                        }
                        else
                        {
                            MessageBox.Show("Invalid Login");
                            log.Info("Invalid Login");
                            // Debug.Print
                        }
                    }

                }
                catch (SqlException  ex)
                {
                    Console.WriteLine(ex.ToString());
                    MessageBox.Show(ex.Message);
                    log.Info(ex.Message);
                }
            }
            else
            {
                return;
            }
        }

        private void PerformBtnRegister_Click(object commandParameter)
        {
            try
            {
                //TODO trim name, check for illegal characters
                using (IDbConnection conn = new SqlConnection(connString))
                {
                    // Hash password typed in password box
                    // string pwd = Hashing.HashPassword(PwdBox);
                    // Find the username in the database
                    var sql = @"SELECT UserName from Users WHERE UserName = @UserName";
                    var result = conn.Query(sql, new { UserName = UserNameBox });
                    // log.Info($"DB queried - {result.FirstOrDefault().UserName}");
                    Debug.WriteLine(result.Count());
                    if (result?.FirstOrDefault()?.UserName == UserNameBox) //We found a corresponding username (already registered)
                    {
                        MessageBox.Show("Username already registered");
                        log.Info("Username already registered");
                    }
                    else // we didn't find a corresponding username - add user and hashed pwd to database
                    {
                        try
                        {
                            var newUser = @"INSERT INTO Users (Username, Pwd) VALUES(@UserName, @Pwd)";
                            //@"INSERT (Username, Pwd) INTO Users VALUES (@UserName, @Pwd)";
                            // INSERT INTO Users (Username, Pwd) VALUES(@UserName, @Pwd)
                            conn.Execute(newUser, new { UserName = UserNameBox, Pwd = Hashing.HashPassword(PwdBox) });
                            MessageBox.Show("Registered successfully");
                            log.Info("username registered successfully");
                        }
                        catch (Exception ex)
                        {
                            Debug.Write(ex.Message);
                            MessageBox.Show("Unable to register user due to database error");
                            log.Info("Unable to register user due to database error");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Info(ex.Message); 
                throw;
            }


        }

        private bool RegexCheck() // not working perfectly
        {
            if (UserNameBox == "" || PwdBox == "")
            {
                MessageBox.Show("One or more fields are empty");
                log.Info("One or more fields are empty");
                return false;

            }
            else if (Regex.Match(UserNameBox, "^[A-Z][a-zA-Z]*$").Success)
            {
                MessageBox.Show("illegal characters in username");
                log.Info("Illegal chars in username");
                return false;
            }
            else
            {
                log.Info("Regex check passed [Regex_check func]");
                return true;
            }

        }


    }
}
