using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    // Добавить красивые хэдеры и свойства

    [SerializeField] private float _currentPollution;
    [SerializeField] private float _pollutionCheckpoint; // Степень загрязнения в начале текущего круга

    [SerializeField] private int _currentMoney;
    [SerializeField] private int _moneyCheckpoint; // Количество денег в начале текущего круга

    public event System.Action<float> OnPollutionChanged;
    public event System.Action<int> OnMoneyChanged;

    public float roundDuration;

    public float decompositionCost;
    public float foodProductionCost;

    public int purifierPrice;
    public int regularTrashBinPrice;
    public int recycleTrashBinPrice;

    public float cratePollutionLevel;
    public float groundPollutionLevel;

    public float squirellPollutionLevel;
    public float treePollutionLevel;

    public float trashCollectionRate;
    public int regularTrashBagCollectionPrice;
    public int recycleTrashBagCollectionPrice;

    public int moneyPerServing;
    public int moneyPerCrate;

    public float rubbishTimeToDecompose;
    public float bagTimeToDecompose;
    
    public bool isPurifierBuilt;

    public bool isTrashBinBuilt;
    public Spawner trashBin;

    public bool isRecycleTrashBinBuilt;
    public Spawner recycleTrashBin;

    public Transform benchPoint;
    public Transform exitPoint;

    public Spawner sellingPoint;

    [SerializeField] private ScenePrefabCatalogSO _scenePrefabCatalog;
    [SerializeField] private Tilemap _groundTilemap;

    private float _timer;

    public float CurrentPollution
    {
        get { return _currentPollution; }
        set
        {
            _currentPollution = value;
            if (_currentPollution < 0)
                _currentPollution = 0;
            
            _groundTilemap.color = new Color(_groundTilemap.color.r, (100f - _currentPollution) / 100f, _groundTilemap.color.b, _groundTilemap.color.a);
            OnPollutionChanged?.Invoke(_currentPollution);
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
            OnMoneyChanged?.Invoke(_currentMoney);
        }
    }

    public ScenePrefabCatalogSO ScenePrefabCatalog
    {
        get { return _scenePrefabCatalog; }
    }

    public static GameController Instance { get; private set; }

    private void Start()
    {
        _currentPollution = 0;
        _pollutionCheckpoint = 0;

        _currentMoney = 0;
        _moneyCheckpoint = 0;
    }

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
        _timer += Time.deltaTime;

        if (_timer >= roundDuration)
        {
            UserInterfaceController.Instance.ShowScorePanel(_currentMoney - _moneyCheckpoint, _currentPollution - _pollutionCheckpoint, _currentPollution);
            _pollutionCheckpoint = _currentPollution;
            _moneyCheckpoint = _currentMoney;
            _timer = 0f;
        }

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