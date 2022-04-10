---
title: プレハブのインスタンス数を管理するクラス
tags: Unity C#
---

# プレハブのインスタンス数を管理するクラス

## はじめに

### 環境条件
- Unity 2018.4、2020.3 で確認しました。
- addressablesを使用しています。

## プレハブを使ってオブジェクトを動的に生成したい

同じオブジェクト(GameObject)を複数作って使う場合、プレハブを使いますよね。
あらかじめシーンに配置して使うこともありますが、私はどちらかというと動的に生成したいことが多いです。

例えば、以下のような使い方で、uGUIでフライテキスト(ダメージの数値表示とかテロップとかの類)を表示するプレハブを作るものとします。

```cs:Game.cs
	_ = FlyText.CreateAsync ("Start!");
```

使う側は、単にクラスの静的メソッドを呼ぶだけです。
このFlyTextクラスは、例えば以下のようになります。

```cs:FlyText.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>フライテキスト</summary>
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
		panelRect.sizeDelta = new Vector2 (text.preferredWidth + 128, panelRect.sizeDelta.y);    // 文字量に応じたサイズ
		Destroy (gameObject, end);
	}
}
```

このクラスを使う場合は、クラス名と同じ名前のプレハブを`Prefabs`フォルダに用意することになります。

しかし、このままでは、タイミング次第で**複数が重なって表示される**ことになります。

## 生成済みのインスタンスを管理したい

上記のフライテキストの場合なら、「既に表示中なら、表示中のものを消してから、新たに表示する」ようにしたいです。
そして、それを組み込むだけなら話は簡単です。
しかし、使い道によっては、「既に表示中なら、表示中のものを残して、新たに表示しようとしたものを破棄する」ような場合もありそうです。
他にも、モーダルダイアログのように複数重ねる可能性があって、最前だけを特別扱いしたいような場合もあるでしょう。

uGUIの例ばかりですが、私の場合は、プレハブから動的に生成して数や挙動を管理したいものが多くあります。
それぞれに同じようなことを書くのが面倒なので、共通化できないか考えた末に、以下のようなアプローチになりました。

https://github.com/tetr4lab/ManagedInstance/blob/d4658c95b18705b801e05807781e7424933a60b5/Assets/Scripts/ManagedInstance.cs#L1-L154

このクラスを使うフライテキストは、以下のようになります。

https://github.com/tetr4lab/ManagedInstance/blob/d4658c95b18705b801e05807781e7424933a60b5/Assets/Scripts/FlyText.cs#L1-L56

フライテキストの基本的な使い方は変わっていません。

```cs:Game.cs
	_ = _FlyText.CreateAsync ("Start!");
```

以下のような使い方が可能になります。

```cs:Game.cs
	if (FlyText.managedInstance.OnMode) {
		// フライテキスト表示中
	}
```

```cs:Game.cs
	// フライテキスト全削除
	FlyText.managedInstance.OnMode = false;
```

```cs:Game.cs
        // 生成1
        await FlyText.CreateAsync (gameObject, "Ready?");
        // 消滅を待つ
        await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // 生成2
        await FlyText.CreateAsync (gameObject, "Start!");
        // 消滅を待つ
        await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // 3秒待つ
        await Task.Delay (3000);
        // 生成3
        await FlyText.CreateAsync (gameObject, "End");
        // 消滅を待つ
        await TaskEx.DelayWhile (() => FlyText.managedInstance.OnMode);
        // 終了
```


モーダルダイアログのように必要なだけ重ねて使いたい場合は、コンストラクタへ渡す引数が省略されて、以下のようになります。

```cs:ModalDialog.cs
public class ModalDialog : MonoBehaviour {
	#region Static
	public static ManagedInstance<ModalDialog> managedInstance { get; protected set; }
	static ModalDialog () {
		if (managedInstance == null) { managedInstance = new ManagedInstance<ModalDialog> (); } // 数を制限しない
	}
	//～
```

そして、複数のダイアログが重なり合っていても、以下のようにして応答すべき最も手前のダイアログを判別できます。

```cs:ModalDialog.cs
	if (managedInstance.LastInstance == this) {
		// 一番手前のダイアログなら
	}
```

## おわりに

最後までお読みいただき、どうもありがとうございます。
ご意見、ご感想、ご提案など、何でもいただければうれしいです。

なお、リポジトリのコードは検証済みですが、記事中のコードは未検証です。
あしからずご了承ください。

### 謝辞
`OnDestroyCallback`は、[コガネブログ様の記事](http://baba-s.hatenablog.com/entry/2017/12/25/085100)を使わせていただきました。
いつも、役立つ記事をありがとうございます。この場を借りてお礼申し上げます。

