using System;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Linq;
using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    public class RendererPropertyListView
    {
        ReorderableList list;
        const float nameFieldWidth = 150f;
        const float labelWidth = 90f;
        const float padding = 16f;

        IRendererProperties target;
        SerializedObject serializedObject;
        SerializedProperty rendererPropertiesProp;

        public delegate void OnChangeDelegate();
        public OnChangeDelegate OnChangeCallback;

        static List<Type> rendererValueTypes = new();
        [InitializeOnLoadMethod]
        static void GetMatchTypes()
        {
            rendererValueTypes.Clear();
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(x => typeof(RendererPropertyBase).IsAssignableFrom(x))).ToList();
            foreach (var type in types)
            {
                if (type == typeof(RendererPropertyBase) || type == typeof(RendererProperty<>))
                    continue;
                rendererValueTypes.Add(type);
            }
        }

        public RendererPropertyListView(SerializedObject serializedObject, UnityEngine.Object target)
        {
            this.serializedObject = serializedObject;
            this.target = target as IRendererProperties;
            rendererPropertiesProp = serializedObject.FindProperty("rendererProperties");
            CreateList();
        }

        public void DoLayoutList()
        {
            list.DoLayoutList();
        }

        void CreateList()
        {
            list = new ReorderableList(serializedObject, rendererPropertiesProp, true, true, true, true);

            list.drawElementCallback = DrawListItems;
            list.drawHeaderCallback = DrawHeader;
            list.onCanAddCallback = CanAdd;
            list.onChangedCallback = OnChange;
            list.onAddDropdownCallback = AddDropdown;
        }

        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
            var nameRect = new Rect(rect.x, rect.y, nameFieldWidth, rect.height);
            var valueRect = new Rect(nameRect.max.x + padding, rect.y, rect.width - nameFieldWidth - padding, rect.height);
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty name = element.FindPropertyRelative("name");
            SerializedProperty value = element.FindPropertyRelative("_value");
            EditorGUI.DelayedTextField(nameRect, name, new GUIContent(""));
            EditorGUI.PropertyField(valueRect, value, new GUIContent(""/*"default value"*/));
            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        void AddDropdown(Rect buttonRect, ReorderableList list)
        {
            List<string> addMenuOptions = new();
            foreach (Type type in rendererValueTypes)
            {
                var nameAttr = type.GetCustomAttributes(typeof(RendererValueTypeNameAttribute), false).FirstOrDefault() as RendererValueTypeNameAttribute;
                if (nameAttr != null)
                    addMenuOptions.Add($"Add {nameAttr.Name}");
                else
                    addMenuOptions.Add($"Add {type.Name}");
            }

            var sum = target.RendererProperties.Sum(p => p.Length);

            var menu = new GenericMenu();
            for (int optionIndex = 0; optionIndex < addMenuOptions.Count; optionIndex++)
            {
                string optionText = addMenuOptions[optionIndex];

                int index = optionIndex;
                var lengthAttr = rendererValueTypes[index].GetCustomAttributes(typeof(RendererValueTypeLengthAttribute), false).FirstOrDefault() as RendererValueTypeLengthAttribute;
                bool optionEnabled = lengthAttr == null || sum + lengthAttr.Length <= 32;

                if (optionEnabled)
                {
                    menu.AddItem(new GUIContent(optionText), false, () =>
                    {
                        var t = rendererValueTypes[index];
                        var o = Activator.CreateInstance(t) as RendererPropertyBase;
                        target.Add(o);
                        //target.UpdateProperties();
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(optionText));
                }
            }
            menu.ShowAsContext();
        }

        void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, new GUIContent("Renderer Properties", "Add Renderer Properties up to 32 bits."));
        }

        bool CanAdd(ReorderableList list)
        {
            return true;
        }

        void OnChange(ReorderableList list)
        {
            serializedObject.ApplyModifiedProperties();
            //target.UpdateProperties();
            OnChangeCallback?.Invoke();
        }
    }
}
