using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMover : PhysicsEntity, IActivatable
{
    [SerializeField]
    private Vector3 deltaPos;
    [SerializeField]
    private float travelTime = 1f;
    [SerializeField]
    private bool smooth = true;

    private bool atEnd = false;
    private bool active = false;
    private bool resetFlag = false;
    private Vector3 moveDelta;

    public void Activate()
    {
        if (!active)
        {
            StartCoroutine(DoMove());
            active = true;
        }
        else
        {
            resetFlag = true;
        }
    }

    public bool IsActive()
    {
        return active;
    }

    new protected void FixedUpdate()
    {
        transform.localPosition += new Vector3(xspd * Time.fixedDeltaTime, yspd * Time.fixedDeltaTime, 0);
    }

    private IEnumerator DoMove()
    {
        Vector3 start = transform.localPosition;
        Vector3 goal = atEnd ? transform.localPosition - deltaPos : transform.localPosition + deltaPos;
        atEnd = !atEnd;
        float param = Time.fixedDeltaTime;
        while (param < 1)
        {
            if (smooth)
            {
                float p = param > 0.5f ? 1 - param : param;
                moveDelta = (goal - start) / travelTime * Mathf.Pow(p * 2, 3) * 4;
            }
            else
            {
                moveDelta = (goal - start) / travelTime;
            }
            xspd = moveDelta.x;
            yspd = moveDelta.y;
            param += Time.fixedDeltaTime / travelTime;
            yield return new WaitForFixedUpdate();
        }
        transform.localPosition = goal;
        xspd = 0;
        yspd = 0;
        active = false;
        if (resetFlag)
        {
            resetFlag = false;
            Activate();
        }
        yield return null;
    }
}
