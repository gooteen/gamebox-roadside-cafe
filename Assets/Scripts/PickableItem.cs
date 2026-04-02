using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour, IInteractable
{
    public ItemType type;
    [SerializeField] private float _interactDistance;
    private System.Action _onDestroyed;

    public bool CanInteract(Vector3 fromPosition)
    {
        if ((transform.position - fromPosition).magnitude >= _interactDistance)
        {
            Debug.Log("NEA");
            return false;
        } else
        {
            return true;
        }
    }
    public void Interact()
    {
        switch(type)
        {
            case ItemType.Crate:
                PlayerController.Instance.PickUp(GameController.Instance.ScenePrefabCatalog.cratePrefab, gameObject);
                break;
            case ItemType.FoodBag:
                PlayerController.Instance.PickUp(GameController.Instance.ScenePrefabCatalog.foodBagPrefab, gameObject);
                break;
        }
        
    }

    public void Init(System.Action onDestroyedCallback)
    {
        _onDestroyed = onDestroyedCallback;
    }

    void OnDestroy()
    {
        _onDestroyed?.Invoke();
    }
}

public enum ItemType
{
    Crate,
    FoodBag
}


