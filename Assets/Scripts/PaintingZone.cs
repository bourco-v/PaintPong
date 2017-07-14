using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Es.InkPainter;

#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

#endif

public enum EPlayerColor
{
	None,
	BluePlayer,
	RedPlayer
}

public class PaintingZone : InkCanvas {

	static Vector2 _mapSize = new Vector2(1024, 1024);

	EPlayerColor[,] _paintedZone = null;
	Dictionary<EPlayerColor, int> _scores = null;

	void OnEnable()
	{
		_mapSize.x = Camera.main.pixelWidth;
		_mapSize.y = Camera.main.pixelHeight;
		ResetPaint();
		Events.Instance.AddListener<OnGameStartEvent>(HandleOnGameStart);
	}

	void HandleOnGameStart(OnGameStartEvent e)
	{
		ResetPaint();
	}

	public override void ResetPaint()
	{
		base.ResetPaint();
		_paintedZone = new EPlayerColor[(int)_mapSize.x, (int)_mapSize.y];
		for (int i = 0; i < (int)_mapSize.x; ++i)
		{
			for (int j = 0; j < (int)_mapSize.y; ++j)
			{
				_paintedZone[i, j] = EPlayerColor.None;
			}
		}
		_scores = new Dictionary<EPlayerColor, int>();
		_scores.Add(EPlayerColor.BluePlayer, 0);
		_scores.Add(EPlayerColor.RedPlayer, 0);
	}

	void WorldCoordToTabCoord(Vector3 worldPos, out int x, out int y)
	{
		x = (int)((worldPos.x * 2f) + (int)(_mapSize.x / 2));
		y = (int)((worldPos.z * 2f) + (int)(_mapSize.y / 2));
	}

	void ScorePaintPoint(Vector3 worldPos)
	{
		int xPos, yPos;
		WorldCoordToTabCoord(worldPos, out xPos, out yPos);
		var lastPlayerColor = _paintedZone[xPos, yPos];
		if (lastPlayerColor != BallPainter.Instance.CurrentPlayerColor) // If player is painting an area that wasn't his before
		{
			if (lastPlayerColor != EPlayerColor.None) // If the last person that applied color was the enemy
			{
				_scores[lastPlayerColor] -= 1; // Reduce the enemy points
			}
			_scores[BallPainter.Instance.CurrentPlayerColor] += 1; // Increases the player points as he painted a zone
		}
		_paintedZone[xPos, yPos] = BallPainter.Instance.CurrentPlayerColor;
	}

	void ScorePaintZone(Vector3 center, float brushScale)
	{
		int pointsGiven = 0;
		int radius = (int)(brushScale / 0.003f) / 5;
		for (int i = 0; i < radius; ++i)
		{
			for (int j = 0; j < radius; ++j)
			{
				if (i + j <= radius)
				{
					ScorePaintPoint(new Vector3(center.x + i, center.y, center.z + j));
					++pointsGiven;
				}
			}
		}
	}

	public override bool Paint(Brush brush, Vector3 worldPos, Camera renderCamera = null)
	{
		if (base.Paint(brush, worldPos, renderCamera) == true)
		{
			ScorePaintZone(worldPos, brush.Scale);
			
			return (true);
		}
		return (false);
	}

	public int GetPlayerScore(EPlayerColor player)
	{
		return (_scores[player]);
	}

	public float GetPlayerScorePercent(EPlayerColor player)
	{
		int blueScore = GetPlayerScore(EPlayerColor.BluePlayer);
		int redScore = GetPlayerScore(EPlayerColor.RedPlayer);

		if (player == EPlayerColor.BluePlayer)
		{
			return ((((float)blueScore / (float)(redScore + blueScore) * 100f)));
		}
		else
		{
			return ((((float)redScore / (float)(blueScore + redScore) * 100f)));
		}
	}

	public EPlayerColor GetWinner()
	{
		return (GetPlayerScore(EPlayerColor.BluePlayer) >= GetPlayerScore(EPlayerColor.RedPlayer) ? EPlayerColor.BluePlayer : EPlayerColor.RedPlayer);
	}

	#region CustomEditor

#if UNITY_EDITOR

