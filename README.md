# CPUTempBigPicture
CPUGPUの温度、クロック、消費電力をデカデカと表示するだけ(Core i7 13700k/Geforce RTX4800のみ動作確認済み)

・Visual Studio 2022 および .NET Core 7.0で作成

・サブモニタ等に全画面表示することを目的としています。ウィンドウを小さくし過ぎると表示がバグります。

・ビルド先のディレクトリにある動画(.mp4)を自動的に開き、負荷軽減のため1/4の速度で背景に流します<br>動画がなければ真っ黒な背景になります

・使用ライブラリ:LibreHardwareMonitorLib 0.9.1 (Nugetでインストールしてください、ビルド時に必要です)
    https://github.com/LibreHardwareMonitor/LibreHardwareMonitor
    
・ライセンス準拠：
    https://licenses.nuget.org/MPL-2.0

