using UnityEngine;
using System.Collections;

public class SM_PrefabGenerator : MonoBehaviour {
	public GameObject[] createThis;  // list of possible prefabs
	
	private float rndNr; // this is for just a random number holder when we need it
	
	public int thisManyTimes=3;
	public float overThisTime=1.0f;
	
	public float xWidth;  // define the square where prefabs will be generated
	public float yWidth;
	public float zWidth;
	
	public float xRotMax;  // define maximum rotation of each prefab
	public float yRotMax=180f;
	public float zRotMax;
	
	public bool allUseSameRotation=false;
	private bool allRotationDecided=false;
	
	public bool detachToWorld=true;
	
	private float x_cur;  // these are used in the random palcement process
	private float y_cur;
	private float z_cur;
	
	
	private float xRotCur;  // these are used in the random protation process
	private float yRotCur;
	private float zRotCur;
	
	private float timeCounter;  // counts the time :p
	private int effectCounter;  // you will guess ti
	
	private float trigger;  // trigger: at which interwals should we generate a particle
	
	
	
	private void Start () {
		if (thisManyTimes<1) 
			thisManyTimes=1; //hack to avoid division with zero and negative numbers
		trigger=(overThisTime/thisManyTimes);  //define the intervals of time of the prefab generation.
		
	}
	
	
	private void Update () {
		
		timeCounter+=Time.deltaTime;
		
		if(timeCounter>trigger&&effectCounter<=thisManyTimes)
		{
			rndNr=Mathf.Floor(Random.value*createThis.Length);  //decide which prefab to create
			
			
			x_cur=transform.position.x+(Random.value*xWidth)-(xWidth*0.5f);  // decide an actual place
			y_cur=transform.position.y+(Random.value*yWidth)-(yWidth*0.5f);
			z_cur=transform.position.z+(Random.value*zWidth)-(zWidth*0.5f);
			
			if(allUseSameRotation==false||allRotationDecided==false)  // basically this plays only once if allRotationDecided=true, otherwise it plays all the time
			{
				xRotCur=transform.rotation.x+(Random.value*xRotMax*2f)-(xRotMax);  // decide rotation
				yRotCur=transform.rotation.y+(Random.value*yRotMax*2f)-(yRotMax);  
				zRotCur=transform.rotation.z+(Random.value*zRotMax*2f)-(zRotMax);  
				allRotationDecided=true;
			}

			
			GameObject justCreated=(GameObject)Instantiate(createThis[(int)rndNr], new Vector3(x_cur, y_cur, z_cur), transform.rotation);  //create the prefab
			justCreated.transform.Rotate(xRotCur, yRotCur, zRotCur);
			
			if(detachToWorld==false)  // if needed we attach the freshly generated prefab to the object that is holding this script
			{
				justCreated.transform.parent=transform;
			}
			
			timeCounter-=trigger;  //administration :p
			effectCounter+=1;
		}
		
		
	}
}
