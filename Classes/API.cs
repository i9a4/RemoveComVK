using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Project_one_series_Zero_Rewrite.Classes
{
    public class API
    {
        private static string lastError;

        public static string LastError
        {
            set => lastError = value;
            get => lastError;
        }

        private static string api;

        private static string Api
        {
            get => api;
            set => api = value;
        }

        private static JObject Json;

        public static bool ReturnEventHandler(JObject json)
        {
            if (json == null) return false;
            else if (json["error"] != null)
            {
                if ((int)json["error"]["error_code"] == 211)
                {
                    return true;
                }
                LastError = "CODE [" + (string)json["error"]["error_code"] + "] " + (string)json["error"]["error_msg"];
                return false;
            }
            else
            {
                return true;
            }
        }

        async public static Task<bool> setKey(string key)
        {
            if (key == null) return false;
            if (key.Length == 0) { lastError = "Введите токен."; return false; }

            HttpClient httpClient = new HttpClient();
            var opt = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("access_token", key),
                new KeyValuePair<string, string>("v", "5.131")
            });

            var resp = await httpClient.PostAsync("https://api.vk.com/method/account.getAppPermissions", opt);
            string result = await resp.Content.ReadAsStringAsync();

            JObject json = JObject.Parse(result);

            if (!ReturnEventHandler(json))
            {
                return false;
            }
            else if ((int)json["response"] < 8192)
            {
                LastError = "Введенный токен, не имеет прав на работу с нужными методами. Авторизуйтесь через VK или введите токен с нужными правами.";
                return false;
            }
            else
            {
                Api = key;
                return true;
            }
        }

        async public static Task<bool> initProfile()
        {
            HttpClient httpClient = new HttpClient();
            var opt = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("access_token", Api),
                new KeyValuePair<string, string>("v", "5.131")
            });

            var resp = await httpClient.PostAsync("https://api.vk.com/method/account.getProfileInfo", opt);
            string result = await resp.Content.ReadAsStringAsync();


            JObject json = JObject.Parse(result);

            if (!ReturnEventHandler(json)) return false;
            else
            {
                Json = json;
                return true;
            }
        }

        public static dynamic getProfileInfo()
        {
            JObject json = Json;

            if (!ReturnEventHandler(json)) return false;
            else
            {

                return new[] { (string)json["response"]["first_name"], (string)json["response"]["last_name"] };
            }
        }

        async public static Task<bool> sendDeleteComm(string owner_id, string comment_id)
        {
            HttpClient httpClient = new HttpClient();
            var opt = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("access_token", Api),
                new KeyValuePair<string, string>("v", "5.131"),
                new KeyValuePair<string, string>("owner_id", owner_id),
                new KeyValuePair<string, string>("comment_id", comment_id)
            });

            var resp = await httpClient.PostAsync("https://api.vk.com/method/wall.deleteComment", opt);
            string result = await resp.Content.ReadAsStringAsync();

            JObject json = JObject.Parse(result);

            if (!ReturnEventHandler(json)) return false;
            else
            {
                /*if ((int)json["response"] == 1) return true;
                else return false;*/
                return true;
            }

        }

        public static dynamic getAvatar()
        {
            JObject json = Json;

            if (!ReturnEventHandler(json)) return false;
            else
            {
                return new BitmapImage(new Uri((string)json["response"]["photo_200"]));
            }
        }

        async public static Task<dynamic> getDate()
        {
            JObject json = Json;

            string scr = "https://vk.com/foaf.php?id=" + json["response"]["id"].ToString();
            HttpClient httpClient = new HttpClient();
            var resp = await httpClient.GetAsync(scr);
            string result = await resp.Content.ReadAsStringAsync();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(result);

            XmlNode node = xml.DocumentElement;
            string list = node.FirstChild.ChildNodes[8].Attributes[0].InnerText;
            DateTime dt = DateTime.Parse(list);

            return dt.Date.ToShortDateString();
        }

        public static dynamic getID()
        {
            JObject json = Json;

            if (!ReturnEventHandler(json)) return false;
            else
            {
                return (String)json["response"]["id"].ToString();
            }
        }
    }
}
