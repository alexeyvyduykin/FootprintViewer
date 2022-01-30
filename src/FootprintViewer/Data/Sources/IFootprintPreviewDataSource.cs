﻿using FootprintViewer.ViewModels;
using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public interface IFootprintPreviewDataSource
    {
        IEnumerable<FootprintPreview> GetFootprintPreviews();
    }
}