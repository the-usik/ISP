using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP.Database.Models {
    public abstract class DatabaseFactory<T> {
        public abstract T Create(SqliteDataReader reader);
    }

}
