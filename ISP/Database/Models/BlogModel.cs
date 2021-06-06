using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Database.Models {
    public class BlogModel : DatabaseFactory<BlogModel> {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
        public int Likes { get; set; }

        public override BlogModel Create(SqliteDataReader reader) {
            return new BlogModel() {
                Id = reader.GetInt32(0),
                AdminId = reader.GetInt32(1),
                Title = reader.GetString(2),
                Content = reader.GetString(3),
                PublishDate = reader.GetDateTime(4),
                Likes = reader.GetInt32(5)
            };
        }
    }
}
