using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Database.Models {
    public class HistoryModel : DatabaseFactory<HistoryModel> {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public DateTime ActivityDate { get; set; }
        public string ActivtyDescription { get; set; }

        public override HistoryModel Create(SqliteDataReader reader) {
            return new HistoryModel() {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                ActivityId = reader.GetInt32(2),
                ActivityDate = reader.GetDateTime(3),
                ActivtyDescription = reader.GetString(4),
            };
        }
    }


}
