namespace FootprintViewer.ViewModels
{
    public interface IRandomSourceViewModel : ISourceViewModel
    {
        int GenerateCount { get; set; }
    }

    public class RandomSourceViewModel : IRandomSourceViewModel
    {
        private readonly string _name;

        public RandomSourceViewModel(string name)
        {
            _name = name;
        }

        public string? Name => _name;

        public int GenerateCount { get; set; }
    }
}
