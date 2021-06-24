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
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using DesignInControl;
using System.Data;
using System.Data.SqlClient;
using HTDB;

namespace HT
{
    public partial class Habits : UserControl
    {
        /* AccessToDatabase */
        DBAccess objDbAccess = new DBAccess();
        DataTable dataTable = new DataTable();

        /* Animation */
        Storyboard storyboard = new Storyboard();
        DoubleAnimation doubleAnimation = new DoubleAnimation();

        /* DependencyObjects for Events */
        DependencyObject clickSource = null;
        DependencyObject parent;
        DependencyObject ellipse;
        DependencyObject arc;
        DependencyObject label;
        DependencyObject checkHabit;
        DependencyObject parentSibling;
        DependencyObject grandParent;

        public Habits()
        {
            InitializeComponent();
            ReloadHabits();
        }

        private void ReloadHabits()
        {
            string query = "select Habits2.Name, Habits2.Id, Habits2.ActualPercentage, ProgressCalculation.TimesAWeek, Datelog2.Date, ProgressCalculation.PercentageValue, Datelog2.IfChecked from Habits2 inner join ProgressCalculation on Habits2.TimesAWeek = ProgressCalculation.TimesAWeek inner join Datelog2 on Habits2.Id = Datelog2.HabitId where CONVERT(VARCHAR(10), Datelog2.Date, 111) = CONVERT(VARCHAR(10), getdate(), 111) order by Habits2.Id asc";
            objDbAccess.readDatathroughAdapter(query, dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                // background epllise
                Ellipse ell = new Ellipse();
                ell.Width = 63;
                ell.Height = 63;
                ell.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                ell.Opacity = 0.5;
                    //ell.Visibility = Visibility.Hidden;

                // background progressbar
                CircularProgressBar bgpb = new CircularProgressBar();
                bgpb.HorizontalAlignment = HorizontalAlignment.Center;
                bgpb.VerticalAlignment = VerticalAlignment.Center;
                bgpb.SegmentColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                bgpb.Opacity = 0.1;

                // check progressbar
                CircularProgressBar cpb = new CircularProgressBar();
                cpb.HorizontalAlignment = HorizontalAlignment.Center;
                cpb.VerticalAlignment = VerticalAlignment.Center;
                cpb.SegmentColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                cpb.StrokeThickness = 1;
                cpb.Radius = 42;
                cpb.Percentage = 0;    //event

                // main progressbar
                CircularProgressBar mpb = new CircularProgressBar();
                mpb.HorizontalAlignment = HorizontalAlignment.Center;
                mpb.VerticalAlignment = VerticalAlignment.Center;
                mpb.SegmentColor = new SolidColorBrush(Color.FromRgb(61, 221, 194));
                mpb.Opacity = 0.4;
                mpb.Percentage = Math.Round((double)row["ActualPercentage"]);    //db

                // percent label
                Label lab = new Label();
                Grid.SetRow(lab, 1);
                lab.Content = mpb.Percentage + "%";
                lab.FontSize = 15;
                    //lab.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    //lab.Opacity = 0.3;
                lab.HorizontalAlignment = HorizontalAlignment.Center;
                lab.VerticalAlignment = VerticalAlignment.Center;

                // percentage grid
                Grid ng = new Grid();
                ng.Uid = row["Id"].ToString();
                ng.Height = 140;
                ng.Width = 100;
                ng.Margin = new Thickness(25);
                ng.HorizontalAlignment = HorizontalAlignment.Left;
                ng.Background = new SolidColorBrush(Color.FromRgb(16, 15, 45));
                RowDefinition rd1 = new RowDefinition();
                rd1.Height = new GridLength(5, GridUnitType.Star);
                RowDefinition rd2 = new RowDefinition();
                rd2.Height = new GridLength(2, GridUnitType.Star);
                ng.RowDefinitions.Add(rd1);
                ng.RowDefinitions.Add(rd2);
                Grid g1 = new Grid();
                Grid.SetRow(g1, 0);
                g1.PreviewMouseDown += new MouseButtonEventHandler(gridArc_PreviewMouseDown);
                    //g1.PreviewMouseUp += new MouseButtonEventHandler(gridArc_PreviewMouseUp);

                if (row["IfChecked"].ToString() == "True")
                {
                    ell.Visibility = Visibility.Visible;
                    lab.Foreground = new SolidColorBrush(Color.FromRgb(56, 54, 96));
                }
                else
                {
                    ell.Visibility = Visibility.Hidden;
                    lab.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    lab.Opacity = 0.3;
                    g1.Cursor = Cursors.Hand;
                }

                g1.Children.Add(ell);
                g1.Children.Add(bgpb);
                g1.Children.Add(cpb);
                g1.Children.Add(mpb);
                g1.Children.Add(lab);

                // habit-name textblock
                TextBlock txtblck = new TextBlock();
                Grid.SetRow(txtblck, 1);
                txtblck.TextWrapping = TextWrapping.Wrap;
                txtblck.TextAlignment = TextAlignment.Center;
                txtblck.HorizontalAlignment = HorizontalAlignment.Center;
                txtblck.VerticalAlignment = VerticalAlignment.Center;
                txtblck.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                txtblck.FontWeight = FontWeights.Light;
                txtblck.FontSize = 12;
                txtblck.FontFamily = new FontFamily("Fonts/#Dosis");
                txtblck.Cursor = Cursors.Help;
                txtblck.Text = row["Name"].ToString();  //db

                // upload elements
                ng.Children.Add(g1);
                ng.Children.Add(txtblck);
                SpaceForHabits.Children.Add(ng);
            }
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scroll.ScrollToHorizontalOffset((scroll.HorizontalOffset + -e.Delta*0.3));
        }



