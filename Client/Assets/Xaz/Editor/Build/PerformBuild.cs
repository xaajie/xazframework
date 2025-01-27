using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

class PerformBuild
{
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();

        foreach(EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if(e==null)
                continue;

            if(e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }

    static string GetBuildiOSPath()
    {
        return "build/Unity-iPhone";
    }

    [UnityEditor.MenuItem("Tools/Packer/Build iOS")]
    static void CommandLineBuildIOS ()
    {

        Debug.Log("Command line build\n------------------\n------------------");

        string[] scenes = GetBuildScenes();
        string path = GetBuildiOSPath();
        if(scenes == null || scenes.Length==0 || path == null)
            //return;
        Debug.Log(string.Format("Path: \"{0}\"", path));
        for(int i=0; i<scenes.Length; ++i)
        {
            Debug.Log(string.Format("Scene[{0}]: \"{1}\"", i, scenes[i]));
        }

        Debug.Log("Starting Build!");
        BuildPipeline.BuildPlayer(scenes, path, BuildTarget.iOS, BuildOptions.None);
    }

    [UnityEditor.MenuItem("Tools/Packer/Build Android")]
    static void CommandLineBuildAndroid ()
    {

        Debug.Log("Command line build\n------------------\n------------------");

        string[] scenes = GetBuildScenes();
        string path = GetBuildAndroidPath();
        if(scenes == null || scenes.Length==0 || path == null)
            //return;
            Debug.Log(string.Format("Path: \"{0}\"", path));
        for(int i=0; i<scenes.Length; ++i)
        {
            Debug.Log(string.Format("Scene[{0}]: \"{1}\"", i, scenes[i]));
        }

        Debug.Log("Starting Build!");
        BuildPipeline.BuildPlayer(scenes, path, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
    }

    static string GetBuildAndroidPath()
    {
        return "build/Unity-Android";
    }
}
