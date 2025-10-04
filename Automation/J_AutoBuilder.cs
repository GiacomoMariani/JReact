#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.BuildAutomation
{
    [CreateAssetMenu(fileName = "J_AutoBuilder", menuName = "Reactive/Automation/AutoBuild")]
    public class J_AutoBuilder : ScriptableObject
    {
        // --------------- CONSTS --------------- //
        private const string _AndroidManifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
        private const string _DoubleDot = "..";
        private const string PasswordSuffix = ".password";

        // --------------- BUILD SETTINGS --------------- //
        [BoxGroup("Setup - Build", true, true, 0), SerializeField]
        private NamedBuildTarget _nameBuildTarget = NamedBuildTarget.Standalone;
        [BoxGroup("Setup - Build", true, true, 0), SerializeField]
        private BuildTargetGroup _buildTargetGroup = BuildTargetGroup.Standalone;
        [BoxGroup("Setup - Build", true, true, 0), SerializeField]
        private BuildTarget _buildTargetPlatform = BuildTarget.StandaloneWindows;
        [BoxGroup("Setup - Build", true, true, 0), SerializeField] private BuildOptions _buildOptions = BuildOptions.None;

        // --------------- PROJECT SETTINGS --------------- //
        [BoxGroup("Setup - Project", true, true, 10), SerializeField] private SceneAsset[] _scenesToBuild;
        [BoxGroup("Setup - Project", true, true, 10), SerializeField] private string _companyName;
        [BoxGroup("Setup - Project", true, true, 10), SerializeField] private string _productName;
        [BoxGroup("Setup - Project", true, true, 10), SerializeField] private bool _useCustomDefines;
        [BoxGroup("Setup - Project", true, true, 10), SerializeField, ShowIf(nameof(_useCustomDefines))]
        private string _customDefines;

        // --------------- ANDROID --------------- //
        [BoxGroup("Setup - Android Specific", true, true, 10), SerializeField, ShowIf(nameof(IsAndroid))]
        private string _bundleVersion;
        [BoxGroup("Setup - Android Specific", true, true, 10), SerializeField, ShowIf(nameof(IsAndroid))]
        private int _versionCode;
        [BoxGroup("Setup - Android Specific", true, true, 10), SerializeField, ShowIf(nameof(IsAndroid))]
        private string _applicationIdentifier;
        [BoxGroup("Setup - Android Specific", true, true, 10), SerializeField, ShowIf(nameof(IsAndroid))]
        private AndroidSdkVersions _minSdkVersion;
        [BoxGroup("Setup - Android Specific", true, true, 10), SerializeField, ShowIf(nameof(IsAndroid))]
        private AndroidSdkVersions _targetSdkVersion;
        [BoxGroup("Setup - Android Specific", true, true, 10), SerializeField, ShowIf(nameof(IsAndroid))]
        private TextAsset _androidManifest;
        [BoxGroup("Setup - Android Specific", true, true, 10), SerializeField, ShowIf(nameof(IsAndroid))]
        private string _keystoreFilePath;
        [BoxGroup("Setup - Android Specific", true, true, 10), SerializeField, ShowIf(nameof(IsAndroid))]
        private string _keyaliasName;

        // --------------- OUTPUT --------------- //
        [BoxGroup("Setup - Output", true, true, 20), SerializeField] private string _buildOutputFormat;
        [BoxGroup("Setup - Output", true, true, 20), SerializeField] private string _outputFolder = "Builds/";

        public bool IsAndroid() => _buildTargetPlatform == BuildTarget.Android;

        private int GenerateVersionCode() => 1;

        [Button(ButtonSizes.Large)]
        public void BuildProject()
        {
            // --------------- EDITOR SETTINGS --------------- //
            if (_buildTargetPlatform != BuildTarget.NoTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(_buildTargetGroup, _buildTargetPlatform);
            }

            Assert.IsTrue(_scenesToBuild.Length > 0);
            EditorBuildSettings.scenes = _scenesToBuild.
                                         Select(scene => new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(scene), true)).
                                         ToArray();

            // --------------- PLAYER SETTINGS --------------- //
            PlayerSettings.companyName = _companyName;
            PlayerSettings.productName = _productName;

            if (IsAndroid()) { SetupForAndroid(); }

            if (_useCustomDefines) { PlayerSettings.SetScriptingDefineSymbols(_nameBuildTarget, _customDefines); }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string buildPath =
                Path.Combine(_outputFolder,
                             string.Format(_buildOutputFormat, PlayerSettings.bundleVersion,
                                           PlayerSettings.Android.bundleVersionCode));

            string[] scenePaths = _scenesToBuild.Select(scene => AssetDatabase.GetAssetPath(scene)).ToArray();

            BuildPipeline.BuildPlayer(scenePaths, buildPath, _buildTargetPlatform, _buildOptions);
        }

        private void SetupForAndroid()
        {
            PlayerSettings.applicationIdentifier    = _applicationIdentifier;
            PlayerSettings.Android.minSdkVersion    = _minSdkVersion;
            PlayerSettings.Android.targetSdkVersion = _targetSdkVersion;

            PlayerSettings.bundleVersion             = _bundleVersion;
            PlayerSettings.Android.bundleVersionCode = GenerateVersionCode();

            if (!string.IsNullOrEmpty(_keystoreFilePath))
            {
                PlayerSettings.Android.keystoreName =
                    Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), _DoubleDot), _keystoreFilePath);

                PlayerSettings.Android.keyaliasName = _keyaliasName;

                var passwordFile = PlayerSettings.Android.keystoreName + PasswordSuffix;
                if (File.Exists(passwordFile))
                {
                    PlayerSettings.Android.keystorePass = File.ReadAllText(passwordFile);
                    PlayerSettings.Android.keyaliasPass = File.ReadAllText(passwordFile);
                }
            }

            if (!_androidManifest) { return; }

            var source      = AssetDatabase.GetAssetPath(_androidManifest);
            var destination = _AndroidManifestPath;
            var root        = Path.Combine(Application.dataPath, _DoubleDot);
            source      = Path.GetFullPath(Path.Combine(root, source));
            destination = Path.GetFullPath(Path.Combine(root, destination));
            File.Copy(source, destination, true);
        }
    }
}
#endif
