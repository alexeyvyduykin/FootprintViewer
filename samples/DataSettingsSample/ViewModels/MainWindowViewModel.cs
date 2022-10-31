using DataSettingsSample.Data;
using DataSettingsSample.Models;
using FDataSettingsSample.Models;
using FootprintViewer.ViewModels.Dialogs;
using FootprintViewer.ViewModels.Navigation;
using ReactiveUI;
using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DataSettingsSample.ViewModels
{
    public class MainWindowViewModel : RoutableViewModel
    {
        private readonly Repository _repository;
        private readonly Dictionary<DbKeys, ListViewModel> _dict = new();

        public MainWindowViewModel(IReadonlyDependencyResolver resolver)
        {
            _repository = resolver.GetExistingService<Repository>();

            static IEnumerable<ItemViewModel> converter(object obj)
            {
                if (obj is double value)
                {
                    yield return new ItemViewModel() { Name = $"{value}" };
                }
                else if (obj is Footprint footprint)
                {
                    yield return new ItemViewModel() { Name = $"{footprint.Name}: {footprint.Value}" };
                }
                else if (obj is GroundTarget groundTarget)
                {
                    yield return new ItemViewModel() { Name = $"{groundTarget.Name}: {groundTarget.Value}" };
                }
                else if (obj is Satellite satellite)
                {
                    yield return new ItemViewModel() { Name = $"{satellite.Name}: {satellite.ValueInt}" };
                }
                else if (obj is GroundStation groundStation)
                {
                    yield return new ItemViewModel() { Name = $"{groundStation.Name}: {groundStation.Value}" };
                }
                else if (obj is UserGeometry userGeometry)
                {
                    yield return new ItemViewModel() { Name = $"{userGeometry.Name}: {userGeometry.Value}" };
                }
            }

            Func<object, IEnumerable<ItemViewModel>> Converter1 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter2 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter3 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter4 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter5 = converter;

            FootprintList = new ListViewModel(DbKeys.Footprints, _repository, Converter1);
            GroundTargetList = new ListViewModel(DbKeys.GroundTargets, _repository, Converter2);
            SatelliteList = new ListViewModel(DbKeys.Satellites, _repository, Converter3);
            GroundStationList = new ListViewModel(DbKeys.GroundStations, _repository, Converter4);
            UserGeometryList = new ListViewModel(DbKeys.UserGeometries, _repository, Converter5);

            _dict.Add(DbKeys.Footprints, FootprintList);
            _dict.Add(DbKeys.GroundTargets, GroundTargetList);
            _dict.Add(DbKeys.Satellites, SatelliteList);
            _dict.Add(DbKeys.GroundStations, GroundStationList);
            _dict.Add(DbKeys.UserGeometries, UserGeometryList);

            UpdateLists(_dict.Keys.ToList());

            OptionCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var settingsDialog = new SettingsViewModel(_repository);

                DialogStack().To(settingsDialog);

                var dialogResult = await settingsDialog.GetDialogResultAsync();

                DialogStack().Clear();

                if (dialogResult.Result is IList<DbKeys> dirtyKeys)
                {
                    UpdateLists(dirtyKeys);
                }
            });

            IsMainContentEnabled = this.WhenAnyValue(s => s.DialogNavigationStack.IsDialogOpen, (s) => !s).ObserveOn(RxApp.MainThreadScheduler);

            this.WhenAnyValue(s => s.DialogNavigationStack.CurrentPage)
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(s => s.SetActive())
                .Subscribe();
        }

        private void UpdateLists(IList<DbKeys> dirtyKeys)
        {
            foreach (var key in dirtyKeys)
            {
                _dict[key].Load.Execute().Subscribe();
            }
        }

        public DialogNavigationStack DialogNavigationStack => DialogStack();

        public ICommand OptionCommand { get; }

        public ListViewModel FootprintList { get; set; }

        public ListViewModel GroundTargetList { get; set; }

        public ListViewModel SatelliteList { get; set; }

        public ListViewModel GroundStationList { get; set; }

        public ListViewModel UserGeometryList { get; set; }

        public IObservable<bool> IsMainContentEnabled { get; }
    }
}
