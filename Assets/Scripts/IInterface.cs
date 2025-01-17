using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInterface
{
    void Interact(Transform interactorTransform);

    Transform GetTransform();

    float GetIndicatorHeight();
}
