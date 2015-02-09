using UnityEngine;
using System.Collections;

public class AlphaMaterial : MonoBehaviour
{
    private Material myMaterial;
	public Color colorTint;

    public GameObject affectedObject;

    public float lerpDuration = 0.8f;
    private float lerpT = 0;
    private Color startColor;
    private Color endColor;

    private ParticleSystem ps;

    private bool activated = false;
	void Start()
	{
        if (myMaterial != null)
        {
            if (affectedObject.particleSystem == null)
            {
                myMaterial = affectedObject.GetComponent<Renderer>().material;
            }

            else
            {
                myMaterial = affectedObject.particleSystem.renderer.material;
            }
        }

        //ps = GetComponentInChildren
		//changedMaterial = renderer.material;
		colorTint = Color.black; //Setting the default value
		//renderer.sharedMaterial = new Material (renderer.sharedMaterial);
	}
	void Update()//Here the color changes, darker values equals less effect, link the value to speed
	{
        /*if (lerpT < 1)
        {*/
            colorTint = Color.Lerp(startColor, endColor, lerpT);
            lerpT += Time.deltaTime / lerpDuration;
       // }
        /*
		if (Input.GetKeyDown (KeyCode.LeftShift)) { //temporarily linked with the sprint button
			colorTint = Color.white;
				} 
		if (Input.GetKeyUp (KeyCode.LeftShift)){ //temporarily linked with the sprint button
			colorTint = Color.grey;
				}*/
		//renderer.material.SetColor("_TintColor", colorTint);
            if (myMaterial != null)
            {
                myMaterial.SetColor("_TintColor", colorTint);
            }
    }//Setting the value to the _TintColor 

    public void SetActivated(bool mActivated)
    {
        //Debug.Log("Set activated " + mActivated);
        if (mActivated && !activated)
        {
            startColor = Color.black;
            endColor = Color.gray;
            lerpT = 0;
            activated = true;
        }
        if(!mActivated && activated)
        {
            startColor = Color.gray;
            endColor = Color.black;
            lerpT = 0;
            activated = false;
        }

    }

}