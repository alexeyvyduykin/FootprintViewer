namespace FootprintViewer.Styles;

public interface IPalette
{

}

public interface ISingleHuePalette : IPalette
{
    System.Drawing.Color GetColor(int index, int number);
}

public interface IColorPalette : IPalette
{
    System.Drawing.Color PickColor(string key);
}

public interface IMapsuiPalette : IPalette
{
    Mapsui.Styles.Color Color { get; }
}
