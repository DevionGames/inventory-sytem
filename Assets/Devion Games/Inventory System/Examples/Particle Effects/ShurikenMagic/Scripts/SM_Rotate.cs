using UnityEngine;
using System.Collections;

public class SM_Rotate : MonoBehaviour {
	public Vector3 rotation;
	public Space space;
	
	private void Update () {
		transform.Rotate (rotation * Time.deltaTime,space);
	}
}
