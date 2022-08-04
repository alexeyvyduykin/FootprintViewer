using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers
{
    public interface ITargetLayerSource : IProvider
    {
        void SetProvider(IProvider provider);

        ReactiveCommand<IEnumerable<IFeature>?, string[]?> Refresh { get; }

        IFeature? GetFeature(string name);

        IList<IFeature> GetFeatures();

        double MinVisible { get; set; }

        double MaxVisible { get; set; }
    }

    public class TargetLayerSource : ITargetLayerSource
    {
        private IProvider? _memoryProvider;
        private MRect? _lastExtent;
        private List<IFeature> _activeFeatures;

        public TargetLayerSource()
        {
            Refresh = ReactiveCommand.Create<IEnumerable<IFeature>?, string[]?>(s => RefreshImpl(s));

            _activeFeatures = new List<IFeature>();

            MinVisible = 0;

            MaxVisible = double.MaxValue;
        }

        public void SetProvider(IProvider provider)
        {
            _memoryProvider = provider;
        }

        public IList<IFeature> GetFeatures()
        {
            return _activeFeatures.ToList();
        }

        public ReactiveCommand<IEnumerable<IFeature>?, string[]?> Refresh { get; }

        public string? CRS { get; set; }

        public double MinVisible { get; set; }

        public double MaxVisible { get; set; }

        public IFeature? GetFeature(string name)
        {
            return _activeFeatures.Where(s => name.Equals((string)s["Name"]!)).FirstOrDefault();
        }

        public async Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
        {
            if (_memoryProvider == null)
            {
                return new List<IFeature>();
            }

            if (MinVisible > fetchInfo.Resolution || MaxVisible < fetchInfo.Resolution)
            {
                Refresh.Execute(null).Subscribe();

                return new List<IFeature>();
            }

            var res = await _memoryProvider.GetFeaturesAsync(fetchInfo);

            if (fetchInfo.Extent.Equals(_lastExtent) == false)
            {
                _activeFeatures = res.ToList();

                Refresh.Execute(res).Subscribe();

                _lastExtent = fetchInfo.Extent;
            }

            return res;
        }

        public MRect? GetExtent() => _memoryProvider?.GetExtent();

        private static string[]? RefreshImpl(IEnumerable<IFeature>? features)
        {
            if (features == null)
            {
                return null;
            }

            return features.Where(s => s.Fields.Contains("Name")).Select(s => (string)s["Name"]!).ToArray();
        }
    }
}