	[CustomEditor(typeof(PaintingZone))]
	[CanEditMultipleObjects]
	private class DynamicCanvasInspectorExtension : Editor
	{
		private Renderer renderer;
		private Material[] materials;
		private List<bool> foldOut;

		public override void OnInspectorGUI()
		{
			var instance = target as PaintingZone;
			if (instance.paintSet == null)
				instance.paintSet = new List<PaintSet>();

			if (renderer == null)
				renderer = instance.GetComponent<Renderer>();
			if (materials == null)
				materials = renderer.sharedMaterials;
			if (foldOut == null)
				foldOut = new List<bool>();

			if (instance.paintSet.Count < materials.Length)
			{
				for (int i = instance.paintSet.Count; i < materials.Length; ++i)
					instance.paintSet.Add(new PaintSet
					{
						mainTextureName = "_MainTex",
						normalTextureName = "_BumpMap",
						heightTextureName = "_ParallaxMap",
						useMainPaint = true,
						useNormalPaint = false
					});
				foldOut.Clear();
			}

			if (instance.paintSet.Count > materials.Length)
			{
				instance.paintSet.RemoveRange(materials.Length, instance.paintSet.Count - materials.Length);
				foldOut.Clear();
			}

			if (foldOut.Count < instance.paintSet.Count)
				for (int i = foldOut.Count; i < instance.paintSet.Count; ++i)
					foldOut.Add(true);

			EditorGUILayout.Space();

			if (EditorApplication.isPlaying)
			{
				#region PlayModeOperation

				EditorGUILayout.HelpBox("Can not change while playing.\n but you can saved painted texture.", MessageType.Info);
				for (int i = 0; i < instance.paintSet.Count; ++i)
				{
					if (foldOut[i] = Foldout(foldOut[i], string.Format("Material \"{0}\"", materials[i].name)))
					{
						EditorGUILayout.BeginVertical("ProgressBarBack");
						var backColorBuf = GUI.backgroundColor;
						GUI.backgroundColor = Color.green;

						var paintSet = instance.paintSet[i];

						if (paintSet.paintMainTexture != null && GUILayout.Button("Save main texture"))
						{
							SaveRenderTextureToPNG(paintSet.mainTexture.name, paintSet.paintMainTexture);
						}

						if (instance.paintSet[i].paintNormalTexture != null && GUILayout.Button("Save normal texture"))
						{
							//TODO:普通にNormalのテクスチャ保存するとちゃんと法線が出力されない？
							SaveRenderTextureToPNG(paintSet.normalTexture.name, paintSet.paintNormalTexture);
						}

						if (instance.paintSet[i].paintHeightTexture != null && GUILayout.Button("Save height texture"))
						{
							SaveRenderTextureToPNG(paintSet.heightTexture.name, paintSet.paintHeightTexture);
						}

						GUI.backgroundColor = backColorBuf;
						EditorGUILayout.EndVertical();
					}
				}

				#endregion PlayModeOperation
			}
			else
			{
				#region Property Setting

				for (int i = 0; i < instance.paintSet.Count; ++i)
				{
					if (foldOut[i] = Foldout(foldOut[i], string.Format("Material \"{0}\"", materials[i].name)))
					{
						EditorGUI.indentLevel = 0;
						EditorGUILayout.BeginVertical("ProgressBarBack");

						//MainPaint
						EditorGUI.BeginChangeCheck();
						instance.paintSet[i].useMainPaint = EditorGUILayout.Toggle("Use Main Paint", instance.paintSet[i].useMainPaint);
						if (EditorGUI.EndChangeCheck())
							ChangeValue(i, "Use Main Paint", p => p.useMainPaint = instance.paintSet[i].useMainPaint);
						if (instance.paintSet[i].useMainPaint)
						{
							EditorGUI.indentLevel++;
							EditorGUI.BeginChangeCheck();
							instance.paintSet[i].mainTextureName = EditorGUILayout.TextField("MainTexture Property Name", instance.paintSet[i].mainTextureName);
							if (EditorGUI.EndChangeCheck())
								ChangeValue(i, "Main Texture Name", p => p.mainTextureName = instance.paintSet[i].mainTextureName);
							EditorGUI.indentLevel--;
						}

						//NormalPaint
						EditorGUI.BeginChangeCheck();
						instance.paintSet[i].useNormalPaint = EditorGUILayout.Toggle("Use NormalMap Paint", instance.paintSet[i].useNormalPaint);
						if (EditorGUI.EndChangeCheck())
							ChangeValue(i, "Use Normal Paint", p => p.useNormalPaint = instance.paintSet[i].useNormalPaint);
						if (instance.paintSet[i].useNormalPaint)
						{
							EditorGUI.indentLevel++;
							EditorGUI.BeginChangeCheck();
							instance.paintSet[i].normalTextureName = EditorGUILayout.TextField("NormalMap Property Name", instance.paintSet[i].normalTextureName);
							if (EditorGUI.EndChangeCheck())
								ChangeValue(i, "Normal Texture Name", p => p.normalTextureName = instance.paintSet[i].normalTextureName);
							EditorGUI.indentLevel--;
						}

						//HeightPaint
						EditorGUI.BeginChangeCheck();
						instance.paintSet[i].useHeightPaint = EditorGUILayout.Toggle("Use HeightMap Paint", instance.paintSet[i].useHeightPaint);
						if (EditorGUI.EndChangeCheck())
							ChangeValue(i, "Use Height Paint", p => p.useHeightPaint = instance.paintSet[i].useHeightPaint);
						if (instance.paintSet[i].useHeightPaint)
						{
							EditorGUI.indentLevel++;
							EditorGUI.BeginChangeCheck();
							instance.paintSet[i].heightTextureName = EditorGUILayout.TextField("HeightMap Property Name", instance.paintSet[i].heightTextureName);
							if (EditorGUI.EndChangeCheck())
								ChangeValue(i, "Height Texture Name", p => p.heightTextureName = instance.paintSet[i].heightTextureName);
							EditorGUI.indentLevel--;
						}

						EditorGUILayout.EndVertical();
						EditorGUI.indentLevel = 0;
					}
				}

				#endregion Property Setting
			}
		}

