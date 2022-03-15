using System;
using UnityEngine;

// ref: http://baba-s.hatenablog.com/entry/2017/12/25/085100 (コガネブログ)
public class OnDestroyCallback : MonoBehaviour {

	Action onDestroy;

	public static void AddOnDestroyCallback (GameObject gameObject, Action callback) {
		OnDestroyCallback onDestroyCallback = gameObject.GetComponent<OnDestroyCallback> ();
		if (!onDestroyCallback) {
			onDestroyCallback = gameObject.AddComponent<OnDestroyCallback> ();
			onDestroyCallback.hideFlags = HideFlags.HideAndDontSave;
		}
		onDestroyCallback.onDestroy += callback;
	}

	private void OnDestroy () {
		if (onDestroy != null) {
			onDestroy ();
			onDestroy = null;
		}
	}
}

public static class OnDestroyCallbackExtensions {
	public static void AddOnDestroyCallback (this GameObject gameObject, Action callback) {
		OnDestroyCallback.AddOnDestroyCallback (gameObject, callback);
	}
}
