using UnityEngine;

public class GameOverScene : MonoBehaviour
{
    private Animator _animator;
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void SetGameOver(bool isGameOver)
    {
        _animator.SetBool("IsGameOver", isGameOver);
    }
}
