using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Database.Models {
    public class ServiceModel : DatabaseFactory<ServiceModel> {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }

        public override ServiceModel Create(SqliteDataReader reader) {
            return new ServiceModel() {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Price = reader.GetDecimal(2)
            };
        }
    }
}
