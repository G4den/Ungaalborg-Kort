//
//  ImagePickAndSave.cs
//  
//
//  Created by Wili on 2018/1/6. contact us : wiliamheart@gmail.com
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ImagePickAndSave : MonoBehaviour {
	
	#if !UNITY_EDITOR && UNITY_ANDROID
	private static AndroidJavaClass mIps = null;
	private static AndroidJavaClass IPS
	{
		get
		{
			if( mIps == null )
			mIps = new AndroidJavaClass( "com.Wili.ImageController.ImageController" );
			return mIps;
		}
	}
	#endif

	#if !UNITY_EDITOR && UNITY_IOS
	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void SaveImageToAlbum(string path);

	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void LoadGallery();

	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void LoadCamera();

	#endif

	public delegate void PickDelegate(string path);

	public delegate void SaveDelegate(string path);

	public delegate void ErrorDelegate(string message);

	public event PickDelegate PickCompleted;

	public  event PickDelegate SaveCompleted;

	public event ErrorDelegate Failed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Browse()
	{
		Browse ("Select", "", 1024);
	}

	private void Browse(string title, string outputFileName, int maxSize)
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		if(IPS != null)
		{
			IPS.CallStatic("Browse", title, outputFileName, maxSize);
		}
		#elif !UNITY_EDITOR && UNITY_IOS
		ImagePickAndSave.LoadGallery();
		#endif
	}

	public void Crop()
	{
		return;
		#if !UNITY_EDITOR && UNITY_ANDROID
		if(IPS != null)
		{
			IPS.CallStatic("Crop");
		}
		#endif
	}

	public void OpenCamera()
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		if(IPS != null)
		{
			IPS.CallStatic("OpenCamera");
		}
		#elif !UNITY_EDITOR && UNITY_IOS
		ImagePickAndSave.LoadCamera();
		#endif
	}

	/// <summary>
	/// Captures the full screen.
	/// </summary>
	public void CaptureScreen()
	{
		StartCoroutine (CaptureScreenByRect (new Rect(0,0,Screen.width,Screen.height))); 
	
	}

	/// <summary>
	/// captureScreen by special rect
	/// </summary>
	/// <param name="rect">Rect.</param>
	public void CaptureScreen(Rect rect)
	{
		rect.Set (Mathf.Abs (rect.x), Mathf.Abs (rect.y), Mathf.Abs (rect.width), Mathf.Abs (rect.height));

		if ( Math.Abs(rect.x) >= Screen.width || Math.Abs(rect.y) >= Screen.height ) {
			Debug.LogError ("Error: the capture rect is out of the bounds ,please check it ");
			return;
		}
		if ((rect.x + rect.width) > Screen.width) {
			rect.width = Screen.width - rect.x;
		}
	
		if ((rect.y + rect.height) > Screen.height) {
			rect.height = Screen.height - rect.y;
		}
		#if  UNITY_EDITOR || UNITY_STANDLONE
		rect.Set (Math.Abs(rect.x), Math.Abs (Screen.height -  Math.Abs (rect.height) - Math.Abs(rect.y) ) ,  Mathf.Clamp( rect.width,0,Screen.width),Mathf.Clamp( rect.height,0,Screen.height));
		#endif
		StartCoroutine (CaptureScreenByRect (rect)); 
	}

	/// <summary>
	/// Captures the screen with no UI by target Camera which you want to render.
	/// </summary>
	/// <param name="tarCam">Tar cam.</param>
	public void CaptureScreenWithNoUI(Camera tarCam)
	{
		RenderTexture rt = new RenderTexture((int)Screen.width, (int)Screen.height, 0);
		tarCam.targetTexture = rt;  
		tarCam.Render();

		RenderTexture.active = rt;  
		Texture2D screenShot = new Texture2D((int)Screen.width, (int)Screen.height, TextureFormat.RGB24,false);  
		screenShot.ReadPixels(new Rect(0,0,Screen.width,Screen.height), 0, 0);//
		screenShot.Apply();
		tarCam.targetTexture = null;  
		RenderTexture.active = null; // : added to avoid errors  
		GameObject.Destroy(rt);  

		SaveToGallery( screenShot, "ImagePickerAndSaver", "screenShot.png" ); 
		DestroyImmediate (screenShot);
		screenShot = null;
	}

	/// <summary>
	/// Captures the screen with no UI by target Camera which you want to render 
	/// </summary>
	/// <param name="tarCam">Tar cam.</param>
	/// <param name="rect">Rect.</param>
	public void CaptureScreenWithNoUI(Camera tarCam,Rect rect)
	{

		rect.Set (Mathf.Abs (rect.x), Mathf.Abs (rect.y), Mathf.Abs (rect.width), Mathf.Abs (rect.height));

		if ( Math.Abs(rect.x) >= Screen.width || Math.Abs(rect.y) >= Screen.height ) {
			Debug.LogError ("Error: the capture rect is out of the bounds ,please check it ");
			return;
		}
		if ((rect.x + rect.width) > Screen.width) {
			rect.width = Screen.width - rect.x;
		}

		if ((rect.y + rect.height) > Screen.height) {
			rect.height = Screen.height - rect.y;
		}

		RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 0);  
		tarCam.targetTexture = rt;  
		tarCam.Render();

		RenderTexture.active = rt;  
		Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24,false);  
		screenShot.ReadPixels(rect, 0, 0);//
		screenShot.Apply();  
		tarCam.targetTexture = null;  
		RenderTexture.active = null; // : added to avoid errors  
		GameObject.Destroy(rt);  
		SaveToGallery( screenShot, "ImagePickerAndSaver", "screenShot.png" ); 
		DestroyImmediate (screenShot);
		screenShot = null;
	}


	IEnumerator CaptureScreenByRect(Rect rect)
	{
		yield return new WaitForEndOfFrame();
		Texture2D screenShot = new Texture2D( (int)rect.width, (int)rect.height, TextureFormat.RGB24, false );
		screenShot.ReadPixels( rect, 0, 0 );
		screenShot.Apply();
		SaveToGallery( screenShot, "ImagePickerAndSaver", "screenShot.png" );
		DestroyImmediate (screenShot);
		screenShot = null;
	}

	private void OnPickComplete(string path)
	{
		var handler = PickCompleted;
		if (handler != null)
		{
			handler(path);
		}
	}


	private void OnSaveComplete(string path)
	{
		var handler = SaveCompleted;
		if (handler != null)
		{
			handler(path);
		}
	}

	private void OnFailure(string message)
	{
		var handler = Failed;
		if (handler != null)
		{
			handler(message);
		}
	}


	private  void SaveToGallery( byte[] mediaBytes, string directoryName, string filename )
	{
		if (mediaBytes == null || mediaBytes.Length == 0) {
			Debug.LogError ("image bytes in SaveToGallery Func is null or empty!");
			return;
		}

		string path = GetLocalPath( directoryName, filename );
		#if !UNITY_WEBPLAYER&& !UNITY_WEBGL
		File.WriteAllBytes( path, mediaBytes );
		#endif

		SaveToGallery( path );
	}

	public  void SaveToGallery( Texture2D image, string directoryName, string filename)
	{
		if (image == null) {
			Debug.LogError ("Texture2d image in SaveToGallery Func is null or empty!");
			return;
		}
		if (filename.EndsWith (".png") || filename.EndsWith (".jpg")) {
			filename = filename.Substring (0, filename.Length - 4);
		} else if (filename.EndsWith ("jpeg")) {
			filename = filename.Substring (0, filename.Length - 5);
		}
		filename = filename + "_" + System.DateTime.Now.ToString ("yyyyMMddHHmmss");
		SaveToGallery( image.EncodeToPNG(), directoryName, filename + ".png" );
	}

	public  void SaveToGallery( string path )
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		using( AndroidJavaClass unityClass = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" ) )
		using( AndroidJavaObject context = unityClass.GetStatic<AndroidJavaObject>( "currentActivity" ) )
		{
			IPS.CallStatic( "MediaScanFile", context, path );
		}
		OnSaveComplete(path);
		Debug.Log( "Saved to gallery: " + path );
		#elif !UNITY_EDITOR && UNITY_IOS
		SaveImageToAlbum( path );

		Debug.Log( "Saving to Pictures: " + Path.GetFileName( path ) );
		#elif UNITY_EDITOR || UNITY_STANDLONE
		StartCoroutine(SaveTextureToPC(path));
		#endif
	}


	IEnumerator SaveTextureToPC(string path)
	{
		while (!File.Exists (path)) {
			yield return new WaitForEndOfFrame ();
		}
		var handler = SaveCompleted;
		if (handler != null)
		{
			handler(path);
		}
	}


	private  string GetLocalPath( string directoryName, string filename )
	{
		string saveDir;
		#if !UNITY_EDITOR && UNITY_ANDROID
		saveDir = IPS.CallStatic<string>( "GetFilePath", directoryName );
		#else
		saveDir = Application.persistentDataPath;
		#endif

		if( filename.Contains( "{0}" ) )
		{
			int fileIndex = 0;
			string path;
			do
			{
				path = Path.Combine( saveDir, string.Format( filename, ++fileIndex ) );
			} while( File.Exists( path ) );

			return path;
		}

		saveDir = Path.Combine( saveDir, filename );

		#if !UNITY_EDITOR && UNITY_IOS
		// iOS internally copies images/videos to Photos directory of the system,
		// but the process is async. The redundant file is deleted by objective-c code
		// automatically after the media is saved but while it is being saved, the file
		// should NOT be overwritten. Therefore, always ensure a unique filename on iOS
		if( File.Exists( saveDir ) )
		{
			return GetLocalPath( directoryName , Path.GetFileNameWithoutExtension( filename ) + " {0}" + Path.GetExtension( filename ) );
		}
		#endif

		return saveDir;
	}


}
