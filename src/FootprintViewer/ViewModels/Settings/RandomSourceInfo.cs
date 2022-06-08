namespace FootprintViewer.ViewModels.Settings
{
    public interface IRandomSourceInfo : ISourceInfo
    {
        int GenerateCount { get; set; }
    }

    public class RandomSourceInfo : IRandomSourceInfo
    {
        private readonly string _name;

        public RandomSourceInfo(string name)
        {
            _name = name;
        }

        public string? Name => _name;

        public int GenerateCount { get; set; }
    }
}
