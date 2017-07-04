﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UniGameTools.BuildMechine;
using UniGameTools.BuildMechine.BuildActions;
using UnityEngine;

public class BuildMechineExample
{
    [MenuItem("Tools/BuildMechine/Example")]
    public static void Build()
    {
        BuildMechine.NewPipeline()
            .AddActions(new BuildAction_Print("Start Build Mechine"))
            .AddActions(new BuildAction_IncreaseBuildNum())
            .AddActions(new BuildAction_SaveAndRefresh(),
                        new BuildAction_SetBundleId("cn.test.test"),
                        new BuildAction_BuildProjectAndroid("Build/"),
                        new BuildAction_BuildProjectWindowsStandalone("game", "Build/Windows/", x64: false),
                        new BuildAction_BuildProjectWindowsStandalone("game", "Build/Windows/", x64: true))
            .Run();
    }
    private class BuildAction_Error : BuildAction
    {
        public override BuildState OnUpdate()
        {
            return BuildState.Failure;
        }
    }

    private class BuildAction_Exception : BuildAction
    {
        public override BuildState OnUpdate()
        {
            throw new NotImplementedException();
        }
    }
}