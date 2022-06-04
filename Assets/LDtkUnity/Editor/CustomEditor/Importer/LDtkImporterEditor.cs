﻿using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace LDtkUnity.Editor
{
    //we are not overriding RenderStaticPreview because it didn't work for scripted importers
    internal abstract class LDtkImporterEditor : ScriptedImporterEditor
    {
        public override bool showImportedObject => false;
        protected override bool useAssetDrawPreview => false;
        //protected override bool ShouldHideOpenButton() => false;

        protected LDtkSectionDependencies SectionDependencies;
        
        public override void OnEnable()
        {
            base.OnEnable();
            SectionDependencies = new LDtkSectionDependencies(serializedObject);
        }

        protected override void Apply()
        {
            base.Apply();
            UpdateDependenciesDrawer();
            
            SectionDependencies.Dispose();
        }

        private void UpdateDependenciesDrawer() //a bit hacky, but gets the job done with little performance issue
        {
            //todo retry against this, it might be better after making these dependencies based on the editor prefs instead
            SectionDependencies.UpdateDependencies();
            EditorApplication.delayCall += () =>
            {
                SectionDependencies.UpdateDependencies();
                EditorApplication.delayCall += SectionDependencies.UpdateDependencies;
            };
        }

        protected bool TryDrawBackupGui<T>(LDtkJsonImporter<T> importer) where T : ScriptableObject, ILDtkJsonFile
        {
            if (!importer.IsBackupFile())
            {
                return false;
            }
            
            const string msg = "This LDtk file is a backup file and as a result, was not imported.\n" +
                               "To import this file, move it to a folder with a name that doesn't contain \"backups\".";

            DrawTextBox(msg, MessageType.Info);
            //AssetDatabase.ForceReserializeAssets();
            return true;

        }
        
        protected static void DrawTextBox(string msg = null, MessageType type = MessageType.Error)
        {
            const string errorContent = "There was a breaking import error; Try reimporting this asset, which might fix it.\n" +
                                        "Check if there are any import errors in the console window, and report to the developer so that it can be addressed.";

            if (msg == null)
            {
                msg = errorContent;
            }

            using (new EditorGUIUtility.IconSizeScope(Vector2.one * 32))
            {
                EditorGUILayout.HelpBox(msg, type);
            }
        }
    }
}