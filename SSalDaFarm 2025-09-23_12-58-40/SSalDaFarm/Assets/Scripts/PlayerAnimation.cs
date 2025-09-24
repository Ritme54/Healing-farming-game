using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    private PlayerMove pm;
    private bool isMoving;

    private void Awake()
    {
        pm = GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {       
        isMoving = (Mathf.Abs(pm.GetMoveDirection().x) + Mathf.Abs(pm.GetMoveDirection().y)) > 0;
        animator.SetBool("IsMove", isMoving);
    }


}

