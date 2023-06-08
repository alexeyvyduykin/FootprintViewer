using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FootprintViewer.Models;

public class DbFactory : IDbFactory
{
    public ISource CreateSource(DbKeys key, string connectionString, string tableName)
    {
        switch (key)
        {
            case DbKeys.UserGeometries:
                {
                    var creator = () => new UserGeometryDbContext(tableName, connectionString);

                    var editor = async (DbCustomContext context, string id, object newValue) =>
                    {
                        if (newValue is UserGeometry newUserGeometry)
                        {
                            var userGeometry = await ((UserGeometryDbContext)context).UserGeometries
                                .Where(b => b.Name == id)
                                .FirstOrDefaultAsync();

                            if (userGeometry != null)
                            {
                                userGeometry.Geometry = newUserGeometry.Geometry;

                                await context.SaveChangesAsync();
                            }
                        }
                    };

                    return new EditableDatabaseSource(creator, editor);
                }
            case DbKeys.PlannedSchedules:
                {
                    var creator = () => new PlannedScheduleDbContext(tableName, connectionString);
                    return new DatabaseSource(creator);
                }
            default:
                throw new Exception($"DBContext for key={key} not register.");
        };
    }

    public ISource CreateSource(DbKeys key, string jsonFilePath)
    {
        switch (key)
        {
            case DbKeys.PlannedSchedules:
                {
                    return new JsonSource(jsonFilePath, path => JsonHelpers.DeserializeFromFile<PlannedScheduleResult>(path)!);
                }
            default:
                throw new Exception($"DBContext for key={key} not register.");
        };
    }

    public DbCustomContext CreateContext(DbKeys key, string connectionString, string tableName)
    {
        switch (key)
        {
            case DbKeys.UserGeometries:
                {
                    return new UserGeometryDbContext(tableName, connectionString);
                }
            case DbKeys.PlannedSchedules:
                {
                    return new PlannedScheduleDbContext(tableName, connectionString);
                }
            default:
                throw new Exception($"DBContext for key={key} not register.");
        };
    }
}
