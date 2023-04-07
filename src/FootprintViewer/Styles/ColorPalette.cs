using System.Collections.Generic;
using System.Drawing;

namespace FootprintViewer.Styles;

public class ColorPalette : IColorPalette
{
    public static ColorPalette DefaultPalette =>
        new(new[]
        {
            Color.FromArgb(11, 132, 165), // #0B84A5                   
            Color.FromArgb(246, 200, 95), // #F6C85F                    
            Color.FromArgb(111, 78, 124), // #6F4E7C                    
            Color.FromArgb(157, 216, 102), // #9DD866                    
            Color.FromArgb(202, 71, 47), // #CA472F                   
            Color.FromArgb(255, 160, 86), // #FFA056               
            Color.FromArgb(141, 221, 208), // #8DDDD0       
        });

    public static ColorPalette RetroMetroPalette =>
        new(new[]
        {
            Color.FromArgb(234, 85, 69), // #ea5545
            Color.FromArgb(244, 106, 155), // #f46a9b       
            Color.FromArgb(239, 155, 32), // #ef9b20        
            Color.FromArgb(237, 191, 51), // #edbf33       
            Color.FromArgb(237, 225, 91), // #ede15b       
            Color.FromArgb(189, 207, 50), // #bdcf32       
            Color.FromArgb(135, 188, 69), // #87bc45       
            Color.FromArgb(39, 174, 239), // #27aeef       
            Color.FromArgb(179, 61, 198), // #b33dc6     
        });

    public static ColorPalette DutchFieldPalette =>
        new(new[]
        {
            Color.FromArgb(230, 0, 73), // #e60049
            Color.FromArgb(11, 180, 255), // #0bb4ff       
            Color.FromArgb(80, 233, 145), // #50e991       
            Color.FromArgb(230, 216, 0), // #e6d800       
            Color.FromArgb(155, 25, 245), // #9b19f5       
            Color.FromArgb(255, 163, 0), // #ffa300       
            Color.FromArgb(220, 10, 180), // #dc0ab4       
            Color.FromArgb(179, 212, 255), // #b3d4ff       
            Color.FromArgb(0, 191, 160), // #00bfa0        
        });

    public static ColorPalette RiverNightsPalette =>
        new(new[]
        {
            Color.FromArgb(179, 0, 0), // #b30000
            Color.FromArgb(124, 17, 88), // #7c1158        
            Color.FromArgb(68, 33, 175), // #4421af        
            Color.FromArgb(26, 83, 255), // #1a53ff       
            Color.FromArgb(13, 136, 230), // #0d88e6       
            Color.FromArgb(0, 183, 199), // #00b7c7       
            Color.FromArgb(90, 212, 90), // #5ad45a       
            Color.FromArgb(139, 224, 78), // #8be04e       
            Color.FromArgb(235, 220, 120), // #ebdc78                                      
        });

    public static ColorPalette SpringPastelsPalette =>
        new(new[]
        {
            Color.FromArgb(253, 127, 111), // #fd7f6f
            Color.FromArgb(126, 176, 213), // #7eb0d5       
            Color.FromArgb(178, 224, 97), // #b2e061       
            Color.FromArgb(189, 126, 190), // #bd7ebe       
            Color.FromArgb(255, 181, 90), // #ffb55a       
            Color.FromArgb(255, 238, 101), // #ffee65       
            Color.FromArgb(190, 185, 219), // #beb9db       
            Color.FromArgb(253, 204, 229), // #fdcce5       
            Color.FromArgb(139, 211, 199), // #8bd3c7                                        
        });

    private readonly Color[] _colors;
    private readonly Dictionary<string, Color> _dictionary = new();

    private ColorPalette(Color[] colors)
    {
        _colors = colors;
    }

    public Color PickColor(string key)
    {
        if (string.IsNullOrWhiteSpace(key) == true)
        {
            throw new ArgumentException(key);
        }

        if (_dictionary.ContainsKey(key) == true)
        {
            return _dictionary[key];
        }

        if (_colors.Length > _dictionary.Count)
        {
            int i = _dictionary.Count;

            _dictionary.Add(key, _colors[i]);

            return _dictionary[key];
        }

        _dictionary.Add(key, GetRandomColor());

        return _dictionary[key];
    }

    private static Color GetRandomColor()
    {
        var random = new Random();

        return Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
    }
}
