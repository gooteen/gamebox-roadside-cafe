using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [SerializeField] private GameObject _squirrel;
    [SerializeField] private Sprite _pollutedSprite;
    private SpriteRenderer _treeRenderer;

    private bool _squirrelHidden;
    private bool _treePolluted;

    private void Awake()
    {
        _treeRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        HandlePollutionChanged(GameController.Instance.CurrentPollution);
        GameController.Instance.OnPollutionChanged += HandlePollutionChanged;

    }

    private void HandlePollutionChanged(float pollution)
    {
        Debug.Log("pollution");
        if (!_squirrelHidden && pollution >= GameController.Instance.treePollutionLevel)
        {
            _squirrel.SetActive(false);
            _squirrelHidden = true;
        }

        if (!_treePolluted && pollution >= GameController.Instance.squirellPollutionLevel)
        {
            _treeRenderer.sprite = _pollutedSprite;
            _treePolluted = true;
        }
    }
}
