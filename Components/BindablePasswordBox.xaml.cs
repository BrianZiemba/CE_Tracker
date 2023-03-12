using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CE_Tracker.Components
{
    /// <summary>
    /// Interaction logic for BindablePasswordBox.xaml
    /// </summary>
    public partial class BindablePasswordBox : UserControl
    {

        private bool _isPasswordChanging;

        // Snippet: propdp
        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox), 
                new PropertyMetadata(string.Empty, PasswordPropertyChanged)); 
        //PasswordPropertyChange is callback, setting value to viewmodel - need to update the PasswordBox with the value from the viewmodel

        private static void PasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           if (d is BindablePasswordBox passwordBox)
            {
                passwordBox.UpdatePassword();
            }    
        }


        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public BindablePasswordBox()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _isPasswordChanging = true;
            Password = passwordBox.Password; // getting value from passwordbox
            _isPasswordChanging = false;
        }

        private void UpdatePassword()
        {
            if (!_isPasswordChanging)
                passwordBox.Password = Password;
        }
    }
}
