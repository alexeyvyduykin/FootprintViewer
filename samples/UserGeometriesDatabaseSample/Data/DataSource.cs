using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGeometriesDatabaseSample.Data
{
    public class DataSource
    {
        private readonly CustomDbContext _db;

        public DataSource(CustomDbContext db)
        {
            _db = db;
        }

        public IEnumerable<UserGeometry> UserGeometries => _db.UserGeometries;
    }
}
