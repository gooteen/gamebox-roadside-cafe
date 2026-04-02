using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// сделать гибко настраиваемым для разных типов спавнеров
public class Spawner : MonoBehaviour
{
    [SerializeField] private SpawnSlot[] _slots; 
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _spawnInterval = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= _spawnInterval)
        {
            TrySpawn();
            timer = 0f;
        }
    }

    void TrySpawn()
    {
        // Найти свободные слоты
        var freeSlots = _slots.Where(s => s.IsFree).ToList();

        if (freeSlots.Count == 0)
            return;

        // Выбрать случайный свободный слот
        var slot = freeSlots[Random.Range(0, freeSlots.Count)];

        SpawnInSlot(slot);
    }

    void SpawnInSlot(SpawnSlot slot)
    {
        GameObject obj = Instantiate(_prefab, slot.point.position, Quaternion.identity);

        slot.currentObject = obj;

        // Подписываемся на уничтожение
        var life = obj.GetComponent<PickableItem>();
        if (life != null)
        {
            life.Init(() => slot.currentObject = null);
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
