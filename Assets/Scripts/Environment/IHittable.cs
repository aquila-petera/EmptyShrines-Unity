using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    bool OnHit(Vector3 hitOrigin);
}
