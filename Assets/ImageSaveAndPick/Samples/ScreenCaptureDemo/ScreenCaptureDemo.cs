//
//  ScreenCaptureDemo.cs
//  
//
//  Created by Wili on 2018/1/6. contact us : wiliamheart@gmail.com
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenCaptureDemo : MonoBehaviour {
	
	public ImagePickAndSave imagectr;
	public GameObject renderObj;
	public Toggle isIncludeUIToggle;
	public Toggle isFullToggle;
	public Camera captureCamera;
	public InputField inputX;
	public InputField inputY;
	public InputField inputW;
	public InputField inputH;

	// Use this for initialization
	void Start () {
		imagectr.SaveCompleted += onSaveCompleted;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void onSaveCompleted(string path)
	{
		StartCoroutine(LoadImage(path));
	}
	IEnumerator LoadImage(string path)
	{
		
		var url = "file://" + path;
		#if UNITY_EDITOR || UNITY_STANDLONE
		url = "file:/"+path;
		#endif
		Debug.Log ("current path is " + url);
		var www = new WWW(url);
		yield return www;

		var texture = www.texture;
		if (texture == null)
		{
			Debug.LogError("Failed to load texture url:" + url);
		}
		Renderer render = renderObj.GetComponent<Renderer> ();

		DestroyImmediate (render.material.mainTexture);
		render.material.mainTexture = texture;

		texture = null;
	}

	public void SaveBtnClicked()
	{
		Debug.Log ("sss enter this Savbnt clicked 01");
		if (isIncludeUIToggle.isOn) {
			if (isFullToggle.isOn) {
				Debug.Log ("sss enter this Savbnt clicked 04");
				imagectr.CaptureScreen ();
			} else {
				int x = int.Parse (inputX.text);
				int y = int.Parse (inputY.text);
				int w = int.Parse (inputW.text);
				int h = int.Parse (inputH.text);
				imagectr.CaptureScreen (new Rect(x,y,w,h));
			}
		} else {
			if (isFullToggle.isOn) {
				imagectr.CaptureScreenWithNoUI (captureCamera);
			} else {
				
				int x = int.Parse (inputX.text);
				int y = int.Parse (inputY.text);
				int w = int.Parse (inputW.text);
				int h = int.Parse (inputH.text);

				imagectr.CaptureScreenWithNoUI (captureCamera,new Rect(x,y,w,h));
			}
		}
	}

	public void LoadScene(string sceneName)
	{
		Application.LoadLevel (sceneName);
	}

}
