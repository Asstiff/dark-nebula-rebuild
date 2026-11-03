#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEngine;
public static class BuildAudit
{
    public static void Run()
    {
        var report=BuildReport.GetLatestReport();
        var groups=report.steps.SelectMany(s=>s.messages).Where(m=>m.type==LogType.Error || m.type==LogType.Exception).GroupBy(m=>m.content).Select(g=>g.Count()+" x "+g.Key).ToArray();
        File.WriteAllLines("../build-audit-ep2.txt",groups);
        foreach(string line in groups)Debug.Log("BUILD_AUDIT "+line);
    }
}
#endif
