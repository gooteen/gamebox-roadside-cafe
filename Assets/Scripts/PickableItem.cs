using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour, IInteractable
{
    public ItemType type;
    [SerializeField] private float _interactDistance;
    [SerializeField] private float _decompositionTime;
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
        GameObject objectToPickup = null;
        switch (type)
        {
            case ItemType.Crate:
                objectToPickup = GameController.Instance.ScenePrefabCatalog.cratePrefab;
                break;
            case ItemType.FoodBag:
                objectToPickup = GameController.Instance.ScenePrefabCatalog.foodBagPrefab;
                break;
            case ItemType.Rubbish:
                objectToPickup = GameController.Instance.ScenePrefabCatalog.rubbishPrefab;
                break;
            case ItemType.TrashBagReqular:
                objectToPickup = GameController.Instance.ScenePrefabCatalog.trashBagRegularPrefab;
                break;
            case ItemType.TrashBagOrganic:
                objectToPickup = GameController.Instance.ScenePrefabCatalog.trashBagOrganicPrefab;
                break;
            case ItemType.TrashBagPaper:
                objectToPickup = GameController.Instance.ScenePrefabCatalog.trashBagPaperPrefab;
                break;
            case ItemType.TrashBagPlastic:
                objectToPickup = GameController.Instance.ScenePrefabCatalog.trashBagPlasticPrefab;
                break;
        }
        PlayerController.Instance.PickUp(objectToPickup, gameObject);
    }

    public void Init(System.Action onDestroyedCallback)
    {
        _onDestroyed = onDestroyedCallback;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (type == ItemType.Crate)
        {
            StartCoroutine(ReceptionRoutine());
        }
    }

    IEnumerator ReceptionRoutine()
    {
        yield return new WaitForSeconds(1f);
        GameController.Instance.CurrentMoney += GameController.Instance.moneyPerCrate;
        Destroy(gameObject);
    }

    void Start()
    {
        if (type == ItemType.Rubbish)
        {
            StartCoroutine(DecompositionRoutine(GameController.Instance.rubbishTimeToDecompose));
        } else if (type == ItemType.TrashBagReqular || type == ItemType.TrashBagOrganic || type == ItemType.TrashBagPaper || type == ItemType.TrashBagPlastic) 
        {
            StartCoroutine(DecompositionRoutine(GameController.Instance.bagTimeToDecompose));
        }
    }

    IEnumerator DecompositionRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        GameController.Instance.CurrentPollution += GameController.Instance.decompositionCost;
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        _onDestroyed?.Invoke();
    }
}

public enum ItemType
{
    Crate,
    FoodBag,
    TrashBagReqular,
    TrashBagOrganic,
    TrashBagPaper,
    TrashBagPlastic,
    Rubbish
}


