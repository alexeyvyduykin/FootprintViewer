﻿using FootprintViewer.Data;
using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.ViewModels.Settings;

public sealed class TableInfoViewModel : DialogViewModelBase<object>
{
    private TableInfoViewModel()
    {
        NextCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Back));
    }

    public TableInfoType Type { get; set; }

    public IList<TableColumnViewModel>? Fields { get; set; }

    public static TableInfoViewModel Build(TableInfoType type)
    {
        return new TableInfoViewModel()
        {
            Type = type,
            Fields = GetFields(type)
        };
    }

    private static List<TableColumnViewModel> GetFields(TableInfoType type) => _dict[type].ToList();

    private static readonly IDictionary<TableInfoType, TableColumnViewModel[]> _dict = new Dictionary<TableInfoType, TableColumnViewModel[]>()
    {
        { TableInfoType.UserGeometry, new[]
            {
                new TableColumnViewModel()
                {
                    FieldName = "Name",
                    FieldType="text",
                    PrimaryKey=true,
                    Description="Имя пользовательского объекта"
                },
                new TableColumnViewModel()
                {
                    FieldName = "Type",
                    FieldType="text",
                    Description="Тип пользовательского объекта",
                    Dimension="«Point», «Rectangle», «Circle», «Polygon»"
                },
                new TableColumnViewModel()
                {
                    FieldName = "Geometry",
                    FieldType="geometry",
                    Hyperlink="(PostGIS)",
                    Description="Геометрия пользовательского объекта",
                    Dimension="Geometry"
                },
            } }
    };
}
