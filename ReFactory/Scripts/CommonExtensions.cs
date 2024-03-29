﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using ReFactory.UISystem;

namespace ReFactory;

public static class CommonExtensions
{
    public static T Random<T>(this IList<T> collection) => collection[System.Random.Shared.Next(0, collection.Count)];
    
    public static IEnumerable<UIElement> Flatten(this UIElement element)
    {
        yield return element;
        element.Children.RemoveAll(el => el == null);
        foreach (UIElement child in element.Children)
        {
            foreach (var childFlatten in child.Flatten())
            {
                yield return childFlatten;
            }
        }
    }
    
    [Pure]
    public static Rectangle SetCenter(this Rectangle rectangle, Point center)
    {
        var rect = new Rectangle(rectangle.Location - (rectangle.Center - center), rectangle.Size);
        return rect;
    }

    [Pure]
    public static Rectangle SetScale(this Rectangle rectangle, float amount)
    {
        var rect = rectangle;
        rect.Inflate(amount, amount);
        return rect;
    }
    
    [Pure]
    public static Rectangle SetSize(this Rectangle rectangle, Point newSize)
    {
        var rect = rectangle;
        rect.Size = newSize;
        rect = rect.SetCenter(rectangle.Center);
        return rect;
    }

    public static Rectangle Lerp(Rectangle from, Rectangle to, float t)
    {
        return new Rectangle(
            Vector2.Lerp(from.Location.ToVector2(), to.Location.ToVector2(), t).ToPoint(),
            Vector2.Lerp(from.Size.ToVector2(), to.Size.ToVector2(), t).ToPoint());
    }

    public static Rectangle LerpTo(this Rectangle from, Rectangle to, float t) => Lerp(from, to, t);

    public static float Range(this Random random, float min, float max)
    {
        return (random.NextSingle() * (max - min)) + min;
    }

    public static Vector2 RotationToVector2(this float rotation)
    {
        switch (rotation)
        {
            default:
                return new Vector2(0, -1);
            case 90:
                return new Vector2(1, 0);
            case 180:
                return new Vector2(0, 1);
            case 270:
                return new Vector2(-1, 0);
        }
    }
}