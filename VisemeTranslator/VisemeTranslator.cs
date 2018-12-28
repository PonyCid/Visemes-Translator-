/**
 * Editor Script that allow you to add all the visemes on a given animation clips
 * set the clip that you want to add the visemes on
 * set the visemes clip that contain all the Japanese Shapes key
 * Click on Do the translation button.
 * The script will add the Japanese visemes and a translation of it on the clip.
 * 
 * @author PonyCid
 * @Date : 27/12/2018
 * @version : 1.0
 */


#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;

namespace VisemeTranslation
{
    public class VisemeTranslation : EditorWindow
    {
        private int columnWidth = 100;
        private int textColumnWidth = 180;
        private string dictionaryFileName = "dictionary.json";
        private TranslationData[] dictionary;

        private AnimationClip clips;
        private AnimationClip visemes;


        [MenuItem("Window/Viseme Translator")]
        static void Init()
        {
            EditorWindow.GetWindow<VisemeTranslation>(false, "VisemeTranslatorEditor");
        }

        private void translation(AnimationClip clip, AnimationClip viseme)
        {
            this.clips = clip;
            this.visemes = viseme;

            if (clips == null)
            {
                Debug.LogError("Clip is empty");
                return;
            }

            if (visemes == null)
            {
                Debug.LogError("Visemes animation is emtpy");
                return;
            }

                loadDictionary();
                if (dictionary == null)
                {
                    Debug.LogError("The dictionary is empty.");
                    return;
                }
                else
                {
                    EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(visemes);
                    foreach (EditorCurveBinding binding in bindings)
                    {
                        AnimationCurve curve = AnimationUtility.GetEditorCurve(visemes, binding);
                        clips.SetCurve(binding.path, binding.type, binding.propertyName, curve);
                    }//End Foreach

                    foreach (EditorCurveBinding binding in bindings)
                    {

                        string newPropertyName = "";
                        string oldPropertyName = "";

                        foreach (TranslationData Translation in dictionary)
                        {
                            if (binding.propertyName == ("blendShape." + Translation.japName))
                            {
                                newPropertyName = ("blendShape." + Translation.newEngName);
                                oldPropertyName = ("blendShape." + Translation.oldEngName);
                            }
                        }

                        if (newPropertyName.Length > 0 && newPropertyName.Length > 0)
                        {
                            AnimationCurve curve = AnimationUtility.GetEditorCurve(visemes, binding);
                            clips.SetCurve(binding.path, binding.type, newPropertyName, curve);

                            if (!newPropertyName.Equals(oldPropertyName))
                            {
                                clips.SetCurve(binding.path, binding.type, oldPropertyName, curve);
                            }
                        }
                    }//End Foreach
                }
            UnityEditorInternal.InternalEditorUtility.RequestScriptReload();

        }

        private void loadDictionary()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, dictionaryFileName);

            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);

                Data LoadedData = JsonUtility.FromJson<Data>(dataAsJson);
                dictionary = LoadedData.dictionary;
            }
            else
            {
                Debug.LogError("Cannot load the Dictionary Data!");

            }
        }//End loadDictionary


        void OnGUI()
        {
            GUILayout.Label("Your Clip:", GUILayout.Width(textColumnWidth));
            this.clips = (AnimationClip)EditorGUILayout.ObjectField(clips, typeof(AnimationClip), true, GUILayout.Width(textColumnWidth));
            GUILayout.Label("Your Viseme", GUILayout.Width(textColumnWidth));
            this.visemes = (AnimationClip)EditorGUILayout.ObjectField(visemes, typeof(AnimationClip), true, GUILayout.Width(textColumnWidth));
            GUILayout.Label("", GUILayout.Width(textColumnWidth));

            if (GUILayout.Button("Do the translation", GUILayout.Width(textColumnWidth)))
            {
                if (clips != null && visemes != null)
                {
                    this.translation(clips, visemes);
                    this.ShowNotification(new GUIContent("Done"));
                }
                else
                {
                    this.ShowNotification(new GUIContent("Incorrect Input"));
                }
            }

        }
    }
}
#endif