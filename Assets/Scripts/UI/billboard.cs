using UnityEngine;
using System.Collections;

public class billboard : MonoBehaviour {

	Camera target_camera;
	// Use this for initialization
	void Start () {
		target_camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt (transform.position + target_camera.transform.rotation * Vector3.forward,
			target_camera.transform.rotation * Vector3.up);
	}
}
