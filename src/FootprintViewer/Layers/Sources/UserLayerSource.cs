using FootprintViewer.Data;
using Mapsui;
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

    public class UserLayerSource : BaseLayerSource<UserGeometry>, IUserLayerSource
    {
        public UserLayerSource(IEditableProvider<UserGeometry> provider) : base(provider)
        {
            Update = ReactiveCommand.CreateFromTask(UpdateAsync);

            Update.InvokeCommand(Init);

            provider.Update.InvokeCommand(Update);
        }

        private ReactiveCommand<Unit, List<UserGeometry>> Update { get; }

        private static string GenerateName(UserGeometryType type)
        {
            return $"{type}_{new string($"{Guid.NewGuid()}".Replace("-", "").Take(10).ToArray())}";
        }

        protected override void LoadingImpl(List<UserGeometry> userGeometries)
        {
            var arr = userGeometries
                .Where(s => s.Geometry != null)
                .Select(s => s.Geometry!.ToFeature(s.Name!));

            Clear();
            AddRange(arr);
            DataHasChanged();
        }

        private async Task<List<UserGeometry>> UpdateAsync()
        {
            return await Provider.GetNativeValuesAsync(null);
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

                        await ((IEditableProvider<UserGeometry>)Provider).EditAsync(name,
                            new UserGeometry()
                            {
                                Geometry = geometry
                            });
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

                    await ((IEditableProvider<UserGeometry>)Provider).AddAsync(model);
                });
            }
        }
    }
}
