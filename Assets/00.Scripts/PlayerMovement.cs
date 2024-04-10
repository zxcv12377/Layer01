using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	//Scriptable object which holds all the player's movement parameters. If you don't want to use it
	//just paste in all the parameters, though you will need to manuly change all references in this script
	public PlayerDataWithDash Data; //Scriptable Object(�÷��̾��� �Ķ����)�� ���� ������

	#region COMPONENTS
	public Rigidbody2D RB { get; private set; }
	public PlayerAnimator animHandler { get; private set; }
	#endregion

	#region STATE PARAMETERS
	//Variables control the various actions the player can perform at any time.
	//These are fields which can are public allowing for other sctipts to read them
	//but can only be privately written to.
	public bool IsFacingRight { get; private set; } // ĳ������ �¿츦 �����ϴ� ����
	public bool IsJumping { get; private set; } // ĳ���Ͱ� ���������� Ȯ���ϴ� ����
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; } // ��������� Ȯ���ϴ� ����
	public bool IsSliding { get; private set; } // �����̵� ������ Ȯ���ϴ� ����

	//Timers (also all fields, could be private and a method returning a bool could be used)
	public float LastOnGroundTime { get; private set; } // ĳ���Ͱ� ���鿡�� �󸶳� ������ �ִ��� �˷��ִ� ����
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    //Jump
    private bool _isJumpCut; // ������ ������ Ȯ���ϴ� ���� (�������̶� Ű�� �Էµ� �ð������� ������ ���̸� �����Ҽ� �ִ� ��)
	private bool _isJumpFalling; // ĳ���Ͱ� ���� �� �����ϴ� ���� Ȯ���ϴ� ����

    //Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    //Dash
    private int _dashesLeft; //
	private bool _dashRefilling; // �뽬�� ��Ÿ��
	private Vector2 _lastDashDir; // �뽬 ����
	private bool _isDashAttacking; // �뽬 ���������� Ȯ���ϴ� ����

	#endregion

	#region INPUT PARAMETERS
	private Vector2 _moveInput; // �Էµ� �������� �����ϴ� ����

	public float LastPressedJumpTime { get; private set; } //�������� ������ ���ϰ� ���� ���� ����
	public float LastPressedDashTime { get; private set; } //�������� �뽬�� ���ϰ� ���� ���� ����
	#endregion

	#region CHECK PARAMETERS
	//Set all of these up in the inspector
	[Header("Checks")]
	[SerializeField] private Transform _groundCheckPoint;
	//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
	[SerializeField] private Vector2 _groundCheckSize = new Vector2(1f, 0.15f);//(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
	[SerializeField] private LayerMask _oneWayLayer;
	#endregion

	private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
		animHandler = GetComponent<PlayerAnimator>();

	}

	private void Start()
	{
		SetGravityScale(Data.gravityScale); // GravityScale�� �ʱ�ȭ
		IsFacingRight = true; // ĳ������ �պκ��� ����������
	}

	private void Update()
	{
		//Debug.Log(RB.velocity.y);
		//Debug.Log(LastOnGroundTime);
		#region TIMERS
		LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
		LastPressedDashTime -= Time.deltaTime;
		#endregion

		#region INPUT HANDLER
		_moveInput.x = Input.GetAxisRaw("Horizontal");
		_moveInput.y = Input.GetAxisRaw("Vertical");
		animHandler.isMove = _moveInput.x != 0 ? true : false; // x���� �Է��� ������� �޸��� ����� ���Ҽ� �ְ� ��.

		if (_moveInput.x != 0)
			CheckDirectionToFace(_moveInput.x > 0);

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C))
		{
			OnJumpInput();
		}

		if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C))
		{
			OnJumpUpInput();
		}

		if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.LeftShift))
		{
			OnDashInput();
		}
        if (Input.GetKeyDown(KeyCode.X))
        {

        }
		#endregion

		CollisionCheck();
		JumpCheck();
		DashCheck();
		//SlideCheck();
		Gravity();
	}

	private void FixedUpdate()
	{
		//Handle Run
		if (!IsDashing)
		{
            if (IsWallJumping)
                Run(Data.wallJumpRunLerp);
            else
                Run(1);
		}
		else if (_isDashAttacking)
		{
			Run(Data.dashEndRunLerp);
		}

		//Handle Slide
		if (IsSliding)
			Slide();
	}

	#region COLLISION CHECKS
	public void CollisionCheck()
    {
		if (!IsDashing && !IsJumping)
		{
			//Ground Check
			if ((Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) 
				|| Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _oneWayLayer)) && !IsJumping) // ������ ��� �ִ��� üũ
			{
                if (LastOnGroundTime < 0.1f)
                {
                    animHandler.justLanded = true;
                }
				animHandler.isWallJump = false;
				animHandler.isGround = true;
                LastOnGroundTime = Data.coyoteTime; //CoyoteTime�̶� ĳ���Ͱ� ���ϸ� ������ �� ������ ���� ���� ���¿��� ���� �ð��� �����ϱ� ���� ������ �� �� �ְ� �ϴ� ��
			}
            else
            {
				animHandler.isGround = false;
			}

			#region WALL CHECK COMMENT
			//Front Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
                 || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
            {
				animHandler.isWallSlide = true;
				LastOnWallRightTime = Data.coyoteTime;
			}
            //Back Wall Check
            else if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
            {
				animHandler.isWallSlide = true;
				LastOnWallLeftTime = Data.coyoteTime;
			}
            else
            {
				animHandler.isWallSlide = false;
			}

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
			#endregion
		}
	}
	#endregion

	#region JUMP CHECKS
	public void JumpCheck()
    {
		// ���� �� ������
		if (IsJumping && RB.velocity.y < 0)
		{
			IsJumping = false;
			animHandler.isJump = IsJumping;
			if (!IsWallJumping) _isJumpFalling = true;
		}

        if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        // �������� �ƴҶ� ������
        if (LastOnGroundTime > 0 && !IsJumping /*&& !IsWallJumping*/)
		{
			_isJumpCut = false;

			if (!IsJumping)
				_isJumpFalling = false;
		}

		if (!IsDashing)
		{
			//Jump
			if (CanJump() && LastPressedJumpTime > 0)
			{
				IsJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				animHandler.isJump = true;
				Jump();

				animHandler.startedJumping = true;
			}
            //WALL JUMP
            else if (CanWallJump() && LastPressedJumpTime > 0)
            {
                IsWallJumping = true;
                IsJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;

                _wallJumpStartTime = Time.time;
                _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

                WallJump(_lastWallJumpDir);
            }
        }
	}
	#endregion

	#region DASH CHECKS
	public void DashCheck()
    {
		if (CanDash() && LastPressedDashTime > 0)
		{
			//Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
			Sleep(Data.dashSleepTime);

			//If not direction pressed, dash forward
			if (_moveInput != Vector2.zero)
				_lastDashDir = _moveInput;
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left; // �뽬�� ���� ����



			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}
	}
	#endregion

	#region SLIDE CHECKS
	//public void SlideCheck()
	//   {
	//       if (CanSlide() && ((/*LastOnWallLeftTime > 0 && */_moveInput.x < 0) || (/*LastOnWallRightTime > 0 && */_moveInput.x > 0)))
	//           IsSliding = true;
	//       else
	//           IsSliding = false;
	//   }
	#endregion

	#region GRAVITY
	public void Gravity()
    {
		if (!_isDashAttacking)
		{
			//Higher gravity if we've released the jump input or are falling
			if (IsSliding)
			{
				SetGravityScale(0);
			}
			else if (RB.velocity.y < 0 && _moveInput.y < 0)
			{
				// �Ʒ��������� �Է��� �ִٸ� ���� ���� �߷��� ������.
				SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
				// �߷��� �ִ�ӵ��̻����� ���� ���ϰ� ��.
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				// ���� �Է� ��ư�� ���� �� ���� �߷��� ������.	
				SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
			}
			else if ((IsJumping /*|| IsWallJumping*/ || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
			{
				// �������϶����� ���� �߷��� �־ ���߿��� ���ݴ� �����Ӱ� ������ �� �ְ� ��.
				SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
			}
			else if (RB.velocity.y < 0)
			{
				//Higher gravity if falling
				SetGravityScale(Data.gravityScale * Data.fallGravityMult);
				//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
			}
			else
			{
				//Default gravity if standing on a platform or moving upwards
				SetGravityScale(Data.gravityScale);
			}
		}
		else
		{
			//No gravity when dashing (returns to normal once initial dashAttack phase over)
			SetGravityScale(0);
		}
	}
	#endregion

	#region INPUT CALLBACKS
	//Methods which whandle input detected in Update()
	public void OnJumpInput()
	{
		LastPressedJumpTime = Data.jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}

	public void OnDashInput()
	{
		LastPressedDashTime = Data.dashInputBufferTime;
	}
	#endregion

	#region GENERAL METHODS
	public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}

	private void Sleep(float duration)
	{
		// StartCoroutine�� �׻� ����� �ʿ䰡 ����.
		// nameof() �� ���ڿ��� �����Ͽ� ������� �ʱ� ����.
		// ���� �ǰų� ����Ǿ ���� �޼����� ����ϱ� ������ �����ϱ⵵ ����.
		StartCoroutine(nameof(PerformSleep), duration);
	}

	private IEnumerator PerformSleep(float duration)
	{	// �뽬�� �ϱ��� DashSleepTime���� �������� �ð���ŭ �ð��� ��� ���ߴ� ���
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
		Time.timeScale = 1;
	}
	#endregion

	//MOVEMENT METHODS
	#region RUN METHODS
	private void Run(float lerpAmount)
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = _moveInput.x * Data.runMaxSpeed;
		// ���� ��ȯ�� �ӵ��� �ε巴�� �ϱ� ���ؼ� Mathf.Lerp()�Լ��� �̿���.
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
		else // ���߿����� �������� ��������.
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		// ������ ���ݴ� �ڿ������� ����� �ֱ� ���ؼ� �������� �ִ� �ӵ��� ������ ��ŭ(jumpHangAccelerationMult, jumpHangMaxSpeedMult) ������.
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			//�ִ� �ӵ��� �Ѿ�� �ʵ��� ����� ������.
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0;
		}
		#endregion


		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - RB.velocity.x;
		//Calculate force along x-axis to apply to thr player

		float movement = speedDif * accelRate;

		//Rigidbody�� �����ϰ� ���ͷ� ��ȯ
		RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

		/*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
	}

	private void Turn()
	{
		//�������� �����Ͽ� ĳ������ �¿� ������ ������. �������� ����ϴ� ������ flip�� ���� �ڽ� ������Ʈ���� �Բ� �������� �ʱ� ������ ���ŷο����� ����.
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}
	#endregion

	#region JUMP METHODS
	private void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Jump
		//We increase the force applied if we are falling
		//This means we'll always feel like we jump the same amount 
		//(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
		float force = Data.jumpForce;
        if (RB.velocity.y < 0) // ��°�� ����ߴ��� �� ���ذ� �ȵ�... coyoteTime ������ ������ ���� ������ �ϱ� ����?
            force -= RB.velocity.y;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}

    private void WallJump(int dir)
    {
        //Ensures we can't call Wall Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        #region Perform Wall Jump
        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
            force.x -= RB.velocity.x;

        if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= RB.velocity.y;

		//Unlike in the run we want to use the Impulse mode.
		//The default mode will apply are force instantly ignoring masss
		animHandler.isWallJump = true;
        RB.AddForce(force, ForceMode2D.Impulse);
        #endregion
    }
    #endregion

    #region DASH METHODS
    //Dash Coroutine
    private IEnumerator StartDash(Vector2 dir)
	{
		LastOnGroundTime = 0;
		LastPressedDashTime = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_isDashAttacking = true;

		SetGravityScale(0);
		animHandler.startedDash = true;

		while (Time.time - startTime <= Data.dashAttackTime)
		{
			RB.velocity = dir.normalized * Data.dashSpeed;
			// �� �������� �ѱ�� ���� ������ ��� ����
			yield return null;
		}

		startTime = Time.time;

		_isDashAttacking = false;

		//�÷��̾�� ������� �ٽ� �Ϻ� ��ȯ���ְ� ������ ������
		SetGravityScale(Data.gravityScale);
		RB.velocity = Data.dashEndSpeed * dir.normalized;
		

		while (Time.time - startTime <= Data.dashEndTime)
		{
			yield return null;
		}
		
		//Dash over
		IsDashing = false;
	}

	//Short period before the player is able to dash again
	private IEnumerator RefillDash(int amount)
	{
		//��ø� ����ؼ� ����� �� ���� ��ٿ��� ��
		_dashRefilling = true;
		yield return new WaitForSeconds(Data.dashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
	}
	#endregion

	#region OTHER MOVEMENT METHODS
	private void Slide() // ũ�� ü���� ���� �ʴ� �κ����� ���ݴ� ���ΰ� �ʿ��� ����.
	{
		//Run()�Լ��� ������ Y�����θ� �ۿ���
		//THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
		float speedDif = Data.slideSpeed - RB.velocity.y;
		float movement = speedDif * Data.slideAccel;
		//So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
		//The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		RB.AddForce(movement * Vector2.up);
	}
    #endregion

    #region ITEM METHODS
	public void ItemApple()
    {
		_dashesLeft = 1;
    }
    #endregion

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

	private bool CanJump()
	{
		return LastOnGroundTime > 0 && !IsJumping;
	}

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
             (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
	{
		return IsJumping && RB.velocity.y > 0;
	}

    private bool CanWallJumpCut()
    {
        return IsWallJumping && RB.velocity.y > 0;
    }

    private bool CanDash()
	{
		if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
		{
			StartCoroutine(nameof(RefillDash), 1);
		}

		return _dashesLeft > 0;
	}

	public bool CanSlide()
	{
		if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
			return true;
		else
			return false;
	}
	#endregion


	#region EDITOR METHODS
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
		Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
	}
	#endregion

}
