using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Tetr4lab;

public class SampleMain : MonoBehaviour {

    private bool destroyed = false;

    async void Start () {
        // ¶¬1
        if (!destroyed) await FlyText.CreateAsync (gameObject, "Ready?");
        // Á–Å‚ğ‘Ò‚Â
        if (!destroyed) await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // ¶¬2
        if (!destroyed) await FlyText.CreateAsync (gameObject, "Start!");
        // Á–Å‚ğ‘Ò‚Â
        if (!destroyed) await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // 3•b‘Ò‚Â
        if (!destroyed) await Task.Delay (3000);
        // ¶¬3
        if (!destroyed) await FlyText.CreateAsync (gameObject, "End");
        // Á–Å‚ğ‘Ò‚Â
        if (!destroyed) await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // I—¹
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    private void OnDestroy () => destroyed = true;

}