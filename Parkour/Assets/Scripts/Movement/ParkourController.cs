using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ParkourController : MonoBehaviour {
	public GameObject arms;
	public GameObject legs;


	public Animator anim;



	private PhotonView photonView;
	public float networkInputH;
	public float networkInputV;

	public bool canControl = true;

	public float maxSpeed = 10f;
	public float maxAcceleration = 20f;
	public float gravity = 10f;
	public float jumpHeight = 1f;

	public CharacterController controller;
	private Vector3 inputMoveDirection = Vector3.zero;

	public bool can_jump = false;
	public bool apply_forces = true;


	// stuff for handling edge data


	public Vector3 currentEdge_left = Vector3.zero;
	public Vector3 currentEdge_right = Vector3.zero;

	//this allows hanging on a sloped edge
	public Vector3 current_hang_point = Vector3.zero;//current point on the current edge to target hanging

	public GameObject current_ledge_object = null;

	//------------------





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
	public IInput inputJump;
	public IInput inputHands;
	public IInput inputFeet;

	public Vector3 currentMovementOffset = Vector3.zero;

	public Vector3 netImpulse = Vector3.zero;

	public interface IInput
	{
		bool Pressed { get; set; }

		void Update();
	}

	//manage input by allowing leeway when pressing buttons
	public class LooseInput : IInput
	{
		public LooseInput(string bt, float lw, bool holdFlag = false){
			Pressed = false;
			leeway = lw;
			button = bt;
			if(holdFlag){//discrete press
				inputCheck = Input.GetButtonDown;
			}else{//continuous press
				inputCheck = Input.GetButton;
			}
		}

		private Func<string,bool> inputCheck;
		private float lastpress = 0;
		public bool Pressed { get; set; }
		private float leeway;
		private string button;

		public void Update(){
			if(inputCheck(button)){//if button is currently pressed
				lastpress = Time.time;
				Pressed = true;
			}
			else{//else check if outside leeway zone
				if((Time.time - lastpress) > leeway){
					Pressed = false;
				}
			}
		}
	}

	public class NetworkInput : IInput
	{
		public bool Pressed { get; set; }

		public void Update()
		{
		}
	}

	public void addImpulse(Vector3 force, float duration, bool amplified = false, Func<bool> checkfunc = null, Action endfunc = null){
		//amplified ==> determine whether player momentum should be taken into account
		//checkfunc ==> this function checks if the impulse should stop prematurely; returns true when it should exit
		//endfunc ==> this function gets called when the impulse ends

		//a negative duration means the impulse will be applied forever until it's terminated by the check function or StopCoroutine() or StopAllCoroutines()


		if(!amplified){
			StartCoroutine(_addImpulse(force, duration, checkfunc, endfunc));
		}else{
			StartCoroutine(_addImpulse2(force, duration, checkfunc, endfunc));
		}
	}

	private IEnumerator _addImpulse(Vector3 force, float duration, Func<bool> checkfunc = null, Action endfunc = null) {
		//adds a constant force over a set period of time
		float endtime = Time.time + duration;
		bool infinite = (duration < 0); //infinite impulse flag

		while(infinite || Time.time <= endtime){
			if(checkfunc != null){
				if(checkfunc()) break;
			}
			netImpulse += force;
			yield return null;
		}
		if(endfunc != null) endfunc();
	}

	private IEnumerator _addImpulse2(Vector3 force, float duration, Func<bool> checkfunc = null, Action endfunc = null) {
		//adds a variable force over a set period of time
		//force is multiplied by player's initial momentum

		bool infinite = (duration < 0); //infinite impulse flag

		//ignore vertical momentum
		float hmag = Mathf.Sqrt(controller.velocity.x * controller.velocity.x + controller.velocity.z * controller.velocity.z);
		print("hmag: " + hmag);
		force = (force.normalized) * (force.magnitude + hmag);

		if(hmag > 1){
			duration /= hmag;
		}

		float endtime = Time.time + duration;

		print("force" + force);
		
		while(infinite || Time.time <= endtime){
			if(checkfunc != null){
				if(checkfunc()) break;
			}
			netImpulse += force;
			yield return null;
		}
		if(endfunc != null) endfunc();
	}
	
	
	void getInput(){
		inputJump.Update();
		inputHands.Update();
		inputFeet.Update();
	}

	Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
	{
		Vector3 vVector1 = vPoint - vA;
		Vector3 vVector2 = (vB - vA).normalized;
		
		float d = Vector3.Distance(vA, vB);
		float t = Vector3.Dot(vVector2, vVector1);
		
		if (t <= 0)
			return vA;
		
		if (t >= d)
			return vB;
		
		Vector3 vVector3 = vVector2 * t;
		
		Vector3 vClosestPoint = vA + vVector3;
		
		return vClosestPoint;
	}


	// Use this for initialization
	void Awake () {
		anim = GetComponentInChildren<Animator>();
		controller = GetComponent<CharacterController>();
		photonView = GetComponent<PhotonView>();
		Physics.IgnoreCollision(controller,arms.GetComponent<Collider>());
		Physics.IgnoreCollision(controller,legs.GetComponent<Collider>());
		Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Players"), LayerMask.NameToLayer ("Players"));

		if (photonView != null && !photonView.isMine) {
			inputJump = new NetworkInput();
			inputHands = new NetworkInput();
			inputFeet = new NetworkInput();
			controller.enabled = false;
			canControl = false;
			//GetComponent<Rigidbody>().useGravity = false;
			//gravity = 0;
			Update ();
			Update ();
		}
		else {
			inputJump = new LooseInput("Jump",.2f,true);
			inputHands = new LooseInput("Fire1",.2f);
			inputFeet = new LooseInput("Fire2",.2f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		getInput();//get input state for buttons 

		float inputH, inputV;
		if (photonView != null && !photonView.isMine) {
			inputH = networkInputH;
			inputV = networkInputV;
		}
		else {
			inputH = Input.GetAxis("Horizontal");
			inputV = Input.GetAxis("Vertical");

			networkInputH = inputH;
			networkInputV = inputV;
		}

		// Get the input vector from keyboard or analog stick
		Vector3 directionVector = new Vector3(inputH, 0, inputV);

		//have to do this every frame because unity 5
		//Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		if(Input.GetKeyDown(KeyCode.Escape)){
			//Application.Quit();
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPaused = true;
			#else
			Application.Quit();
			#endif
		}

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

		if(apply_forces){
			inputMoveDirection = transform.rotation * directionVector;
			Vector3 velocity = controller.velocity;

				// Update velocity based on input
			velocity = ApplyInputVelocityChange(velocity);
				// Apply gravity and jumping force
			velocity = ApplyGravityAndJumping (velocity);

			currentMovementOffset = (velocity) * Time.deltaTime;
		}else{
			currentMovementOffset = Vector3.zero;
		}


		netImpulse = Vector3.zero;

	}

	void LateUpdate(){
		//if (photonView != null && !photonView.isMine)
			//return;

		netImpulse *= Time.deltaTime;

		//debug stuff
		if (vtxt != null) vtxt.text = "Velocity: " + controller.velocity;
		if (ptxt != null) ptxt.text = "Position: " + transform.position;
		if (otxt != null) otxt.text = "Rotation: " + transform.rotation.eulerAngles;
		if (ltxt != null) ltxt.text = "Arms: " + armState + "\nLegs: " + legState;
		if (itxt != null) itxt.text = "HangPoint: " + current_hang_point;
		//--------------------------

		//moved to lateupdate to allow coroutines to execute
		if(apply_forces){//if regular forces should be applied
			//print("NetImpulse: " + netImpulse + " combined: " + (currentMovementOffset + netImpulse));
			controller.Move (currentMovementOffset + netImpulse);
		}else{
			controller.Move (netImpulse);
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

		if (controller.isGrounded){
			anim.SetBool("jumping",false);
			velocity.y = Mathf.Min(0, velocity.y) - gravity * Time.deltaTime;
		}
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
		if (inputJump.Pressed) {
			if (controller.isGrounded) {//can jump off ground
				anim.SetBool("jumping",true);
				velocity += transform.up * CalculateJumpVerticalSpeed (jumpHeight);
				inputJump.Pressed = false;
				lastInputMoveDirection = inputMoveDirection;
			}
			else if(can_jump){//if can jump off a jumpable surface
				velocity += transform.up *  CalculateJumpVerticalSpeed((GetDesiredHorizontalVelocity().magnitude/maxSpeed) * jumpHeight);
				lastInputMoveDirection = inputMoveDirection;
				can_jump = false;
				inputJump.Pressed = false;
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
		}
		return transform.TransformDirection(desiredLocalDirection * maxSpeed);
	}


	float CalculateJumpVerticalSpeed (float targetJumpHeight) {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt (2 * targetJumpHeight * gravity);
	}

	void OnCollisionEnter(Collision col){
		if(col.gameObject.tag == "Parkour"){
			//raycast on object to get triangle data
			foreach( ContactPoint P in col.contacts){
				RaycastHit hit;
				Ray ray = new Ray(P.point + P.normal * 0.05f, -P.normal);
				if (P.otherCollider.Raycast(ray, out hit, 0.1f)){
					int triangle = hit.triangleIndex;
					if(triangle == -1) continue;//this means we collided with a non-mesh collider

					ObjectData objd = GeometryManager.Instance.objectDict[col.gameObject];
					
					SurfaceType s = objd.triType[triangle];

					if(P.thisCollider.gameObject == arms){
						armState |= s;
					}
					else if(P.thisCollider.gameObject == legs){
						legState |= s;
					}
				}
			}
		}
		else if (col.gameObject.tag == "Player") {
			Physics.IgnoreCollision(controller, col.collider);
			if(controller.velocity.sqrMagnitude > col.gameObject.GetComponent<ParkourController>().controller.velocity.sqrMagnitude){
				col.gameObject.BroadcastMessage("OnFlagDrop");
			}
		}
	}

	void OnCollisionStay(Collision col){
		if(col.gameObject.tag == "Parkour"){
			//raycast on object to get triangle data
			foreach( ContactPoint P in col.contacts){
				RaycastHit hit;
				Ray ray = new Ray(P.point + P.normal * 0.05f, -P.normal);
				if (P.otherCollider.Raycast(ray, out hit, 0.1f)){
					int triangle = hit.triangleIndex;
					if(triangle == -1) continue;//this means we collided with a non-mesh collider

					ObjectData objd = GeometryManager.Instance.objectDict[col.gameObject];

					SurfaceType s = objd.triType[triangle];
			
					if(P.thisCollider.gameObject == arms){
						armState |= s;

						HalfEdge tmpe = objd.edges[triangle * 3];
	
					
						if(current_ledge_object == null){
							if(tmpe.ledge){
								currentEdge_left = objd.verts[tmpe.leftVert];
								currentEdge_right = objd.verts[tmpe.rightVert];
							}
							if(tmpe.adjacentEdge.ledge){
								currentEdge_left = objd.verts[tmpe.adjacentEdge.leftVert];
								currentEdge_right = objd.verts[tmpe.adjacentEdge.rightVert];
							}
							if(tmpe.adjacentEdge.adjacentEdge.ledge){
								currentEdge_left = objd.verts[tmpe.adjacentEdge.adjacentEdge.leftVert];
								currentEdge_right = objd.verts[tmpe.adjacentEdge.adjacentEdge.rightVert];
							}

							current_hang_point = ClosestPointOnLine(currentEdge_left,currentEdge_right,transform.position);
							//print(current_hang_point);



							if(Vector3.Distance(current_hang_point,arms.transform.position) <= .75f){//ensure arms are actually in range to grab
								current_ledge_object = col.gameObject;
							}

						}
					}
					else if(P.thisCollider.gameObject == legs){
						legState |= s;
					}
				}
			}
		}
		/*else if (col.gameObject.tag == "Player") {
			Physics.IgnoreCollision(controller, col.collider);

			col.gameObject.BroadcastMessage("OnTackle");

		}*/
	}

	void OnCollisionExit(Collision col){
		if(col.gameObject.tag == "Parkour"){
			armState = legState = 0;

			if(col.gameObject == current_ledge_object){
				//currentEdge_left = currentEdge_right = current_hang_point = Vector3.zero;//current point on the current edge to target hanging
				current_ledge_object = null;
			}
		}
	}
}
