using FootprintViewer.Data.Sources;

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

        public RandomSourceViewModel(IRandomSource randomSource) : this(randomSource.Name)
        {
            
        }

        public string? Name => _name;

        public int GenerateCount { get; set; }
    }
}
