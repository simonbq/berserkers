using UnityEngine;
using System.Collections;

public class SeriousSettingsController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(QualitySettings.GetQualityLevel() == QualitySettings.names.Length - 1) {

		}
	}
}
