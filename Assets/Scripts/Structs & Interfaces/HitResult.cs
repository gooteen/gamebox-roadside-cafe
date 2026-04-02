using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType
{
    None,
    Ground,
    Interactable
}

public struct HitResult
{
    public HitType Type;
    public Vector2 Point;
    public IInteractable InteractableObject;

    public static HitResult Ground(Vector2 point) => new HitResult { Type = HitType.Ground, Point = point };
    public static HitResult Interactable(Collider2D col) => new HitResult
    {
        Type = HitType.Interactable,
        InteractableObject = col.GetComponent<IInteractable>()
    };
    public static HitResult None() => new HitResult { Type = HitType.None };
}
