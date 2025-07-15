// 이 스크립트는 탑다운 뷰의 4방향 이동을 구현합니다.
// 설정을 위해 다음 단계를 따르세요:
//
// 1. 이 스크립트를 플레이어 GameObject에 추가하면 'Player Input'과 'Rigidbody2D' 컴포넌트가 자동으로 추가됩니다.
//
// 2. 'Rigidbody2D' 컴포넌트의 'Gravity Scale'을 0으로 설정하여 중력의 영향을 받지 않게 합니다.
//
// 3. 'Player Input' 컴포넌트의 'Actions' 필드에 사용할 Input Action Asset을 연결합니다.
//    이 Asset에는 'Move'라는 이름의 Action이 정의되어 있어야 합니다. (Control Type: Vector2)
//
// 4. 'Player Input' 컴포넌트의 'Behavior'를 'Send Messages' (기본값)으로 설정하면
//    아래에 정의된 OnMove 함수가 자동으로 호출됩니다.

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D))]
public class PlayerMoveTop : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    // private components and state
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 탑다운 뷰에서는 중력이 필요 없으므로 Gravity Scale을 0으로 설정합니다.
        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        Vector2 moveDirection = Vector2.zero;

        // 입력이 있는지 확인 (데드존 처리)
        if (moveInput.sqrMagnitude > 0.1f)
        {
            // x축 입력의 절댓값이 y축보다 크면 수평 이동
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                moveDirection.x = Mathf.Sign(moveInput.x);
            }
            // y축 입력의 절댓값이 x축보다 크거나 같으면 수직 이동
            else
            {
                moveDirection.y = Mathf.Sign(moveInput.y);
            }
        }

        // 최종적으로 계산된 방향으로 속도를 설정합니다.
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    // --- Input System Events ---
    public void OnMove(InputValue value)
    {
        // 'Move' Action에 바인딩된 입력 값을 Vector2 형태로 받아옵니다.
        moveInput = value.Get<Vector2>();
    }
}