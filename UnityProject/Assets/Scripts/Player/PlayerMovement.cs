using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Animator))] // 强制自动添加 Animator 组件，防止遗漏
public class PlayerMovement : NetworkBehaviour
{
    [Header("移动设置")] // 帮你修复了原本乱码的中文字符
    public float speed = 3.0f;
    public float smoothTime = 0.1f;

    private Vector2 moveInput;
    private Vector2 moveTarget;
    private Rigidbody2D rb;

    // --- 新增：动画组件与朝向记录 ---
    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.down; // 默认开局朝下
    public override void OnNetworkSpawn()
    {
        // 如果这不是我控制的角色（即远程玩家在我的屏幕上的投影）
        if (!IsOwner)
        {
            // 找到子对象（或者父对象）身上的 AudioListener
            AudioListener listener = GetComponentInChildren<AudioListener>();

            if (listener != null)
            {
                // 关键一步：把他的耳朵禁用掉！
                listener.enabled = false;
                Debug.Log($"已禁用非本地玩家 {OwnerClientId} 的耳朵。");
            }
        }
        else
        {
            // 如果是我自己的角色，确保耳朵是张开的
            Debug.Log("这是我的角色，耳朵已准备就绪！");
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>(); // 获取 Animator
    }

    void Update()
    {
        if (!IsOwner) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // --- 新增：动画状态机传值 ---
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        moveTarget = moveInput * speed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, moveTarget, smoothTime);
    }

    // --- 新增：独立的动画处理方法，保持 Update 干净整洁 ---
    private void UpdateAnimation()
    {
        if (animator == null) return;

        // 如果玩家正在按下方向键
        if (moveInput != Vector2.zero)
        {
            // 更新最后的朝向
            lastMoveDirection = moveInput;

            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
        }
        else
        {
            // 如果玩家松开了按键，保持传递最后的面朝方向，防止静止时朝向重置
            animator.SetFloat("MoveX", lastMoveDirection.x);
            animator.SetFloat("MoveY", lastMoveDirection.y);
        }

        // 传递速度给 Animator (sqrMagnitude 性能比 magnitude 更好，静止时为0，移动时大于0)
        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }
}