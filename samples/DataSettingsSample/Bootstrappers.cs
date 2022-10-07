using DataSettingsSample.Data;
using DataSettingsSample.ViewModels;
using Splat;
using System;
using System.IO;
using System.Reflection;

namespace DataSettingsSample
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.InitializeSplat();

            RegisterViewModels(services, resolver);
        }

        private static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var uri1 = new Uri("avares://DataSettingsSample/Assets/File_test_01.json");
            var uri2 = new Uri("avares://DataSettingsSample/Assets/File_test_02.json");
            var uri3 = new Uri("avares://DataSettingsSample/Assets/File_test_03.json");
            var uri4 = new Uri("avares://DataSettingsSample/Assets/File_test_04.json");
            var uri5 = new Uri("avares://DataSettingsSample/Assets/File_test_05.json");
            var path1 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_06.json"));
            var path2 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_07.json"));
            var path3 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_08.json"));
            var path4 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_09.json"));
            var path5 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_10.json"));
            var path6 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_11.json"));
            var path7 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_12.json"));
            var path8 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_13.json"));
            var path9 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_14.json"));
            var path10 = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets", "File_test_15.json"));

            var source1 = new StreamSource<CustomJsonObject>(uri1);
            var source2 = new StreamSource<CustomJsonObject>(uri2);
            var source3 = new StreamSource<CustomJsonObject>(uri3);
            var source4 = new StreamSource<CustomJsonObject>(uri4);
            var source5 = new StreamSource<CustomJsonObject>(uri5);
            var source6 = new PathSource<CustomJsonObject>(path1);
            var source7 = new PathSource<CustomJsonObject>(path2);
            var source8 = new PathSource<CustomJsonObject>(path3);
            var source9 = new PathSource<CustomJsonObject>(path4);
            var source10 = new PathSource<CustomJsonObject>(path5);
            var source11 = new PathSource<CustomJsonObject>(path6);
            var source12 = new PathSource<CustomJsonObject>(path7);
            var source13 = new PathSource<CustomJsonObject>(path8);
            var source14 = new PathSource<CustomJsonObject>(path9);
            var source15 = new PathSource<CustomJsonObject>(path10);

            var repository = new Repository();

            repository.RegisterSource("footprints", source1);
            repository.RegisterSource("groundTargets", source2);
            repository.RegisterSource("satellites", source3);
            repository.RegisterSource("groundStations", source4);
            repository.RegisterSource("userGeometries", source5);

            repository.RegisterSource("footprints", source6);
            repository.RegisterSource("groundTargets", source7);
            repository.RegisterSource("satellites", source8);
            repository.RegisterSource("groundStations", source9);
            repository.RegisterSource("userGeometries", source10);

            repository.RegisterSource("footprints", source11);
            repository.RegisterSource("groundTargets", source12);
            repository.RegisterSource("satellites", source13);
            repository.RegisterSource("groundStations", source14);
            repository.RegisterSource("userGeometries", source15);

            services.RegisterConstant(repository, typeof(Repository));

            services.RegisterLazySingleton<MainWindowViewModel>(() => new MainWindowViewModel(resolver));
        }
    }
}
