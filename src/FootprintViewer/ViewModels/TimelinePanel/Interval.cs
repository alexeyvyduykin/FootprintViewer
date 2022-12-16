﻿using System;

namespace FootprintViewer.ViewModels.TimelinePanel;

public class Interval
{
    public string Category { get; init; } = "Default";

    public DateTime BeginTime { get; set; }

    public DateTime EndTime { get; set; }
}