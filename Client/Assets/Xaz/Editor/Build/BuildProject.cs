//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace XazEditor
{
	public class BuildProject
	{
		public delegate int NumberOfBuildScenes();
		public delegate void PreBuildPlayer(Dictionary<string, string> args, ref BuildOptions options);
		public delegate void BuildAssetsDelegate(Dictionary<string, string> args);

		static public PreBuildPlayer onPreBuildPlayer;
		static public BuildAssetsDelegate onBuildAssets;
		static public NumberOfBuildScenes numberOfBuildScenes;

		/// <summary>
		/// Build Android
		/// </summary>
		public static void BuildAndroid(Dictionary<string, string> args)
		{
			if (args == null) {
				args = GetArgs("BuildAndroid", true);
			}
			if (!args.ContainsKey("exportProjectPath")) {
				Debug.LogError("Cannot get 【exportProjectPath】 from args");
				EditorApplication.Exit(1);
			}

			UpdatePlayerSettings(args);

			if (args.ContainsKey("keystoreName") && !string.IsNullOrEmpty(args["keystoreName"])) {
				PlayerSettings.Android.keystoreName = args["keystoreName"];
				PlayerSettings.Android.keystorePass = args["keystorePass"];
				PlayerSettings.Android.keyaliasName = args["keyaliasName"];
				PlayerSettings.Android.keyaliasPass = args["keyaliasPass"];
			}
			if (args.ContainsKey("bundleVersionCode") && !string.IsNullOrEmpty(args["bundleVersionCode"])) {
				PlayerSettings.Android.bundleVersionCode = Convert.ToInt32(args["bundleVersionCode"]);
			}
			PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
			PlayerSettings.Android.useAPKExpansionFiles = args.ContainsKey("useObb");

			BuildOptions options = BuildOptions.None;
			if (onPreBuildPlayer != null) {
				onPreBuildPlayer(args, ref options);
			}
#if UNITY_ANDROID
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
#endif
            BuildPipeline.BuildPlayer(GetBuildScenes(), args["exportProjectPath"], BuildTarget.Android, options | BuildOptions.AcceptExternalModificationsToPlayer );
		}

		/// <summary>
		/// Build IOS
		/// </summary>
		public static void BuildIOS(Dictionary<string, string> args)
		{
			if (args == null) {
				args = GetArgs("BuildIOS", true);
			}
			if (!args.ContainsKey("exportProjectPath")) {
				Debug.LogError("Cannot get 【exportProjectPath】 from args");
				EditorApplication.Exit(1);
			}

			UpdatePlayerSettings(args);

			PlayerSettings.iOS.prerenderedIcon = true;
			PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2);

			BuildOptions options = BuildOptions.None;
			if (onPreBuildPlayer != null) {
				onPreBuildPlayer(args, ref options);
			}
#if UNITY_4
			BuildPipeline.BuildPlayer(GetBuildScenes(), args["exportProjectPath"], BuildTarget.iPhone, options);
#else
			BuildPipeline.BuildPlayer(GetBuildScenes(), args["exportProjectPath"], BuildTarget.iOS, options);
#endif
		}

		/// <summary>
		/// Build Windows Applications
		/// </summary>
		public static void BuildWindows(Dictionary<string, string> args)
		{
			if (args == null) {
				args = GetArgs("BuildWindows", true);
			}
			if (!args.ContainsKey("exportProjectPath")) {
				Debug.LogError("Cannot get 【exportProjectPath】 from args");
				EditorApplication.Exit(1);
			}

			UpdatePlayerSettings(args);

			BuildOptions options = BuildOptions.None;
			if (onPreBuildPlayer != null) {
				onPreBuildPlayer(args, ref options);
			}
			BuildPipeline.BuildPlayer(GetBuildScenes(), args["exportProjectPath"], BuildTarget.StandaloneWindows64, options);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string[] GetBuildScenes()
		{
			int count = 0;
			int sceneCount = int.MaxValue;
			if (numberOfBuildScenes != null) {
				sceneCount = numberOfBuildScenes();
			}
			List<string> names = new List<string>();
			foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes) {
				if (e != null && e.enabled) {
					names.Add(e.path);
					if (++count >= sceneCount)
						break;
				}
			}
			Debug.LogFormat("Build Scenes Count: {0}", count);
			return names.ToArray();
		}

		static public void SetScriptingDefineSymbols()
		{
			string defines = string.Empty;
			Dictionary<string, string> args = GetArgs("SetScriptingDefineSymbols", true);
			if (args.ContainsKey("defines")) {
				defines = args["defines"];
			}

#if UNITY_IOS
#if UNITY_4
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone, defines);
#else
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defines);
#endif
#endif

#if UNITY_ANDROID
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines);
#endif

#if UNITY_STANDALONE
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines);
#endif
		}

		static private void UpdatePlayerSettings(Dictionary<string, string> args)
		{
			//PlayerSettings.strippingLevel = StrippingLevel.UseMicroMSCorlib;
			if (args.ContainsKey("bundleName") && !string.IsNullOrEmpty(args["bundleName"])) {
				PlayerSettings.productName = args["bundleName"];
			}
			if (args.ContainsKey("bundleIdentifier") && !string.IsNullOrEmpty(args["bundleIdentifier"])) {
#if UNITY_5_6_OR_NEWER
				PlayerSettings.applicationIdentifier = args["bundleIdentifier"];
#else
				PlayerSettings.bundleIdentifier = args["bundleIdentifier"];
#endif
			}

			if (args.ContainsKey("bundleVersion") && !string.IsNullOrEmpty(args["bundleVersion"])) {
				PlayerSettings.bundleVersion = args["bundleVersion"];
			}
#if UNITY_4
			PlayerSettings.shortBundleVersion = PlayerSettings.bundleVersion;
#endif

			PlayerSettings.stripEngineCode = false;
			if (args.ContainsKey("backend")) {
				var backend = args["backend"] == "il2cpp" ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x;
#if UNITY_5_5_OR_NEWER
				PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, backend);
				PlayerSettings.SetIncrementalIl2CppBuild(EditorUserBuildSettings.selectedBuildTargetGroup, backend == ScriptingImplementation.IL2CPP);
#else
				PlayerSettings.SetPropertyInt("ScriptingBackend", (int)backend, EditorUserBuildSettings.activeBuildTarget);
#endif
			}

		
		}

		static public Dictionary<string, string> GetArgs(string methodName)
		{
			return GetArgs(methodName, false);
		}

		static private Dictionary<string, string> GetArgs(string methodName, bool native)
		{
			if (native) {
				methodName = "XazEditor.BuildProject." + methodName;
			}

			bool isArg = false;
			Dictionary<string, string> args = new Dictionary<string, string>();
			foreach (string arg in System.Environment.GetCommandLineArgs()) {
				if (isArg) {
					if (arg.StartsWith("--")) {
						int splitIndex = arg.IndexOf("=");
						if (splitIndex > 0) {
							args.Add(arg.Substring(2, splitIndex - 2), arg.Substring(splitIndex + 1));
						} else {
							args.Add(arg.Substring(2), "true");
						}
					}
				} else if (arg == methodName) {
					isArg = true;
				}
			}
			return args;
		}

		public static void BuildAssets(Dictionary<string, string> args)
		{
			if (args == null) {
				args = GetArgs("BuildAssets", true);
			}
			if (onBuildAssets != null) {
				onBuildAssets(args);
			}
		}

		public static void EncryptLua(string srcPath, string dstPath, int key)
		{
			
		}
	}
}
