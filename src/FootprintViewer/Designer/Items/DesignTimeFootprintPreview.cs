using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintPreview : FootprintPreview
    {
        private static readonly Random _random = new Random();

        public DesignTimeFootprintPreview() : base("38-50-lr_3857")
        {
            var unitBitmap = new System.Drawing.Bitmap(1, 1);
            unitBitmap.SetPixel(0, 0, System.Drawing.Color.White);

            Date = new DateTime(2001, 6, 1, 12, 0, 0).ToShortDateString();
            SatelliteName = "Satellite1";
            SunElevation = 71.0;
            CloudCoverFull = 84.0;
            TileNumber = "38-50-lr_3857";
            Image = new System.Drawing.Bitmap(unitBitmap);
        }

        public static FootprintPreview Build()
        {
            var names = new[] { "02-65-lr_2000-3857-lite", "36-65-ur_2000-3857-lite", "38-50-ll_3857-lite", "38-50-lr_3857-lite", "38-50-ul_3857-lite", "38-50-ur_3857-lite", "41-55-ul_2000-3857-lite", "44-70-ur_2000-3857-lite" };
            var satellites = new[] { "Satellite1", "Satellite2", "Satellite3" };

            var name = names[_random.Next(0, names.Length - 1)].Replace("lite", "").Replace("2000", "").Replace("3857", "").Replace("_", "").Replace("-", "");
            var date = DateTime.UtcNow;

            var unitBitmap = new System.Drawing.Bitmap(1, 1);
            unitBitmap.SetPixel(0, 0, System.Drawing.Color.White);

            return new FootprintPreview(name.ToUpper())
            {
                Date = date.Date.ToShortDateString(),
                SatelliteName = satellites[_random.Next(0, satellites.Length - 1)],
                SunElevation = _random.Next(0, 90),
                CloudCoverFull = _random.Next(0, 100),
                TileNumber = name.ToUpper(),
                Image = new System.Drawing.Bitmap(unitBitmap)
            };
        }
    }
}
