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
    /// Логика взаимодействия для Progress.xaml
    /// </summary>
    public partial class Progress : Window
    {
        public Progress()
        {

            InitializeComponent();
            Bar.Value = 0;

        }

        async public void setProg(int i)
        {
            await Task.Run(() => {
                Dispatcher.InvokeAsync(() => Bar.Value = i);
            });
        }

        public void operationName(String a)
        {
            NameOperation.Content = a;
        }
    }
}
