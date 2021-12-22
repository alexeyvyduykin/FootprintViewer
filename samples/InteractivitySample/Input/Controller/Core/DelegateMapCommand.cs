﻿using System;

namespace InteractivitySample.Input.Controller.Core
{
    public class DelegateMapCommand<T> : DelegateViewCommand<T>
        where T : InputEventArgs
    {
        public DelegateMapCommand(Action<IMapView, IController, T> handler)
            : base((v, c, e) => handler((IMapView)v, c, e))
        {
        }
    }
}