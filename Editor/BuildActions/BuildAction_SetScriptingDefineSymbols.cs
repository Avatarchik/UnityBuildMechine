﻿
using System.Text;
using UnityEditor;


public class BuildAction_SetScriptingDefineSymbols : BuildAction
{
    public string[] symbols;
    public BuildTargetGroup BuildTargetGroup = BuildTargetGroup.Standalone;

    public BuildAction_SetScriptingDefineSymbols(params string[] symbols)
    {
        this.symbols = symbols;
    }

    public BuildAction_SetScriptingDefineSymbols()
    {

    }

    public BuildAction_SetScriptingDefineSymbols(BuildTargetGroup buildTargetGroup, params string[] symbols)
    {
        this.symbols = symbols;
        BuildTargetGroup = buildTargetGroup;
    }

    public override void Build()
    {
        if (symbols == null)
        {
            symbols = new[] { "" };
        }

        var sb = new StringBuilder();

        foreach (var symbol in symbols)
        {
            sb.Append(symbol + ";");
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup, sb.ToString());
        State = BuildState.Succeed;
    }

    public override BuildProgress GetProgress()
    {
        return null;
    }
}