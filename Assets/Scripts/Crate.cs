using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour, IInteractable
{
    [SerializeField] private float _interactDistance;
    [SerializeField] private GameObject _cratePrefab;
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
        PlayerController.Instance.PickUp(GameController.Instance.ScenePrefabCatalog.cratePrefab);
        Destroy(gameObject);
    }
}
