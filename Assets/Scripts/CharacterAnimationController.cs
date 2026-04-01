using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private int _numberOfSlices = 8;

    // 8-directional movement

    private string[] _staticDirections = { "Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE" };
    private string[] _runDirections = { "Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE" };


    private Animator _anim;

    private int _lastDirection;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }


    public void AnimateDirection(Vector2 direction)
    {
        string[] _directionArray = null;
        if (direction.magnitude == 0)
        {
            _directionArray = _staticDirections;
        }
        else
        {
            _directionArray = _runDirections;
            _lastDirection = DirectionToIndex(direction);
        }
        
        _anim.Play(_directionArray[_lastDirection]);
    }


    public int DirectionToIndex(Vector2 direction)
    {
        float step = 360 / _numberOfSlices;
        float offset = step / 2;

        float angle = Vector2.SignedAngle(Vector2.up, direction.normalized);

        angle += offset;
        if (angle < 0)
        {
            angle += 360;
        }

        float stepCount = angle / step;
        return Mathf.FloorToInt(stepCount);
    }
}
