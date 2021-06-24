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
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using HTDB;

namespace HT
{
    public partial class Form : Window
    {
        /* AccessToDatabase */
        DBAccess objDbAccess = new DBAccess();

        public Form()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.IsEnabled = true;
            Application.Current.MainWindow.Opacity = 1;
            this.Close();
        }

        private void SendNewHabitToDB(object sender, RoutedEventArgs e)
        {
            string newHabit = habitBox.Text;
            int newFrequency = (int)timesAWeek.Value;
            SqlCommand insertCommand = new SqlCommand("Insert into Habits2 (Name, TimesAWeek) values (@newHabit, @newFrequency)");
            insertCommand.Parameters.AddWithValue("@newHabit", newHabit);
            insertCommand.Parameters.AddWithValue("@newFrequency", newFrequency);
            objDbAccess.executeQuery(insertCommand);
            //close
            Application.Current.MainWindow.IsEnabled = true;
            MyFlags.IsUpdatedForHabits = true;
            MyFlags.IsUpdatedForStatistic = true;
            Application.Current.MainWindow.Opacity = 1;
            this.Close();
        }
    }
}
