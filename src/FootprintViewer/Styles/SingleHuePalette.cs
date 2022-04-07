using System;
using System.Collections.Generic;
using System.Drawing;

namespace FootprintViewer.Styles
{
    public class SingleHuePalette
    {
        private Dictionary<int, List<Color>> _palette = new();

        private static SingleHuePalette? _orangesPalette;
        private static SingleHuePalette? _redsPalette;
        private static SingleHuePalette? _greensPalette;
        private static SingleHuePalette? _purplesPalette;
        private static SingleHuePalette? _greysPalette;
        private static SingleHuePalette? _bluesPalette;

        private static readonly int _minNumber = 1;
        private static readonly int _maxNumber = 9;

        public static SingleHuePalette Oranges => _orangesPalette ??= CreateOrangesPalette();
        public static SingleHuePalette Reds => _redsPalette ??= CreateRedsPalette();
        public static SingleHuePalette Greens => _greensPalette ??= CreateGreensPalette();
        public static SingleHuePalette Purples => _purplesPalette ??= CreatePurplesPalette();
        public static SingleHuePalette Greys => _greysPalette ??= CreateGreysPalette();
        public static SingleHuePalette Blues => _bluesPalette ??= CreateBluesPalette();

        private static SingleHuePalette CreateOrangesPalette()
        {
            return new SingleHuePalette()
            {
                _palette = new Dictionary<int, List<Color>>()
                {
                    { 1, new (){ Color.FromArgb(230,85,13) } },
                    { 2, new (){ Color.FromArgb(253,174,107), Color.FromArgb(230,85,13) } },
                    { 3, new (){ Color.FromArgb(254,230,206), Color.FromArgb(253,174,107), Color.FromArgb(230,85,13) } },
                    { 4, new (){ Color.FromArgb(254,237,222), Color.FromArgb(253,190,133), Color.FromArgb(253,141,60), Color.FromArgb(217,71,1) } },
                    { 5, new (){ Color.FromArgb(254,237,222), Color.FromArgb(253,190,133), Color.FromArgb(253,141,60), Color.FromArgb(230,85,13), Color.FromArgb(166,54,3) } },
                    { 6, new (){ Color.FromArgb(254,237,222), Color.FromArgb(253,208,162), Color.FromArgb(253,174,107), Color.FromArgb(253,141,60), Color.FromArgb(230,85,13), Color.FromArgb(166,54,3) } },
                    { 7, new (){ Color.FromArgb(254,237,222), Color.FromArgb(253,208,162), Color.FromArgb(253,174,107), Color.FromArgb(253,141,60), Color.FromArgb(241,105,19), Color.FromArgb(217,72,1), Color.FromArgb(140,45,4) } },
                    { 8, new (){ Color.FromArgb(255,245,235), Color.FromArgb(254,230,206), Color.FromArgb(253,208,162), Color.FromArgb(253,174,107), Color.FromArgb(253,141,60), Color.FromArgb(241,105,19), Color.FromArgb(217,72,1), Color.FromArgb(140,45,4) } },
                    { 9, new (){ Color.FromArgb(255,245,235), Color.FromArgb(254,230,206), Color.FromArgb(253,208,162), Color.FromArgb(253,174,107), Color.FromArgb(253,141,60), Color.FromArgb(241,105,19), Color.FromArgb(217,72,1), Color.FromArgb(166,54,3), Color.FromArgb(127,39,4) } },
                }
            };
        }

        private static SingleHuePalette CreateRedsPalette()
        {
            return new SingleHuePalette()
            {
                _palette = new Dictionary<int, List<Color>>()
                {
                    { 1, new (){ Color.FromArgb(222,45,38) } },
                    { 2, new (){ Color.FromArgb(252,146,114), Color.FromArgb(222,45,38) } },
                    { 3, new (){ Color.FromArgb(254,224,210), Color.FromArgb(252,146,114), Color.FromArgb(222,45,38) } },
                    { 4, new (){ Color.FromArgb(254,229,217), Color.FromArgb(252,174,145), Color.FromArgb(251,106,74), Color.FromArgb(203,24,29) } },
                    { 5, new (){ Color.FromArgb(254,229,217), Color.FromArgb(252,174,145), Color.FromArgb(251,106,74), Color.FromArgb(222,45,38), Color.FromArgb(165,15,21) } },
                    { 6, new (){ Color.FromArgb(254,229,217), Color.FromArgb(252,187,161), Color.FromArgb(252,146,114), Color.FromArgb(251,106,74), Color.FromArgb(222,45,38), Color.FromArgb(165,15,21) } },
                    { 7, new (){ Color.FromArgb(254,229,217), Color.FromArgb(252,187,161), Color.FromArgb(252,146,114), Color.FromArgb(251,106,74), Color.FromArgb(239,59,44), Color.FromArgb(203,24,29), Color.FromArgb(153,0,13) } },
                    { 8, new (){ Color.FromArgb(255,245,240), Color.FromArgb(254,224,210), Color.FromArgb(252,187,161), Color.FromArgb(252,146,114), Color.FromArgb(251,106,74), Color.FromArgb(239,59,44), Color.FromArgb(203,24,29), Color.FromArgb(153,0,13) } },
                    { 9, new (){ Color.FromArgb(255,245,240), Color.FromArgb(254,224,210), Color.FromArgb(252,187,161), Color.FromArgb(252,146,114), Color.FromArgb(251,106,74), Color.FromArgb(239,59,44), Color.FromArgb(203,24,29), Color.FromArgb(165,15,21), Color.FromArgb(103,0,13) } },
                }
            };
        }

