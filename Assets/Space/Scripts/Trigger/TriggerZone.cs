// 이 스크립트는 간단한 트리거 존을 구현합니다.
// 설정을 위해 다음 단계를 따르세요:
//
// 1. 이 스크립트를 비어있는 GameObject에 추가합니다. 'BoxCollider2D'가 자동으로 추가됩니다.
//
// 2. 'BoxCollider2D' 컴포넌트에서 'Is Trigger' 체크박스를 반드시 활성화해야 합니다.
//    (이것을 체크하지 않으면 물리적 충돌만 일어나고 트리거 이벤트는 발생하지 않습니다.)
//
// 3. 트리거를 발동시킬 플레이어 GameObject에 "Player"라는 태그가 설정되어 있는지 확인하세요.
//    (태그가 다를 경우, 아래 코드의 "Player" 부분을 원하는 태그로 수정하세요.)
//
// 4. 씬(Scene) 뷰에서 콜라이더의 크기와 위치를 원하는 트리거 영역에 맞게 조절합니다.

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TriggerZone : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("트리거가 발동될 때 콘솔에 출력될 메시지입니다.")]
    [SerializeField] private string triggerMessage = "플레이어가 트리거 존에 진입했습니다!";

    private void Awake()
    {
        // 스크립트가 제대로 작동하려면 콜라이더가 반드시 트리거로 설정되어 있어야 합니다.
        // 사용자의 실수를 방지하기 위해, 코드에서도 한 번 더 확인하고 설정해줍니다.
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 트리거 영역에 들어온 오브젝트가 "Player" 태그를 가지고 있는지 확인합니다.
        if (other.CompareTag("Player"))
        {
            // 조건이 맞으면 설정된 메시지를 디버그 콘솔에 출력합니다.
            Debug.Log(triggerMessage);
        }
    }
}
