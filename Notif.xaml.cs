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

namespace RemoveComVK
{
    /// <summary>
    /// Логика взаимодействия для Notif.xaml
    /// </summary>
    public partial class Notif : Window
    {
        public Notif(String notif)
        {
            InitializeComponent();
            init(notif);
        }

        private void init(String a)
        {
            Text.Text = a;
            ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
