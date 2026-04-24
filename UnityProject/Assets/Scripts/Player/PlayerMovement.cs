using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("ÒÆ¶¯²ÎÊý")]
    public float speed = 5.0f;
    public float smoothTime = 0.1f;

    private Vector2 moveInput;
    private Vector2 moveTarget;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;
    }
    private void FixedUpdate()
    {
        if(!IsOwner) return;
        moveTarget = moveInput * speed;
        rb.linearVelocity=Vector2.Lerp(rb.linearVelocity, moveTarget, smoothTime);
    }
}
