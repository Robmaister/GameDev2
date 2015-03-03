using UnityEngine;
using System.Collections;

public class DoParkour : MonoBehaviour {
	/// Parkour moveset:
	//	Edge: Climb up/mantel, tac, vault
	//	Side: wall run, tac
	//	Top: jumping, walking/running/sprinting, 


	//tac --> if jump while hitting wall, jump off wall at angle
	//climb --> if use hands or feet while hitting wall, climb up wall
	//wall run --> if collide with wall, allow movement along wall, ignoring gravity
	//


	//run into wall --> jump up wall




	//mantle --> if player arms near edge, pull self up and over
	//wall jump --> if player contacts wall, jump off it

	public bool canControl = true;

	public float maxSpeed = 10f;
	public float maxAcceleration = 20f;
	public float gravity = 10f;
	public float jumpHeight = 2f;

	private CharacterController controller;
	private Vector3 inputMoveDirection = Vector3.zero;

	[System.NonSerializedAttribute]
	public looseInput inputJump = new looseInput("Jump",.2f);
	public looseInput inputHands = new looseInput("Fire1",.2f);
	public looseInput inputFeet = new looseInput("Fire2",.2f);

	public class looseInput{//manage input by allowing leeway when pressing buttons
		public looseInput(string bt, float lw){
			leeway = lw;
			button = bt;
		}
		private float lastpress = 0;
		public bool pressed = false;
		private float leeway;
		private string button;

		public void checkInput(){
			if(Input.GetButtonDown(button)){//if button is currently pressed
				lastpress = Time.time;
				pressed = true;
			}
			else{//else check if outside leeway zone
				if((Time.time - lastpress) > leeway){
					pressed = false;
				}
			}
		}
	}

	void getInput(){
		inputJump.checkInput();
		inputHands.checkInput();
		inputFeet.checkInput();
	}

	// Use this for initialization
	void Awake () {
		controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

		getInput();//get input state for buttons

		Screen.showCursor = false;
		Screen.lockCursor = true;





		if(Input.GetKeyDown(KeyCode.Escape)){
			//Application.Quit();
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPaused = true;
			#endif
		}





		// Get the input vector from keyboard or analog stick
		Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		
		if (directionVector != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		// Apply the direction to the CharacterMotor
		inputMoveDirection = transform.rotation * directionVector;

		Vector3 velocity = controller.velocity;

		// Update velocity based on input
		velocity = ApplyInputVelocityChange(velocity);
		// Apply gravity and jumping force
		velocity = ApplyGravityAndJumping (velocity);

		Vector3 currentMovementOffset = velocity * Time.deltaTime;
		controller.Move (currentMovementOffset);
	}


	Vector3 ApplyInputVelocityChange (Vector3 velocity) {	
		if (!canControl)
			inputMoveDirection = Vector3.zero;
		
		// Find desired velocity
		Vector3 desiredVelocity;

		desiredVelocity = GetDesiredHorizontalVelocity();

		velocity.y = 0;
		
		// Enforce max velocity change
		float maxVelocityChange = maxAcceleration * Time.deltaTime;
		Vector3 velocityChangeVector = (desiredVelocity - velocity);
		if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange) {
			velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
		}
		// If we're in the air and don't have control, don't apply any velocity change at all.
		// If we're on the ground and don't have control we do apply it - it will correspond to friction.
		if (controller.isGrounded && canControl)
			velocity += velocityChangeVector;
		
		if (controller.isGrounded) {
			// When going uphill, the CharacterController will automatically move up by the needed amount.
			// Not moving it upwards manually prevent risk of lifting off from the ground.
			// When going downhill, DO move down manually, as gravity is not enough on steep hills.
			velocity.y = Mathf.Min(velocity.y, 0);
		}
		
		return velocity;
	}

	Vector3 ApplyGravityAndJumping (Vector3 velocity) {

		if (controller.isGrounded)
			velocity.y = Mathf.Min(0, velocity.y) - gravity * Time.deltaTime;
		else {
			velocity.y = controller.velocity.y - gravity * Time.deltaTime;
			velocity.y = Mathf.Max (velocity.y, -maxSpeed);
		}
		if (controller.isGrounded) {
			if (inputJump.pressed) {
				velocity += transform.up * CalculateJumpVerticalSpeed (jumpHeight);
				inputJump.pressed = false;
			}
		}		
		return velocity;
	}

	private Vector3 GetDesiredHorizontalVelocity () {
		// Find desired velocity
		Vector3 desiredLocalDirection = transform.InverseTransformDirection(inputMoveDirection);
		return transform.TransformDirection(desiredLocalDirection * maxSpeed);
	}


	float CalculateJumpVerticalSpeed (float targetJumpHeight) {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt (2 * targetJumpHeight * gravity);
	}








	//void OnControllerColliderHit (ControllerColliderHit hit) {
		//Debug.DrawRay(transform.position,hit.normal,Color.red);
		//print(Vector3.Angle(transform.up, hit.normal));
	//}


}
