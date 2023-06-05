using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveComVK.Classes
{
    public class CommentsItem
    {
        private int id;
        public int ID
        {
            get => id;
            set => id = value;
        }
        private string date;
        public string Date
        {
            get => date;
            set => date = value;
        }

        private string comments;
        public string Comments
        {
            get => comments;
            set => comments = value;
        }

        private string link;
        public string Link
        {
            get => link;
            set => link = value;
        }
        public CommentsItem(int id, string Date, string Comments, string Link)
        {
            this.Date = Date;
            this.Comments = Comments;
            this.Link = Link;
            this.ID = id;


        }
    }
}