		private void SaveRenderTextureToPNG(string textureName, RenderTexture renderTexture, Action<TextureImporter> importAction = null)
		{
			string path = EditorUtility.SaveFilePanel("Save to png", Application.dataPath, textureName + "_painted.png", "png");
			if (path.Length != 0)
			{
				var newTex = new Texture2D(renderTexture.width, renderTexture.height);
				RenderTexture.active = renderTexture;
				newTex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
				newTex.Apply();

				byte[] pngData = newTex.EncodeToPNG();
				if (pngData != null)
				{
					File.WriteAllBytes(path, pngData);
					AssetDatabase.Refresh();
					var importer = AssetImporter.GetAtPath(path) as TextureImporter;
					if (importAction != null)
						importAction(importer);
				}

				Debug.Log(path);
			}
		}

		private void ChangeValue(int paintSetIndex, string recordName, Action<PaintSet> assign)
		{
			Undo.RecordObjects(targets, "Change " + recordName);
			foreach (var t in targets.Where(_t => _t is InkCanvas).Select(_t => _t as PaintingZone))
				if (t.paintSet.Count > paintSetIndex)
				{
					assign(t.paintSet[paintSetIndex]);
					EditorUtility.SetDirty(t);
				}
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}

		public bool Foldout(bool foldout, string content)
		{
			var style = new GUIStyle("ShurikenModuleTitle");
			style.font = new GUIStyle(EditorStyles.label).font;
			style.border = new RectOffset(1, 7, 4, 4);
			style.fixedHeight = 28;
			style.contentOffset = new Vector2(20f, -2f);

			var rect = GUILayoutUtility.GetRect(16f, 22f, style);
			GUI.Box(rect, content, style);

			var e = Event.current;

			var toggleRect = new Rect(rect.x + 4f, rect.y + 5f, 13f, 13f);
			if (e.type == EventType.Repaint)
			{
				EditorStyles.foldout.Draw(toggleRect, false, false, foldout, false);
			}

			if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
			{
				foldout = !foldout;
				e.Use();
			}

			return foldout;
		}
	}

#endif

	#endregion CustomEditor
}
