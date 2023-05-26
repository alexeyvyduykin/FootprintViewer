using NetTopologySuite.Geometries;

namespace FootprintViewer.Fluent.ViewModels.SidePanel;

public interface IFilter<T>
{
    IObservable<Func<T, bool>> FilterObservable { get; }
}

public interface IAOIFilter
{
    void SetAOIObservable(IObservable<Geometry?> observable);

    bool IsFullCoverAOI { get; set; }

    bool IsAOIActive { get; set; }
}

public interface IAOIFilter<T> : IAOIFilter, IFilter<T>
{
    IObservable<Func<T, bool>> AOIObservable { get; }
}