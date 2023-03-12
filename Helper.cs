using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CE_Tracker.Model;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;


// https://stackoverflow.com/questions/38616642/log4net-not-working-when-running-wpf-applications-executable

namespace CE_Tracker 
{
    public static class Helper
    {
        static Helper()
        {
            //log4net
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }
        //log4net
        public static ILog GetLoggerRollingFileAppender(string logName, string fileName)
        {
            var log = LogManager.Exists(logName);

            if (log != null) return log;

            var appenderName = $"{logName}Appender";
            log = LogManager.GetLogger(logName);
            ((Logger)log.Logger).AddAppender(GetRollingFileAppender(appenderName, fileName));

            return log;
        }
        // log4net
        public static RollingFileAppender GetRollingFileAppender(string appenderName, string fileName)
        {
            var layout = new PatternLayout { ConversionPattern = "%date{dd.MM.yyyy HH:mm:ss.fff}  [%-5level]  %message%newline" };
            layout.ActivateOptions();

            var appender = new RollingFileAppender
            {
                Name = appenderName,
                File = fileName,
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaxSizeRollBackups = 2,
                MaximumFileSize = "500KB",
                Layout = layout,
                ImmediateFlush = true,
                LockingModel = new FileAppender.MinimalLock(),
                Encoding = Encoding.UTF8,
            };

            appender.ActivateOptions();

            return appender;
        }

        // Helper function to convert Observable Collection to generic list then back to remove persistence for Undo/Revert function
        //
        public static void ConvertTable(ObservableCollection<CE_TableModel> source, ref ObservableCollection<CE_TableModel> dest)
        {
            var temp = source.ToList(); // convert into stringly typed list
            dest = Helper.ConvertToObservableCollection(temp); // and back into ObserverableCollection
        }

        public static ObservableCollection<CE_TableModel> ConvertToObservableCollection(IEnumerable<CE_TableModel> items)
        {
            ObservableCollection<CE_TableModel> collection = new ObservableCollection<CE_TableModel>();
            foreach (var item in items)
            {
                CE_TableModel tablemodel = item;
                collection.Add(new CE_TableModel(tablemodel.ModId, tablemodel.Id, tablemodel.CEmod, tablemodel.Subject, tablemodel.Hours, tablemodel.Comments));
            }
            return collection;
        }

        /*
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> source) // extension
        {
            return new ObservableCollection<T>(source);
        }
        */
    }


    public class Hashing
    {
        private static string GetRandomSalt()
        {
            return BCrypt.BCryptHelper.GenerateSalt(12);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.BCryptHelper.HashPassword(password, GetRandomSalt());
        }

        public static bool ValidatePassword(string password, string correctHash)
        {
            return BCrypt.BCryptHelper.CheckPassword(password, correctHash);
        }
    }

    public class CommandHandler : ICommand
    {

        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