        private static SingleHuePalette CreateGreensPalette()
        {
            return new SingleHuePalette()
            {
                _palette = new Dictionary<int, List<Color>>()
                {
                    { 1, new (){ Color.FromArgb(49,163,84) } },
                    { 2, new (){ Color.FromArgb(161,217,155), Color.FromArgb(49,163,84) } },
                    { 3, new (){ Color.FromArgb(229,245,224), Color.FromArgb(161,217,155), Color.FromArgb(49,163,84) } },
                    { 4, new (){ Color.FromArgb(237,248,233), Color.FromArgb(186,228,179), Color.FromArgb(116,196,118), Color.FromArgb(35,139,69) } },
                    { 5, new (){ Color.FromArgb(237,248,233), Color.FromArgb(186,228,179), Color.FromArgb(116,196,118), Color.FromArgb(49,163,84), Color.FromArgb(0,109,44) } },
                    { 6, new (){ Color.FromArgb(237,248,233), Color.FromArgb(199,233,192), Color.FromArgb(161,217,155), Color.FromArgb(116,196,118), Color.FromArgb(49,163,84), Color.FromArgb(0,109,44) } },
                    { 7, new (){ Color.FromArgb(237,248,233), Color.FromArgb(199,233,192), Color.FromArgb(161,217,155), Color.FromArgb(116,196,118), Color.FromArgb(65,171,93), Color.FromArgb(35,139,69), Color.FromArgb(0,90,50) } },
                    { 8, new (){ Color.FromArgb(247,252,245), Color.FromArgb(229,245,224), Color.FromArgb(199,233,192), Color.FromArgb(161,217,155), Color.FromArgb(116,196,118), Color.FromArgb(65,171,93), Color.FromArgb(35,139,69), Color.FromArgb(0,90,50) } },
                    { 9, new (){ Color.FromArgb(247,252,245), Color.FromArgb(229,245,224), Color.FromArgb(199,233,192), Color.FromArgb(161,217,155), Color.FromArgb(116,196,118), Color.FromArgb(65,171,93), Color.FromArgb(35,139,69), Color.FromArgb(0,109,44), Color.FromArgb(0,68,27) } },
                }
            };
        }

        private static SingleHuePalette CreatePurplesPalette()
        {
            return new SingleHuePalette()
            {
                _palette = new Dictionary<int, List<Color>>()
                {
                    { 1, new (){ Color.FromArgb(117,107,177) } },
                    { 2, new (){ Color.FromArgb(188,189,220), Color.FromArgb(117,107,177) } },
                    { 3, new (){ Color.FromArgb(239,237,245), Color.FromArgb(188,189,220), Color.FromArgb(117,107,177) } },
                    { 4, new (){ Color.FromArgb(242,240,247), Color.FromArgb(203,201,226), Color.FromArgb(158,154,200), Color.FromArgb(106,81,163) } },
                    { 5, new (){ Color.FromArgb(242,240,247), Color.FromArgb(203,201,226), Color.FromArgb(158,154,200), Color.FromArgb(117,107,177), Color.FromArgb(84,39,143) } },
                    { 6, new (){ Color.FromArgb(242,240,247), Color.FromArgb(218,218,235), Color.FromArgb(188,189,220), Color.FromArgb(158,154,200), Color.FromArgb(117,107,177), Color.FromArgb(84,39,143) } },
                    { 7, new (){ Color.FromArgb(242,240,247), Color.FromArgb(218,218,235), Color.FromArgb(188,189,220), Color.FromArgb(158,154,200), Color.FromArgb(128,125,186), Color.FromArgb(106,81,163), Color.FromArgb(74,20,134) } },
                    { 8, new (){ Color.FromArgb(252,251,253), Color.FromArgb(239,237,245), Color.FromArgb(218,218,235), Color.FromArgb(188,189,220), Color.FromArgb(158,154,200), Color.FromArgb(128,125,186), Color.FromArgb(106,81,163), Color.FromArgb(74,20,134) } },
                    { 9, new (){ Color.FromArgb(252,251,253), Color.FromArgb(239,237,245), Color.FromArgb(218,218,235), Color.FromArgb(188,189,220), Color.FromArgb(158,154,200), Color.FromArgb(128,125,186), Color.FromArgb(106,81,163), Color.FromArgb(84,39,143), Color.FromArgb(63,0,125) } },
                }
            };
        }

