using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum SpawnMode {
    TimerBased,
    ValueBased
}
public enum SpawnerType
{
    Kitchen,
    Garden,
    Customer,
    Bin
}


/* 
 * Общая логика спавна сущностей в игре. Работает в двух режимах:
 * 
 * TimerBased - спавнить сущность раз в заданный интервал времени
 * ValueBased - спавнить сущность, когда счетчик достигает заданного значения
 */

public class Spawner : MonoBehaviour
{
    [SerializeField] private SpawnSlot[] _slots; 
    [SerializeField] private GameObject[] _prefabs;
    [SerializeField] private SpawnMode _mode;
    [SerializeField] private SpawnerType _type;
    private float _spawnInterval;
    private int _counterValueForSpawn;

    private float _timer;
    private int _rubbishCounter;

    public int RubbishCounter
    {
        get { return _rubbishCounter; }
        set 
        { 
            _rubbishCounter = value; 
            if (_rubbishCounter >= _counterValueForSpawn)
            {
                _rubbishCounter = 0;
                TrySpawn();
            }
        }
    }

    public int BagCount
    {
        get { return _slots.Where(s => !s.IsFree).ToList().Count; }
    }

    public bool TryTakeItem()
    {
        var slot = _slots.FirstOrDefault(s => !s.IsFree);
        if (slot == null) return false;

        Destroy(slot.currentObject);
        return true;
    }

    public void ClearSlots()
    {
        foreach (var slot in _slots)
        {
            if (slot.currentObject != null)
                Destroy(slot.currentObject);
            slot.currentObject = null;
        }
    }

    void TrySpawn()
    {
        if (_type != SpawnerType.Garden || GameController.Instance.CurrentPollution <= GameController.Instance.cratePollutionLevel)
        {
            // Найти свободные слоты
            var freeSlots = _slots.Where(s => s.IsFree).ToList();

            if (freeSlots.Count == 0)
                return;

            // Выбрать случайный свободный слот
            var slot = freeSlots[Random.Range(0, freeSlots.Count)];
            if (_type == SpawnerType.Kitchen && !GameController.Instance.isPurifierBuilt)
            {
                GameController.Instance.CurrentPollution += GameController.Instance.foodProductionCost;
            }
            SpawnInSlot(slot);
        }
    }

    void SpawnInSlot(SpawnSlot slot)
    {
        int index = Random.Range(0, _prefabs.Length);
        Debug.Log(index);
        GameObject prefab = _prefabs[index];
        GameObject obj = Instantiate(prefab, slot.point.position, Quaternion.identity);

        slot.currentObject = obj;

        // Подписываемся на уничтожение
        var life = obj.GetComponent<PickableItem>();
        if (life != null)
        {
            life.Init(() => slot.currentObject = null);
        }
    }

    void Start()
    {
        if (_mode == SpawnMode.ValueBased)
        {
            _rubbishCounter = 0;
            switch (_type)
            {
                case SpawnerType.Bin:
                    _counterValueForSpawn = GameController.Instance.trashBagSpawnCount;
                    break;
            }
        } else
        {
            switch (_type)
            {
                case SpawnerType.Garden:
                    _spawnInterval = GameController.Instance.crateSpawnRate;
                    break;
                case SpawnerType.Customer:
                    _spawnInterval = GameController.Instance.customerSpawnRate;
                    break;
                case SpawnerType.Kitchen:
                    _spawnInterval = GameController.Instance.foodSpawnRate;
                    break;
            }
        }
    }

    void Update()
    {
        if (_mode == SpawnMode.TimerBased)
        {
            _timer += Time.deltaTime;

            if (_timer >= _spawnInterval)
            {
                TrySpawn();
                _timer = 0f;
            }
        }
    }
}

[System.Serializable]
public class SpawnSlot
{
    public Transform point;   // позиция спавна
    public GameObject currentObject;

    public bool IsFree => currentObject == null;
}
