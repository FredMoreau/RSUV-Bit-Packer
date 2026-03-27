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

        int? sum = null;

        static List<Type> rendererValueTypes = new();
        static List<string> dropDownLabels = new();
        static List<string> rendererValueTypeNames = new();
        static Dictionary<string, uint> rendererValueLengths = new();
        static Dictionary<string, string> rendererValueTooltips = new(); // TODO: add a help icon to display tooltip if available
        [InitializeOnLoadMethod]
        static void ReflectRendererPropertyTypesAndStoreMenuItems()
        {
            dropDownLabels.Clear();
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(x => typeof(RendererPropertyBase).IsAssignableFrom(x))).ToList();
            foreach (var type in types)
            {
                if (type == typeof(RendererPropertyBase) || type == typeof(RendererProperty<>) || type == typeof(RendererProperty<,>))
                    continue;

                rendererValueTypes.Add(type);
                rendererValueTypeNames.Add(type.Name);

                var nameAttr = type.GetCustomAttributes(typeof(RendererValueTypeNameAttribute), false).FirstOrDefault() as RendererValueTypeNameAttribute;
                var name = nameAttr != null ? nameAttr.Name : type.Name;
                dropDownLabels.Add($"Add {name}");

                var sizeAttr = type.GetCustomAttributes(typeof(RendererValueTypeLengthAttribute), false).FirstOrDefault() as RendererValueTypeLengthAttribute;
                rendererValueLengths.Add(type.Name, sizeAttr != null ? sizeAttr.Length : 0);

                var tooltipAttr = type.GetCustomAttributes(typeof(RendererValueTypeTooltipAttribute), false).FirstOrDefault() as RendererValueTypeTooltipAttribute;
                if (tooltipAttr != null)
                    rendererValueTooltips.Add(type.Name, tooltipAttr.Tooltip);
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
            if (sum != null)
                EditorGUILayout.HelpBox($"{sum} / 32 bits.", sum.Value <= 32 ? MessageType.Info : MessageType.Warning, true);
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

            UpdateSum();

            Undo.undoRedoEvent += (in UndoRedoInfo info) =>
            {
                UpdateSum();
            };
        }

        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
            var nameRect = new Rect(rect.x, rect.y, nameFieldWidth, rect.height);
            var valueRect = new Rect(nameRect.max.x + padding, rect.y, rect.width - nameFieldWidth - padding, rect.height);
            
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty name = element.FindPropertyRelative(RendererPropertyBase.nameFieldName);
            EditorGUI.DelayedTextField(nameRect, name, new GUIContent(""));

            SerializedProperty value = element.FindPropertyRelative(RendererProperty<bool>.valueFieldName);

            SerializedProperty settings = element.FindPropertyRelative(RendererProperty<int, uint>.settingsFieldName);
            if (settings == null)
            {
                EditorGUI.PropertyField(valueRect, value, new GUIContent(""));
            }
            else
            {
                EditorGUIUtility.labelWidth = 24;
                var vRect = new Rect(valueRect.x, valueRect.y, valueRect.width * .5f, valueRect.height);
                var sRect = new Rect(vRect.max.x, valueRect.y, valueRect.width * .5f, valueRect.height);
                EditorGUI.PropertyField(vRect, value, new GUIContent("v"));
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(sRect, settings, new GUIContent("s"));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    UpdateSum();
                }
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        void AddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();
            for (int optionIndex = 0; optionIndex < dropDownLabels.Count; optionIndex++)
            {
                string optionText = dropDownLabels[optionIndex];
                int index = optionIndex;
                bool optionEnabled = sum + rendererValueLengths[rendererValueTypeNames[index]] <= 32;

                if (optionEnabled)
                {
                    menu.AddItem(new GUIContent(optionText), false, () =>
                    {
                        Undo.RecordObject(target as UnityEngine.Object, optionText);
                        var t = rendererValueTypes[index];
                        var o = Activator.CreateInstance(t) as RendererPropertyBase;
                        target.Add(o);
                        UpdateSum();
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
            OnChangeCallback?.Invoke();
            UpdateSum();
        }

        void UpdateSum()
        {
            sum = (int)target.RendererProperties.Sum(p => p.Length);
        }
    }
}
