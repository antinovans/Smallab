using UnityEditor;

public class BuildScript {

	[MenuItem("SMALLab Learning/Build Windows App")]
	static void BuildWindowsApp() { 
				BuildPipeline.BuildPlayer(new string[]{"Assets/_Scenes/Memory.unity"}, "../SMALLab_Builds/Memory/Memory.exe", BuildTarget.StandaloneWindows, BuildOptions.None); 
	} 
	

}
