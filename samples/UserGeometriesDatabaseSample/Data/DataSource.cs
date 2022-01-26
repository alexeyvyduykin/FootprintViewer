using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace UserGeometriesDatabaseSample.Data
{
    public class DataSource : ReactiveObject, IDataSource
    {
        private readonly CustomDbContext _context;

        public DataSource(CustomDbContext db)
        {
            _context = db;

            Update = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> Update { get; }

        //public void Add(UserGeometry geometry)
        //{
        //    _context.UserGeometries.Add(geometry);
        //    _context.SaveChanges();

        //    Update.Execute().Subscribe();
        //}

        public async Task AddAsync(UserGeometry geometry)
        {
            await Task.Delay(3000);

            await _context.UserGeometries.AddAsync(geometry);
            await _context.SaveChangesAsync();

            Update.Execute().Subscribe();
        }

        public void Remove(UserGeometry geometry)
        {
            _context.UserGeometries.Remove(geometry);
            _context.SaveChanges();

            Update.Execute().Subscribe();
        }

        public async Task<List<UserGeometry>> LoadUsersAsync()
        {
            await Task.Delay(2000);

            return await Task.Run(() => _context.UserGeometries.ToList());
        }
    }
}
