using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Tetr4lab;

public class SampleMain : MonoBehaviour {

    private bool destroyed = false;

    async void Start () {
        // 生成1
        if (!destroyed) await FlyText.CreateAsync (gameObject, "Ready?");
        // 消滅を待つ
        if (!destroyed) await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // 生成2
        if (!destroyed) await FlyText.CreateAsync (gameObject, "Start!");
        // 消滅を待つ
        if (!destroyed) await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // 3秒待つ
        if (!destroyed) await Task.Delay (3000);
        // 生成3
        if (!destroyed) await FlyText.CreateAsync (gameObject, "End");
        // 消滅を待つ
        if (!destroyed) await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // 終了
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    private void OnDestroy () => destroyed = true;

}