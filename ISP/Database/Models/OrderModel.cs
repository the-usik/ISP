using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Database.Models {
    public class OrderModel : DatabaseFactory<OrderModel> {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsDone { get; set; }
        public DateTime CompletionDate { get; set; }

        public override OrderModel Create(SqliteDataReader reader) {
            return new OrderModel() {
                Id = reader.GetInt32(0),
                ServiceId = reader.GetInt32(1),
                OrderDate = reader.GetDateTime(2),
                IsDone = reader.GetBoolean(3),
                CompletionDate = reader.GetDateTime(4)
            };
        }
    }
}
