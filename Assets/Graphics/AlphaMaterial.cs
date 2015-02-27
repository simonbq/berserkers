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
        if (affectedObject != null)
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

        colorTint = Color.Lerp(startColor, endColor, lerpT);
        lerpT += Time.deltaTime / lerpDuration;

        if (colorTint == endColor && !activated)
            affectedObject.SetActive(false);

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
            affectedObject.SetActive(true);
            startColor = Color.black;
            endColor = Color.gray;
            lerpT = 0;
            activated = true;
        }
        if (!mActivated && activated)
        {
            startColor = Color.gray;
            endColor = Color.black;
            lerpT = 0;
            activated = false;
        }

    }

}