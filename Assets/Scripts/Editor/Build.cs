using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class Build : IPreprocessBuildWithReport
{
	public int callbackOrder => 0;
	
    public void OnPreprocessBuild(BuildReport report)
    {
		Debug.Log("MyCustomBuildProcessor.OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
		
		GeneralAudio.Instance.SetBGMVolumeOnBuild(0.15f);
    }
}