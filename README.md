# サンプルカレンダー 
ライセンス等はライセンスフォルダを確認してください

---

### 🚀 主な機能
* **カレンダー自動生成**: 実行時の年月を自動取得し、日付を正しく配置。
* **月切り替え**: 「前月」「次月」ボタンで過去や未来の予定を自由に確認。
* **土日の色分け**: 
    * <span style="color: #d93025; font-weight: bold;">日曜日（Red）</span>
    * <span style="color: #1a73e8; font-weight: bold;">土曜日（Blue）</span>
* **列拡張ギミック**: 日付の数字をクリックすると、その列（曜日）が大きく広がり、メモが書きやすくなります。
* **自動保存**: 入力した内容は即座にキャッシュ（LocalStorageやJSON）に保存されます。


---

### 🛠 対応言語とビルド方法


| 言語 | 実行・ビルド方法 | 保存先 |
| :--- | :--- | :--- |
| **HTML / JS** | `index.html` をブラウザで開く | ブラウザキャッシュ |
| **Python** | `pip install pyinstaller` <br> `pyinstaller --onefile --noconsole main.py` | `calendar_data.json` |
| **C# (.NET)** | Visual Studio等でビルドして `.exe` を生成 | `calendar_data.json` |

---

### 📁 ディレクトリ構成
```text
.
index.html# ブラウザ版（HTML5/JS）
README.md 説明
├── python/
│   └── main.py         # Python版（Tkinter）
└── csharp/
    └── Form1.cs        # C#版（Windows Forms）
