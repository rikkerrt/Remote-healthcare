using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DoctorClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        
       Connection connection = new Connection();
       MainWindow MainWindow;

        public LoginWindow()
        {
            InitializeComponent();

            try
            {

                connection.Connect();
                connection.Write("d");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error..... " + ex.StackTrace);
            }
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredPassword = passwordBox.Password;

            

            if (CheckPassword(enteredPassword))
            {
                MainWindow = new MainWindow(connection);
                MainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Ongeldig wachtwoord");
            }
        }

        private Boolean CheckPassword(string password)
        {
            //connection.Write("WW|" + password);

            ////string input = connection.CheckPassword().ToString();
            ////connection.Write(input);


            //if (connection.CheckPassword())
            //{
            //    return true;
            //}

            //else return false;
            return true;
        }


    }
}
