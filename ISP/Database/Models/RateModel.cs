using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Database.Models {
    public class RateModel : DatabaseFactory<RateModel> {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Fee { get; set; }
        public int Speed { get; set; }

        public override RateModel Create(SqliteDataReader reader) {
            return new RateModel() {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Fee = reader.GetDecimal(2),
                Speed = reader.GetInt32(3)
            };
        }
    }
}
