using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEntity : MonoBehaviour
{
    [SerializeField]
    protected float gravity = 5f;
    [SerializeField]
    protected float maxFallSpeed = 2;
    [SerializeField]
    protected float slopeLimit = 0.01f;
    [SerializeField]
    protected float slipThreshold = 0.5f;
    [SerializeField]
    protected bool pushable = false;

    new protected Rigidbody rigidbody;
    new protected BoxCollider collider;
    protected float xspd;
    protected float yspd;
    protected bool grounded;
    protected bool groundSnapFlag;
    protected bool forceGrounded;

    private readonly float TOLERANCE = 0.0001f;

    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        // Check on ground
        RaycastHit hit = new RaycastHit();
        float dx = xspd, dy = yspd;
        if (Physics.BoxCast(collider.bounds.center + Vector3.up * Time.fixedDeltaTime, collider.bounds.size / 2, Vector3.down, out hit, Quaternion.identity, Time.fixedDeltaTime * 2, 1 << LayerMask.NameToLayer("Solid")))
        {
            if (Vector3.Dot(hit.normal, Vector3.up) > slipThreshold)
            {
                float slopeY;
                if (grounded && dx != 0 && pushable)
                {
                    if (CheckSlopeAscent(collider.bounds.center + Vector3.right * dx * Time.fixedDeltaTime, collider.bounds.extents, out slopeY))
                    {
                        transform.Translate(Vector3.up * slopeY);
                    }
                    else if (CheckSlopeDescent(collider.bounds.center + Vector3.right * dx * Time.fixedDeltaTime, collider.bounds.extents, out slopeY))
                    {
                        transform.Translate(Vector3.up * slopeY);
                    }
                    else if (CheckColliderSeam(collider.bounds.center, collider.bounds.extents, Time.fixedDeltaTime * Mathf.Sign(dx)))
                    {
                        transform.Translate(Vector3.right * Time.fixedDeltaTime * Mathf.Sign(dx));
                    }
                }

                if (yspd < 0)
                {
                    grounded = true;
                    dy = 0;
                    yspd = 0;
                    TrySnapToGround(Time.fixedDeltaTime * 2);
                }

                PhysicsEntity root = hit.collider.GetComponent<PhysicsEntity>();
                if (grounded && root != null)
                {
                    dx += root.xspd;
                    dy += root.yspd;
                }
            }
            else
            {
                grounded = false;
            }
        }
        else
        {
            grounded = false;
        }

        if (forceGrounded)
        {
            OnLand();
            grounded = true;
            forceGrounded = false;
        }

        // Check head bonk and vertical crush
        if (Physics.BoxCast(collider.bounds.center + Vector3.down * Time.fixedDeltaTime, 
            collider.bounds.extents, Vector3.up, out hit, Quaternion.identity, Time.fixedDeltaTime * 2, 1 << LayerMask.NameToLayer("Solid")))
        {
            PhysicsEntity other = hit.collider.GetComponent<PhysicsEntity>();
            if (grounded && ((other != null && other.yspd < 0) || dy > 0))
            {
                OnCrush();
            }
            if (yspd > 0)
            {
                yspd = 0;
            }
        }

        if (!grounded && gravity != 0)
        {
            yspd = Mathf.Max(yspd - Time.fixedDeltaTime * gravity, -maxFallSpeed);
        }

        rigidbody.velocity = new Vector3(dx, dy, 0);
    }

    public void SetXSpeed(float xs)
    {
        xspd = xs;
    }

    public void SetYSpeed(float ys)
    {
        yspd = ys;
    }

    public Vector2 GetSpeed()
    {
        return new Vector2(xspd, yspd);
    }

    protected virtual void OnLand() { }

    protected virtual void OnCrush() { }

    public void TrySnapToGround(float maxDistance = 1000)
    {
        StartCoroutine(SnapToGround(maxDistance));
    }

    public bool IsSolid(GameObject obj)
    {
        return LayerMask.LayerToName(obj.layer) == "Solid";
    }

    public void StopMovement()
    {
        xspd = 0;
        yspd = 0;
        rigidbody.velocity = Vector3.zero;
    }

    private bool CheckSlopeAscent(Vector3 checkPos, Vector3 checkExtents, out float slopeY)
    {
        RaycastHit hit;
        bool collision = Physics.BoxCast(checkPos + Vector3.up * slopeLimit, checkExtents, Vector3.down, out hit, transform.rotation, slopeLimit * (1f - Time.fixedDeltaTime));
        if (collision && IsSolid(hit.collider.gameObject))
        {
            slopeY = hit.point.y - collider.bounds.min.y;
            if (Mathf.Abs(slopeY) < TOLERANCE)
                return false;
            return true;
        }
        slopeY = 0;
        return false;
    }

    private bool CheckSlopeDescent(Vector3 checkPos, Vector3 checkExtents, out float slopeY)
    {
        RaycastHit hit;
        bool collision = Physics.BoxCast(checkPos, checkExtents, Vector3.down, out hit, transform.rotation, slopeLimit);
        if (collision && IsSolid(hit.collider.gameObject))
        {
            slopeY = hit.point.y - collider.bounds.min.y;
            if (Mathf.Abs(slopeY) < TOLERANCE)
                return false;
            return true;
        }
        slopeY = 0;
        return false;
    }

    private bool CheckColliderSeam(Vector3 checkPos, Vector3 checkExtents, float distance)
    {
        RaycastHit hit;
        bool collisionLevel = Physics.BoxCast(checkPos + new Vector3(-Mathf.Sign(distance) * Time.fixedDeltaTime, -Time.fixedDeltaTime, 0), checkExtents, Vector3.right * Mathf.Sign(distance), out hit, transform.rotation, Mathf.Abs(distance));
        bool collisionElevated = Physics.BoxCast(checkPos + new Vector3(-Mathf.Sign(distance) * Time.fixedDeltaTime, Time.fixedDeltaTime, 0), checkExtents, Vector3.right * Mathf.Sign(distance), out hit, transform.rotation, Mathf.Abs(distance));
        if (collisionLevel && !collisionElevated)
        {
            return true;
        }
        return false;
    }

    private IEnumerator SnapToGround(float maxDistance)
    {
        RaycastHit hit;
        groundSnapFlag = false;
        yield return new WaitForFixedUpdate();
        bool collision = Physics.BoxCast(collider.bounds.center, collider.bounds.extents, Vector3.down, out hit, transform.rotation, maxDistance);
        if (collision)
        {
            float surfaceY = hit.point.y;
            if (IsSolid(hit.collider.gameObject))
            {
                transform.Translate(0, surfaceY - collider.bounds.min.y, 0, Space.World);
                yspd = 0;
                forceGrounded = true;
            }
        }
        yield return null;
    }
}
