using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncFilteringSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<List<User>> _users;
        private readonly ObservableAsPropertyHelper<bool> _isUpdating;

        public MainWindowViewModel()
        {
            LoadUsers = ReactiveCommand.CreateFromTask(LoadUsersAsync);

            _users = LoadUsers.ToProperty(this, x => x.Users, scheduler: RxApp.MainThreadScheduler);

            this.WhenAnyValue(s => s.CurrentWidth).Throttle(TimeSpan.FromSeconds(0.4)).Subscribe(_ => LoadUsers.Execute().Subscribe());

            _isUpdating = LoadUsers.IsExecuting.ToProperty(this, x => x.IsUpdating);
        }

        private async Task<List<User>> LoadUsersAsync()
        {          
            return await Task.Run(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                Random random = new Random();

                var list = new List<User>();

                for (int i = 0; i < 100; i++)
                {
                    list.Add(new User($"Item{(i + 1):000}", random.Next(0, 100), random.Next(0, 100)));
                }

                return list;
            });
        }

        public ReactiveCommand<Unit, List<User>> LoadUsers { get; }

        [Reactive]
        public int CurrentWidth { get; set; }

        public bool IsUpdating => _isUpdating.Value;

        public List<User> Users => _users.Value;

        //[Reactive]
        //public ObservableCollection<User> Users { get; private set; }

    }

    public class User : ReactiveObject
    {
        private readonly string _name;
        private readonly int _width;
        private readonly int _height;

        public User(string name, int width, int height)
        {
            _name = name;
            _width = width;
            _height = height;
        }

        public string Description => $"{Name}: Width={Width}; Height={Height}";

        public string Name => _name;

        public int Width => _width;

        public int Height => _height;
    }
}