        private void gridArc_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            clickSource = e.Source as DependencyObject;

            parent = VisualTreeHelper.GetParent(clickSource);
            grandParent = VisualTreeHelper.GetParent(parent);
            parentSibling = VisualTreeHelper.GetChild(grandParent, 1);
            ellipse = VisualTreeHelper.GetChild(parent, 0);
            checkHabit = VisualTreeHelper.GetChild(parent, 2);
            arc = VisualTreeHelper.GetChild(parent, 3);
            label = VisualTreeHelper.GetChild(parent, 4);
            // disable if the habit is checked
            if (ellipse.GetValue(VisibilityProperty).ToString() == "Hidden")
            {
                doubleAnimation.From = 0;
                doubleAnimation.To = 100;
                doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
                storyboard.Children.Add(doubleAnimation);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(CircularProgressBar.PercentageProperty));
                Storyboard.SetTarget(doubleAnimation, checkHabit);
                storyboard.Begin(this, true);
            }     
        }

        private void gridArc_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (clickSource == null)
                return;
            if (checkHabit.GetValue(CircularProgressBar.PercentageProperty).ToString() == "100")
            {
                // update DB
                MyFlags.IsUpdatedForHabits = true;
                MyFlags.IsUpdatedForStatistic = true;
                string uid = grandParent.GetValue(Grid.UidProperty).ToString();
                DataRow[] dr = dataTable.Select("Id = " + uid);
                double oldValue = (double)dr[0].ItemArray[2];
                double progress = (double)dr[0].ItemArray[5];
                double newValue = oldValue + progress;
                UpdateHabit(uid, newValue);

                parent.SetValue(CursorProperty, Cursors.Arrow);
                ellipse.SetValue(VisibilityProperty, Visibility.Visible);
                arc.SetValue(CircularProgressBar.PercentageProperty,Math.Round(newValue));
                // onValueChange Event
                label.SetValue(Label.ContentProperty, String.Format("{0}%",
                    Math.Floor(((double)arc.GetValue(CircularProgressBar.PercentageProperty)))));
                label.SetValue(Label.OpacityProperty, 1.0);
                label.SetValue(Label.ForegroundProperty,
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#383660")));
            }
            storyboard.Stop(this);
        }

        private void SpaceForHabits_MouseLeave(object sender, MouseEventArgs e)
        {
            storyboard.Stop(this);
        }

        private void UpdateHabit(string id, double nv)
        {
            string value = nv.ToString().Replace(",",".");
            SqlCommand updateCommand = new SqlCommand("Update Habits2 SET ActualPercentage = '" + @value + "' where Id = '" + @id + "'");
            updateCommand.Parameters.AddWithValue("@id", id);
            updateCommand.Parameters.AddWithValue("@value", value);
            objDbAccess.executeQuery(updateCommand);
        }
    }
}