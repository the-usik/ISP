using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Database.Models {
    public class ActivtyTypeModel : DatabaseFactory<ActivtyTypeModel> {
        public int Id { get; set; }
        public string Title { get; set; }

        public override ActivtyTypeModel Create(SqliteDataReader reader) {
            return new ActivtyTypeModel() {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1)
            };
        }
    }
}
