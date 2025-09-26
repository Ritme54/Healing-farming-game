using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] Vector2 moveDirection;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;
   

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();  
    }

    void Update()
    {
        GetDirection();
    }
    void GetDirection()
    {
        moveDirection.y = Input.GetAxisRaw("Vertical");
        moveDirection.x = Input.GetAxisRaw("Horizontal");

        moveDirection.Normalize();

        if (moveDirection.x > 0)
        {
            sr.flipX = false;
        }
        else if (moveDirection.x < 0)
        {
            sr.flipX = true;
        }
    }

    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        Vector2 newPos = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }
}
