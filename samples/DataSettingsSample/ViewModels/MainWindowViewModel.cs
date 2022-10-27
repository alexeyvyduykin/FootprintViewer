using DataSettingsSample.Data;
using DataSettingsSample.Models;
using FDataSettingsSample.Models;
using FootprintViewer.ViewModels.Dialogs;
using FootprintViewer.ViewModels.Navigation;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Input;

namespace DataSettingsSample.ViewModels
{
    public class MainWindowViewModel : RoutableViewModel
    {
        private readonly Repository _repository;

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
                    yield return new ItemViewModel() { Name = $"{footprint.Value}" };
                }
                else if (obj is GroundTarget groundTarget)
                {
                    yield return new ItemViewModel() { Name = $"{groundTarget.Value}" };
                }
                else if (obj is Satellite satellite)
                {
                    yield return new ItemViewModel() { Name = $"{satellite.Value}" };
                }
                else if (obj is GroundStation groundStation)
                {
                    yield return new ItemViewModel() { Name = $"{groundStation.Value}" };
                }
                else if (obj is UserGeometry userGeometry)
                {
                    yield return new ItemViewModel() { Name = $"{userGeometry.Value}" };
                }
                else if (obj is CustomJsonObject jsonObject)
                {
                    foreach (var item in jsonObject.Values)
                    {
                        yield return new ItemViewModel() { Name = $"{item}" };
                    }
                }
            }

            Func<object, IEnumerable<ItemViewModel>> Converter1 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter2 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter3 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter4 = converter;
            Func<object, IEnumerable<ItemViewModel>> Converter5 = converter;

            FootprintList = new ListViewModel("footprints", _repository, Converter1);
            GroundTargetList = new ListViewModel("groundTargets", _repository, Converter2);
            SatelliteList = new ListViewModel("satellites", _repository, Converter3);
            GroundStationList = new ListViewModel("groundStations", _repository, Converter4);
            UserGeometryList = new ListViewModel("userGeometries", _repository, Converter5);

            FootprintList.Load.Execute().Subscribe();
            GroundTargetList.Load.Execute().Subscribe();
            SatelliteList.Load.Execute().Subscribe();
            GroundStationList.Load.Execute().Subscribe();
            UserGeometryList.Load.Execute().Subscribe();

            OptionCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var settingsDialog = new SettingsViewModel(_repository);

                DialogStack().To(settingsDialog);

                var dialogResult = await settingsDialog.GetDialogResultAsync();

                DialogStack().Clear();
            });

            IsMainContentEnabled = this.WhenAnyValue(s => s.DialogNavigationStack.IsDialogOpen, (s) => !s).ObserveOn(RxApp.MainThreadScheduler);

            this.WhenAnyValue(s => s.DialogNavigationStack.CurrentPage)
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(s => s.SetActive())
                .Subscribe();
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
