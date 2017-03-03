using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainCreator))]
public class TerrainCreatorInspector : Editor {

	private TerrainCreator creator;

	private void OnEnable () {
		creator = target as TerrainCreator;
		Undo.undoRedoPerformed += RefreshCreator;
	}

	private void OnDisable () {
		Undo.undoRedoPerformed -= RefreshCreator;
	}

	private void RefreshCreator () {
		if (Application.isPlaying) {
			creator.RenderTerrain ();
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