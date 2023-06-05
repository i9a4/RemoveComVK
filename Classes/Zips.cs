using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoveComVK;
using System.Windows;

namespace RemoveComVK.Classes
{
    public class Zips
    {
        private static String path_to_dir;

        public Zips()
        {
        }

        private static String genWord(int a)
        {
            Random rd = new Random();
            List<char> words = new List<char> { 'q', 'w', 'e', 'r', 't', 'y' };
            char[] ch = new char[a];
            for (int i = 0; i < a; i++)
            {
                ch[i] = words[rd.Next(words.Count)];
            }

            return new String(ch).ToUpper() + "_tested";
        }
        public static dynamic unZip(string a)
        {
            String[] Files;
            try
            {
                FastZip zip = new FastZip();
                String path = Path.GetTempPath() + genWord(10);
                path_to_dir = path;
                zip.ExtractZip(a, path, null);

                String path_to = path + "\\comments";

                Files = Directory.GetFiles(path_to);
            }
            catch (Exception e)
            {
                clearZip();
                Task.Run(() => Application.Current.Dispatcher.Invoke(() => new Notif(e.Message)));
                return false;
            }

            return Files;
        }

        public static void clearZip()
        {
            try
            {
                Directory.Delete(path_to_dir, true);
            }
            catch { Console.WriteLine("[ZIP] Not found"); }
        }
    }
}
