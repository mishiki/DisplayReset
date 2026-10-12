# DisplayReset / 描画リセット

Windowsに `Win + Ctrl + Shift + B` を送る、小さなWindows Formsアプリです。
リモート操作中にショートカットを接続先へ送れない場合のために作りました。

## 使い方

1. Releasesから `DisplayReset.exe` をダウンロードします。
2. **操作したい接続先のWindows上で**起動します。
3. 「描画をリセット」を押します。

インストール不要です。画面が一瞬暗くなったり、音が鳴る場合があります。
表示する「送信しました」はキー入力の送信成功を示し、OS側のリセット完了や不具合の改善を保証しません。
GPUデバイスの無効化・再有効化、Windowsの再起動は行いません。

## 動作環境

- Windows 10 / 11
- .NET Framework 4.x
- 操作対象のユーザーデスクトップで実行

EXEはコード署名されていません。初版はコンパイル確認済みで、アプリからリセットする実動作テストは未実施です。

## ビルド

Windows PowerShellで次を実行します。

```powershell
.\build.ps1
```

Windows標準の.NET Framework C#コンパイラを使います。出力は `DisplayReset.exe` です。
