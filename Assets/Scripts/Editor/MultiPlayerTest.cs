using UnityEditor;
using UnityEngine;

public class MultiPlayerTest
{
    [MenuItem("Tools/Run Multiplayer/Win64/1 Players")]
    static void PerformWin64Build1()
    {
        PerformWin64Build(1);
    }

    #region Window
    [MenuItem("Tools/Run Multiplayer/Win64/2 Players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }

    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        for (int i = 1; i <= playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(),
                "Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
                BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
        }
    }
    #endregion

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}