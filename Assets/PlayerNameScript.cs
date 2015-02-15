using UnityEngine;
using System.Collections;

public class PlayerNameScript : MonoBehaviour {
    public float fadeoutSpeed = 5f;
    public float fadeoutTiming = 2.5f;
    private float target = 0;
    private TextMesh textMesh;
	private Color color;
    private string text;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        Reset();
    }

    void Update()
    {
        textMesh.text = text;
        Color c = textMesh.color;
		c.r = color.r;
		c.g = color.g;
		c.b = color.b;

        c.a = Mathf.Lerp(c.a, target, 5f * Time.deltaTime);

        if (c.a < 0.05f)
        {
            c.a = 0;
        }

        textMesh.color = c;
        transform.forward = Camera.main.transform.forward;

        transform.position = transform.parent.transform.position + Vector3.forward * 1.25f;
    }

    public void SetName(string name, bool local)
    {
        text = name;

		if(local)
		{
			color = Color.green;
			Debug.Log ("Local player");
		}
    }

    public void Reset()
    {
        target = 1;
        Invoke("FadeOut", fadeoutTiming);
    }

    void FadeOut()
    {
        target = 0;
    }
}
