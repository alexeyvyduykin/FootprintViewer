﻿using FootprintViewer.Data.DataManager;
using FootprintViewer.Data.DataManager.Sources;
using System;

namespace FootprintViewer.AppStates.Extensions;

public static class SourceExtensions
{
    public static ISourceState ToState(this ISource source)
    {
        if (source is FileSource fileSource)
        {
            return new FileSourceState()
            {
                Key = fileSource.Key,
                Paths = fileSource.Paths
            };
        }
        else if (source is JsonSource jsonSource)
        {
            return new JsonSourceState()
            {
                Key = jsonSource.Key,
                Paths = jsonSource.Paths
            };
        }
        else if (source is DatabaseSource databaseSource)
        {
            return new DatabaseSourceState()
            {
                IsEditable = false,
                Key = databaseSource.Key,
                ConnectionString = databaseSource.ConnectionString,
                TableName = databaseSource.TableName
            };
        }
        else if (source is EditableDatabaseSource editableDatabaseSource)
        {
            return new DatabaseSourceState()
            {
                IsEditable = true,
                Key = editableDatabaseSource.Key,
                ConnectionString = editableDatabaseSource.ConnectionString,
                TableName = editableDatabaseSource.TableName
            };
        }
        else
        {
            throw new Exception();
        }
    }

    //public static ISource ToSource(this ISourceState state)
    //{

    //}
}
