using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ParkourController : MonoBehaviour {
	public GameObject arms;
	public GameObject legs;



	public bool canControl = true;

	public float maxSpeed = 10f;
	public float maxAcceleration = 20f;
	public float gravity = 10f;
	public float jumpHeight = 1f;

	public CharacterController controller;
	private Vector3 inputMoveDirection = Vector3.zero;

	private Vector3 lastPos = Vector3.zero;

	public bool can_jump = false;
	public bool apply_forces = true;

	//for debugging purposes
	public Text vtxt;
	public Text otxt;
	public Text ptxt;
	public Text ltxt;
	public Text itxt;
	//----------------------

	private Vector3 lastInputMoveDirection = Vector3.zero;

	[System.NonSerializedAttribute]
	public SurfaceType armState = 0;
	public SurfaceType legState = 0;
	public looseInput inputJump = new looseInput("Jump",.2f);
	public looseInput inputHands = new looseInput("Fire1",.2f);
	public looseInput inputFeet = new looseInput("Fire2",.2f);

	private Vector3 currentMovementOffset = Vector3.zero;

	private Vector3 netImpulse = Vector3.zero;

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

	public void addImpulse(Vector3 force, float duration, bool amplified = false){
		if(!amplified){
			StartCoroutine(_addImpulse(force, duration));
		}else{
			StartCoroutine(_addImpulse2(force, duration));
		}
	}

	private IEnumerator _addImpulse(Vector3 force, float duration) {
		//adds a constant force over a set period of time
		float endtime = Time.time + duration;

		while(Time.time < endtime){
			netImpulse += force;
			yield return null;
		}
	}

	private IEnumerator _addImpulse2(Vector3 force, float duration) {
		//adds a variable force over a set period of time
		//force is multiplied by player's initial momentum

		//ignore vertical momentum
		float hmag = Mathf.Sqrt(controller.velocity.x * controller.velocity.x + controller.velocity.y * controller.velocity.y);

		float mul = (force.magnitude + hmag) / 2;
		print("mul: " + mul);

		force = force.normalized * mul;

		duration /= mul;

		float endtime = Time.time + duration;

		print("force" + force);
		
		while(Time.time < endtime){
			netImpulse += force;
			yield return null;
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
		Physics.IgnoreCollision(controller,arms.GetComponent<Collider>());
		Physics.IgnoreCollision(controller,legs.GetComponent<Collider>());

	}
	
	// Update is called once per frame
	void Update () {
		getInput();//get input state for buttons

		//have to do this every frame because unity 5
		//Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

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

		currentMovementOffset = (velocity) * Time.deltaTime;

		netImpulse = Vector3.zero;

	}

	void LateUpdate(){
		netImpulse *= Time.deltaTime;


		//debug stuff
		vtxt.text = "Velocity: " + controller.velocity;
		ptxt.text = "Position: " + transform.position;
		otxt.text = "Rotation: " + transform.rotation.eulerAngles;
		ltxt.text = "Arms: " + armState + "\nLegs: " + legState;
		itxt.text = "Impulse: " + netImpulse;
		//--------------------------

		//moved to lateupdate to allow coroutines to execute
		if(apply_forces){//if regular forces should be applied
			//print("NetImpulse: " + netImpulse + " combined: " + (currentMovementOffset + netImpulse));
			controller.Move (currentMovementOffset + netImpulse);
			lastPos = transform.position;
		}else{
			controller.Move (netImpulse);
			lastPos = transform.position;
		}
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
			if((controller.velocity.y > 0) && (Mathf.Abs (controller.velocity.x) < .1f || Mathf.Abs(controller.velocity.z) < .1f)){
				Vector3 desiredVelocity = GetDesiredHorizontalVelocity();


				float maxVelocityChange = maxAcceleration * Time.deltaTime;
				Vector3 velocityChangeVector = (desiredVelocity - velocity);
				if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange) {
					velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
				}
				velocity += velocityChangeVector;
			}

			velocity.y = controller.velocity.y - gravity * Time.deltaTime;
			velocity.y = Mathf.Max (velocity.y, -maxSpeed);

		}
		if (inputJump.pressed) {
			if (controller.isGrounded) {//can jump off ground
				velocity += transform.up * CalculateJumpVerticalSpeed (jumpHeight);
				inputJump.pressed = false;
				lastInputMoveDirection = inputMoveDirection;
			}
			else if(can_jump){//if can jump off a jumpable surface
				//velocity = GetDesiredHorizontalVelocity();
				velocity += transform.up *  CalculateJumpVerticalSpeed((GetDesiredHorizontalVelocity().magnitude/maxSpeed) * jumpHeight);
				lastInputMoveDirection = inputMoveDirection;
				can_jump = false;
				inputJump.pressed = false;
			}
		}		
		return velocity;
	}

	private Vector3 GetDesiredHorizontalVelocity () {
		// Find desired velocity
		Vector3 desiredLocalDirection;
		if(controller.isGrounded){
			desiredLocalDirection = transform.InverseTransformDirection(inputMoveDirection);
		}else{
			desiredLocalDirection = transform.InverseTransformDirection(lastInputMoveDirection);
			//lastInputMoveDirection /= (controller.velocity.y + 0.001f);
		}
		return transform.TransformDirection(desiredLocalDirection * maxSpeed);
	}


	float CalculateJumpVerticalSpeed (float targetJumpHeight) {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt (2 * targetJumpHeight * gravity);
	}


	void OnCollisionEnter(Collision col){
		//raycast on object to get triangle data
		foreach( ContactPoint P in col.contacts){
			RaycastHit hit;
			Ray ray = new Ray(P.point + P.normal * 0.05f, -P.normal);
			if (P.otherCollider.Raycast(ray, out hit, 0.1f)){
				int triangle = hit.triangleIndex;
				if(triangle == -1) continue;//this means we collided with a non-mesh collider

				SurfaceType s = GeometryManager.Instance.objectDict[col.gameObject].triType[triangle];

				if(P.thisCollider.gameObject == arms){
					armState |= s;
				}
				else if(P.thisCollider.gameObject == legs){
					legState |= s;
				}
			}
		}
	}

	void OnCollisionStay(Collision col){
		//raycast on object to get triangle data
		foreach( ContactPoint P in col.contacts){
			RaycastHit hit;
			Ray ray = new Ray(P.point + P.normal * 0.05f, -P.normal);
			if (P.otherCollider.Raycast(ray, out hit, 0.1f)){
				int triangle = hit.triangleIndex;
				if(triangle == -1) continue;//this means we collided with a non-mesh collider
				
				SurfaceType s = GeometryManager.Instance.objectDict[col.gameObject].triType[triangle];
				
				if(P.thisCollider.gameObject == arms){
					armState |= s;
				}
				else if(P.thisCollider.gameObject == legs){
					legState |= s;
				}
			}
		}
	}

	void OnCollisionExit(Collision col){
		if(col.gameObject.tag == "Parkour"){
			armState = legState = 0;
		}
	}
}
