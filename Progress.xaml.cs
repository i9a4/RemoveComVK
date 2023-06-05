using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RemoveComVK
{
    /// <summary>
    /// Логика взаимодействия для Progress.xaml
    /// </summary>
    public partial class Progress : Window
    {
        private bool IsAllowClose = false;

        public bool isAllowClose
        {
            set => IsAllowClose = value;
            get => IsAllowClose;
        }
        public Progress()
        {

            InitializeComponent();
            Bar.Value = 0;
            this.Closing += CloseUser;

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

        private void CloseUser(object sender, CancelEventArgs e)
        {
            if (!IsAllowClose)
            {
                e.Cancel = true;
            }

        }
    }
}
