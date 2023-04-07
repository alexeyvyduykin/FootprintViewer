using NetTopologySuite.Geometries;
using System;

namespace FootprintViewer.Fluent.ViewModels.SidePanel;

public interface IFilter<T>
{
    IObservable<Func<T, bool>> FilterObservable { get; }
}

public interface IAOIFilter
{
    Geometry? AOI { get; set; }

    bool IsFullCoverAOI { get; set; }

    bool IsAOIActive { get; set; }
}

public interface IAOIFilter<T> : IAOIFilter, IFilter<T>
{
    IObservable<Func<T, bool>> AOIFilterObservable { get; }
}