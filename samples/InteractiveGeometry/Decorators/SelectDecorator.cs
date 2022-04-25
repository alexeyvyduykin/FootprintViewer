using Mapsui;
using Mapsui.Layers;
using Mapsui.UI;
using System;

namespace InteractiveGeometry
{
    public interface ISelectDecorator : IDisposable
    {
        BaseFeature? SelectFeature { get; }

        event EventHandler? Select;
        event EventHandler? Unselect;
    }

    public class SelectDecorator : ISelectDecorator
    {
        private readonly Map _map;
        private readonly ILayer _layer;
        private BaseFeature? _saveFeature;

        public SelectDecorator(Map map, ILayer layer)
        {
            _map = map;
            _layer = layer;

            map.Info += Map_Info;
        }

        public BaseFeature? SelectFeature => _saveFeature;

        public event EventHandler? Select;

        public event EventHandler? Unselect;

        private void Map_Info(object? sender, MapInfoEventArgs e)
        {
            if (e.MapInfo != null && e.MapInfo.Layer == _layer && e.MapInfo.Feature != null)
            {
                var feature = e.MapInfo.Feature;

                if (feature != _saveFeature)
                {
                    SelectImpl(feature);
                }
                else
                {
                    UnselectImpl();
                }

                return;
            }
        }

        private void SelectImpl(IFeature feature)
        {
            _saveFeature = (BaseFeature)feature;

            Select?.Invoke(this, EventArgs.Empty);
        }

        private void UnselectImpl()
        {
            if (_saveFeature != null)
            {
                _saveFeature = null;

                Unselect?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnselectImpl();

                //HACK: call feature style changing, if select after dispose
                _layer.DataHasChanged();

                _map.Info -= Map_Info;
            }
        }
    }
}
