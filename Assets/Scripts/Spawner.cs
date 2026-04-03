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
    Other
}


// сделать наследование позже?
public class Spawner : MonoBehaviour
{
    [SerializeField] private SpawnSlot[] _slots; 
    [SerializeField] private GameObject[] _prefabs;
    [SerializeField] private SpawnMode _mode;
    [SerializeField] private SpawnerType _type;
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private int _counterValueForSpawn = 5;

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
