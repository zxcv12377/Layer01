using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	//Scriptable object which holds all the player's movement parameters. If you don't want to use it
	//just paste in all the parameters, though you will need to manuly change all references in this script
	public PlayerDataWithDash Data; //Scriptable Object(플레이어의 파라미터)를 가진 데이터

	#region COMPONENTS
	public Rigidbody2D RB { get; private set; }
	public PlayerAnimator animHandler { get; private set; }
	#endregion

	#region STATE PARAMETERS
	//Variables control the various actions the player can perform at any time.
	//These are fields which can are public allowing for other sctipts to read them
	//but can only be privately written to.
	public bool IsFacingRight { get; private set; } // 캐릭터의 좌우를 지정하는 변수
	public bool IsJumping { get; private set; } // 캐릭터가 점프중인지 확인하는 변수
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; } // 대시중인지 확인하는 변수
	public bool IsSliding { get; private set; } // 슬라이드 중인지 확인하는 변수

	//Timers (also all fields, could be private and a method returning a bool could be used)
	public float LastOnGroundTime { get; private set; } // 캐릭터가 지면에서 얼마나 떨어져 있는지 알려주는 변수
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    //Jump
    private bool _isJumpCut; // 점프컷 중인지 확인하는 변수 (점프컷이란 키가 입력된 시간에따라 점프의 높이를 조절할수 있는 것)
	private bool _isJumpFalling; // 캐릭터가 점프 후 낙하하는 것을 확인하는 변수

    //Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    //Dash
    private int _dashesLeft; //
	private bool _dashRefilling; // 대쉬의 쿨타임
	private Vector2 _lastDashDir; // 대쉬 방향
	private bool _isDashAttacking; // 대쉬 어택중인지 확인하는 변수

	#endregion

	#region INPUT PARAMETERS
	private Vector2 _moveInput; // 입력된 움직임은 저장하는 변수

	public float LastPressedJumpTime { get; private set; } //연속적인 점프를 못하게 막기 위한 변수
	public float LastPressedDashTime { get; private set; } //연속적인 대쉬를 못하게 막기 위한 변수
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
		SetGravityScale(Data.gravityScale); // GravityScale을 초기화
		IsFacingRight = true; // 캐릭터의 앞부분을 오른쪽으로
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
		animHandler.isMove = _moveInput.x != 0 ? true : false; // x축의 입력이 있을경우 달리는 모션을 취할수 있게 함.

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
				|| Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _oneWayLayer)) && !IsJumping) // 지면을 밟고 있는지 체크
			{
                if (LastOnGroundTime < 0.1f)
                {
                    animHandler.justLanded = true;
                }
				animHandler.isWallJump = false;
				animHandler.isGround = true;
                LastOnGroundTime = Data.coyoteTime; //CoyoteTime이란 캐릭터가 낙하를 시작할 때 점프를 하지 않은 상태에서 일정 시간에 도달하기 전에 점프를 할 수 있게 하는 것
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
		// 점프 후 낙하중
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

        // 점프중이 아닐때 낙하중
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
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left; // 대쉬의 방향 지정



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
				// 아래방향으로 입력이 있다면 더욱 강한 중력을 적용함.
				SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
				// 중력을 최대속도이상으로 가지 못하게 함.
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				// 점프 입력 버튼을 때면 더 강한 중력을 적용함.	
				SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
			}
			else if ((IsJumping /*|| IsWallJumping*/ || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
			{
				// 수직낙하때보다 약한 중력을 주어서 공중에서 조금더 자유롭게 움직일 수 있게 함.
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
		// StartCoroutine을 항상 사용할 필요가 없음.
		// nameof() 는 문자열에 의존하여 사용하지 않기 위함.
		// 삭제 되거나 변경되어도 에러 메세지를 출력하기 때문에 대응하기도 좋음.
		StartCoroutine(nameof(PerformSleep), duration);
	}

	private IEnumerator PerformSleep(float duration)
	{	// 대쉬를 하기전 DashSleepTime으로 지정해준 시간만큼 시간을 잠시 멈추는 기능
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
		// 방향 전환과 속도를 부드럽게 하기 위해서 Mathf.Lerp()함수를 이용함.
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
		else // 공중에서의 가감속을 설정해줌.
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		// 점프를 조금더 자연스럽게 만들어 주기 위해서 정점에서 최대 속도가 지정된 만큼(jumpHangAccelerationMult, jumpHangMaxSpeedMult) 증가함.
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
			//최대 속도를 넘어가지 않도록 운동량을 보존함.
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0;
		}
		#endregion


		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - RB.velocity.x;
		//Calculate force along x-axis to apply to thr player

		float movement = speedDif * accelRate;

		//Rigidbody에 적용하고 벡터로 변환
		RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

		/*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
	}

	private void Turn()
	{
		//스케일을 조정하여 캐릭터의 좌우 방향을 지정함. 스케일을 사용하는 이유는 flip을 쓰면 자식 오브젝트들은 함께 움직이지 않기 때문에 번거로워지기 때문.
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
        if (RB.velocity.y < 0) // 어째서 사용했는지 잘 이해가 안됨... coyoteTime 점프를 했을때 높게 점프를 하기 위함?
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
			// 한 프레임을 넘기기 위해 루프를 잠시 멈춤
			yield return null;
		}

		startTime = Time.time;

		_isDashAttacking = false;

		//플레이어에게 제어권을 다시 일부 반환해주고 가속을 제한함
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
		//대시를 계속해서 사용할 수 없게 쿨다운을 줌
		_dashRefilling = true;
		yield return new WaitForSeconds(Data.dashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
	}
	#endregion

	#region OTHER MOVEMENT METHODS
	private void Slide() // 크게 체감이 되지 않는 부분으로 조금더 공부가 필요할 듯함.
	{
		//Run()함수와 같지만 Y축으로만 작용함
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
