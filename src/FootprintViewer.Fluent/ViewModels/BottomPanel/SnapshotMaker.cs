using CliWrap;
using FootprintViewer.FileSystem;
using FootprintViewer.Helpers;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;
using Splat;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace FootprintViewer.Fluent.ViewModels;

public sealed class SnapshotMaker : ViewModelBase
{
    private readonly List<string> _extensions;
    private readonly ReactiveCommand<Unit, Unit> _create;
    private readonly ReactiveCommand<Unit, Unit> _openFolder;
    private readonly SolutionFolder _solutionFolder;

    public SnapshotMaker(IReadonlyDependencyResolver dependencyResolver)
    {
        var map = (Map)dependencyResolver.GetExistingService<IMap>();
        var mapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();

        _solutionFolder = new SolutionFolder("snapshots");

        _extensions = ValidExtensions();

        SelectedExtension = Extensions.FirstOrDefault() ?? ToExt(SKEncodedImageFormat.Png);

        _create = ReactiveCommand.CreateFromObservable<Unit, Unit>(s =>
        Observable.Start(() =>
        {
            Save(mapNavigator.Viewport, map.Layers);
        }).Delay(TimeSpan.FromSeconds(1)));

        _openFolder = ReactiveCommand.CreateFromObservable<Unit, Unit>(s =>
        Observable.Start(() =>
        {
            var path = _solutionFolder.FolderDirectory;
            Cli.Wrap("cmd").WithArguments($"/K start {path} && exit").ExecuteAsync();
        }));
    }

    private static List<string> ValidExtensions()
    {
        return new[] { SKEncodedImageFormat.Png, SKEncodedImageFormat.Jpeg, SKEncodedImageFormat.Webp }.Select(s => ToExt(s)).ToList();
    }

    private static string ToExt(SKEncodedImageFormat type)
    {
        return $"*.{Enum.GetName(type)!.ToLower()}";
    }

    private void Save(IReadOnlyViewport? viewport, IEnumerable<ILayer> layers)
    {
        using var memoryStream = new Mapsui.Rendering.Skia.MapRenderer().RenderToBitmapStream(viewport, layers);

        if (memoryStream != null)
        {
            var ext = SelectedExtension.Replace("*.", "");
            var type = Enum.Parse<SKEncodedImageFormat>(ext[..1].ToUpper() + ext[1..].ToString());

            var snapshot = UniqueNameHelper.Create("Snapshot", ext);
            var path = Path.Combine(_solutionFolder.FolderDirectory, snapshot);

            using var image = SKImage.FromEncodedData(memoryStream.ToArray());
            using var data = image.Encode(type, 100);
            using var stream = File.OpenWrite(path);

            data.SaveTo(stream);
        }
    }

    public ICommand Create => _create;

    public ICommand OpenFolder => _openFolder;

    [Reactive]
    public string SelectedExtension { get; set; }

    public List<string> Extensions => _extensions;

    //private static void WindowsExplorerOpen(string path)
    //{
    //    CommandLine(path, $"start {path}");
    //}

    //private static void CommandLine(string workingDirectory, string Command)
    //{
    //    ProcessStartInfo ProcessInfo;
    //    Process? Process;

    //    ProcessInfo = new("cmd.exe", "/K " + Command + " && exit");
    //    ProcessInfo.WorkingDirectory = workingDirectory;
    //    ProcessInfo.CreateNoWindow = true;
    //    ProcessInfo.UseShellExecute = true;
    //    ProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;

    //    Process = Process.Start(ProcessInfo);
    //    Process?.WaitForExit();
    //}
}
