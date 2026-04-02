using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D walkCursor;
    public Texture2D interactCursor;
    public Texture2D disabledCursor;

    public static CursorController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void UpdateCursor(HitResult hit, Vector2 playerPos)
    {
        switch (hit.Type)
        {
            case HitType.Ground:
                SetCursor(walkCursor);
                break;

            case HitType.Interactable:
                if (hit.InteractableObject != null && hit.InteractableObject.CanInteract(playerPos))
                    SetCursor(interactCursor);
                else
                    SetCursor(disabledCursor);
                break;

            default:
                SetCursor(null);
                break;
        }
    }

    void SetCursor(Texture2D texture)
    {
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
    }
}
