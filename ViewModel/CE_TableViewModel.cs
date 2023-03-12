using CE_Tracker.Model;
using Dapper;
using FirstFloor.ModernUI.Presentation;
using log4net;
using Microsoft.Data.SqlClient;
using ServiceStack;
using ServiceStack.Script;
using System;
using System.Collections.ObjectModel;
using System.Data;
//using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

// TODO:
// Hours left of each categry, start and stop date for licensure period



namespace CE_Tracker.ViewModel
{
    public class CE_TableViewModel : ModelBase
    {
        private readonly string connString = Properties.Settings.Default.connString;
        private static readonly ILog log = Helper.GetLoggerRollingFileAppender("TestAppender", "D:/log.txt");
        private readonly MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        // For undo function.  Just copies entire datagrid.
        private ObservableCollection<CE_TableModel> tempTable = new ObservableCollection<CE_TableModel>();


        // for Hours check, but doesn't catch weird input
        public bool IsInRange(float i) => i is (>= 0.25f and <= 120);

        private ObservableCollection<CE_TableModel> _CE_TableModels;
        public ObservableCollection<CE_TableModel>   CE_TableModels
        {
            get { return _CE_TableModels; }
            set 
            { 
                _CE_TableModels = value;
                OnPropertyChanged();
            }
        }

        // All properties associated with textboxes I add Box at the end.

        // Mod date / txtMod
        private DateTime _dateBox; 
        public DateTime DateBox
        {
            get { return _dateBox; }
            set
            {
                _dateBox = value;
                OnPropertyChanged();
            }
        }

        // Subject textbox
        private string _subjectBox;
        public string SubjectBox
        {
            get { return _subjectBox; }
            set
            {
                _subjectBox = value;
                OnPropertyChanged();
            }
        }

        // Hours of CE textbox
        private float _hoursBox;
        public float HoursBox
        {
            get 
            { return _hoursBox;  }
            set
            {
                _hoursBox = value;
                OnPropertyChanged();
            }
        }

        // Any comments about CE mod textbox
        private string _commentsBox;
        public string CommentsBox
        {
            get { return _commentsBox; }
            set
            {
                _commentsBox = value;
                OnPropertyChanged();
            }
        }

        // Status bar
        private string _statusBlock;
        public string StatusBlock
        {
            get { return _statusBlock; }
            set
            {
                _statusBlock = value;
                OnPropertyChanged();
            }
        }

