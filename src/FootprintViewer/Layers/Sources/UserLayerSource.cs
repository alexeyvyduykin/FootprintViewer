using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers
{
    public interface IUserLayerSource : ILayer
    {
        void EditFeature(IFeature feature);

        void AddUserGeometry(IFeature feature, UserGeometryType type);
    }

    public class UserLayerSource : WritableLayer, IUserLayerSource
    {
        private readonly UserGeometryProvider _provider;

        public UserLayerSource(UserGeometryProvider provider)
        {
            _provider = provider;

            Update = ReactiveCommand.CreateFromTask(UpdateAsync);

            Loading = ReactiveCommand.Create<List<UserGeometryInfo>>(LoadingImpl);

            Update.InvokeCommand(Loading);

            provider.Update.Select(_ => Unit.Default).InvokeCommand(Update);

            provider.Loading.InvokeCommand(Loading);
        }

        private ReactiveCommand<Unit, List<UserGeometryInfo>> Update { get; }

        private ReactiveCommand<List<UserGeometryInfo>, Unit> Loading { get; }

        private static string GenerateName(UserGeometryType type)
        {
            return $"{type}_{new string($"{Guid.NewGuid()}".Replace("-", "").Take(10).ToArray())}";
        }

        private void LoadingImpl(List<UserGeometryInfo> userGeometries)
        {
            var arr = userGeometries
                .Where(s => s.Geometry != null)
                .Select(s => s.Geometry.Geometry!.ToFeature(s.Name));

            Clear();
            AddRange(arr);
        }

        private async Task<List<UserGeometryInfo>> UpdateAsync()
        {
            return await _provider.GetValuesAsync(null);
        }

        public void EditFeature(IFeature feature)
        {
            if (feature is GeometryFeature gf)
            {
                Task.Run(async () =>
                {
                    if (gf.Fields.Contains("Name") == true)
                    {
                        var name = (string)gf["Name"]!;

                        var geometry = gf.Geometry!;

                        await _provider.UpdateGeometry(name, geometry);
                    }
                });
            }
        }

        public void AddUserGeometry(IFeature feature, UserGeometryType type)
        {
            if (feature is GeometryFeature gf)
            {
                var name = GenerateName(type);

                gf["Name"] = name;

                Add(gf);

                Task.Run(async () =>
                {
                    var model = new UserGeometry()
                    {
                        Type = type,
                        Name = name,
                        Geometry = gf.Geometry
                    };

                    await _provider.AddAsync(new UserGeometryInfo(model));
                });
            }
        }
    }
}
