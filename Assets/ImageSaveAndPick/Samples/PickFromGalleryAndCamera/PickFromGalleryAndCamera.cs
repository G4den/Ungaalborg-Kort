//
//  PickFromGalleryAndCamera.cs
//  
//
//  Created by Wili on 2018/1/6. contact us : wiliamheart@gmail.com
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PickFromGalleryAndCamera : MonoBehaviour {

	public ImagePickAndSave imageCtr;

    public Image img1;
    public Image img2;

	int renderMode =0;

	void Start () {
		imageCtr.PickCompleted += OnPickCompleted;
	}

	void OnPickCompleted(string path)
	{
		if (renderMode == 1) {
			StartCoroutine(	LoadImage (path));
		} else if (renderMode == 2) {
			StartCoroutine(	LoadImage (path));
		}
	}

	public void GalleryLoad()
	{
		renderMode = 1;
		imageCtr.Browse ();
	}

	public void CameraLoad()
	{
		renderMode = 2;
		imageCtr.OpenCamera();
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

        img1.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100);
        img2.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100);

        texture = null;
	}

	public void LoadScene(string sceneName)
	{
		Application.LoadLevel (sceneName);
	}
}
