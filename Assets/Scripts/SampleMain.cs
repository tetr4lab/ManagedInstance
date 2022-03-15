using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Tetr4lab;

public class SampleMain : MonoBehaviour {

    async void Start () {
        // ����1
        await FlyText.CreateAsync (gameObject, "Ready?");
        // ���ł�҂�
        await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // ����2
        await FlyText.CreateAsync (gameObject, "Start!");
        // ���ł�҂�
        await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // 3�b�҂�
        await Task.Delay (3000);
        // ����3
        await FlyText.CreateAsync (gameObject, "End");
        // ���ł�҂�
        await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // �I��
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

}