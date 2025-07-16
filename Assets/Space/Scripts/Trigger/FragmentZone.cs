using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class FragmentZone : MonoBehaviour, IInteractable
{
    [Tooltip("이 조각과 연결된 NPC의 Interaction Zone을 여기에 연결하세요.")]
    public NpcInteractionZone npcInteractionZone;

    private bool canInteract = false; // 플레이어가 영역 안에 있는지 확인하는 플래그

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Interact()
    {
        // NPC 스크립트가 연결되었는지 확인
        if (npcInteractionZone != null)
        {
            npcInteractionZone.AddFragment();
        }
        else
        {
            Debug.LogWarning("FragmentZone에 NpcInteractionZone이 연결되지 않았습니다.", this);
        }

        // --- 컴포넌트 비활성화 ---
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        this.enabled = false; // 이 스크립트도 비활성화
    }
}