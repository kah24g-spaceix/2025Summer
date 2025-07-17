public interface IInteractable
{
    // 이제 Interact 함수는 상호작용을 요청한 PlayerController를 매개변수로 받습니다.
    void Interact(PlayerController playerController);
}