using Mapsui.Layers;

namespace FootprintViewer
{
    public static class Extensions
    {
        public static void Replace(this LayerCollection collection, string name, ILayer layer)
        {
            int index = 0;
            ILayer? removable = null;

            var count = collection.Count;

            for (int i = 0; i < count; i++)
            {
                if (collection[i].Name.Equals(name) == true)
                {
                    removable = collection[i];
                    index = i;
                    break;
                }
            }

            if (removable != null)
            {
                collection.Remove(removable);

                layer.Name = name;

                collection.Insert(index, layer);
            }
        }
    }
}
