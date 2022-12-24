using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningMovement : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 8;

    void FixedUpdate()
    {
        transform.Rotate(0, Time.fixedDeltaTime * rotationSpeed, 0);
    }
}
