using FootprintViewer.Models;
using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.ToolBars;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeCustomToolBar : CustomToolBar
    {
        public DesignTimeCustomToolBar() : base(new DesignTimeData())
        {
            //ZoomIn = new ToolClick()
            //{
            //    Title = "ZoomIn",
            //    Tooltip = "Приблизить",
            //};

            //ZoomOut = new ToolClick()
            //{
            //    Title = "ZoomOut",
            //    Tooltip = "Отдалить",
            //};

            //var addRectangle = new ToolCheck()
            //{
            //    Title = "AddRectangle",
            //    Tooltip = "Нарисуйте прямоугольную AOI",
            //    Group = "Group1",
            //};

            //var addPolygon = new ToolCheck()
            //{
            //    Title = "AddPolygon",
            //    Tooltip = "Нарисуйте полигональную AOI",
            //    Group = "Group1",
            //};

            //var addCircle = new ToolCheck()
            //{
            //    Title = "AddCircle",
            //    Tooltip = "Нарисуйте круговую AOI",
            //    Group = "Group1",
            //};

            //AOICollection = new ToolCollection();
            //AOICollection.AddItem(addRectangle);
            //AOICollection.AddItem(addPolygon);
            //AOICollection.AddItem(addCircle);

            //RouteDistance = new ToolCheck()
            //{
            //    Title = "Route",
            //    Tooltip = "Измерить расстояние",
            //    Group = "Group1",
            //};

            //SelectGeometry = new ToolCheck()
            //{
            //    Title = "Select",
            //    Tooltip = "SelectGeometry",
            //    Group = "Group2",
            //};

            //var rectangle = new ToolCheck()
            //{
            //    Title = "Rectangle",
            //    Tooltip = "RectangleGeometry",
            //    Group = "Group2",
            //};

            //var circle = new ToolCheck()
            //{
            //    Title = "Circle",
            //    Tooltip = "CircleGeometry",
            //    Group = "Group2",
            //};

            //var polygon = new ToolCheck()
            //{
            //    Title = "Polygon",
            //    Tooltip = "PolygonGeometry",
            //    Group = "Group2",
            //};

            //GeometryCollection = new ToolCollection();
            //GeometryCollection.AddItem(rectangle);
            //GeometryCollection.AddItem(circle);
            //GeometryCollection.AddItem(polygon);

            //TranslateGeometry = new ToolCheck()
            //{
            //    Title = "Translate",
            //    Tooltip = "TranslateGeometry",
            //    Group = "Group2",
            //};

            //RotateGeometry = new ToolCheck()
            //{
            //    Title = "Rotate",
            //    Tooltip = "RotateGeometry",
            //    Group = "Group2",
            //};

            //ScaleGeometry = new ToolCheck()
            //{
            //    Title = "Scale",
            //    Tooltip = "ScaleGeometry",
            //    Group = "Group2",
            //};

            //EditGeometry = new ToolCheck()
            //{
            //    Title = "Edit",
            //    Tooltip = "EditGeometry",
            //    Group = "Group2",
            //};

            //AddTool(ZoomIn);
            //AddTool(ZoomOut);
            //AddTool(AOICollection);
            //AddTool(RouteDistance);
            //AddTool(SelectGeometry);
            //AddTool(GeometryCollection);
            //AddTool(TranslateGeometry);
            //AddTool(RotateGeometry);
            //AddTool(ScaleGeometry);
            //AddTool(EditGeometry);
        }

        //public ToolClick ZoomIn { get; }

        //public ToolClick ZoomOut { get; }

        //public IToolCollection AOICollection { get; }

        //public ToolCheck RouteDistance { get; }

        //public ToolCheck SelectGeometry { get; }

        //public IToolCollection GeometryCollection { get; }

        //public ToolCheck TranslateGeometry { get; }

        //public ToolCheck RotateGeometry { get; }

        //public ToolCheck ScaleGeometry { get; }

        //public ToolCheck EditGeometry { get; }

    }
}
