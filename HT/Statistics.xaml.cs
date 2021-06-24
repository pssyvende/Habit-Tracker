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
using System.Data;
using System.Data.SqlClient;
using HTDB;

namespace HT
{
    public partial class Statistics : UserControl
    {
        /* AccessToDatabase */
        DBAccess objDbAccess = new DBAccess();
        DataTable dataTable = new DataTable();

        /* DependencyObjects for Events */
        DependencyObject clickSource = null;
        DependencyObject lastClickSource = null;
        Calendar cal = new Calendar();
        static int count = 0;



        public Statistics()
        {
            InitializeComponent();
            cal.Visibility = Visibility.Hidden;
            calendar.Children.Add(cal);

            string query = "select Habits2.Name, Habits2.Id from Habits2 order by Habits2.Id asc";
            objDbAccess.readDatathroughAdapter(query, dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                Button btn = new Button();
                btn.Content = row["Name"].ToString();
                btn.Uid = row["Id"].ToString();
                btn.IsEnabled = true;
                btn.Click += new RoutedEventHandler(SelectHabit);
                Habits.Children.Add(btn);
            }

        }

        private void SelectHabit(object sender, RoutedEventArgs e)
        {
            if(count>0)
                lastClickSource.SetValue(IsEnabledProperty, true);
            clickSource = e.Source as DependencyObject;
            clickSource.SetValue(IsEnabledProperty, false);
            string id = clickSource.GetValue(Button.UidProperty).ToString();
            //clickSource.SetValue(BackgroundProperty, new SolidColorBrush(Color.FromRgb(255,255,255)));
            calendar.Children.RemoveAt(0);
            Calendar calen = new Calendar();
            calen.VerticalAlignment = VerticalAlignment.Center;
            calen.HorizontalAlignment = HorizontalAlignment.Center;

            DataTable dataTable2 = new DataTable();
            string query2 = "select Date from Datelog2 where HabitId = " + id + " and IfChecked = 1";
            objDbAccess.readDatathroughAdapter(query2, dataTable2);
            foreach (DataRow row in dataTable2.Rows)
            {
                string data = row.ItemArray[0].ToString();
                string[] splitedData = data.Split('.');
                string[] yearAndTheRest = splitedData[2].Split(' ');
                int[] date = { int.Parse(splitedData[0]), int.Parse(splitedData[1]), int.Parse(yearAndTheRest[0]) };

                calen.BlackoutDates.Add(new CalendarDateRange(new DateTime(date[2], date[1], date[0]), new DateTime(date[2], date[1], date[0])));
            }
            calendar.Children.Add(calen);
            lastClickSource = clickSource;
            count++;
        }
    }
}