        // which record we have selected in the data grid.
        private CE_TableModel _selectedRow;
        public CE_TableModel SelectedRow
        {
            get 
            { return _selectedRow;  }
            set
            {
               _selectedRow = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _menu_logout;
        public ICommand Menu_Logout => _menu_logout ??= new RelayCommand(PerformMenuLogout_Click);

        private RelayCommand _menu_Quit;
        public ICommand Menu_Quit => _menu_Quit ??= new RelayCommand(PerformMenu_Quit);

        private RelayCommand _btnAdd_Click;
        public ICommand BtnAdd_Click => _btnAdd_Click ??= new RelayCommand(PerformBtnAdd_Click);

        private RelayCommand _btnModify_Click;
        public ICommand BtnModify_Click => _btnModify_Click ??= new RelayCommand(PerformBtnModify_Click);

        private RelayCommand _btnRefresh_Click;
        public ICommand BtnRefresh_Click => _btnRefresh_Click ??= new RelayCommand(PerformBtnRefresh_Click);

        private RelayCommand _btnRevert_Click;
        public ICommand BtnRevert_Click => _btnRevert_Click ??= new RelayCommand(PerformBtnRevert_Click);

        private RelayCommand _btnDelete_Click;
        public ICommand BtnDelete_Click => _btnDelete_Click ??= new RelayCommand(PerformBtnDelete_Click);

        private RelayCommand _gridselectionChanged;
        public ICommand GridSelectionChanged => _gridselectionChanged ??= new RelayCommand(PerformGridSelectionChanged);

        //not yet implemented
//        private RelayCommand _txtHoursValidate;
//        public ICommand TxtHoursValidate => _txtHoursValidate ??= new RelayCommand(ValidateTxtHours);

        public CE_TableViewModel()
        {
            CE_TableModels = new ObservableCollection<CE_TableModel>();
            mainWindow.Title += LoginViewModel.CurrentUser;
            DateBox = DateTime.Now;
            GetRows();
            log.Info("CE_TableViewModel instantiated, getting rows");
            Debug.WriteLine("CE_TableViewModel instantiated, getting rows"); //should consolidate log.info with debug.writeline

    }
      
        private bool ValidateHours(float hours)
        {
         
            return IsInRange(hours);
        }

        /*
        private void ValidateTxtHours(object commandParameter)
        {



        }
        */
        public void PerformBtnRefresh_Click(object sender)
        {
            GetRows();
        }

        // Undo last change
        private void PerformBtnRevert_Click(object commandParameter)
        {
            Debug.WriteLine("Before revert CE_TableModels");
            foreach (var item in CE_TableModels)
            {
                Debug.WriteLine($"{item.ModId} {item.CEmod} {item.Subject} {item.Hours} {item.Comments}");
            }
            Debug.WriteLine("Before revert tempTable");
            foreach (var item in tempTable)
            {
                Debug.WriteLine($" Before change tempTable: {item.ModId} {item.CEmod} {item.Subject} {item.Hours} {item.Comments}");
            }
            // revert to last iteration of CE_TableModels (datagrid)
            CE_TableModels = tempTable;
            Debug.WriteLine("after revert CE_TableModels");
            foreach (var item in CE_TableModels)
            {
                Debug.WriteLine($"{item.ModId} {item.CEmod} {item.Subject} {item.Hours} {item.Comments}");
            }

            UpdateRows("update");
        }

        private void PerformBtnAdd_Click(object commandParameter)
        {
            // is Datepicker or Subject empty?
            if (!(string.IsNullOrEmpty(DateBox.AsString()) || string.IsNullOrEmpty(SubjectBox)))
            {
                
                // check if hours only 0.25-120
                if (ValidateHours(HoursBox))  //regex.IsMatch(HoursBox) && !(string.IsNullOrEmpty(HoursBox))) // (HoursBox > 0 && HoursBox <= 121)
                {
                    // variable for getting current ID of record in database (then +1 for new record being added)
                    int nextId = 0;

                    if (CE_TableModels.Count > 0)// Create a reference to CE_TableModels (r) and return the value ModId in (r) + 1
                        nextId = CE_TableModels.Max(r => r.ModId + 1); //get highest ModId and add 1 for new entry



                    CE_TableModels.Add(new CE_TableModel { ModId = nextId, Id = LoginViewModel.CurrentId, CEmod = DateBox, Hours = HoursBox, Subject = SubjectBox, Comments = CommentsBox });


                    UpdateRows("update");
                    //           listRows.ForEach(r => Debug.WriteLine($"ADDED \n {r.ModId} {r.CEmod} {r.Subject} {r.Hours} {r.Comments}"));
                    StatusUpdate($"Added new record with id {nextId} @ ");
                }
                else
                {
                    StatusUpdate("Illegal characters found in Hours field");
                    MessageBox.Show("Please only enter a number  greater than 0 through 120 in the Hours field ");
                }
            }
            else
            {
                MessageBox.Show("Mod date or Subject must not be empty");
            }
        }

        // pull records from SQL database and update datagrid
        public void GetRows()
        {
            try
            {

                using (IDbConnection conn = new SqlConnection(connString))
                {
                    var sql = @"SELECT * from CE_Entry WHERE Id=@Id";

                    var listRows = conn.Query<CE_TableModel>(sql, new { Id = LoginViewModel.CurrentId }).ToList();

                    // dapper doesn't support observable collections, have to convert
                    CE_TableModels = Helper.ConvertToObservableCollection(listRows); 
                    

                    Debug.WriteLine("Table records are type: " + CE_TableModels.GetType());
                    foreach(var item in CE_TableModels)
                    {
                        Debug.WriteLine($"{item.ModId} {item.CEmod} {item.Subject} {item.Hours} {item.Comments}");
                    }

                    StatusUpdate("Refreshed ");
                    Debug.WriteLine(conn.ConnectionString);

                }
            }
            catch (Exception ex)
            {
                log.Info(ex.ToString());
                StatusUpdate("Unable to refresh database");

            }

            
        }
 
        // method for updating OR deleting a record in SQL database
        // to add NEW record, choose "update" - that statement will detect whether to modify existing or add new record.
        private void UpdateRows(string method)
        {

                try
                {
                    using (IDbConnection conn = new SqlConnection(connString))
                    {

                        string sql = "";

                        switch (method)
                        {
                            // Update record, if no match found, insert new record
                            case "update":
                                sql = @"MERGE INTO CE_Entry AS T
                                USING (VALUES (@ModId, @Id, @CEmod, @Subject, @Hours, @Comments)) AS S (ModId, Id, CEmod, Subject, Hours, Comments)
                                ON T.ModId = S.ModId
                                WHEN MATCHED THEN
                                UPDATE SET T.CEmod = @CEmod, T.Subject = @Subject, T.Hours = @Hours, T.Comments = @Comments 
                                WHEN NOT MATCHED BY TARGET THEN
                                INSERT (CEmod, Id, Subject, Hours, Comments) 
                                VALUES (@CEmod, @Id, @Subject, @Hours, @Comments);";
                                //Don't get T reference mixed up with TARGET - "WHEN NOT MATCHED BY TARGET THEN"
                                conn.Execute(sql, CE_TableModels); 

                                GetRows(); // refresh grid
                                StatusUpdate("Changes Successfully Committed");
                                log.Info("Changes Successfully Committed");
                                break;

                            case "delete":

                                int tempRow = SelectedRow.ModId;
                              

                                sql = @"DELETE FROM CE_Entry
                                WHERE ModId=@ModId";
                                conn.Execute(sql, new { ModId = SelectedRow.ModId }); //send SQL statement

                               // Refresh grid
                               GetRows(); 
                               StatusUpdate($"Record successfully deleted at id {tempRow}");
                                
                                break;
                            default:
                                StatusUpdate("Invalid parameter, please use update or delete");
                                break;
                        }

                    }

                }
                catch (Exception ex)
                {
                    log.Info(ex.ToString());
                    StatusUpdate("Error updating record");
                    MessageBox.Show(ex.ToString());
                }
      
        
         
}


        // Delete a record/tuple
        public void PerformBtnDelete_Click(object sender)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the selected record?", "Delete Record Confirmation",
            MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                // break persistence 
                Helper.ConvertTable(CE_TableModels, ref tempTable);
                UpdateRows("delete");

            }
        }

 

