using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// State-машина для реализации логики поведения покупателей

public enum CustomerState
{
    GoingToCashier,
    WaitingForFood,
    GoingToExit,
    GoingToBench,
    WaitingOnBench,
    GoingToTrashBin
}

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
        // Нацелиться на кассу
        _currentDestination = transform.position;
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        GoToCashier();
    }

    void Update()
    {
        Vector2 dirToAnimate = Vector2.zero;
        _charAnim.AnimateDirection(_currentDestination - transform.position);
        switch (_state)
        {
            case CustomerState.GoingToCashier:
                CheckArrival(GameController.Instance.sellingPoint.transform.position, OnReachedCashier);
                dirToAnimate = _currentDestination - transform.position;
                break;

            case CustomerState.WaitingForFood:
                TryTakeFood();
                dirToAnimate = Vector2.zero;
                break;

            case CustomerState.GoingToExit:
                CheckArrival(GameController.Instance.exitPoint.transform.position, OnReachedExit);
                dirToAnimate = _currentDestination - transform.position;
                HandleTrashLogic();
                break;

            case CustomerState.GoingToBench:
                CheckArrival(GameController.Instance.benchPoint.transform.position, OnReachedBench);
                dirToAnimate = _currentDestination - transform.position;
                HandleTrashLogic();
                break;
            
            // Режим "ожидания", если на кассе нет еды
            case CustomerState.WaitingOnBench:
                HandleTrashLogic();
                dirToAnimate = Vector2.zero;

                break;

            case CustomerState.GoingToTrashBin:
                CheckArrival(_selectedBin.transform.position, OnReachedTrashBin);
                dirToAnimate = _currentDestination - transform.position;
                break;
        }
        _charAnim.AnimateDirection(dirToAnimate);
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
            if (GameController.Instance.CurrentPollution < GameController.Instance.benchPollutionLevel)
            {
                GoToBench(); // Пойти к скамейке, если условия выполняются
            } else
            {
                PickTrashBinRoute();
            }
        }
    }

    // При наличии одной из мусорок пойти к ним, иначе - к выходу
    void PickTrashBinRoute()
    {
        if (GameController.Instance.isTrashBinBuilt && GameController.Instance.isRecycleTrashBinBuilt)
        {
            if (Random.value < 0.5f)
            {
                GoToTrashBin(GameController.Instance.trashBin);
            }
            else
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
        else
        {
            GoToExit();
        }
    }

    void GoToExit()
    {
        _state = CustomerState.GoingToExit;
        _agent.SetDestination(GameController.Instance.exitPoint.transform.position);
        _currentDestination = GameController.Instance.exitPoint.transform.position;
    }

    // Оставить мелкий мусор, если условия выполняются
    void HandleTrashLogic()
    {
        if (!_hasFood || _droppedTrash || GameController.Instance.isTrashBinBuilt || GameController.Instance.isRecycleTrashBinBuilt)
            return;

        if (Random.value < 0.001f) // Шанс оставить мусор каждый кадр
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

        GoToExit();
    }

    void OnReachedExit()
    {
        if (GameController.Instance.CurrentPollution < GameController.Instance.secondOrderPollutionLevel && Random.value < 0.5f)
        {
            GoToCashier(); // Выполнить повторную покупку, если условия выполняются
        } else
        {
            Destroy(gameObject);
        }
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

        if (_hasFood)
        {
            PickTrashBinRoute();
        } else
        {
            GoToExit();
        }
    }

    // Универсальный метод для обработки "прибытия" к точке
    void CheckArrival(Vector3 target, System.Action onArrive)
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.2f)
        {
            onArrive?.Invoke();
        }
    }
}
