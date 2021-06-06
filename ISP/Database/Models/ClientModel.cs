using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Database.Models {
    public class ClientModel : DatabaseFactory<ClientModel> {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RateId { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public decimal LastPaymentAmount { get; set; }
        public decimal TotalPaymentAmount { get; set; }

        public override ClientModel Create(SqliteDataReader reader) {
            return new ClientModel() {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                RateId = reader.GetInt32(2),
                LastPaymentDate = reader.GetDateTime(3),
                LastPaymentAmount = reader.GetDecimal(4),
                TotalPaymentAmount = reader.GetDecimal(5)
            };
        }
    }
}