        // When the user selects a tuple
        public void PerformGridSelectionChanged(object sender)
        {
            try
            {

                 if (SelectedRow != null)
                  {

                    DateBox         = SelectedRow.CEmod; //index.CEmod; //  DateBox = index.CEmod.ToShortDateString();
                    HoursBox        = SelectedRow.Hours;
                    SubjectBox      = SelectedRow.Subject;
                    CommentsBox     = SelectedRow.Comments ??= ""; //if null change to empty field "".

                    StatusUpdate("Selection Changed");
                }
                else 
                {
                    StatusUpdate("SelectedRow is null");
                    Debug.WriteLine("SelectedRow is null");  // ohh boy
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("unable to select row");
            //    log.Info(ex);
                StatusUpdate(ex.ToString());
            }

        }



        // When user modifies an existing record/tuple
        public void PerformBtnModify_Click(object sender)
        {

            // maybe change this to switch statement
            // Check to see if Date or Subject are not null or empty
            if (!(string.IsNullOrEmpty(DateBox.AsString()) || string.IsNullOrEmpty(SubjectBox)))
            {
                //convert CE_TableModels to generic list than back to ObservableCollection to break persistance 
                Helper.ConvertTable(CE_TableModels, ref tempTable);

                // is the entry from the Hours textbox within valid range?
                if (ValidateHours(HoursBox)) // not working.  change to parse to string then use regex?
                {
                    try
                    {
                    
                        if (SelectedRow != null)
                        {
                           
                            SelectedRow.CEmod = DateBox; // index.CEmod = newDateTime;
                            SelectedRow.Hours = HoursBox;
                            SelectedRow.Subject = SubjectBox;
                            SelectedRow.Comments = CommentsBox;

                            UpdateRows("update");

                            // for debugging in case values aren't modified
                            foreach (var item in tempTable)
                            {
                                Debug.WriteLine($" After change tempTable: {item.ModId} {item.CEmod} {item.Subject} {item.Hours} {item.Comments}");
                            }

                        }
                        else
                        {
                            StatusUpdate("Index is Null");
                           
                        }


                    }
                    catch (Exception ex)
                    {
                        log.Info(ex);
                        StatusUpdate("Unable to modify record!");
                    }


                }
                else
                {

                    StatusUpdate("Illegal characters found in Hours field");
                    MessageBox.Show("Please only enter a number between 0 and 120 in the Hours field");
                }
            }
            else
            {
                StatusUpdate("Date or Subject field invalid");
                MessageBox.Show("Date or Subject field invalid");
            }
        }


        // Command for logging out via menu (other than closing the window via X)
        public void PerformMenuLogout_Click(object sender)
        {
            //CONFIRMATION to close
            //Application.Current.MainWindow.Close();
            log.Info("logging out");
            Login login = new Login();
            login.Show(); //.Hide();
            mainWindow.Close();
        }

        private void PerformMenu_Quit(object commandParameter)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the application?", "Quit Confirmation",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                //add confirmation
                log.Info("Exited through menu");
                Application.Current.Shutdown();
            }
        }

        // Update method for status bar at bottom of window
        public void StatusUpdate(string s) 
        {

                log.Info(s);
                StatusBlock = s + " " + DateTime.Now;
 
        }



    }
}
