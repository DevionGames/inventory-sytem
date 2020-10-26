using UnityEngine;
using System.Collections;

public class SM_RandomScale : MonoBehaviour {

	public float minScale=1f;
	public float maxScale=2f;

	private void Start ()
	{
		var actualRandom=Random.Range(minScale, maxScale);
		transform.localScale=new Vector3(actualRandom, actualRandom, actualRandom);
		
	}

}
