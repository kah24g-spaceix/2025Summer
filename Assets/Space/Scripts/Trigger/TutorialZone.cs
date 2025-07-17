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
public class TutorialZone : MonoBehaviour
{
    public GameObject tutorialKey;
    private bool pressKey;
    private void Awake()
    {
        tutorialKey = gameObject.GetComponentInChildren<Transform>().gameObject;
        tutorialKey.SetActive(false);
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            pressKey = true;       
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 트리거 영역에 들어온 오브젝트가 "Player" 태그를 가지고 있는지 확인합니다.
        if (other.CompareTag("Player"))
        {
            if (pressKey) tutorialKey.SetActive(false);
            else tutorialKey.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // 트리거 영역에 들어온 오브젝트가 "Player" 태그를 가지고 있는지 확인합니다.
        if (other.CompareTag("Player"))
        {
            tutorialKey.SetActive(false);
        }
    }
}
