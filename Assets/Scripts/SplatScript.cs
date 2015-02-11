using UnityEngine;
using System.Collections;

public class SplatScript : MonoBehaviour {
    public float fadeOut = 1f;
    public float fadeIn = 25f;
    public float stayTime = 30f;
    private float cutoff = 1;
    private float target = 0;
    private float currentSpeed;
    private Projector proj;
    private Material mat;

	void Start () {
        proj = GetComponent<Projector>();
        mat = Instantiate(proj.material) as Material;
        proj.material = mat;

        currentSpeed = fadeIn;
        Invoke("Fade", stayTime);
	}
	
	void FixedUpdate () {
        cutoff = Mathf.Lerp(cutoff, target, currentSpeed * Time.fixedDeltaTime);
        mat.SetFloat("_Cutoff", cutoff);

        if (target == 1 &&
            cutoff > 0.95f)
        {
            Destroy(this.gameObject);
        }
	}

    void Fade()
    {
        target = 1;
        currentSpeed = fadeOut;
    }
}
