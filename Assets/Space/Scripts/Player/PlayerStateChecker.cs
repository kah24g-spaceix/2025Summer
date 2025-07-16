using UnityEngine;

public class PlayerStateChecker : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [Range(0, 1)]
    [SerializeField] private float groundCheckWidth = 0.9f;
    [SerializeField] private float maxGroundAngle = 45f;

    [Header("Wall Check Settings")]
    [SerializeField] private float wallCheckDistance = 0.1f;

    public bool IsGrounded { get; private set; }
    public bool IsAgainstWall { get; private set; }
    public int WallDirection { get; private set; }

    private CapsuleCollider2D capsuleCollider;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
        CheckIfAgainstWall();
    }

    private void CheckIfGrounded()
    {
        bool groundFound = false;
        Bounds bounds = capsuleCollider.bounds;
        float raycastWidth = bounds.size.x * groundCheckWidth * 0.5f;
        Vector2[] rayOrigins = new Vector2[3]
        {
            new Vector2(bounds.center.x, bounds.min.y),
            new Vector2(bounds.center.x - raycastWidth, bounds.min.y),
            new Vector2(bounds.center.x + raycastWidth, bounds.min.y)
        };

        foreach (Vector2 origin in rayOrigins)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
            if (hit.collider != null)
            {
                if (Vector2.Angle(hit.normal, Vector2.up) < maxGroundAngle)
                {
                    groundFound = true;
                    break;
                }
            }
        }

        IsGrounded = groundFound;
    }

    private void CheckIfAgainstWall()
    {
        Bounds bounds = capsuleCollider.bounds;
        Vector2 rayOrigin = bounds.center;

        bool hitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, bounds.size.x / 2 + wallCheckDistance, groundLayer);
        bool hitRight = Physics2D.Raycast(rayOrigin, Vector2.right, bounds.size.x / 2 + wallCheckDistance, groundLayer);

        IsAgainstWall = hitLeft || hitRight;
    }

    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;

        Bounds bounds = capsuleCollider.bounds;

        // Ground Check Gizmos
        float raycastWidth = bounds.size.x * groundCheckWidth * 0.5f;
        Vector2[] rayOrigins = new Vector2[3]
        {
            new Vector2(bounds.center.x, bounds.min.y),
            new Vector2(bounds.center.x - raycastWidth, bounds.min.y),
            new Vector2(bounds.center.x + raycastWidth, bounds.min.y)
        };

        foreach (Vector2 origin in rayOrigins)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
            if (hit.collider != null && Vector2.Angle(hit.normal, Vector2.up) < maxGroundAngle)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
        }

        // Wall Check Gizmos
        Gizmos.color = Color.blue;
        Vector2 wallRayOrigin = bounds.center;
        Gizmos.DrawLine(wallRayOrigin, wallRayOrigin + Vector2.left * (bounds.size.x / 2 + wallCheckDistance));
        Gizmos.DrawLine(wallRayOrigin, wallRayOrigin + Vector2.right * (bounds.size.x / 2 + wallCheckDistance));
    }
}