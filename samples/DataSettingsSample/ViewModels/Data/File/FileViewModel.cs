using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.ViewModels
{
    public class FileViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> _isVerified;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        private bool _dirty = true;

        public FileViewModel()
        {
            Command = ReactiveCommand.CreateFromTask<string, bool>(GetModelAsync, outputScheduler: RxApp.MainThreadScheduler);

            _isVerified = Command
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.IsVerified);

            _isLoading = Command.IsExecuting
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.IsLoading);
        }

        private ReactiveCommand<string, bool> Command { get; }

        private async Task<bool> GetModelAsync(string key)
        {
            if (Path == null)
            {
                return false;
            }

            await Task.Delay(TimeSpan.FromSeconds(3));

            string jsonString;

            try
            {
                jsonString = await File.ReadAllTextAsync(Path);
            }
            catch (Exception)
            {

                return false;
            }

            return Equals(JsonConvert.DeserializeObject<JsonObject>(jsonString)?.Name, key);
        }

        private class JsonObject
        {
            [JsonProperty("Name")]
            public string? Name { get; set; }
        }

        public void Verified(string key)
        {
            if (_dirty == true)
            {
                Command.Execute(key).Subscribe();

                _dirty = false;
            }
        }

        public bool IsVerified => _isVerified.Value;

        public bool IsLoading => _isLoading.Value;

        [Reactive]
        public string? Name { get; set; }

        [Reactive]
        public string? Path { get; set; }

        [Reactive]
        public bool IsSelected { get; set; }
    }
}
