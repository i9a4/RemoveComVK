using Microsoft.Win32;
using Project_one_series_Zero_Rewrite.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_one_series_Zero_Rewrite
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Auth auth = new Auth();
            auth.ShowDialog();
            Enter(auth.Token);
        }

        private void BtnToken(object sender, RoutedEventArgs e)
        {
            String token = TokenBox.Text;
            Enter(token);
        }

        private async void Enter(string token)
        {
            if (token == null) return;

            Window.IsEnabled = false;
            if (!await API.setKey(token))
            {
                new Notif(API.LastError);
                Window.IsEnabled = true;
                return;
            }
            else
            {
                if (!await API.initProfile())
                {
                    new Notif("Ошибка инициализации профиля"); Window.IsEnabled = true; return;
                }

            }

            Popup pop = new Popup();
            pop.ShowDialog();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Архив (*.zip)|*.zip";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Title = "Выберите архив выгруженный с VK";
            dialog.Multiselect = false;

            dialog.ShowDialog();

            String patch = dialog.FileName;
            if (patch == "") { Window.IsEnabled = true; return; }

            new General(patch);
            Close();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Разработчик/Developer: Maxim D.\nЦените свободу своего слова.", "О программе", System.Windows.Forms.MessageBoxButtons.OK);
        }
    }
}
