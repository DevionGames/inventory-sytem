using UnityEngine;
using System.Collections;

public class SM_AnimSpeedRandomizer : MonoBehaviour {
	public float minSpeed=0.7f;
	public float maxSpeed=1.5f;
	
	private void Start () {
		var anim=this.gameObject.GetComponent<Animation>();
		anim[anim.clip.name].speed = Random.Range(minSpeed, maxSpeed);
	}
}
