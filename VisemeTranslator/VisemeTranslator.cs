/**
 * Editor Script that allow you to add all the visemes on a given animation clips
 * set the clip that you want to add the visemes on
 * set the visemes clip that contain all the Japanese Shapes key
 * Click on Do the translation button.
 * The script will add the Japanese visemes and a translation of it on the clip.
 * 
 * @author PonyCid
 * @Date : 27/12/2018
 * @updated : 07/04/2019
 * @version : 1.1
 */


#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;
using System;

namespace VisemeTranslation
{
    public class VisemeTranslation : EditorWindow
    {
        private int windowSize = 180;
        private int columnWidth = 100;
        private int textColumnWidth = 180;
        private string dictionaryFileName = "dictionary.json";
        private TranslationData[] dictionary;

        private bool vrChatViseme = false;
		private string[] jpVisemes =  new string[6]{"blendShape.あ", "blendShape.い", "blendShape.う", "blendShape.え", "blendShape.お", "blendShape.ん"};

        private AnimationClip clips;
        private AnimationClip visemes;

        [MenuItem("Window/Viseme Translator")]
        static void Init()
        {
            EditorWindow.GetWindow<VisemeTranslation>(false, "VisemeTranslatorEditor");
        }

        private void translation()
        {
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
                    string newPropertyName = "";
                    string oldPropertyName = "";
                   
				   if(!vrChatViseme || (vrChatViseme && Array.IndexOf(jpVisemes, binding.propertyName) == -1)){
					   
					    AnimationCurve curve = AnimationUtility.GetEditorCurve(visemes, binding);
						clips.SetCurve(binding.path, binding.type, binding.propertyName, curve);

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
							clips.SetCurve(binding.path, binding.type, newPropertyName, curve);

							if (!newPropertyName.Equals(oldPropertyName))
							{
								clips.SetCurve(binding.path, binding.type, oldPropertyName, curve);
							}
						}
				   }
                   

                }//End Foreach
            }
            UnityEditorInternal.InternalEditorUtility.RequestScriptReload();
        }
		
		
		private void vrChatVisemes(){
			EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(visemes);
            foreach (EditorCurveBinding binding in bindings)
            {
				string vrcPropertyName = "";
				
				AnimationCurve curve = AnimationUtility.GetEditorCurve(visemes, binding);
				switch (binding.propertyName)
                {
					case "blendShape.あ"://aa
						vrcPropertyName = "blendShape.vrc.v_aa";
                        break;
                    case "blendShape.い"://ih
                        vrcPropertyName = "blendShape.vrc.v_ih";
                        break;
                    case "blendShape.う"://ou
                        vrcPropertyName = "blendShape.vrc.v_ou";
                        break;
                    case "blendShape.え"://e
                        vrcPropertyName = "blendShape.vrc.v_e";
                        break;
                    case "blendShape.お"://oh
                        vrcPropertyName = "blendShape.vrc.v_oh";
                        break;
                    case "blendShape.ん"://nn
                        vrcPropertyName = "blendShape.vrc.v_nn";
                        break;
                }

                if(vrcPropertyName.Length > 0)
                {
                    clips.SetCurve(binding.path, binding.type, vrcPropertyName, curve);
                }
			}
		}//EndvrChatVisemes

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
            GUILayout.BeginArea(new Rect(30, 10, windowSize, windowSize));
            GUILayout.Label("Your Clip:", GUILayout.Width(textColumnWidth));
            this.clips = (AnimationClip)EditorGUILayout.ObjectField(clips, typeof(AnimationClip), true, GUILayout.Width(textColumnWidth));
            GUILayout.Label("Visemes File", GUILayout.Width(textColumnWidth));
            this.visemes = (AnimationClip)EditorGUILayout.ObjectField(visemes, typeof(AnimationClip), true, GUILayout.Width(textColumnWidth));

            GUILayout.Label("", GUILayout.Width(textColumnWidth));
            vrChatViseme = GUILayout.Toggle(vrChatViseme, "Create VrChat Visemes ?", GUILayout.Width(textColumnWidth));
            GUILayout.Label("", GUILayout.Width(textColumnWidth));

			if(this.clips != null && this.visemes != null){
			   if (GUILayout.Button("Do the translation", GUILayout.Width(textColumnWidth)))
				{
					if (clips != null && visemes != null)
					{
						translation();
						
						if (vrChatViseme)
						{
							vrChatVisemes();
						}
						ShowNotification(new GUIContent("Done") );
						this.clips = null; 
						this.visemes = null;
					}
					else
					{
						ShowNotification(new GUIContent("Incorrect Input"));
					}
				}
			}
         
            GUILayout.EndArea();
        }
    }
}
#endif
