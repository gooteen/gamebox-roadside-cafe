using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private ScenePrefabCatalogSO _scenePrefabCatalog;

    public ScenePrefabCatalogSO ScenePrefabCatalog
    {
        get { return _scenePrefabCatalog; }
    }

    public static GameController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        HitResult hit = CursorDetector.Instance.Detect();

        CursorController.Instance.UpdateCursor(hit, PlayerController.Instance.gameObject.transform.position);

        if (Input.GetMouseButtonDown(0))
        {
            PlayerController.Instance.HandleClick(hit);
        }

        if (Input.GetMouseButtonDown(1))
        {
            PlayerController.Instance.DropObject();
        }
    }
}