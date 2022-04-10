//	Copyright© tetr4lab.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tetr4lab {

	/// <summary>
	/// プレハブのインスタンス数を管理するクラス
	/// 概要
	///		クラスTはオブジェクトの制御ルーチン (プレハブへあらかじめアタッチされていなくても、インスタンスには勝手にアタッチされる)
	///		デフォルトでクラスTと同じ名前のプレハブを使う
	///		例えば、MaxInstances=1、AutoDelete=true だと、常に最後のひとつだけが存在するようになる
	///	使い方
	///		※このManagedInstanceクラスはオブジェクトにアタッチせずに使います。
	///		public class T : MonoBehaviour {
	///			#region Static
	///			public static ManagedInstance<T> managedInstance { get; protected set; }
	///			static T () {
	///				if (managedInstance == null) { managedInstance = new ManagedInstance<T> (); }
	///			}
	///			public static T Create (GameObject parent) {
	///				return managedInstance.Create (parent);
	///			}
	///			#endregion
	///		}
	/// </summary>
	public class ManagedInstance<T> : IDisposable where T : MonoBehaviour {

		/// <summary>インスタンス数</summary>
		public int Count => Instances.Count;
		
		/// <summary>最大インスタンス数 (0で無制限)</summary>
		public int MaxInstances { get; protected set; }
		
		/// <summary>上限数を超えたら最古を破棄する</summary>
		public bool AutoDelete { get; protected set; }
		
		/// <summary>インスタンス一覧</summary>
		public List<T> Instances { get; protected set; }
		
		/// <summary>最新のインスタンス</summary>
		public T LastInstance => (Instances == null || Count <= 0) ? null : Instances [Count - 1];
		
		/// <summary>ひとつ以上生成されている</summary>
		public bool OnMode {
			get { return (Instances != null && Count > 0); }
			set {
				if (!value) { // 全インスタンス破棄
					foreach (var instance in Instances) {
						if (instance != null) { GameObject.Destroy (instance.gameObject); }
					}
				}
			}
		}

		/// <summary>デフォルトプレハブ</summary>
		public async Task<GameObject> GetPrefabAsync () {
			if (!prefab) {
				prefab = await Addressables.LoadAssetAsync<GameObject> ($"Prefabs/{typeof (T).Name}.prefab").Task;
			}
			return prefab;
		}
		private GameObject prefab;

		/// <summary>コンストラクタ</summary>
		/// <param name="max">最大インスタンス数 (0で無制限)</param>
		/// <param name="autoDelete">上限数を超えたら最古を破棄する</param>
		public ManagedInstance (int max = 0, bool autoDelete = false) {
			if (Instances == null) {
				MaxInstances = max;
				AutoDelete = autoDelete;
				Instances = new List<T> { };
				_ = GetPrefabAsync (); // ブロックしない
			}
		}

		/// <summary>
		/// uGUIオブジェクト生成
		/// </summary>
		/// <param name="parent">ヒエラルキー上の親</param>
		/// <param name="prefab">プレハブ</param>
		/// <param name="control">管理下 (除外可能)</param>
		/// <returns>生成したオブジェクトの制御ルーチン</returns>
		public async Task<T> CreateAsync (GameObject parent, GameObject prefab = null, bool control = true) {
			if (!preCreate ()) { return null; }
			try {
				return postCreate (GameObject.Instantiate (prefab ?? await GetPrefabAsync (), parent.transform), control);
			}
			catch {
				return null;
			}
		}

		/// <summary>3Dオブジェクト生成</summary>
		public async Task<T> CreateAsync (Vector3 position, Quaternion rotation, GameObject prefab = null, bool control = true) {
			if (!preCreate ()) { return null; }
			try {
				return postCreate (GameObject.Instantiate (prefab ?? await GetPrefabAsync (), position, rotation), control);
			}
			catch {
				return null;
			}
		}

		/// <summary>生成前処理</summary>
		private bool preCreate () {
			if (MaxInstances > 0 && Count >= MaxInstances) { // 制限数オーバー
				if (AutoDelete) { // 最古を破棄して成り代わり
					GameObject.Destroy (Instances [0].gameObject);
				} else {
					return false; // 生成忌避
				}
			}
			return true;
		}

		/// <summary>生成後処理</summary>
		private T postCreate (GameObject obj, bool control) {
			var instance = obj.GetComponent<T> () ?? obj.AddComponent<T> ();
			if (instance != null) {
				if (control) {
					Instances.Add (instance);
					obj.AddOnDestroyCallback (() => Instances.Remove (instance));
				}
			} else {
				GameObject.Destroy (obj);
			}
			return instance;
		}

		/// <summary>全インスタンス破棄</summary>
		public void Destroy () {
			foreach (var instance in Instances) {
				if (instance != null) { GameObject.Destroy (instance.gameObject); }
			}
		}

		/// <summary>後始末</summary>
		public void Dispose () {
			OnMode = false;
			if (prefab) {
				Resources.UnloadAsset (prefab);
				prefab = null;
			}
		}

	}

}

