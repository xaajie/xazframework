//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿#if UNITY_IOS
namespace XazEditor
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using UnityEditor.iOS;
	using UnityEditor.iOS.Xcode;

	public class XCodePostProcess
	{
		[PostProcessBuild(999)]
		public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
		{
			string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
			string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);

			PlistDocument plist = new PlistDocument();
			plist.ReadFromFile(plistPath);

			PBXProject project = new PBXProject();
			project.ReadFromFile(projectPath);
			var target = project.TargetGuidByName(PBXProject.GetUnityTargetName());

			string searchPath = Application.dataPath + "/../";
			// Find and run through all projmods files to patch the project.
			// Please pay attention that ALL projmods files in your project folder will be excuted!
			string[] files = Directory.GetFiles(searchPath, "*.projmods", SearchOption.AllDirectories);
			foreach (string file in files) {
				ProjMod mod = ProjMod.ReadFromFile(file);
				if (mod != null) {
					mod.Apply(plist, project, target);
				}
			}
			//disable bitcode
			project.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
			DisableWarnings(project, target);

			plist.WriteToFile(plistPath);
			project.WriteToFile(projectPath);

			// hack-insert Xaz codes
			// ProcessUnityAppController(pathToBuiltProject + "/Classes/UnityAppController.mm");
		}

		private static void DisableWarnings(PBXProject project, string target)
		{
			project.SetBuildProperty(target, "CLANG_WARN_DEPRECATED_OBJC_IMPLEMENTATIONS", "NO");
			project.SetBuildProperty(target, "CLANG_WARN_BOOL_CONVERSION", "NO");
			project.SetBuildProperty(target, "CLANG_WARN_CONSTANT_CONVERSION", "NO");
			// CLANG_WARN_DIRECT_OBJC_ISA_USAGE = YES_ERROR;
			project.SetBuildProperty(target, "CLANG_WARN_EMPTY_BODY", "NO");
			project.SetBuildProperty(target, "CLANG_WARN_ENUM_CONVERSION", "NO");
			project.SetBuildProperty(target, "CLANG_WARN_INT_CONVERSION", "NO");
			// CLANG_WARN_OBJC_ROOT_CLASS = YES_ERROR;
			project.SetBuildProperty(target, "CLANG_WARN__DUPLICATE_METHOD_MATCH", "NO");
			project.SetBuildProperty(target, "GCC_WARN_64_TO_32_BIT_CONVERSION", "NO");
			// project.SetBuildProperty(target, "GCC_WARN_ABOUT_RETURN_TYPE" = YES_ERROR;"
			project.SetBuildProperty(target, "GCC_WARN_UNDECLARED_SELECTOR", "NO");
			project.SetBuildProperty(target, "GCC_WARN_UNINITIALIZED_AUTOS", "NO");
			project.SetBuildProperty(target, "GCC_WARN_UNUSED_FUNCTION", "NO");
		}

		private static void ProcessUnityAppController(string filePath)
		{
			if (!File.Exists(filePath))
				return;

			string content = File.ReadAllText(filePath);
			content = "#import \"XazSDKController.h\"\n" + content;

			ReplaceReturnValue(ref content, "didFinishLaunchingWithOptions", "[XazSDKController application:application didFinishLaunchingWithOptions:launchOptions];");
			ReplaceReturnValue(ref content, "openURL", "[XazSDKController application:application openURL:url sourceApplication:sourceApplication annotation:annotation];");

			InsertCodeBeforeEnd(ref content, "applicationDidBecomeActive", "[XazSDKController applicationDidBecomeActive:application];");
			InsertCodeBeforeEnd(ref content, "applicationDidReceiveMemoryWarning", "[XazSDKController applicationDidReceiveMemoryWarning:application];");
			InsertCodeBeforeEnd(ref content, "applicationWillTerminate", "[XazSDKController applicationWillTerminate:application];");
			InsertCodeBeforeEnd(ref content, "applicationDidEnterBackground", "[XazSDKController applicationDidEnterBackground:application];");
			InsertCodeBeforeEnd(ref content, "applicationWillEnterForeground", "[XazSDKController applicationWillEnterForeground:application];");

			File.WriteAllText(filePath, content);
		}

		private static void ReplaceReturnValue(ref string content, string func, string replaceValue)
		{
			int index = content.IndexOf(func);
			if (index > 0) {
				index = content.IndexOf("return YES;", index + 1);
				if (index < 0) {
					Debug.LogWarning("Hack '" + func + "' failed.");
				} else {
					content = content.Substring(0, index) + "return " + replaceValue + content.Substring(index + 11);
				}
			} else {
				Debug.LogWarning("Can't find the function '" + func + "'.");
			}
		}

		private static void InsertCodeBeforeEnd(ref string content, string func, string insertCode)
		{
			int index = content.IndexOf(func);
			if (index > 0) {
				index = content.IndexOf("\n}", index + 1);
				if (index < 0) {
					Debug.LogWarning("Hack '" + func + "' failed.");
				} else {
					content = content.Substring(0, index + 1) + "\t" + insertCode + "\n" + content.Substring(index + 1);
				}
			} else {
				Debug.LogWarning("Can't find the function '" + func + "'.");
			}
		}

		class ProjMod
		{
			private Hashtable m_Data;
			private string m_GroupName;
			private string m_DirectoryName;

			private ProjMod(string filePath, Hashtable data)
			{
				m_Data = data;
				m_DirectoryName = Path.GetDirectoryName(filePath);
				m_GroupName = data.ContainsKey("group") ? data["group"].ToString() : string.Empty;
			}

			public void Apply(PlistDocument plist, PBXProject project, string target)
			{
				AddLibs(project, target);
				AddFrameworks(project, target);
				AddHeaderPaths(project, target);
				AddFolders(project, target);
				AddFiles(project, target);
				AddCompilerFlags(project, target);
				AddLinkerFlags(project, target);
				AddEmbedBinaries(project, target);
				AddShellScripts(project, target);
				AddPlist(plist);
				
				project.SetBuildProperty(target, m_GroupName.ToUpper(), Path.GetFullPath(m_DirectoryName));
			}

			private void AddLibs(PBXProject project, string target)
			{
				var libs = (ArrayList)m_Data["libs"];
				if (libs != null && libs.Count > 0) {
					foreach (string lib in libs) {
						string libName = lib.Replace(".dylib", ".tbd");
						string filePath = Path.Combine("usr/lib", libName);
						project.AddFileToBuild(target, project.AddFile(filePath, "Frameworks/" + libName, PBXSourceTree.Sdk));
						Debug.LogFormat("AddLibs: {0}", filePath);
					}
				}
			}
			private void AddFrameworks(PBXProject project, string target)
			{
				var frameworks = (ArrayList)m_Data["frameworks"];
				if (frameworks != null && frameworks.Count > 0) {
					foreach (string framework in frameworks) {
						int index = framework.LastIndexOf(":");
						bool weak = index > 0;
						string filePath = weak ? framework.Substring(0, index) : framework;
						project.AddFrameworkToProject(target, filePath, weak);
						Debug.LogFormat("AddFrameworks: {0}", filePath);
					}
				}
			}
			private void AddFiles(PBXProject project, string target)
			{
				var files = (ArrayList)m_Data["files"];
				if (files != null && files.Count > 0) {
					foreach (string file in files) {
						string flags = string.Empty;
						string filePath = file;
						int index = file.LastIndexOf(":");
						if (index > 0) {
							flags = file.Substring(index + 1);
							filePath = file.Substring(0, index);
						}
						filePath = Path.Combine(m_DirectoryName, filePath);
						AddFile(project, target, filePath, Path.GetFileName(filePath), flags);
						Debug.LogFormat("AddFiles: {0}", filePath);
					}
				}
			}

			private HashSet<string> m_BuildPaths = new HashSet<string>();
			private void AddFile(PBXProject project, string target, string srcPath, string dstPath, string flags)
			{
				var extensions = new HashSet<string>(new string[] { ".a", ".framework", ".dylib", ".tbd" });
				if (!string.IsNullOrEmpty(m_GroupName)) {
					dstPath = Path.Combine(m_GroupName, dstPath);
				}
				var fileGuid = project.AddFile(srcPath, dstPath, PBXSourceTree.Absolute);
				if (!string.IsNullOrEmpty(flags)) {
					project.AddFileToBuildWithFlags(target, fileGuid, flags);
				} else {
					project.AddFileToBuild(target, fileGuid);
				}
				if (extensions.Contains(Path.GetExtension(srcPath))) {
					var directoryName = Path.GetDirectoryName(srcPath);
					if (m_BuildPaths.Add(directoryName)) {
						if (File.Exists(srcPath)) {
							project.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", directoryName);
							Debug.LogFormat("AddLibrarySearchPaths: {0}", directoryName);
						} else {
							project.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", directoryName);
							Debug.LogFormat("AddFrameworkSearchPaths: {0}", directoryName);
						}
					}
				}
				Debug.LogFormat("AddFile: {0} => {1} | flags = {2}", srcPath, dstPath, flags);
			}

			private void AddFolders(PBXProject project, string target)
			{
				var folders = (ArrayList)m_Data["folders"];
				if (folders != null && folders.Count > 0) {
					var excludes = (ArrayList)m_Data["excludes"];
					string regexExcludes = string.Format(@"{0}", excludes == null ? "" : string.Join("|", Array.ConvertAll(excludes.ToArray(), (v) => v.ToString())));
					foreach (string folder in folders) {
						string flags = string.Empty;
						string folderPath = folder;
						int index = folder.LastIndexOf(":");
						if (index > 0) {
							flags = folder.Substring(index + 1);
							folderPath = folder.Substring(0, index);
						}
						folderPath = Path.Combine(m_DirectoryName, folderPath);
						if (Directory.Exists(folderPath)) {
							AddFolder(project, target, folderPath, Path.GetFileName(folderPath), regexExcludes, flags);
						}
						Debug.LogFormat("AddFolders: {0}", folderPath);
					}
				}
			}

			private void AddFolder(PBXProject project, string target, string srcPath, string dstPath, string regexExcludes, string flags)
			{
				foreach (var file in Directory.GetFiles(srcPath, "*.*", SearchOption.TopDirectoryOnly)) {
					if (Regex.IsMatch(file, regexExcludes)) {
						continue;
					}
					AddFile(project, target, file, Path.Combine(dstPath, Path.GetFileName(file)), flags);
				}

				foreach (var dir in Directory.GetDirectories(srcPath)) {
					var path = Path.Combine(dstPath, Path.GetFileName(dir));
					if (dir.EndsWith(".framework") || dir.EndsWith(".bundle")) {
						AddFile(project, target, dir, path, "");
					} else {
						AddFolder(project, target, dir, path, regexExcludes, flags);
					}
				}
			}

			private void AddHeaderPaths(PBXProject project, string target)
			{
				var headerpaths = (ArrayList)m_Data["headerpaths"];
				if (headerpaths != null && headerpaths.Count > 0) {
					foreach (string headerpath in headerpaths) {
						var fullpath = Path.Combine(m_DirectoryName, headerpath);
						project.AddBuildProperty(target, "HEADER_SEARCH_PATHS", fullpath);
						Debug.LogFormat("AddHeaderPaths: {0}", fullpath);
					}
				}
			}
			private void AddCompilerFlags(PBXProject project, string target)
			{
				var flags = (ArrayList)m_Data["compiler_flags"];
				if (flags != null && flags.Count > 0) {
					foreach (string flag in flags) {
						project.AddBuildProperty(target, "OTHER_CFLAGS", flag);
						Debug.LogFormat("AddCompilerFlags: {0}", flag);
					}
				}
			}
			private void AddLinkerFlags(PBXProject project, string target)
			{
				var flags = (ArrayList)m_Data["linker_flags"];
				if (flags != null && flags.Count > 0) {
					foreach (string flag in flags) {
						project.AddBuildProperty(target, "OTHER_LDFLAGS", flag);
						Debug.LogFormat("AddLinkerFlags: {0}", flag);
					}
				}
			}
			private void AddEmbedBinaries(PBXProject project, string target)
			{
			}
			private void AddShellScripts(PBXProject project, string target)
			{
				//string sh = "\"${PROJECT_DIR}/Fabric.framework/run\" fd15e9852355bb4ed7b677207f875892b3a36e18 ed52f17a58efb61ac55e7a22f37fc6ea02e0e047d7ced020a21418100547ba79";
				//Debug.Log("======AddShellScripts"+ sh);
			    //project.AddShellScriptToBuild(target, sh);

				var scripts = (ArrayList)m_Data["shellscripts"];
				if (scripts != null && scripts.Count > 0) {
					foreach (string script in scripts) {
						project.AddShellScriptToBuild(target, script);
						Debug.LogFormat("AddShellScripts: {0}", script);
					}
				}
			}
			private void AddPlist(PlistDocument plist)
			{
				var table = (Hashtable)m_Data["plist"];
				if (table != null && table.Count > 0) {
					var root = plist.root;
					foreach (DictionaryEntry entry in table) {
						AddPlistElement(root, true, entry.Key.ToString(), entry.Value);
					}
				}
			}
			private void AddPlistElement(PlistElement root, bool isDict, string key, object value)
			{
				if (value is Hashtable) {
					var dict = isDict ? root.AsDict().CreateDict(key) : root.AsArray().AddDict();
					foreach (DictionaryEntry entry in (Hashtable)value) {
						AddPlistElement(dict, true, entry.Key.ToString(), entry.Value);
					}
				} else if (value is ArrayList) {
					var list = isDict ? root.AsDict().CreateArray(key) : root.AsArray().AddArray();
					foreach (object v in (ArrayList)value) {
						AddPlistElement(list, false, "", v);
					}
				} else if (value is bool) {
					if (isDict) {
						root.AsDict().SetBoolean(key, (bool)value);
					} else {
						root.AsArray().AddBoolean((bool)value);
					}
				} else if (value is int) {
					if (isDict) {
						root.AsDict().SetInteger(key, (int)value);
					} else {
						root.AsArray().AddInteger((int)value);
					}
				} else {
					if (isDict) {
						root.AsDict().SetString(key, (string)value);
					} else {
						root.AsArray().AddString((string)value);
					}
				}
			}

			static public ProjMod ReadFromFile(string file)
			{
				var content = File.ReadAllText(file);
				Hashtable data = (Hashtable)MiniJSON.jsonDecode(content);
				if (data == null) {
					Debug.Log(content);
					throw new UnityException("Parse error in file " + System.IO.Path.GetFileName(file) + "! Check for typos such as unbalanced quotation marks, etc.");
				}
				return new ProjMod(file, data);
			}
		}
	}

	static public class PBXProjectExtension
	{
		static public void AddShellScriptToBuild(this PBXProject project, string target, string shellScript)
		{
			var projectData = project.GetType().GetField("m_Data", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(project);

			var PBXProjectData = projectData.GetType();
			var assembly = PBXProjectData.Assembly;
			var PBXGUID = assembly.GetType("UnityEditor.iOS.Xcode.PBX.PBXGUID");
			var GUIDList = assembly.GetType("UnityEditor.iOS.Xcode.PBX.GUIDList");
			var PBXShellScriptBuildPhaseData = assembly.GetType("UnityEditor.iOS.Xcode.PBX.PBXShellScriptBuildPhaseData");
			var SetPropertyString = PBXShellScriptBuildPhaseData.GetMethod("SetPropertyString", BindingFlags.Instance | BindingFlags.NonPublic);

			var guid = PBXGUID.GetMethod("Generate").Invoke(null, null);
			var shellScriptData = Activator.CreateInstance(PBXShellScriptBuildPhaseData);

		

			PBXShellScriptBuildPhaseData.GetField("guid").SetValue(shellScriptData, guid);
			PBXShellScriptBuildPhaseData.GetField("files").SetValue(shellScriptData, Activator.CreateInstance(GUIDList));
			SetPropertyString.Invoke(shellScriptData, new object[] { "isa", "PBXShellScriptBuildPhase" });
			SetPropertyString.Invoke(shellScriptData, new object[] { "buildActionMask", "2147483647" });
			SetPropertyString.Invoke(shellScriptData, new object[] { "runOnlyForDeploymentPostprocessing", "0" });
			//SetPropertyString.Invoke(shellScriptData, new object[] { "shellPath", "/bin/sh" });
			//SetPropertyString.Invoke(shellScriptData, new object[] { "shellScript", shellScript });
		

			PBXShellScriptBuildPhaseData.GetField("shellPath").SetValue(shellScriptData, "/bin/sh");
			PBXShellScriptBuildPhaseData.GetField("shellScript").SetValue(shellScriptData, shellScript);


			var shellScripts = PBXProjectData.GetField("shellScripts").GetValue(projectData);
			shellScripts.GetType().GetMethod("AddEntry").Invoke(shellScripts, new object[] { shellScriptData });

			var nativeTargets = PBXProjectData.GetField("nativeTargets").GetValue(projectData);
			var nativeTarget = nativeTargets.GetType().GetProperty("Item").GetValue(nativeTargets, new object[] { target });
			var phases = nativeTarget.GetType().GetField("phases").GetValue(nativeTarget);
			phases.GetType().GetMethod("AddGUID").Invoke(phases, new object[] { guid });
		}
	}
}
#endif