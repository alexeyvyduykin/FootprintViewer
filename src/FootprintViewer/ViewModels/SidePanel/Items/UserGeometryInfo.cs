﻿using FootprintViewer.Data;
using ReactiveUI;

namespace FootprintViewer.ViewModels
{
    public class UserGeometryInfo : ReactiveObject, IViewerItem, ISelectorItem
    {
        private readonly UserGeometry _model;

        public UserGeometryInfo(UserGeometry model)
        {
            _model = model;
        }

        public string GetKey() => _model.Type.ToString();

        public UserGeometry Model => _model;

        public string Name => _model.Name!;

        public UserGeometryType Type => _model.Type;

        public bool IsShowInfo { get; set; }
    }
}
