﻿using System;

namespace ReFactory.UISystem;

public interface IDragHandler : IHasInteractiveRect
{
    public Type GetType();
    public bool IsDropTarget { get; }

    public bool OnDrag()
    {
        return true;
    }
    public void OnDragCancelled() {}
}