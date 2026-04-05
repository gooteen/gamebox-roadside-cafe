using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Стройкплощадка. При взаимодействии создаёт соответствующий тип постройки

public enum ConstructionType
{
    RegularTrashBin,
    RecycleTrashBin,
    PurifyingStation,
}

public class ConstructibleObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ConstructionType _type;
    [SerializeField] private GameObject _constructionPrefab;
    [SerializeField] private float _interactDistance;
    private int _constructionPrice;

    public bool CanInteract(Vector3 fromPosition)
    {
        if ((transform.position - fromPosition).magnitude >= _interactDistance)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void Interact()
    {
        if (GameController.Instance.CurrentMoney >= _constructionPrice)
        {
            GameController.Instance.CurrentMoney -= _constructionPrice;
            GameObject obj = Instantiate(_constructionPrefab, transform.position, Quaternion.identity);
            switch (_type)
            {
                case ConstructionType.PurifyingStation:
                    GameController.Instance.isPurifierBuilt = true;
                    break;
                case ConstructionType.RegularTrashBin:
                    GameController.Instance.isTrashBinBuilt = true;
                    GameController.Instance.trashBin = obj.GetComponent<Spawner>();
                    break;
                case ConstructionType.RecycleTrashBin:
                    GameController.Instance.isRecycleTrashBinBuilt = true;
                    GameController.Instance.recycleTrashBin = obj.GetComponent<Spawner>();
                    break;
            }
            Destroy(gameObject);
        }
    }

    public Vector3 GetInteractionPoint()
    {
        return transform.position;
    }

    void Start()
    {
        switch(_type)
        {
            case ConstructionType.PurifyingStation:
                _constructionPrice = GameController.Instance.purifierPrice;
                break;
            case ConstructionType.RegularTrashBin:
                _constructionPrice = GameController.Instance.regularTrashBinPrice;
                break;
            case ConstructionType.RecycleTrashBin:
                _constructionPrice = GameController.Instance.recycleTrashBinPrice;
                break;
        }
    }
}
