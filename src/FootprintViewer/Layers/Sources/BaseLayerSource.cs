using FootprintViewer.Data;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public interface ILayerSource : ILayer
    {
        ReactiveCommand<Unit, Unit> Init { get; }

        IFeature? GetFeature(string name);

        void SelectFeature(string name);

        void ShowHighlight(string name);

        void HideHighlight();
    }

    public abstract class BaseLayerSource<TNative> : WritableLayer, ILayerSource
    {
        private readonly IProvider<TNative> _provider;
        private IFeature? _lastSelected;

        public BaseLayerSource(IProvider<TNative> provider)
        {
            _provider = provider;

            Init = ReactiveCommand.CreateFromObservable(() => Observable.Start(() => LoadingImpl(provider.GetNativeValuesAsync(null).Result)));

            if (provider is Provider<TNative> pvd)
            {
                pvd.UpdateSources.InvokeCommand(Init);
            }
        }

        public IFeature? GetFeature(string name)
        {
            return GetFeatures().Where(s => s.Fields.Contains("Name") && s["Name"]!.Equals(name)).FirstOrDefault();
        }

        public void SelectFeature(string name)
        {
            var feature = GetFeatures().Where(s => name.Equals((string)s["Name"]!)).First();

            if (_lastSelected != null)
            {
                _lastSelected["State"] = "Unselected";
            }

            feature["State"] = "Selected";

            _lastSelected = feature;

            DataHasChanged();
        }

        public void ShowHighlight(string name)
        {
            var feature = GetFeatures().Where(s => name.Equals((string)s["Name"]!)).First();

            feature["Highlight"] = true;

            DataHasChanged();
        }

        public void HideHighlight()
        {
            foreach (var item in GetFeatures())
            {
                item["Highlight"] = false;
            }

            DataHasChanged();
        }

        protected IProvider<TNative> Provider => _provider;

        public ReactiveCommand<Unit, Unit> Init { get; }

        protected abstract void LoadingImpl(List<TNative> values);
    }
}
