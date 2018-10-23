/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/


	using UnityEditor;
	using System;

	#if UNITY_IOS
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using System.IO;
	#endif

public static class ConifgEditor {

		private const string
		CameraUsageKey = @"NSCameraUsageDescription",
		CameraUsageDescription = @"Allow this app to use the camera.", // Change this as necessary

		PhotoLibraryUsageKey = @"NSPhotoLibraryUsageDescription",
		PhotoLibraryUsageDescription = @"Allow Access the Gallery.",// Change this as necessary;

		PhotoLibraryAddUsageKey = @"NSPhotoLibraryAddUsageDescription",
		PhotoLibraryAddUsageDescription = @"Allow Access the Gallery",// Change this as necessary;

		VersionNumber = "ImageSaveAndPick_10";

		#if UNITY_IOS


		[PostProcessBuild]
		static void SetPermissions (BuildTarget buildTarget, string path) {
			if (buildTarget != BuildTarget.iOS) return;
			string plistPath = path + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));
			PlistElementDict rootDictionary = plist.root;
			rootDictionary.SetString(CameraUsageKey, CameraUsageDescription);
			rootDictionary.SetString(PhotoLibraryUsageKey, PhotoLibraryUsageDescription);
			rootDictionary.SetString(PhotoLibraryAddUsageKey, PhotoLibraryAddUsageDescription);
			File.WriteAllText(plistPath, plist.WriteToString());
		}
		#endif
	}