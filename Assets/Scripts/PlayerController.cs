using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private CharacterAnimationController _charAnim;


    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal") * _movementSpeed * Time.deltaTime, Input.GetAxis("Vertical") * _movementSpeed * Time.deltaTime);
        transform.position += direction;
        _charAnim.AnimateDirection(direction);
    }
}
