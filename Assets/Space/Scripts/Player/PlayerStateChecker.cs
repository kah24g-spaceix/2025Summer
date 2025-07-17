using UnityEngine;

public class PlayerStateChecker : MonoBehaviour
{
    [Header("Ground Check Settings")]
    private LayerMask groundLayer;
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

        // "Ground"라는 이름의 레이어를 찾아서 groundLayer 변수에 할당합니다.
        int groundLayerIndex = LayerMask.NameToLayer("Ground");

        // 만약 "Ground" 레이어가 존재한다면
        if (groundLayerIndex != -1)
        {
            // 해당 레이어만 포함하는 LayerMask를 생성합니다.
            groundLayer = 1 << groundLayerIndex;
        }
        else
        {
            // "Ground" 레이어를 찾지 못했을 경우, 개발자에게 경고 메시지를 보냅니다.
            Debug.LogWarning("'Ground' 레이어가 존재하지 않습니다. Project Settings > Tags and Layers에서 설정해주세요.");
        }
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