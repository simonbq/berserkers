using UnityEngine;
using System.Collections;

public class AlphaMaterial : MonoBehaviour
{
	public Material changedMaterial;
	public Color colorTint;

	void Start()
	{
		changedMaterial = renderer.material;
		colorTint = Color.black; //Setting the default value
		//renderer.sharedMaterial = new Material (renderer.sharedMaterial);
	}
	void Update()//Here the color changes, darker values equals less effect, link the value to speed
	{

		if (Input.GetKeyDown (KeyCode.LeftShift)) { //temporarily linked with the sprint button
			colorTint = Color.white;
				} 
		if (Input.GetKeyUp (KeyCode.LeftShift)){ //temporarily linked with the sprint button
			colorTint = Color.grey;
				}
		//renderer.material.SetColor("_TintColor", colorTint);
		changedMaterial.SetColor ("_TintColor", colorTint);	}//Setting the value to the _TintColor 
}