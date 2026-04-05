using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Интерфейс, реализуемый всеми интерактивными объектами:
 * ConstructibleObject - площадка для создания постройки
 * PickableItem - предмет, который можно поднять / добавить в инвентарь
 */

public interface IInteractable
{
    public bool CanInteract(Vector3 fromPosition);
    public void Interact();
    public Vector3 GetInteractionPoint();
}
