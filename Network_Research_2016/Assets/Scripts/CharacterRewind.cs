#region Notes
// Link this script to the character whose Transform position and rotation you want log and recall. 
// I hope this helps. -- Nick Hwang
#endregion


using UnityEngine;
using System.Collections;

public class CharacterRewind : MonoBehaviour {

	//variable to associate transform
	public Transform cTransform;

	//two arrays, one for position in world space and one for rotation of object
	private Vector3[] positions;
	private Quaternion[] rotations;

	// states (should always be exclusive from each other)
	public bool storing;
	public bool recalling;

	// how quickly the position of gameobject moves back through prev positions
	public float speed = 1.0f;

	// index of arrays (See recalling method)
	private int currIndex;

	// size of array and the interval to record the positions & rotations
	public int arraySize = 10;
	public float arrayInterval = 0.5f;

	// Use this for initialization
	void Start () {
		cTransform = gameObject.transform;
		positions = new Vector3[arraySize];   // set size of array
		rotations = new Quaternion[arraySize]; // set size of array

		resetValues ();

		storing = true;
	}
	
	// Update is called once per frame
	void Update () {

		// turn on recall -- Y button, toggles OFF storing, toggles ON recalling
		if(Input.GetKeyDown(KeyCode.Y)){
			if(!recalling){
				storing = false;
				CancelInvoke ("storePositions");

				currIndex = 0;
				recalling = true;
			}
		}
			
		if (storing) {
			if (!IsInvoking("storePositions")) {
				InvokeRepeating ("storePositions", 0f, arrayInterval);
			}
		}

		if (recalling) {
			//lerp stuff
			recallPositions();
		}
		
	}

	void storePositions(){
		// most recent in array stored at 0.

		//shift previous positions by 1
		//starts at index 8->9, 7->8, etc. lastly 0->1.
		for (int i = positions.Length-2; i > -1; i--) {
			positions [i+1] = positions [i];
			rotations [i+1] = rotations [i];
		}

		//add newest position & rotation to index 0.
		positions[0] = cTransform.position;
		rotations [0] = cTransform.rotation;
//		Debug.Log("Added point:" + cTransform.position);
	}

	void recallPositions(){
		// modified 'for loop'

		Vector3 destPos = positions [currIndex];
		Quaternion destRot = rotations [currIndex];

		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, destPos, step * Time.deltaTime);

		transform.rotation =  Quaternion.RotateTowards(transform.rotation, destRot, step * 100f * Time.deltaTime);
		//Debug.Log (currIndex + (Mathf.Abs (Vector3.Distance (destPos, transform.position))));

		// when position reaches destination, increment "currIndex", if reached arraySize, stop. switch back to storing. 
		if ((Mathf.Abs (Vector3.Distance (destPos, transform.position)) < 0.01f)) {
			Debug.Log ("Moving to next point: " + currIndex);
			currIndex++;
			if (currIndex > positions.Length-1) {
				recalling = false;
				storing = true;
				resetValues ();
				Debug.Log ("Finished");
			}
		}
	}
		
	// fills all array members to current position & rotation. Experiment with 'arraySize' and 'arrayInterval' to limit the time and # of waypoints. 
	void resetValues(){
//		for (int i = 0; i < positions.Length - 1; i++) {
//			positions [i] = transform.position;
//			rotations [i] = transform.rotation;
//		}

		foreach (Vector3 pos in positions) {
			pos = transform.position;
		}

		foreach (Vector3 rot in rotations) {
			rot = transform.rotation;
		}
	}
}
