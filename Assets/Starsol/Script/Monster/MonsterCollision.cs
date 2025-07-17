using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("플레이어가 몬스터에 닿았습니다! 게임을 재시작합니다.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
