using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserInterfaceController : MonoBehaviour
{
    [SerializeField] private GameObject _scorePanel;
    [SerializeField] private GameObject _elementsPanel;

    [SerializeField] private TMP_Text _moneyContainerScore;
    [SerializeField] private TMP_Text _pollutionContainerScore;

    [SerializeField] private TMP_Text _moneyContainerElements;
    [SerializeField] private TMP_Text _pollutionContainerElements;

    [SerializeField] private TMP_Text _binContainer;
    [SerializeField] private TMP_Text _recycleBinContainer;
    [SerializeField] private TMP_Text _purifyerContainer;

    [SerializeField] private Image _pollutionBar;
    [SerializeField] private Image _placeHolder;

    public static UserInterfaceController Instance { get; private set; }

    public void ShowScorePanel(int moneyDelta, float pollutionDelta, float pollutionTotal)
    {
        Time.timeScale = 0f;
        _scorePanel.SetActive(true);
        _elementsPanel.SetActive(false);
        _moneyContainerScore.text = moneyDelta.ToString();
        _pollutionContainerScore.text = $"+{pollutionDelta}";
        _pollutionBar.fillAmount = pollutionTotal / 100f;
    }

    public void NextRound()
    {
        Time.timeScale = 1f;
        _scorePanel.SetActive(false);
        _elementsPanel.SetActive(true);
    }

    public void Equip(Sprite objectIcon)
    {
        _placeHolder.gameObject.SetActive(true);
        _placeHolder.sprite = objectIcon;
    }

    public void Unequip()
    {
        _placeHolder.gameObject.SetActive(false);
        _placeHolder.sprite = null;
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

    void Start()
    {
        HandlePollutionChanged(0);
        HandleMoneyChanged(0);

        _binContainer.text = $"- {GameController.Instance.regularTrashBinPrice}$";
        _recycleBinContainer.text = $"- {GameController.Instance.recycleTrashBinPrice}$";
        _purifyerContainer.text = $"- {GameController.Instance.purifierPrice}$";

        _scorePanel.SetActive(false);
        _elementsPanel.SetActive(true);

        GameController.Instance.OnPollutionChanged += HandlePollutionChanged;
        GameController.Instance.OnMoneyChanged += HandleMoneyChanged;

        Unequip();
    }

    private void HandlePollutionChanged(float pollution)
    {
        _pollutionContainerElements.text = $"{pollution}/100";

    }

    private void HandleMoneyChanged(int money)
    {
        _moneyContainerElements.text = money.ToString();

    }

}
