using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool CanInteract(Vector3 fromPosition);
    public void Interact();
}
