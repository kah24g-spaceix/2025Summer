// 이 스크립트는 플레이어가 특정 구역 안에서 Z키를 눌렀을 때 상호작용을 구현합니다.
// 설정을 위해 다음 단계를 따르세요:
//
// 1. 이 스크립트를 상호작용이 일어날 GameObject에 추가합니다. 'BoxCollider2D'가 자동으로 추가됩니다.
//
// 2. 'BoxCollider2D' 컴포넌트에서 'Is Trigger' 체크박스를 반드시 활성화해야 합니다.
//
// 3. 상호작용할 플레이어 GameObject에 "Player"라는 태그가 설정되어 있는지 확인하세요.
//
// 4. 씬(Scene) 뷰에서 콜라이더의 크기와 위치를 원하는 상호작용 영역에 맞게 조절합니다.

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractionZone : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("상호작용이 성공했을 때 콘솔에 출력될 메시지입니다.")]
    [SerializeField] private string interactionMessage = "플레이어가 Z키로 상호작용했습니다!";

    // 플레이어가 현재 트리거 존 안에 있는지 여부를 저장하는 변수
    private bool isPlayerInside = false;

    private void Awake()
    {
        // 콜라이더가 반드시 트리거로 설정되도록 보장합니다.
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void Update()
    {
        // 플레이어가 존 안에 있을 때만 입력을 확인합니다.
        if (isPlayerInside)
        {
            // 새로운 Input System을 사용하여 'Z' 키가 이번 프레임에 눌렸는지 확인합니다.
            // Keyboard.current가 null이 아닌지 확인하여 에러를 방지합니다.
            if (Keyboard.current != null && Keyboard.current.zKey.wasPressedThisFrame)
            {
                // 상호작용 메시지를 콘솔에 출력합니다.
                Debug.Log(interactionMessage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 들어온 오브젝트가 "Player" 태그를 가지고 있는지 확인합니다.
        if (other.CompareTag("Player"))
        {
            // 플레이어가 들어왔다고 표시합니다.
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 나간 오브젝트가 "Player" 태그를 가지고 있는지 확인합니다.
        if (other.CompareTag("Player"))
        {
            // 플레이어가 나갔다고 표시합니다.
            isPlayerInside = false;
        }
    }
}
