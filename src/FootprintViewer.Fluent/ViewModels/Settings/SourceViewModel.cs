﻿using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.Fluent.ViewModels.Settings;

public class SourceViewModel : ReactiveObject, ISourceViewModel
{
    private readonly ISource _source;

    public SourceViewModel(ISource source)
    {
        _source = source;
    }

    public ISource Source => _source;

    [Reactive]
    public string? Name { get; set; } = "Default";
}