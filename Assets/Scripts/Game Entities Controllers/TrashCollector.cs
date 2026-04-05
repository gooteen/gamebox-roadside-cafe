using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCollector : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    private float _timer;

    void Update()
    {
        
        _timer += Time.deltaTime;

        if (_timer >= GameController.Instance.trashCollectionRate)
        {
            _anim.Play("Trash Collector Drive-Through");
            CollectBags();
            _timer = 0f;
        }
        
    }

    private void CollectBags()
    {
        if (GameController.Instance.isTrashBinBuilt)
        {
            GameController.Instance.CurrentMoney -= GameController.Instance.trashBin.BagCount * GameController.Instance.regularTrashBagCollectionPrice;
            GameController.Instance.trashBin.ClearSlots();
        }

        if (GameController.Instance.isRecycleTrashBinBuilt)
        {
            GameController.Instance.CurrentMoney -= GameController.Instance.recycleTrashBin.BagCount * GameController.Instance.recycleTrashBagCollectionPrice;
            GameController.Instance.recycleTrashBin.ClearSlots();
        }

    }
}
