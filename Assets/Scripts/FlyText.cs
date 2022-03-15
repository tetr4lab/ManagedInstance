//	Copyright© tetr4lab.

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tetr4lab;

/// <summary>フライテキスト</summary>
public class FlyText : MonoBehaviour {

	#region Static

	/// <summary>マネージャ</summary>
	public static ManagedInstance<FlyText> managedInstance { get; protected set; }

	/// <summary>クラス初期化</summary>
	//protected static void Init () {
	static FlyText () {
		if (managedInstance == null) { managedInstance = new ManagedInstance<FlyText> (1, true); }
	}

	/// <summary>先読み</summary>
	public static async Task PreLoadAsync () { await managedInstance.GetPrefabAsync (); }

	/// <summary>生成</summary>
	/// <param name="parent">親</param>
	/// <param name="message">テロップ文</param>
	/// <param name="start">開始時刻</param>
	/// <param name="end">終了時刻</param>
	/// <returns>インスタンス</returns>
	public static async Task CreateAsync (GameObject parent, string message, float end = 2.5f, Func<bool> endCondition = null) {
		if (endCondition != null && endCondition ()) { return; }
		var instance = await managedInstance.CreateAsync (parent);
		if (instance != null) {
			if (endCondition != null && endCondition ()) { Destroy (instance.gameObject); }
			instance.init (message, end, endCondition);
		}
	}

	#endregion

	/// <summary>初期化</summary>
	private void init (string message, float end, Func<bool> endCondition) {
		transform.SetAsLastSibling ();
		gameObject.SetActive (true);
		var text = GetComponentInChildren<Text> ();
		if (text != null) { text.text = message; }
		var panelRect = GetComponent<RectTransform> ();
		panelRect.sizeDelta = new Vector2 (text.preferredWidth + 128f, text.preferredHeight + 16f);// panelRect.sizeDelta.y);    // 文字量に応じたサイズ
		Destroy (gameObject, end);
	}

}
