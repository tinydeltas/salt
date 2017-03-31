using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainCreatorScript))]
public class TerrainCreatorInspector : Editor {

	private TerrainCreatorScript creator;

	private void OnEnable () {
		creator = target as TerrainCreatorScript;
		Undo.undoRedoPerformed += RefreshCreator;
	}

	private void OnDisable () {
		Undo.undoRedoPerformed -= RefreshCreator;
	}

	private void RefreshCreator () {
		if (Application.isPlaying) {
			creator.UpdateTerrain ();
		}
	}

	public override void OnInspectorGUI () {
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck()) {
			RefreshCreator();
		}
	}
}