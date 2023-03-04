# CPUTempBigPicture
CPUGPUの温度、クロック、消費電力をデカデカと表示するだけ(i7 13700k/RTX4080のみ動作確認済)<br>
クロックと消費電力は16秒ごとに交互に表示<br>
<span style="color: red; ">[NOTE] ASUS ROG FONTを使っています。ROGFONTがない場合はOS標準フォントでの表示になります</span>

![CPUTempmini](https://user-images.githubusercontent.com/125875827/222867720-ad8e4159-e428-4e14-ba09-0dfbf124e211.jpg)

・Visual Studio 2022 および .NET Core 7.0で作成 (.NET 7.0ランタイムのインストールが必要です)

・右下の▲ボタンを押すとフルスクリーンに切り替わります。

・サブモニタ等に全画面表示することを目的としています。ウィンドウを小さくし過ぎると表示がバグります。

・ビルド先のディレクトリにある動画(.mp4)を自動的に開き、負荷軽減のため1/4の速度で背景に流します<br>
　動画がなければ真っ黒な背景になります<br>
　背景素材は以下をお借りしています<br>
　https://pixabay.com/ja/videos/3-d-%E3%83%AC%E3%83%B3%E3%83%80%E3%83%AA%E3%83%B3%E3%82%B0-%E5%8B%95%E3%81%8D-24715/

・使用ライブラリ:LibreHardwareMonitorLib 0.9.1 (Nugetでインストールしてください、ビルド時に必要です)<br>
　　　https://github.com/LibreHardwareMonitor/LibreHardwareMonitor
    
・ライセンス準拠：
    https://licenses.nuget.org/MPL-2.0

