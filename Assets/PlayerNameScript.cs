using UnityEngine;
using System.Collections;

public class PlayerNameScript : MonoBehaviour {
    public float fadeoutSpeed = 5f;
    public float fadeoutTiming = 2.5f;
    private float target = 0;
    private TextMesh textMesh;
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
        c.a = Mathf.Lerp(c.a, target, 5f * Time.deltaTime);

        if (c.a < 0.05f)
        {
            c.a = 0;
        }

        textMesh.color = c;
        transform.forward = Camera.main.transform.forward;
    }

    public void SetName(string name)
    {
        text = name;
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
