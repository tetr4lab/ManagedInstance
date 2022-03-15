using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Tetr4lab;

public class SampleMain : MonoBehaviour {

    async void Start () {
        // ê∂ê¨1
        await FlyText.CreateAsync (gameObject, "Ready?");
        // è¡ñ≈Çë“Ç¬
        await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // ê∂ê¨2
        await FlyText.CreateAsync (gameObject, "Start!");
        // è¡ñ≈Çë“Ç¬
        await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // 3ïbë“Ç¬
        await Task.Delay (3000);
        // ê∂ê¨3
        await FlyText.CreateAsync (gameObject, "End");
        // è¡ñ≈Çë“Ç¬
        await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // èIóπ
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

}