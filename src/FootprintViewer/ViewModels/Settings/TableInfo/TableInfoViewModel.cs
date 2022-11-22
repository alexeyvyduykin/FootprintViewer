using FootprintViewer.ViewModels.Dialogs;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.ViewModels.Settings;

public class TableInfoViewModel : DialogViewModelBase<object>
{
    protected TableInfoViewModel()
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

    protected static List<TableColumnViewModel> GetFields(TableInfoType type) => _dict[type].ToList();

    private static readonly IDictionary<TableInfoType, TableColumnViewModel[]> _dict = new Dictionary<TableInfoType, TableColumnViewModel[]>()
    {
        { TableInfoType.Footprint, new[]
        {
            new TableColumnViewModel()
            {
                FieldName = "Name",
                FieldType = "text",
                PrimaryKey = true,
                Description = "Имя снимка"
            },
            new TableColumnViewModel()
            {
                FieldName = "SatelliteName",
                FieldType = "text",
                Description = "Имя космического аппарата"
            },
            new TableColumnViewModel()
            {
                FieldName = "TargetName",
                FieldType = "text",
                Description = "Имя наземной цели"
            },
            new TableColumnViewModel()
            {
                FieldName = "Center",
                FieldType = "geometry",
                Hyperlink = "(PostGIS)",
                Description = "Координаты центра наземной цели",
                Dimension = "Point"
            },
            new TableColumnViewModel()
            {
                FieldName = "Points",
                FieldType = "geometry",
                Hyperlink="(PostGIS)",
                Description = "Массив координат ограничивающих снимок",
                Dimension="LineString (по часовой стрелке)"
            },
            new TableColumnViewModel()
            {
                FieldName = "Begin",
                FieldType = "timestamp without time zone",
                Description = "Дата начала съёмки"
            },
            new TableColumnViewModel()
            {
                FieldName = "Duration",
                FieldType = "double precision",
                Description = "Продолжительность съемки",
                Dimension="сек"
            },
            new TableColumnViewModel()
            {
                FieldName = "Node",
                FieldType = "integer",
                Description = "Номер витка",
                Dimension="1…n"
            },
            new TableColumnViewModel()
            {
                FieldName = "Direction",
                FieldType = "text",
                Description = "Направление полосы обзора",
                Dimension="«Left», «Right»"
            },
        } },
        { TableInfoType.GroundTarget, new[]
        {
            new TableColumnViewModel()
            {
                FieldName = "Name",
                FieldType = "text",
                PrimaryKey = true,
                Description = "Имя наземной цели"
            },
            new TableColumnViewModel()
            {
                FieldName = "Type",
                FieldType = "text",
                Description = "Тип наземной цели",
                Dimension = "«Point», «Route», «Area»"
            },
            new TableColumnViewModel()
            {
                FieldName = "Points",
                FieldType = "geometry",
                Hyperlink = "(PostGIS)",
                Description = "Геометрия наземной цели",
                Dimension = "Geometry"
            }
        } },
        { TableInfoType.Satellite, new[]
        {
            new TableColumnViewModel()
            {
                FieldName = "Name",
                FieldType = "text",
                PrimaryKey = true,
                Description = "Имя космического аппарата"
            },
            new TableColumnViewModel()
            {
                FieldName = "Semiaxis",
                FieldType="double precision",
                Description="Большая полуось",
                Dimension="км"
            },
            new TableColumnViewModel()
            {
                FieldName = "Eccentricity",
                FieldType="double precision",
                Description="Эксцентриситет"
            },
            new TableColumnViewModel()
            {
                FieldName = "InclinationDeg",
                FieldType="double precision",
                Description="Наклонение",
                Dimension="град"
            },
            new TableColumnViewModel()
            {
                FieldName = "ArgumentOfPerigeeDeg",
                FieldType="double precision",
                Description="Аргумент перигея",
                Dimension="град"
            },
            new TableColumnViewModel()
            {
                FieldName = "LongitudeAscendingNodeDeg",
                FieldType="double precision",
                Description="Долгота восходящего узла",
                Dimension="град"
            },
            new TableColumnViewModel()
            {
                FieldName = "RightAscensionAscendingNodeDeg",
                FieldType="double precision",
                Description="Прямое восхождение восходящего узла",
                Dimension="град"
            },
            new TableColumnViewModel()
            {
                FieldName = "Period",
                FieldType="double precision",
                Description="Период",
                Dimension="сек"
            },
            new TableColumnViewModel()
            {
                FieldName = "Epoch",
                FieldType="timestamp without time zone",
                Description="Дата начала рабочей программы"
            },
            new TableColumnViewModel()
            {
                FieldName = "InnerHalfAngleDeg",
                FieldType="double precision",
                Description="Внутренний полуугол съемочной аппаратуры",
                Dimension="град"
            },
            new TableColumnViewModel()
            {
                FieldName = "OuterHalfAngleDeg",
                FieldType="double precision",
                Description="Внешний полуугол съемочной аппаратуры",
                Dimension="град"
            },
        } },
        { TableInfoType.GroundStation, new[]
        {
            new TableColumnViewModel()
            {
                FieldName = "Name",
                FieldType="text",
                PrimaryKey=true,
                Description="Имя наземной станции"
            },
            new TableColumnViewModel()
            {
                FieldName = "Center",
                FieldType="geometry",
                Hyperlink="(PostGIS)",
                Description="Координаты центра наземной станции",
                Dimension="Point"
            },
            new TableColumnViewModel()
            {
                FieldName = "Angles",
                FieldType="double precision[]",
                Description="Центральные углы определяющие области покрытия",
                Dimension="град"
            },
        } },
        { TableInfoType.UserGeometry,
            new[]
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
