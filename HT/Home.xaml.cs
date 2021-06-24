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

namespace HT
{
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            stripes.Opacity = 1;
            quote.Visibility = Visibility.Visible;
            addHabit.Visibility = Visibility.Visible;
        }


        private void Click_AddHabit(object sender, RoutedEventArgs e)
        {
            Form form = new Form();
            form.Show();
            form.WindowStyle = WindowStyle.None;
            form.ResizeMode = ResizeMode.NoResize;
            Application.Current.MainWindow.Opacity = 0.5;
            Application.Current.MainWindow.IsEnabled = false;
        }

    }
}
