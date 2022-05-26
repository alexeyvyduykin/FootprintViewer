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
    public interface IUserLayerSource : ILayerSource
    {
        void EditFeature(IFeature feature);

        void AddUserGeometry(IFeature feature, UserGeometryType type);
    }

    public class UserLayerSource : BaseLayerSource<UserGeometryInfo>, IUserLayerSource
    {
        public UserLayerSource(IEditableProvider<UserGeometryInfo> provider) : base(provider)
        {
            Update = ReactiveCommand.CreateFromTask(UpdateAsync);

            Update.InvokeCommand(Init);

            provider.Update.InvokeCommand(Update);
        }

        private ReactiveCommand<Unit, List<UserGeometryInfo>> Update { get; }

        private static string GenerateName(UserGeometryType type)
        {
            return $"{type}_{new string($"{Guid.NewGuid()}".Replace("-", "").Take(10).ToArray())}";
        }

        protected override void LoadingImpl(List<UserGeometryInfo> userGeometries)
        {
            var arr = userGeometries
                .Where(s => s.Geometry != null)
                .Select(s => s.Geometry.Geometry!.ToFeature(s.Name));

            Clear();
            AddRange(arr);
            DataHasChanged();
        }

        private async Task<List<UserGeometryInfo>> UpdateAsync()
        {
            return await Provider.GetValuesAsync(null);
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

                        await ((IEditableProvider<UserGeometryInfo>)Provider).EditAsync(name,
                            new UserGeometryInfo(
                                new UserGeometry()
                                {
                                    Geometry = geometry
                                }));
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

                    await ((IEditableProvider<UserGeometryInfo>)Provider).AddAsync(new UserGeometryInfo(model));
                });
            }
        }
    }
}
