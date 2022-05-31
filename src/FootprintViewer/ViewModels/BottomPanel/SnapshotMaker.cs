using CliWrap;
using FootprintViewer.FileSystem;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace FootprintViewer.ViewModels
{
    public class SnapshotMaker : ReactiveObject
    {
        private readonly List<string> _extensions;
        private readonly ReactiveCommand<Unit, Unit> _create;
        private readonly ReactiveCommand<Unit, Unit> _openFolder;
        private readonly SolutionFolder _solutionFolder;

        public SnapshotMaker()
        {
            _solutionFolder = new SolutionFolder("snapshots");

            _extensions = new List<string>() { "*.png", "*.jrpg", "*.pdf" };

            SelectedExtension = Extensions.FirstOrDefault();

            _create = ReactiveCommand.CreateFromObservable<Unit, Unit>(s =>
                Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1)));

            var openFolderCommand = Cli.Wrap("cmd").WithArguments($"/K start {_solutionFolder.FolderDirectory} && exit");

            _openFolder = ReactiveCommand.CreateFromObservable<Unit, Unit>(s =>
                Observable.Start(() => { openFolderCommand.ExecuteAsync(); }));
        }

        public ICommand Create => _create;

        public ICommand OpenFolder => _openFolder;

        [Reactive]
        public string? SelectedExtension { get; set; }

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
}