        private static SingleHuePalette CreateGreysPalette()
        {
            return new SingleHuePalette()
            {
                _palette = new Dictionary<int, List<Color>>()
                {
                    { 1, new (){ Color.FromArgb(99,99,99) } },
                    { 2, new (){ Color.FromArgb(189,189,189), Color.FromArgb(99,99,99) } },
                    { 3, new (){ Color.FromArgb(240,240,240), Color.FromArgb(189,189,189), Color.FromArgb(99,99,99) } },
                    { 4, new (){ Color.FromArgb(247,247,247), Color.FromArgb(204,204,204), Color.FromArgb(150,150,150), Color.FromArgb(82,82,82) } },
                    { 5, new (){ Color.FromArgb(247,247,247), Color.FromArgb(204,204,204), Color.FromArgb(150,150,150), Color.FromArgb(99,99,99), Color.FromArgb(37,37,37) } },
                    { 6, new (){ Color.FromArgb(247,247,247), Color.FromArgb(217,217,217), Color.FromArgb(189,189,189), Color.FromArgb(150,150,150), Color.FromArgb(99,99,99), Color.FromArgb(37,37,37) } },
                    { 7, new (){ Color.FromArgb(247,247,247), Color.FromArgb(217,217,217), Color.FromArgb(189,189,189), Color.FromArgb(150,150,150), Color.FromArgb(115,115,115), Color.FromArgb(82,82,82), Color.FromArgb(37,37,37) } },
                    { 8, new (){ Color.FromArgb(255,255,255), Color.FromArgb(240,240,240), Color.FromArgb(217,217,217), Color.FromArgb(189,189,189), Color.FromArgb(150,150,150), Color.FromArgb(115,115,115), Color.FromArgb(82,82,82), Color.FromArgb(37,37,37) } },
                    { 9, new (){ Color.FromArgb(255,255,255), Color.FromArgb(240,240,240), Color.FromArgb(217,217,217), Color.FromArgb(189,189,189), Color.FromArgb(150,150,150), Color.FromArgb(115,115,115), Color.FromArgb(82,82,82), Color.FromArgb(37,37,37), Color.FromArgb(0,0,0) } },
                }
            };
        }

        private static SingleHuePalette CreateBluesPalette()
        {
            return new SingleHuePalette()
            {
                _palette = new Dictionary<int, List<Color>>()
                {
                    { 1, new (){ Color.FromArgb(49,130,189) } },
                    { 2, new (){ Color.FromArgb(158,202,225), Color.FromArgb(49,130,189) } },
                    { 3, new (){ Color.FromArgb(222,235,247), Color.FromArgb(158,202,225), Color.FromArgb(49,130,189) } },
                    { 4, new (){ Color.FromArgb(239,243,255), Color.FromArgb(189,215,231), Color.FromArgb(107,174,214), Color.FromArgb(33,113,181) } },
                    { 5, new (){ Color.FromArgb(239,243,255), Color.FromArgb(189,215,231), Color.FromArgb(107,174,214), Color.FromArgb(49,130,189), Color.FromArgb(8,81,156) } },
                    { 6, new (){ Color.FromArgb(239,243,255), Color.FromArgb(198,219,239), Color.FromArgb(158,202,225), Color.FromArgb(107,174,214), Color.FromArgb(49,130,189), Color.FromArgb(8,81,156) } },
                    { 7, new (){ Color.FromArgb(239,243,255), Color.FromArgb(198,219,239), Color.FromArgb(158,202,225), Color.FromArgb(107,174,214), Color.FromArgb(66,146,198), Color.FromArgb(33,113,181), Color.FromArgb(8,69,148) } },
                    { 8, new (){ Color.FromArgb(247,251,255), Color.FromArgb(222,235,247), Color.FromArgb(198,219,239), Color.FromArgb(158,202,225), Color.FromArgb(107,174,214), Color.FromArgb(66,146,198), Color.FromArgb(33,113,181), Color.FromArgb(8,69,148) } },
                    { 9, new (){ Color.FromArgb(247,251,255), Color.FromArgb(222,235,247), Color.FromArgb(198,219,239), Color.FromArgb(158,202,225), Color.FromArgb(107,174,214), Color.FromArgb(66,146,198), Color.FromArgb(33,113,181), Color.FromArgb(8,81,156), Color.FromArgb(8,48,107) } },
                }
            };
        }

        public Color GetColor(int index, int number)
        {
            int num = Math.Max(_minNumber, Math.Min(_maxNumber, number));
            int i = Math.Max(0, Math.Min(num - 1, index));
            return _palette[num][num - 1 - i];
        }
    }
}
