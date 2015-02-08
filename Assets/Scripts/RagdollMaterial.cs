using UnityEngine;
using System.Collections;

public class RagdollMaterial : MonoBehaviour {

	public GameObject model;
	public GameObject pelvis;

	public void SetMaterial(Material mat)
	{
		if(model != null)
		{
			foreach (Transform t in model.transform)
			{
				if(t.renderer != null)
				{
					t.renderer.material = mat;
				}
			}
		}
	}
}
