param(
    [ValidateSet(1,2)][int]$Episode = 2,
    [string]$UnityEditor = $env:DARK_NEBULA_UNITY,
    [string]$Jdk = $env:DARK_NEBULA_JDK17
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
if (-not $UnityEditor) { $UnityEditor = Join-Path $env:LOCALAPPDATA 'Unity/Editors/6000.4.11f1/Editor/Unity.exe' }
if (-not (Test-Path -LiteralPath $UnityEditor)) { throw 'Set DARK_NEBULA_UNITY to the Unity 6000.4.11f1 editor executable.' }
if (-not $Jdk) { $Jdk = $env:JAVA_HOME }
if (-not $Jdk -or -not (Test-Path -LiteralPath (Join-Path $Jdk 'bin/java.exe'))) { throw 'Set DARK_NEBULA_JDK17 to a JDK 17 directory.' }
$env:DARK_NEBULA_JDK17 = $Jdk
$projectName = if ($Episode -eq 1) { 'EpisodeOne' } else { 'EpisodeTwo' }
$method = if ($Episode -eq 1) { 'CodexBuildEpisodeOne.Build' } else { 'CodexBuildEpisodeTwo.Build' }
$project = Join-Path $repo $projectName
$builds = Join-Path $repo 'Builds'
New-Item -ItemType Directory -Force -Path $builds | Out-Null
$log = Join-Path $builds "episode$Episode-unity.log"
& $UnityEditor -batchmode -quit -buildTarget Android -projectPath $project -executeMethod $method -exportGradle -logFile $log
if ($LASTEXITCODE -ne 0 -or -not ([IO.File]::ReadAllText($log).Contains('CODEX_BUILD_RESULT Succeeded errors=0'))) { throw "Unity export failed. See $log" }
$export = Join-Path $repo "android-final-exports/episode$Episode"
$editorDir = Split-Path -Parent $UnityEditor
$gradle = Join-Path $editorDir 'Data/PlaybackEngines/AndroidPlayer/Tools/gradle/lib/gradle-launcher-9.1.0.jar'
& (Join-Path $Jdk 'bin/java.exe') '-Xmx4096m' '-Dfile.encoding=UTF-8' '-Dorg.gradle.daemon=false' '-Dorg.gradle.internal.instrumentation.agent=false' -classpath $gradle org.gradle.launcher.GradleMain -p $export '-Dorg.gradle.jvmargs=-Xmx4096m -Dfile.encoding=UTF-8' --max-workers=2 --init-script (Join-Path $PSScriptRoot 'gradle-javac.gradle') assembleRelease
if ($LASTEXITCODE -ne 0) { throw 'Gradle packaging failed.' }
$apk = Join-Path $export 'launcher/build/outputs/apk/release/launcher-release.apk'
$destination = Join-Path $builds "DarkNebula-Episode$Episode-Android64.apk"
Copy-Item -LiteralPath $apk -Destination $destination
Write-Output "Built $destination"
