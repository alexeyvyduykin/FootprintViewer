using Mapsui;
using Mapsui.Layers;
using System;

namespace FootprintViewer.Styles;

public class FeatureManager
{
    private ILayer? _layer;
    private ILayer? _lastLayer;
    private IFeature? _lastSelectFeature;
    private IFeature? _lastEnterFeature;
    private Action<IFeature>? _selectAction;
    private Action<IFeature>? _unselectAction;
    private Action<IFeature>? _enterAction;
    private Action<IFeature>? _leaveAction;

    public FeatureManager OnLayer(ILayer? layer)
    {
        if (layer != null)
        {
            _lastLayer = _layer;

            _layer = layer;
        }

        return this;
    }

    public FeatureManager WithSelect(Action<IFeature> action)
    {
        _selectAction = action;

        return this;
    }

    public FeatureManager WithUnselect(Action<IFeature> action)
    {
        _unselectAction = action;

        return this;
    }

    public FeatureManager WithEnter(Action<IFeature> action)
    {
        _enterAction = action;

        return this;
    }

    public FeatureManager WithLeave(Action<IFeature> action)
    {
        _leaveAction = action;

        return this;
    }

    public void Select(IFeature? feature)
    {
        if (feature != null)
        {
            if (_lastSelectFeature != null)
            {
                _unselectAction?.Invoke(_lastSelectFeature);

                _lastLayer?.DataHasChanged();
            }

            _selectAction?.Invoke(feature);

            _lastSelectFeature = feature;

            _layer?.DataHasChanged();
        }
    }

    public void Unselect()
    {
        //if (_lastPointeroverFeature != null)
        //{
        //    _lastPointeroverFeature[InteractiveFields.Hover] = false;

        //    _lastPointeroverLayer?.DataHasChanged();
        //}


        if (_lastSelectFeature != null)
        {
            _unselectAction?.Invoke(_lastSelectFeature);

            _lastLayer?.DataHasChanged();
        }
    }

    public void Enter(IFeature? feature)
    {
        if (feature != null)
        {
            if (_lastEnterFeature != null)
            {
                _leaveAction?.Invoke(_lastEnterFeature);
            }

            _enterAction?.Invoke(feature);

            _lastEnterFeature = feature;

            _layer?.DataHasChanged();
        }
    }

    public void Leave()
    {
        if (_lastEnterFeature != null)
        {
            _leaveAction?.Invoke(_lastEnterFeature);

            _lastLayer?.DataHasChanged();
        }
    }
}
