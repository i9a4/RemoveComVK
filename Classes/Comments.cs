using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace RemoveComVK.Classes
{
    public class Comments
    {


        /*           {"01 марта 2023", "Комментарий", "https://free.navalny.com" },
                     {"02 марта 2023", "Комментарий 2", "https://google.com" },
                     {"03 марта 2023", "Комментарий 3", "https://google.com" },
                     {"04 марта 2023", "Комментарий 4", "https://google.com" },
                     {"05 марта 2023", "Комментарий 5", "https://google.com" }*/

        private Object[][] comments2;

        public dynamic getAllComments(String FileName)
        {
            return parseHTML(FileName);
        }
        public Comments()
        {
        }

        private dynamic parseHTML(String FileName)
        {
            String[] files = { };

            object prefiles = Zips.unZip(FileName);
            if (prefiles.GetType() == typeof(bool))
            {
                if ((bool)prefiles == false)
                {
                    return false;
                }
            }
            else
            {
                files = (String[])prefiles;
            }

            comments2 = new object[files.Length * 100][];
            int fill = 0;

            HtmlDocument doc = new HtmlDocument();
            for (int i = 0; i < files.Length; i++)
            {
                byte[] bytes = File.ReadAllBytes(files[i]);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding encoder = Encoding.GetEncoding("windows-1251");
                string html = encoder.GetString(bytes, 0, bytes.Length);
                doc.LoadHtml(html);
                HtmlNodeCollection xPatch1 = doc.DocumentNode.SelectNodes("//div[@class='item']/div[3]");
                HtmlNodeCollection xPatch2 = doc.DocumentNode.SelectNodes("//div[@class='item']/div[2]");
                HtmlNodeCollection xPatch3 = doc.DocumentNode.SelectNodes("//div[@class='item']/div[3]");



                for (int a = 0; a < xPatch1.Count; a++)
                {
                    comments2[fill] = new object[3];
                    comments2[fill][0] = xPatch3[a].NextSibling.InnerText;
                    comments2[fill][1] = xPatch2[a].PreviousSibling.InnerText;
                    comments2[fill][2] = xPatch1[a].PreviousSibling.PreviousSibling.InnerText;
                    fill++;
                }


            }

            Zips.clearZip();


            return comments2;


        }
    }
}
