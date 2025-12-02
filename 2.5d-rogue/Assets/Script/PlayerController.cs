using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public float walkSpeed = 400f;    // 走路力量
    public float runSpeed = 600f;       // 跑步力量

    [Header("组件引用")]
    public Rigidbody2D rb;
    public Animator anim;

    // 私有变量
    private Vector2 movementInput;
    private bool isRunning;
    private bool isAttacking;
    private string currentDirection = "right"; // 默认朝右

    void Start()
    {
        // 初始化组件引用
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (anim == null)
            anim = GetComponent<Animator>();

        // 设置刚体属性以确保平滑移动
        if (rb != null)
        {
            rb.gravityScale = 0; // 关闭重力
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 锁定旋转
        }

        void Update()
        {
            HandleInput();
            UpdateAnimations();
        }

        void FixedUpdate()
        {
            HandleMovement();
        } 
    }


    /// <summary>
    /// 处理玩家输入
    /// </summary>
    private void HandleInput()
    {
        // 获取WASD输入
        float horizontal = Input.GetAxisRaw("Horizontal"); // Raw 避免缓动
        float vertical = Input.GetAxisRaw("Vertical");

        movementInput = new Vector2(horizontal, vertical).normalized;

        // 检测Shift键是否按下
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // 检测J键攻击
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartAttack();
        }
    }

    /// <summary>
    /// 处理角色移动
    /// </summary>
    private void HandleMovement()
    {
        if (!isAttacking) // 攻击期间可以继续移动，如果需要锁定移动，可以去掉这个条件
        {
            // 确定当前速度和方向
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            // 使用AddForce实现平滑移动，也可以用Velocity直接设置
            Vector2 targetVelocity = movementInput * currentSpeed * Time.fixedDeltaTime;
            rb.velocity = targetVelocity;

            // 处理角色朝向
            HandleFacingDirection();
        }
    }

    /// <summary>
    /// 处理角色朝向
    /// </summary>
    private void HandleFacingDirection()
    {
        // 如果有移动输入，才更新朝向
        if (movementInput.magnitude > 0.1f)
        {
            // 优先根据水平方向决定朝向
            if (movementInput.x != 0)
            {
                currentDirection = movementInput.x > 0 ? "right" : "left";
            }

            // 传递给动画控制器
            anim.SetFloat("Horizontal", movementInput.x);
            anim.SetFloat("Vertical", movementInput.y);
        }
    }

    /// <summary>
    /// 更新动画参数
    /// </summary>
    private void UpdateAnimations()
    {
        // 计算移动幅度（用于判断是否在移动）
        float moveMagnitude = movementInput.magnitude;
        anim.SetFloat("Speed", moveMagnitude);
        anim.SetBool("IsRunning", isRunning);

        // 设置朝向参数
        anim.SetBool("FaceRight", currentDirection == "right");
        anim.SetBool("FaceLeft", currentDirection == "left");
    }

    /// <summary>
    /// 开始攻击
    /// </summary>
    private void StartAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");

        // 如果是跑步时攻击，触发跑砍动画
        if (isRunning && movementInput.magnitude > 0.1f)
        {
            anim.SetTrigger("RunAttack");
        }
    }

    /// <summary>
    /// 攻击结束回调（在动画事件中调用）
    /// </summary>
    public void OnAttackEnd()
    {
        isAttacking = false;
    }
}