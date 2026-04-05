using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Обработка попадания Raytrace-лучей из курсора по игровому миру

public class CursorDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _interactableLayer;

    public static CursorDetector Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public HitResult Detect()
    {
        // Если игра не на паузе (окно раундов)
        if (Time.timeScale != 0f)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Сначала проверяем интерактивные объекты (приоритет выше)
            RaycastHit2D hitInteractable = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, _interactableLayer);
            if (hitInteractable.collider != null)
            {
                return HitResult.Interactable(hitInteractable.collider);
            }

            // Потом землю
            RaycastHit2D hitGround = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, _groundLayer);
            if (hitGround.collider != null)
            {
                return HitResult.Ground(hitGround.point);
            }

            return HitResult.None();
        } else
        {
            return HitResult.None();
        }
    }
}

