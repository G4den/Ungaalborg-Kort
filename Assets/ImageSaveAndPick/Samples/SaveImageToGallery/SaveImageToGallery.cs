//
//  ImagePickAndSave.cs
//  
//
//  Created by Wili on 2018/1/6. contact us : wiliamheart@gmail.com
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveImageToGallery : MonoBehaviour {

	public ImagePickAndSave imageCtr;
	public Texture2D sampleTex1;
	public Texture2D sampleTex2;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void clickTexture1()
	{
		imageCtr.SaveToGallery (sampleTex1, "ImageSaveAndPick", "sample1.png");
	}

	public void clickTexture2()
	{
		imageCtr.SaveToGallery (sampleTex2, "ImageSaveAndPick", "sample2.png");
	}
	public void LoadScene(string sceneName)
	{
		Application.LoadLevel (sceneName);
	}
}
