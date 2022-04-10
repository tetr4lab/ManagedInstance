---
title: �v���n�u�̃C���X�^���X�����Ǘ�����N���X
tags: Unity C#
---

# �v���n�u�̃C���X�^���X�����Ǘ�����N���X

## �͂��߂�

### ������
- Unity 2018.4�A2020.3 �Ŋm�F���܂����B
- addressables���g�p���Ă��܂��B

## �v���n�u���g���ăI�u�W�F�N�g�𓮓I�ɐ���������

�����I�u�W�F�N�g(GameObject)�𕡐�����Ďg���ꍇ�A�v���n�u���g���܂���ˁB
���炩���߃V�[���ɔz�u���Ďg�����Ƃ�����܂����A���͂ǂ��炩�Ƃ����Ɠ��I�ɐ������������Ƃ������ł��B

�Ⴆ�΁A�ȉ��̂悤�Ȏg�����ŁAuGUI�Ńt���C�e�L�X�g(�_���[�W�̐��l�\���Ƃ��e���b�v�Ƃ��̗�)��\������v���n�u�������̂Ƃ��܂��B

```cs:Game.cs
	_ = FlyText.CreateAsync ("Start!");
```

�g�����́A�P�ɃN���X�̐ÓI���\�b�h���ĂԂ����ł��B
����FlyText�N���X�́A�Ⴆ�Έȉ��̂悤�ɂȂ�܂��B

```cs:FlyText.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>�t���C�e�L�X�g</summary>
public class FlyText : MonoBehaviour {
	#region Static
	public static async Task<GameObject> Prefab {
		get {
			if (!prefab) {
				prefab = await Addressables.LoadAssetAsync<GameObject> ($"Prefabs/{typeof (FlyText).Name}.prefab").Task;
			}
			return prefab;
		}
	}
	private GameObject prefab;

	public static async Task<FlyText> CreateAsync (GameObject parent, string message, float end = 2.5f) {
		var instance = Instantiate (await Prefab, parent.transform);
		instance.init (message, end);
		return instance;
	}
	#endregion

	private void init (string message, float end) {
		transform.SetAsLastSibling ();
		gameObject.SetActive (true);
		var text = GetComponentInChildren<Text> ();
		if (text != null) { text.text = message; }
		var panelRect = GetComponent<RectTransform> ();
		panelRect.sizeDelta = new Vector2 (text.preferredWidth + 128, panelRect.sizeDelta.y);    // �����ʂɉ������T�C�Y
		Destroy (gameObject, end);
	}
}
```

���̃N���X���g���ꍇ�́A�N���X���Ɠ������O�̃v���n�u��`Prefabs`�t�H���_�ɗp�ӂ��邱�ƂɂȂ�܂��B

�������A���̂܂܂ł́A�^�C�~���O�����**�������d�Ȃ��ĕ\�������**���ƂɂȂ�܂��B

## �����ς݂̃C���X�^���X���Ǘ�������

��L�̃t���C�e�L�X�g�̏ꍇ�Ȃ�A�u���ɕ\�����Ȃ�A�\�����̂��̂������Ă���A�V���ɕ\������v�悤�ɂ������ł��B
�����āA�����g�ݍ��ނ����Ȃ�b�͊ȒP�ł��B
�������A�g�����ɂ���ẮA�u���ɕ\�����Ȃ�A�\�����̂��̂��c���āA�V���ɕ\�����悤�Ƃ������̂�j������v�悤�ȏꍇ�����肻���ł��B
���ɂ��A���[�_���_�C�A���O�̂悤�ɕ����d�˂�\���������āA�őO��������ʈ����������悤�ȏꍇ������ł��傤�B

uGUI�̗�΂���ł����A���̏ꍇ�́A�v���n�u���瓮�I�ɐ������Đ��⋓�����Ǘ����������̂���������܂��B
���ꂼ��ɓ����悤�Ȃ��Ƃ������̂��ʓ|�Ȃ̂ŁA���ʉ��ł��Ȃ����l�������ɁA�ȉ��̂悤�ȃA�v���[�`�ɂȂ�܂����B

https://github.com/tetr4lab/ManagedInstance/blob/d4658c95b18705b801e05807781e7424933a60b5/Assets/Scripts/ManagedInstance.cs#L1-L154

���̃N���X���g���t���C�e�L�X�g�́A�ȉ��̂悤�ɂȂ�܂��B

https://github.com/tetr4lab/ManagedInstance/blob/d4658c95b18705b801e05807781e7424933a60b5/Assets/Scripts/FlyText.cs#L1-L56

�t���C�e�L�X�g�̊�{�I�Ȏg�����͕ς���Ă��܂���B

```cs:Game.cs
	_ = _FlyText.CreateAsync ("Start!");
```

�ȉ��̂悤�Ȏg�������\�ɂȂ�܂��B

```cs:Game.cs
	if (FlyText.managedInstance.OnMode) {
		// �t���C�e�L�X�g�\����
	}
```

```cs:Game.cs
	// �t���C�e�L�X�g�S�폜
	FlyText.managedInstance.OnMode = false;
```

```cs:Game.cs
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
```


���[�_���_�C�A���O�̂悤�ɕK�v�Ȃ����d�˂Ďg�������ꍇ�́A�R���X�g���N�^�֓n���������ȗ�����āA�ȉ��̂悤�ɂȂ�܂��B

```cs:ModalDialog.cs
public class ModalDialog : MonoBehaviour {
	#region Static
	public static ManagedInstance<ModalDialog> managedInstance { get; protected set; }
	static ModalDialog () {
		if (managedInstance == null) { managedInstance = new ManagedInstance<ModalDialog> (); } // ���𐧌����Ȃ�
	}
	//�`
```

�����āA�����̃_�C�A���O���d�Ȃ荇���Ă��Ă��A�ȉ��̂悤�ɂ��ĉ������ׂ��ł���O�̃_�C�A���O�𔻕ʂł��܂��B

```cs:ModalDialog.cs
	if (managedInstance.LastInstance == this) {
		// ��Ԏ�O�̃_�C�A���O�Ȃ�
	}
```

## ������

�Ō�܂ł��ǂ݂��������A�ǂ������肪�Ƃ��������܂��B
���ӌ��A�����z�A����ĂȂǁA���ł�����������΂��ꂵ���ł��B

�Ȃ��A���|�W�g���̃R�[�h�͌��؍ς݂ł����A�L�����̃R�[�h�͖����؂ł��B
�������炸���������������B

### �ӎ�
`OnDestroyCallback`�́A[�R�K�l�u���O�l�̋L��](http://baba-s.hatenablog.com/entry/2017/12/25/085100)���g�킹�Ă��������܂����B
�����A�𗧂L�������肪�Ƃ��������܂��B���̏���؂�Ă���\���グ�܂��B

