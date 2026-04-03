using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CustomerState
{
    GoingToCashier,
    WaitingForFood,
    GoingToExit,
    GoingToBench,
    WaitingOnBench,
    GoingToTrashBin
}

// Вынести харкдод в GameController

public class Customer : MonoBehaviour
{
    [SerializeField] private GameObject _trashPrefab;
    [SerializeField] private CharacterAnimationController _charAnim;

    private CustomerState _state;

    private NavMeshAgent _agent;

    private bool _hasFood;
    private bool _droppedTrash;
    private Vector3 _currentDestination;
    private Spawner _selectedBin;

    void Start()
    {
        _currentDestination = transform.position;
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        GoToCashier();
    }

    void Update()
    {
        _charAnim.AnimateDirection(_currentDestination - transform.position);
        switch (_state)
        {
            case CustomerState.GoingToCashier:
                CheckArrival(GameController.Instance.sellingPoint.transform.position, OnReachedCashier);
                break;

            case CustomerState.WaitingForFood:
                TryTakeFood();
                break;

            case CustomerState.GoingToExit:
                CheckArrival(GameController.Instance.exitPoint.transform.position, OnReachedExit);
                HandleTrashLogic();
                break;

            case CustomerState.GoingToBench:
                CheckArrival(GameController.Instance.benchPoint.transform.position, OnReachedBench);
                HandleTrashLogic();
                break;

            case CustomerState.GoingToTrashBin:
                CheckArrival(_selectedBin.transform.position, OnReachedTrashBin);
                break;
        }
    }

    void GoToCashier()
    {
        _state = CustomerState.GoingToCashier;
        _agent.SetDestination(GameController.Instance.sellingPoint.transform.position);
        _currentDestination = GameController.Instance.sellingPoint.transform.position;
    }

    void OnReachedCashier()
    {
        _state = CustomerState.WaitingForFood;
    }

    void TryTakeFood()
    {
        if (GameController.Instance.sellingPoint.TryTakeItem())
        {
            GameController.Instance.CurrentMoney += GameController.Instance.moneyPerServing;
            _hasFood = true;
            if (GameController.Instance.isTrashBinBuilt && GameController.Instance.isRecycleTrashBinBuilt)
            {
                if (Random.value < 0.5f)
                {
                    GoToTrashBin(GameController.Instance.trashBin);
                } else
                {
                    GoToTrashBin(GameController.Instance.recycleTrashBin);
                }
            } 
            else if (GameController.Instance.isTrashBinBuilt)
            {
                GoToTrashBin(GameController.Instance.trashBin);
            } 
            else if (GameController.Instance.isRecycleTrashBinBuilt)  
            {
                GoToTrashBin(GameController.Instance.recycleTrashBin);
            }
             else if (GameController.Instance.CurrentPollution < 0.5f)
            {
                GoToBench();
            } else
            {
                GoToExit();
            }
        }
    }
    void GoToExit()
    {
        _state = CustomerState.GoingToExit;
        _agent.SetDestination(GameController.Instance.exitPoint.transform.position);
        _currentDestination = GameController.Instance.exitPoint.transform.position;
    }

    void HandleTrashLogic()
    {
        if (!_hasFood || _droppedTrash)
            return;

        if (Random.value < 0.001f) // шанс каждый кадр
        {
            DropTrash();
        }
    }

    void DropTrash()
    {
        _droppedTrash = true;
        _hasFood = false;

        Instantiate(_trashPrefab, transform.position, Quaternion.identity);
    }

    void GoToTrashBin(Spawner bin)
    {
        _selectedBin = bin;
        _state = CustomerState.GoingToTrashBin;
        _agent.SetDestination(_selectedBin.transform.position);
        _currentDestination = _selectedBin.transform.position;
    }

    void OnReachedTrashBin()
    {
        _selectedBin.RubbishCounter += 1;
        _hasFood = false;

        if (GameController.Instance.CurrentPollution < 0.5f && Random.value < 0.5f)
        {
            GoToCashier(); // повторная покупка
        } else if (GameController.Instance.CurrentPollution < 0.5f)
        {
            GoToBench();
        } else
        {
            GoToExit();
        }
    }

    void OnReachedExit()
    {
        Destroy(gameObject);
    }

    void GoToBench()
    {
        _state = CustomerState.GoingToBench;
        _agent.SetDestination(GameController.Instance.benchPoint.transform.position);
        _currentDestination = GameController.Instance.benchPoint.transform.position;
    }

    void OnReachedBench()
    {
        StartCoroutine(BenchRoutine());
    }

    IEnumerator BenchRoutine()
    {
        _state = CustomerState.WaitingOnBench;

        yield return new WaitForSeconds(5f);

        GoToExit();
    }

    void CheckArrival(Vector3 target, System.Action onArrive)
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.2f)
        {
            onArrive?.Invoke();
        }
    }
}
