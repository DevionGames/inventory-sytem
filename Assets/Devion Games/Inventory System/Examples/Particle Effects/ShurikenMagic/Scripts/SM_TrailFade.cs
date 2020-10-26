using UnityEngine;
using System.Collections;

public class SM_TrailFade : MonoBehaviour {
	public float fadeInTime=0.1f;
	public float stayTime=1f;
	public float fadeOutTime=0.7f;
	public TrailRenderer thisTrail;
	private float timeElapsed=0f;
	private float timeElapsedLast=0f;
	private float percent;
	
	
	private void Start ()
	{
		thisTrail.material.SetColor ("_TintColor", new Color(0.5f,0.5f,0.5f,1f));
		if(fadeInTime<0.01f) 
			fadeInTime=0.01f; //hack to avoid division with zero

		percent=timeElapsed/fadeInTime;
		
	}
	
	
	private void Update () {
		timeElapsed+=Time.deltaTime;
		
		
		if(timeElapsed<=fadeInTime) //fade in
		{
			percent=timeElapsed/fadeInTime;
			thisTrail.material.SetColor ("_TintColor", new Color(0.5f,0.5f,0.5f, percent));
		}
		
		if((timeElapsed>fadeInTime)&&(timeElapsed<fadeInTime+stayTime)) //set the normal color
		{
			thisTrail.material.SetColor ("_TintColor", new Color(0.5f,0.5f,0.5f,1f));
		}
		
		if(timeElapsed>=fadeInTime+stayTime&&timeElapsed<fadeInTime+stayTime+fadeOutTime) //fade out
		{
			timeElapsedLast+=Time.deltaTime;
			percent=1-(timeElapsedLast/fadeOutTime);
			thisTrail.material.SetColor ("_TintColor", new Color(0.5f,0.5f,0.5f, percent));
		}
		
		
		
	}
	

}
