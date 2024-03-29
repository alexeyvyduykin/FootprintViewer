﻿namespace FootprintViewer.Data;

public abstract class BaseEditableSource : BaseSource, IEditableSource
{
    public abstract Task AddAsync(string key, object value);

    public abstract Task RemoveAsync(string key, object value);

    public abstract Task EditAsync(string key, string id, object newValue);
}
