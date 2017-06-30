﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildMechine
{
    public static BuildMechine Instance;

    public List<object> Info = new List<object>();

    public List<BuildAction> Actions = new List<BuildAction>();

    public int CurrentActionIndex;

    public static bool IsBuilding
    {
        get { return EditorPrefs.GetBool("BuildMechine.IsBuilding", false); }
        set { EditorPrefs.SetBool("BuildMechine.IsBuilding", value); }
    }

    public static BuildMechine JsonInstance
    {
        get
        {

            var s = EditorPrefs.GetString("BuildMechine.JsonInstance", "");
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            Debug.Log("Load Json Instance");

            //            JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            //            return JsonConvert.DeserializeObject<BuildMechine>(s, setting);

            return JsonUtility.FromJson<BuildMechine>(s);
        }
        set
        {
            if (value == null)
            {
                EditorPrefs.DeleteKey("BuildMechine.JsonInstance");
            }
            else
            {
                //                JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                //                var json = JsonConvert.SerializeObject(value, Formatting.Indented, setting);
                //                EditorPrefs.SetString("BuildMechine.JsonInstance", json);
                var json = JsonUtility.ToJson(value);
                EditorPrefs.SetString("BuildMechine.JsonInstance", json);

                //                Debug.Log("Save : ");
                //                Debug.Log(serializeObject);
            }
        }
    }

    public BuildAction CurrentBuildAction
    {
        get
        {
            return Actions.Count > CurrentActionIndex ? Actions[CurrentActionIndex] : null;
        }
    }

    public BuildProgress GetProgress()
    {
        if (CurrentBuildAction != null) return CurrentBuildAction.GetProgress();

        return null;
    }

    //    public void Update()
    //    {
    //        EditorApplication.delayCall += () =>
    //        {
    //            UpdateMethod();
    //        };
    //    }

    public void UpdateMethod()
    {
        if (CurrentBuildAction != null)
        {
            switch (CurrentBuildAction.State)
            {
                case BuildState.None:
                    {
                        Debug.Log("Start Action: " + CurrentBuildAction.GetType());

                        CurrentBuildAction.State = BuildState.Building;
                        CurrentBuildAction.Build();
                    }
                    break;
                case BuildState.Building:
                    {
                        CurrentBuildAction.Update();
                    }
                    break;
                case BuildState.Succeed:
                    {
                        Info.AddRange(CurrentBuildAction.Infos);

                        CurrentActionIndex++;

                        if (CurrentBuildAction != null)
                        {
                            Debug.Log("Start Next Step : " + CurrentBuildAction.GetType());
                            CurrentBuildAction.State = BuildState.None;
                            JsonInstance = this;
                        }
                        else
                        {
                            BuildFinished();
                        }


                    }
                    break;
                case BuildState.Fail:
                    {
                        Info.AddRange(CurrentBuildAction.Infos);
                        Debug.LogError("打包结束。打包失败了");
                        CurrentActionIndex = int.MaxValue;
                        BuildFinished();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void BuildFinished()
    {
        // Log All Errors;
        foreach (var error in Info)
        {
            Debug.LogError(error);
        }

        JsonInstance = null;

        EditorWindow.GetWindow<BuildMechineWindows>().Close();

        EditorUtility.ClearProgressBar();

        Debug.Log("--------------------打包结束--------------------");
    }

    public bool IsFinished
    {
        get { return CurrentBuildAction == null; }
    }

    /// <summary>
    /// 准备好之后。调用Update就开始进入建造管道了
    /// </summary>
    public static void SetPipeline(params BuildAction[] actions)
    {
        var window = EditorWindow.GetWindow<BuildMechineWindows>();

        window.Focus();

        Instance = new BuildMechine();

        Instance.Actions = actions.ToList();

        Instance.CurrentActionIndex = 0;

        //        for (var i = 0; i < actions.Length; i++)
        //        {
        //            var a = actions[i];
        //
        //            a.Mechine = Instance;
        //
        //            if (i + 1 < actions.Length)
        //            {
        //                a.NextAction = actions[i + 1];
        //            }
        //        }
        //
        //        Instance.CurrentBuildAction = actions[0];
    }

    public static void ShowProgress()
    {
        if (Instance != null)
        {
            if (Instance.IsFinished) return;

            var progress = Instance.GetProgress();
            if (progress != null)
            {
                EditorUtility.DisplayProgressBar(progress.Title, progress.Content, progress.Porgress);
            }
        }
    }
}