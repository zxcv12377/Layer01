using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 

public class CharacterMove : MonoBehaviour { 


	private Rigidbody2D myRidgidbody; 

	private Animator myAnimator;

	[SerializeField]
	private float maxSpeed;

	//Do player look at the right
	private bool facingRight;

	//Jump Variables
	public float jump;
	float moveVelocity;
	bool grounded = false;



	void Start () 
	{ 
		facingRight = true;
		myRidgidbody = GetComponent<Rigidbody2D>(); 
		myAnimator = GetComponent<Animator> ();
	} 


	void Update()
	{
		//Jump
		if (Input.GetKeyDown (KeyCode.Space))
		{
			
			if (grounded) 
			{
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (
				GetComponent <Rigidbody2D> ().velocity.x, jump);
		
			}
		}
	}

	//Check if player is grounded
	private void OnTriggerEnter2D()
	{
		grounded = true;
	}

	private void OnTriggerExit2D()
	{
		grounded = false;
	}

	void FixedUpdate() 
	{ 
		float horizontal = Input.GetAxis ("Horizontal");
		HandleMovement (horizontal);

		Flip (horizontal);
	} 


	private void HandleMovement(float horizontal) 
	{
		myRidgidbody.velocity = new Vector2 (horizontal * maxSpeed, myRidgidbody.velocity.y);
	
		myAnimator.SetFloat ("speed", Mathf.Abs(horizontal));
	} 


	private void Flip(float horizontal)
	{
		//If we are not looking right and moving then flip player to left and opposite
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) 
		{
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;

			theScale.x *= -1;

			transform.localScale = theScale;
		}
	}
} 