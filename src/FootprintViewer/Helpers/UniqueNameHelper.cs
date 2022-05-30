using System;

namespace FootprintViewer.Helpers
{
    public static class UniqueNameHelper
    {
        public static string Create(string name, string ext)
        {
            string date = DateTime.Now.ToString("yyyy-MM-ddTHHmmss");

            return string.Format("{0}.{1}.{2}", name, date, ext);
        }
    }
}
