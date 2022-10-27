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

            var source1 = new JsonSource(DbKeys.Footprints, uri1);
            var source2 = new JsonSource(DbKeys.GroundTargets, uri2);
            var source3 = new JsonSource(DbKeys.Satellites, uri3);
            var source4 = new JsonSource(DbKeys.GroundStations, uri4);
            var source5 = new JsonSource(DbKeys.UserGeometries, uri5);
            var source6 = new JsonSource(DbKeys.Footprints, path1);
            var source7 = new JsonSource(DbKeys.GroundTargets, path2);
            var source8 = new JsonSource(DbKeys.Satellites, path3);
            var source9 = new JsonSource(DbKeys.GroundStations, path4);
            var source10 = new JsonSource(DbKeys.UserGeometries, path5);
            var source11 = new JsonSource(DbKeys.Footprints, path6);
            var source12 = new JsonSource(DbKeys.GroundTargets, path7);
            var source13 = new JsonSource(DbKeys.Satellites, path8);
            var source14 = new JsonSource(DbKeys.GroundStations, path9);
            var source15 = new JsonSource(DbKeys.UserGeometries, path10);

            var connectionString = @"Host=localhost;Username=postgres;Password=user;Database=DataSettingsSampleDatabase2";
            var source16 = new DatabaseSource(DbKeys.Footprints, connectionString, "Footprints");
            var source17 = new DatabaseSource(DbKeys.GroundTargets, connectionString, "GroundTargets");
            var source18 = new DatabaseSource(DbKeys.Satellites, connectionString, "Satellites");
            var source19 = new DatabaseSource(DbKeys.GroundStations, connectionString, "GroundStations");
            var source20 = new DatabaseSource(DbKeys.UserGeometries, connectionString, "UserGeometries");

            var repository = new Repository();

            repository.RegisterSource(DbKeys.Footprints.ToString(), source1);
            repository.RegisterSource(DbKeys.GroundTargets.ToString(), source2);
            repository.RegisterSource(DbKeys.Satellites.ToString(), source3);
            repository.RegisterSource(DbKeys.GroundStations.ToString(), source4);
            repository.RegisterSource(DbKeys.UserGeometries.ToString(), source5);

            repository.RegisterSource(DbKeys.Footprints.ToString(), source6);
            repository.RegisterSource(DbKeys.GroundTargets.ToString(), source7);
            repository.RegisterSource(DbKeys.Satellites.ToString(), source8);
            repository.RegisterSource(DbKeys.GroundStations.ToString(), source9);
            repository.RegisterSource(DbKeys.UserGeometries.ToString(), source10);

            repository.RegisterSource(DbKeys.Footprints.ToString(), source11);
            repository.RegisterSource(DbKeys.GroundTargets.ToString(), source12);
            repository.RegisterSource(DbKeys.Satellites.ToString(), source13);
            repository.RegisterSource(DbKeys.GroundStations.ToString(), source14);
            repository.RegisterSource(DbKeys.UserGeometries.ToString(), source15);

            repository.RegisterSource(DbKeys.Footprints.ToString(), source16);
            repository.RegisterSource(DbKeys.GroundTargets.ToString(), source17);
            repository.RegisterSource(DbKeys.Satellites.ToString(), source18);
            repository.RegisterSource(DbKeys.GroundStations.ToString(), source19);
            repository.RegisterSource(DbKeys.UserGeometries.ToString(), source20);

            services.RegisterConstant(repository, typeof(Repository));

            services.RegisterLazySingleton<MainWindowViewModel>(() => new MainWindowViewModel(resolver));
        }
    }
}
