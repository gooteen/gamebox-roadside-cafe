using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private CharacterAnimationController _charAnim;
    [SerializeField] private Transform _offloadPoint;

    private NavMeshAgent _agent;
    private Vector3 _currentDestination;

    [SerializeField] private GameObject _objectInInventory;

    public static PlayerController Instance { get; private set; }
    public GameObject ObjectInInventory
    {
        get
        {
            return _objectInInventory;
        }
        set
        {
            _objectInInventory = value;
        }
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

    private void Start()
    {
        _currentDestination = transform.position;
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    void Update()
    {
        _charAnim.AnimateDirection(_currentDestination - transform.position);
    }

    public void HandleClick(HitResult hit)
    {
        switch (hit.Type)
        {
            case HitType.Ground:
                MoveTo(hit.Point);
                break;

            case HitType.Interactable:
                HandleInteraction(hit.InteractableObject);
                break;
        }
    }

    public void PickUp(GameObject objectToPick, GameObject objectToDestroy)
    {
        if (_objectInInventory == null)
        {
            _objectInInventory = objectToPick;
            Destroy(objectToDestroy);
        }
    }

    public void DropObject()
    {
        if (_objectInInventory != null)
        {
            Instantiate(_objectInInventory, _offloadPoint.position, _offloadPoint.rotation);
            _objectInInventory = null;
        }
    }

    void MoveTo(Vector2 point)
    {
        _currentDestination = point;
        _agent.SetDestination(_currentDestination);
    }

    void HandleInteraction(IInteractable interactable)
    {
        if (interactable.CanInteract(transform.position))
        {
            interactable.Interact();
        }
    }
}
