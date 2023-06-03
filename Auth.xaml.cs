using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        private string token;

        public string Token
        {
            get => token;
        }

        

        private string ParseURL(string u)
        {
            string url = u.Remove(0, 1);

            url = url.Split('&')[0];
            url = url.Split('=')[1];

            return url;
        }

        public Auth()
        {
            InitializeComponent();

            
            WebView.Source = new Uri("https://oauth.vk.com/oauth/authorize?client_id=6121396&redirect_uri=https://oauth.vk.com/blank.html&scope=8192&display=page&response_type=token&revoke=1");
            WebView.CoreWebView2InitializationCompleted += (object sender, CoreWebView2InitializationCompletedEventArgs e) =>
            {
                WebView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
                WebView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
                WebView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                WebView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            };
            this.Closed += (async (object sende, EventArgs e) => {WebView.CoreWebView2.Stop(); string path = WebView.CoreWebView2.Profile.ProfilePath; var proc = WebView.CoreWebView2.BrowserProcessId; var procid = System.Convert.ToInt32(proc); System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(procid); process.Kill(); while(true){ if (process.HasExited) { try { await Task.Run(() => Thread.Sleep(500)); Directory.Delete(path.Replace("\\EBWebView\\Default", ""), true); break; } catch(Exception err) { Console.WriteLine(err.Message); } } }; Console.WriteLine("Close"); });
            
        }

        private void WebView_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            Uri url = new Uri(WebView.Source.AbsoluteUri);

            if (url.AbsolutePath == "/blank.html")
            {
                token = ParseURL(url.Fragment);
                WebView.Stop();
                Close();
            }
        }
    }
}
