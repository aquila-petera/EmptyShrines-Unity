using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    private readonly float TOLERANCE = 0.02f;

    [SerializeField]
    protected float gravity = 5f;
    [SerializeField]
    protected float maxYSpd = 2;
    [SerializeField]
    private float slopeLimit = 0.025f;
    [SerializeField]
    protected string ignoreLayer;
    [SerializeField]
    protected string solidLayer;
    [SerializeField]
    protected bool pushable;

    protected bool delayInit;
    protected float xspd, yspd, zspd;
    protected List<Vector3> forces;
    protected List<BoxCollider> boxColliders;
    protected BoxCollider mainBoxCollider;
    protected bool grounded = true;
    protected bool forceGrounded = false;
    protected bool groundSnapFlag; // Used to snap objects to ground when scene loads

    private PhysicsObject root;

    public bool IsSolid(GameObject obj)
    {
        return obj.layer == LayerMask.NameToLayer(solidLayer);
    }

    public void StopMovement()
    {
        xspd = 0;
        yspd = 0;
    }

    public void AddMovement(float xs, float ys)
    {
        xspd += xs;
        yspd += ys;
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

    protected void Start()
    {
        if (delayInit)
            return;
        InitPhysicsObject();
    }

    protected void OnDestroy()
    {
        if (boxColliders == null)
        {
            return;
        }
        foreach (var col in boxColliders)
        {
            CollisionManager.UnregisterCollider(col);
        }
    }

    protected void FixedUpdate()
    {
        if (delayInit)
        {
            InitPhysicsObject();
            delayInit = false;
        }
        float dx = root != null ? xspd + root.GetXSpd() : xspd;
        float dy = root != null ? yspd + root.GetYSpd() : yspd;
        if (dx != 0 || dy != 0)
        {
            AttemptMove(dx, dy);
        }

        if (gravity != 0)
        {
            RaycastHit[] hits = Physics.BoxCastAll(mainBoxCollider.bounds.center, mainBoxCollider.size / 2, Vector3.down, transform.rotation, Time.fixedDeltaTime);
            bool rootContact = false;
            bool onGround = forceGrounded;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                    continue;
                onGround = onGround || IsSolid(hit.collider.gameObject);
                if (root != null && root == hit.collider.GetComponent<PhysicsObject>())
                {
                    rootContact = true;
                }
                // Check for landing on a new root platform
                else if (onGround && !grounded && root == null && yspd <= 0)
                {
                    grounded = true;
                    root = hit.collider.GetComponent<PhysicsObject>();
                    yspd = 0;
                    OnLand();
                }
            }

            if (!onGround)
            {
                grounded = false;
                yspd = Mathf.Max(yspd - gravity * Time.fixedDeltaTime, -maxYSpd);
                root = null;
            }

            // Check for jumping off root platform or walking between platforms
            if (root != null && !rootContact)
            {
                if (hits.Length == 0)
                {
                    root = null;
                }
                else
                {
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.gameObject == gameObject)
                            continue;
                        root = hit.collider.GetComponent<PhysicsObject>();
                        if (root != null)
                            break;
                    }
                }
            }
        }

        forceGrounded = false;
    }

    protected void InitPhysicsObject()
    {
        boxColliders = new List<BoxCollider>();
        boxColliders.AddRange(GetComponentsInChildren<BoxCollider>());
        if (boxColliders.Count == 0)
        {
            Debug.LogWarning($"PhysicsObject {gameObject.name} has no colliders!");
            return;
        }
        mainBoxCollider = boxColliders[0];
        foreach (var col in boxColliders)
        {
            CollisionManager.RegisterCollider(col);
        }
        if (maxYSpd <= 0)
            maxYSpd = float.MaxValue;
        forces = new List<Vector3>();
    }

    private bool CheckSlopeAscent(Vector3 checkPos, Vector3 checkSize, Collider checkCol, out float slopeY)
    {
        RaycastHit hit;
        bool collision = Physics.BoxCast(checkPos + Vector3.up * slopeLimit, checkSize, Vector3.down, out hit, transform.rotation, slopeLimit * (1 + TOLERANCE));
        if (collision && hit.collider.Equals(checkCol))
        {
            slopeY = hit.point.y;
            return true;
        }
        slopeY = 0;
        return false;
    }

    private bool CheckSlopeDescent(Vector3 checkPos, Vector3 checkSize, out float slopeY)
    {
        RaycastHit hit;
        bool collision = Physics.BoxCast(checkPos, checkSize, Vector3.down, out hit, transform.rotation, slopeLimit * (2 + TOLERANCE));
        if (collision && IsSolid(hit.collider.gameObject) && (root == null || hit.collider.gameObject == root.gameObject))
        {
            slopeY = hit.point.y;
            return true;
        }
        slopeY = 0;
        return false;
    }

    protected void AttemptMove(float dx, float dy)
    {
        Vector3 checkPos = transform.position + mainBoxCollider.center;
        RaycastHit hit;
        Vector3 checkSize = new Vector3(mainBoxCollider.size.x / 2 * (1 - TOLERANCE), mainBoxCollider.size.y / 2 * (1 - TOLERANCE), mainBoxCollider.size.z / 2);
        bool solid = false;
        bool collision = Physics.BoxCast(checkPos, checkSize, Vector3.right * Mathf.Sign(dx), out hit, transform.rotation, Mathf.Abs(dx) * Time.fixedDeltaTime);
        bool upSlope = false, downSlope;
        if (collision && hit.collider.gameObject.layer != LayerMask.NameToLayer(ignoreLayer))
        {
            float cDelta = 0;
            float surfaceX = hit.point.x;
            if (hit.point.x > checkPos.x)
            {
                cDelta = checkSize.x / 2;
            }
            else
            {
                cDelta = -checkSize.x / 2;
            }
            OnCollision(hit.collider.gameObject, hit.point);
            if (IsSolid(hit.collider.gameObject))
            {
                float slopeY = 0;
                upSlope = pushable && grounded && CheckSlopeAscent(checkPos + Vector3.right * dx * Time.fixedDeltaTime, checkSize, hit.collider, out slopeY);
                if (!upSlope)
                {
                    solid = true;
                    xspd = 0;
                }
                else
                {
                    dy += (slopeY - mainBoxCollider.bounds.min.y) / Time.fixedDeltaTime;
                }
            }
        }
        if (!solid)
        {
            Vector3 delta = new Vector3(dx * Time.fixedDeltaTime, 0, 0);

            if (delta.x != 0)
            {
                transform.Translate(delta, Space.World);
                checkPos += delta;
                float slopeY = 0;
                downSlope = !upSlope && pushable && grounded && CheckSlopeDescent(checkPos, checkSize, out slopeY);
                if (downSlope)
                {
                    dy += (slopeY - mainBoxCollider.bounds.min.y) / Time.fixedDeltaTime;
                }
            }
        }

        solid = false;
        collision = Physics.BoxCast(checkPos, checkSize, Vector3.up * Mathf.Sign(dy), out hit, transform.rotation, Mathf.Abs(dy) * Time.fixedDeltaTime);
        if (collision && hit.collider.gameObject.layer != LayerMask.NameToLayer(ignoreLayer))
        {
            float cDelta = 0;
            float surfaceY = 0;
            if (hit.point.y > checkPos.y)
            {
                cDelta = -mainBoxCollider.bounds.extents.y;
                surfaceY = hit.point.y;
            }
            else
            {
                cDelta = mainBoxCollider.bounds.extents.y;
                surfaceY = hit.point.y;
            }
            OnCollision(hit.collider.gameObject, hit.point);
            if (IsSolid(hit.collider.gameObject))
            {
                transform.Translate(0, surfaceY - checkPos.y + cDelta, 0, Space.World);
                solid = true;
                if (yspd < 0)
                {
                    OnLand();
                }
                yspd = 0;
            }
        }
        if (dy != 0 && !solid)
        {
            transform.Translate(0, dy * Time.fixedDeltaTime, 0, Space.World);
        }
    }

    protected virtual void OnLand() { }

    public virtual void OnCollision(GameObject other, Vector3 collisionPoint)
    {
        PhysicsObject otherPhys = other.GetComponent<PhysicsObject>();
        if (!pushable)
            return;
        var otherBounds = other.GetComponent<Collider>().bounds;
        Vector3 right = transform.position + Vector3.right * mainBoxCollider.bounds.extents.x;
        Vector3 left = transform.position + Vector3.left * mainBoxCollider.bounds.extents.x;
        Vector3 up = transform.position + Vector3.up * mainBoxCollider.bounds.extents.y;
        Vector3 down = transform.position + Vector3.down * mainBoxCollider.bounds.extents.y;
        if (otherPhys != null && IsSolid(other) && pushable)
        {
            float deltaX = 0, deltaY = 0;
            if (otherBounds.Contains(right))
            {
                deltaX = collisionPoint.x - right.x;
                if (xspd > 0)
                    xspd = 0;
            }
            else if (otherBounds.Contains(left))
            {
                deltaX = collisionPoint.x - left.x;
                if (xspd < 0)
                    xspd = 0;
            }
            if (otherBounds.Contains(up))
            {
                deltaY = collisionPoint.y - up.y;
                if (yspd > 0)
                    yspd = 0;
            }
            else if (otherBounds.Contains(down))
            {
                if (root == null)
                {
                    root = otherPhys;
                }
                deltaY = collisionPoint.y - down.y;
                if (yspd < 0)
                {
                    OnLand();
                    yspd = 0;
                }
            }
            transform.Translate(deltaX, deltaY, 0);
        }
    }

    public float GetXSpd()
    {
        float spd = xspd;
        if (transform.parent != null)
        {
            PhysicsObject parentObj = transform.parent.GetComponent<PhysicsObject>();
            if (parentObj != null)
                spd += parentObj.GetXSpd();
        }
        return spd;
    }

    public float GetYSpd()
    {
        float spd = yspd;
        if (transform.parent != null)
        {
            PhysicsObject parentObj = transform.parent.GetComponent<PhysicsObject>();
            if (parentObj != null)
                spd += parentObj.GetYSpd();
        }
        return spd;
    }

    public void TrySnapToGround()
    {
        StartCoroutine(SnapToGround());
    }

    private IEnumerator SnapToGround()
    {
        RaycastHit hit;
        groundSnapFlag = false;
        yield return new WaitForFixedUpdate();
        bool collision = Physics.BoxCast(mainBoxCollider.bounds.center + Vector3.up * Time.fixedDeltaTime, mainBoxCollider.size / 2, Vector3.down, out hit, transform.rotation, 1000);
        if (collision && hit.collider.gameObject.layer != LayerMask.NameToLayer(ignoreLayer))
        {
            float cDelta = 0;
            float surfaceY = 0;
            if (hit.point.y > mainBoxCollider.bounds.center.y)
            {
                cDelta = -mainBoxCollider.bounds.extents.y;
                surfaceY = hit.collider.bounds.min.y;
            }
            else
            {
                cDelta = mainBoxCollider.bounds.extents.y;
                surfaceY = hit.collider.bounds.max.y;
            }
            if (IsSolid(hit.collider.gameObject))
            {
                transform.Translate(0, (surfaceY - mainBoxCollider.bounds.center.y) + cDelta, 0, Space.World);
                yspd = 0;
                forceGrounded = true;
                OnLand();
            }
        }
        yield return null;
    }
}
