using FootprintViewer.Data;

namespace FootprintViewer.ViewModels
{
    public interface IRandomSourceViewModel : ISourceViewModel
    {
        int GenerateCount { get; set; }
    }

    public class RandomSourceViewModel : IRandomSourceViewModel
    {
        public RandomSourceViewModel()
        {

        }

        public RandomSourceViewModel(IRandomSource randomSource)
        {
            Name = randomSource.Name;
            GenerateCount = randomSource.GenerateCount;
        }

        public string? Name { get; init; }

        public int GenerateCount { get; set; }
    }
}
