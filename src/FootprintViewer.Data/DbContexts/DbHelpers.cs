using FootprintViewer.Data.Models;

namespace FootprintViewer.Data.DbContexts;

public static class DbHelpers
{
    public static bool TryValidateContext(DbKeys key, Func<DbCustomContext> creator)
    {
        using var context = creator.Invoke();
        return context.Valid(GetType(key));
    }

    public static bool IsKeyEquals(DbKeys? key, DbKeys? dbKeys)
    {
        return Equals(key, dbKeys);
    }

    public static bool IsKeyEquals(string? key, DbKeys? dbKeys)
    {
        var res = Enum.TryParse<DbKeys>(key, true, out var result);

        if (res == true)
        {
            return Equals(result, dbKeys);
        }

        return res;
    }

    public static TableInfoType GetTableType(string? key)
    {
        Enum.TryParse<DbKeys>(key, true, out var result);
        return result switch
        {
            DbKeys.UserGeometries => TableInfoType.UserGeometry,
            _ => throw new Exception($"Table info for key={key} not register."),
        };
    }

    public static Type GetType(DbKeys key)
    {
        return key switch
        {
            DbKeys.UserGeometries => typeof(UserGeometry),
            DbKeys.PlannedSchedules => typeof(PlannedScheduleResult),
            _ => throw new Exception(),
        };
    }

    public static Type GetType(string key)
    {
        Enum.TryParse<DbKeys>(key, true, out var result);
        return result switch
        {
            DbKeys.UserGeometries => typeof(UserGeometry),
            DbKeys.PlannedSchedules => typeof(PlannedScheduleResult),
            _ => throw new Exception(),
        };
    }
}