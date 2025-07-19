using RemoveComVK.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace RemoveComVK
{
    /// <summary>
    /// Логика взаимодействия для General.xaml
    /// </summary>
    public partial class General : Window
    {
        private Object[][] com2;
        private Object[][] search2;

        private Progress prog;


        private List<int> selCom = new List<int>();

        private int acceptCom = 0;

        private int fail_delete_com = 0;
        private int pass_delete_com = 0;


        public ObservableCollection<CommentsItem> comments = new ObservableCollection<CommentsItem>();
        public ObservableCollection<CommentsItem> comments_find = new ObservableCollection<CommentsItem>();



        public General(String filename)
        {
            InitializeComponent();
            PreInit(filename);
        }

        private Progress initProgBar(string a)
        {
            prog = new Progress();
            prog.operationName(a);
            prog.Show();
            ComScreen.IsEnabled = false;
            ButtonScreen.IsEnabled = false;

            return prog;
        }


        private async void PreInit(string filename)
        {
            Comments.Items.Clear();
            Comments.ItemsSource = comments;

            Progress pr = initProgBar("Загрузка комментариев...");

            var progress = new System.Progress<int>(value =>
            {
                pr.setProg(value);
            });

            object fun = await Task.Run(() => init(filename, progress));

            pr.isAllowClose = true;

            if (fun.GetType() == typeof(bool))
            {
                pr.Close();
                new Login().Show();
                Close();

                return;
            }
            else
            {
                ComScreen.IsEnabled = true;
                ButtonScreen.IsEnabled = true;
            }
            object[] res = (object[])fun;

            Owner.Content = (String)res[0];
            DateRegistration.Content = (string)res[1];
            comCount.Content = "Найдено всего комментариев: \n" + acceptCom;

            object avatar = API.getAvatar();

            if (avatar.GetType() != typeof(bool))
            {
                ImageBrush img = new ImageBrush();
                img.Stretch = Stretch.Fill;
                img.ImageSource = (ImageSource)avatar;
                Avatar.Fill = img;
            }

            pr.Close();
            Show();

        }

        private async Task<dynamic> init(string filename, IProgress<int> p)
        {
            object[] result = new object[2];

            Comments com = new Comments();

            p.Report(10);

            object get = await Task.Run(() => com.getAllComments(filename));
            if (get.GetType() == typeof(bool))
            {
                if (!(bool)get)
                {
                    Application.Current.Dispatcher.Invoke(() => new Notif("Ошибка парсинга комментариев."));
                    return false;

                }
            }
            p.Report(20);

            await Task.Run(() => eatComments((object[][])get));
            p.Report(30);
            var fn = API.getProfileInfo();
            p.Report(40);
            if (fn != null)
            {
                result[0] = fn[0] + " " + fn[1];
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => new Notif("Ошибка получения информации об аккаунте."));
                return false;
            }
            p.Report(50);

            p.Report(90);
            result[1] = "Дата регистрации: " + await API.getDate();
            p.Report(100);

            return result;
        }

        public async void eatComments(Object[][] obj)
        {
            com2 = obj;
            for (int i = 0; i < com2.Length; i++)
            {
                if (com2[i] != null)
                {
                    acceptCom++;
                }
            }

            //await Dispatcher.InvokeAsync(()=> comCount.Content = "Найдено всего комментариев: \n" + acceptCom);

            /*string[] urls = {  };

            string[][] test_com = new string[3][];
            for (int i = 0; i < test_com.Length; i++)
            {
                test_com[i] = new string[3];
                test_com[i][0] = "test1 + " + i;
                test_com[i][1] = "test2 + " + i;
                test_com[i][2] = urls[i];
            }

            for (int i = 0; i < 3; i++)
            {
                await Dispatcher.InvokeAsync(() => comments.Add(new CommentsItem(i, (string)test_com[i][0], (string)test_com[i][1], (string)test_com[i][2])));
            }
            return;*/

            for (int i = 0; i < acceptCom; i++)
            {
                await Dispatcher.InvokeAsync(() => comments.Add(new CommentsItem(i, (string)com2[i][0], (string)com2[i][1], (string)com2[i][2])));
            }

        }

        private dynamic parseSrc(string src)
        {
            if (!string.IsNullOrEmpty(src))
            {
                string[] parsed = src.Split('/')[3].Split("?&=_".ToCharArray());

                string wall = parsed[0].Replace("wall", "");
                string reply = parsed[3];

                Console.WriteLine(parsed);
                foreach (var p in parsed)
                {
                    Console.WriteLine(p);
                }

                return new String[] { wall, reply };

            }
            return false;
        }

        private async Task<dynamic> DeleteSelCom(IProgress<int> p)
        {
            fail_delete_com = 0;
            pass_delete_com = 0;
            int selcom = selCom.ToArray().Length;
            int inum = 0;

            foreach (var i in selCom.ToArray())
            {

                object src = parseSrc(comments[i].Link);

                String[] src_arr = (String[])src;
                bool deleteCom = await API.sendDeleteComm(src_arr[0], src_arr[1]);

                if (deleteCom)
                {
                    await Dispatcher.InvokeAsync(() => comments.Remove(comments.Where(a => a.ID == i).Single()));
                    selCom.Remove(i);
                    pass_delete_com++;
                    await Task.Run(() => Thread.Sleep(2000));
                }
                else
                {
                    fail_delete_com++;

                }

                inum++;
                Console.WriteLine("progress: " + (inum * 100) / selcom);
                p.Report((inum * 100) / selcom);
            }
            Console.WriteLine("one: " + Comments.ItemsSource.GetHashCode() + " two: " + comments_find.GetHashCode());

            if (Comments.ItemsSource.GetHashCode() == comments_find.GetHashCode())
            {
                await Dispatcher.InvokeAsync(() => Button_Find(new object(), new RoutedEventArgs()));
            }

            return new object[] { pass_delete_com, selcom };
        }

        private async Task<dynamic> DeleteAll(IProgress<int> p)
        {

            if (Comments.ItemsSource.GetHashCode() == comments_find.GetHashCode())
            {
                fail_delete_com = 0;
                pass_delete_com = 0;
                int findcom = comments_find.ToArray().Length;
                int fnum = 0;

                foreach (var i in comments_find.ToArray())
                {

                    object src = parseSrc(i.Link);
                    if (src.GetType() == typeof(bool))
                    {

                        if (!(bool)src)
                        {
                            new Notif("Ошибка в точке parseSrc");
                            return false;
                        }

                    }
                    String[] src_arr = (String[])src;
                    bool deleteCom = await API.sendDeleteComm(src_arr[0], src_arr[1]);

                    if (deleteCom)
                    {
                        await Dispatcher.InvokeAsync(() => comments.Remove(i));
                        pass_delete_com++;
                        await Task.Run(() => Thread.Sleep(2000));
                    }
                    else
                    {
                        fail_delete_com++;

                    }

                    fnum++;
                    Console.WriteLine("progress: " + (fnum * 100) / findcom);
                    p.Report((fnum * 100) / findcom);
                }
                Console.WriteLine("one: " + Comments.ItemsSource.GetHashCode() + " two: " + comments_find.GetHashCode());

                await Dispatcher.InvokeAsync(() => Button_Find(new object(), new RoutedEventArgs()));
                return new object[] { pass_delete_com, findcom };

            }
            fail_delete_com = 0;
            pass_delete_com = 0;
            int selcom = comments.ToArray().Length;
            int inum = 0;

            foreach (var i in comments.ToArray())
            {

                object src = parseSrc(i.Link);
                if (src.GetType() == typeof(bool))
                {

                    if (!(bool)src)
                    {
                        new Notif("Ошибка в точке parseSrc");
                        return false;
                    }

                }
                String[] src_arr = (String[])src;
                bool deleteCom = await API.sendDeleteComm(src_arr[0], src_arr[1]);

                if (deleteCom)
                {
                    await Dispatcher.InvokeAsync(() => comments.Remove(i));
                    pass_delete_com++;
                    await Task.Run(() => Thread.Sleep(2000));
                }
                else
                {
                    fail_delete_com++;

                }

                inum++;
                Console.WriteLine("progress: " + (inum * 100) / selcom);
                p.Report((inum * 100) / selcom);
            }
            Console.WriteLine("one: " + Comments.ItemsSource.GetHashCode() + " two: " + comments_find.GetHashCode());

            return new object[] { pass_delete_com, selcom };
        }

        private void ClearBtn(object sender, RoutedEventArgs e)
        {
            Search("");
            FindBox.Clear();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            Progress pr = initProgBar("Удаление комментариев...");

            var progress = new System.Progress<int>(value =>
            {
                pr.setProg(value);
            });

            

            object[] res = await Task.Run(() => DeleteSelCom(progress));
            pr.isAllowClose = true;
            pr.Close();
            ComScreen.IsEnabled = true;
            ButtonScreen.IsEnabled = true;
            new Notif("Удалено " + res[0] + "/" + res[1] + " комментариев");
        }

        private async void Button_ClickAll(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult box = System.Windows.Forms.MessageBox.Show("Вы действительно хотите удалить ВСЕ НАЙДЕННЫЕ комментарии? ЭТО НЕОБРАТИМО!", "Уведомление", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (box == System.Windows.Forms.DialogResult.No) return;
            Progress pr = initProgBar("Удаление комментариев...");
            
            var progress = new System.Progress<int>(value =>
            {
                pr.setProg(value);
            });

            object[] res = await Task.Run(() => DeleteAll(progress));
            pr.isAllowClose = true;
            pr.Close();
            ComScreen.IsEnabled = true;
            ButtonScreen.IsEnabled = true;
            new Notif("Удалено " + res[0] + "/" + res[1] + " комментариев");
        }

        private void Button_Find(object sender, RoutedEventArgs e)
        {
            Search(FindBox.Text);
        }

        private void openLink(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            String tag = (String)bt.Tag;
            Process.Start(new ProcessStartInfo
            {
                FileName = tag,
                UseShellExecute = true
            });

        }

        private void Check(object sender, RoutedEventArgs e)
        {

            var send = (CheckBox)sender;

            for (int i = 0; i < comments.Count; i++)
            {
                if (comments[i].ID == (int)send.Tag)
                {
                    selCom.Add(i);
                    return;
                }
            }
        }

        private void UnCheck(object sender, RoutedEventArgs e)
        {
            var send = (CheckBox)sender;

            for (int i = 0; i < comments.Count; i++)
            {
                if (comments[i].ID == (int)send.Tag)
                {
                    selCom.Remove(i);
                    return;
                }
            }

        }

        private void Search(string a)
        {
            selCom.Clear();
            if (!string.IsNullOrEmpty(a))
            {
                comments_find.Clear();

                foreach (var i in comments.Where(i => i.Comments.LastIndexOf(a) != -1).ToArray())
                {
                    comments_find.Add(i);
                }

                Comments.ItemsSource = comments_find;
            }
            else
            {
                comments_find.Clear();
                Comments.ItemsSource = comments;
            }
        }

        private void FindBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search(FindBox.Text);
            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.Help();
        }

        private void CopyTokenBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(API.getToken());
        }
    }
}
