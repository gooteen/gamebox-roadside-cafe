using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Добавить красивые хэдеры и свойства

    [SerializeField] private float _currentPollution;
    [SerializeField] private float _pollutionCheckpoint; // Степень загрязнения в начале текущего круга

    [SerializeField] private int _currentMoney;
    [SerializeField] private int _moneyCheckpoint; // Количество денег в начале текущего круга

    public bool _isPurifierBuilt;
    public GameObject _purifier;

    public bool _isTrashBinBuilt;
    public GameObject _trashBin;

    public bool _isRecycleTrashBinBuilt;
    public GameObject _recycleTrashBin;

    public Transform _benchPoint;
    public Transform _exitPoint;

    public Spawner _sellingPoint;

    [SerializeField] private ScenePrefabCatalogSO _scenePrefabCatalog;

    public float CurrentPollution
    {
        get { return _currentPollution; }
        set
        {
            _currentPollution = value;
            if (_currentPollution < 0)
                _currentPollution = 0;
        }
    }

    public int CurrentMoney
    {
        get { return _currentMoney; }
        set
        {
            _currentMoney = value;
            if (_currentMoney < 0)
                _currentMoney = 0;
        }
    }

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