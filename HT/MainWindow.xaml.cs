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
using System.Windows.Media.Animation;
using System.Data;
using System.Data.SqlClient;
using HTDB;
using System.Globalization;

namespace HT
{
    public static class MyFlags
    {
        public static bool IsUpdatedForHabits = false;
        public static bool IsUpdatedForStatistic = false;
    }

    public partial class MainWindow : Window
    {
        /* AccessToDatabase */
        DBAccess objDbAccess = new DBAccess();
        DataTable dataTable = new DataTable();
        DataTable dataTable2 = new DataTable();

        void CheckTodayUpdate()
        {
            string query = "Select Today from TodayDate";
            string query2 = "Select Id, ActualPercentage from Habits2";
            objDbAccess.readDatathroughAdapter(query, dataTable);
            objDbAccess.readDatathroughAdapter(query2, dataTable2);

            string date = dataTable.Rows[0].ItemArray[0].ToString();
            string dateTime = DateTime.Today.ToString();
            if (date != dateTime)
            {
                // update date
                SqlCommand updateCommand = new SqlCommand("Update TodayDate SET Today = getdate() where Id =  1");
                objDbAccess.executeQuery(updateCommand);
                //insert datelog 
                foreach (DataRow row in dataTable2.Rows)
                {
                    int i = int.Parse(row.ItemArray[0].ToString());
                    float percent = float.Parse(row.ItemArray[1].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    //should be inserted
                    SqlCommand insertCommand = new SqlCommand("Insert into Datelog2 (HabitId, Percentage) values (@i, @percent)");
                    insertCommand.Parameters.AddWithValue("@i", i);
                    insertCommand.Parameters.AddWithValue("@percent", percent);
                    objDbAccess.executeQuery(insertCommand);
                }
            }
        }

        private Home newHome;
        private Habits newHabits;
        private Statistics newStatistics;

        public MainWindow()
        {
            CheckTodayUpdate();
            // load the controls only once
            newHome = new Home();
            newHabits = new Habits();
            newStatistics = new Statistics();

            InitializeComponent();
            mainControler.Content = newHome;
        }

        public void ToHabits()
        {
            mainControler.Content = new Habits();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Click_AddHabit(object sender, RoutedEventArgs e)
        {
            Form form = new Form();
            form.Show();
            Application.Current.MainWindow.Opacity = 0.5;
            Application.Current.MainWindow.IsEnabled = false;
        }
        private void closeMainWindow(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void homeMenu(object sender, RoutedEventArgs e)
        {
            menuOpatictyControlerDown();
            mainControler.Content = newHome;
            menuOpatictyControlerUp();
        }

        private void habitsMenu(object sender, RoutedEventArgs e)
        {
            menuOpatictyControlerDown();
            if (MyFlags.IsUpdatedForHabits)
            {
                newHabits = new Habits();
                MyFlags.IsUpdatedForHabits = false;
            }
            mainControler.Content = newHabits;
            menuOpatictyControlerUp();
        }

        private void statisticsMenu(object sender, RoutedEventArgs e)
        {
            menuOpatictyControlerDown();
            if (MyFlags.IsUpdatedForStatistic)
            {
                newStatistics = new Statistics();
                MyFlags.IsUpdatedForStatistic = false;
            }
            mainControler.Content = newStatistics;
            menuOpatictyControlerUp();
        }

        private void menuOpatictyControlerDown()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 1;
            doubleAnimation.To = 0;
            doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            storyboard.Children.Add(doubleAnimation);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(Grid.OpacityProperty));
            Storyboard.SetTargetName(doubleAnimation, mainControler.Name);
            storyboard.Begin(this);
        }
        private void menuOpatictyControlerUp()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            storyboard.Children.Add(doubleAnimation);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(Grid.OpacityProperty));
            Storyboard.SetTargetName(doubleAnimation, mainControler.Name);
            storyboard.Begin(this);
        }
    }
}
