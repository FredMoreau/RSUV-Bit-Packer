using System;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Linq;
using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    internal class RendererPropertyListView
    {
        private const string emptySelectionMessage = "Select a Renderer Property to edit its settings.";
        static GUIContent headerLabel = new("Renderer Properties", "Add Renderer Properties up to 32 bits.");
        static GUIContent propertyNameFieldLabel = new GUIContent("Name", "The property name, as displayed and used to query its index.");
        static GUIContent propertySettingsFoldoutLabel = new("Property Settings", "");

        ReorderableList list;
        IRendererProperties target;
        SerializedObject serializedObject;
        SerializedProperty rendererPropertiesProp;
        SerializedProperty selectedProperty;

        internal delegate void OnChangeDelegate();
        internal OnChangeDelegate OnChangeCallback;

        int? sum = null;
        int lastItemFittingIndex = -1;

        static List<Type> rendererValueTypes = new();
        static List<string> dropDownLabels = new();
        static List<string> rendererValueTypeNames = new();
        static Dictionary<string, uint> rendererValueLengths = new();
        static Dictionary<string, GUIContent> rendererValueTooltips = new();
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
                rendererValueLengths.Add(type.Name, sizeAttr != null ? sizeAttr.Length : 1);

                var tooltipAttr = type.GetCustomAttributes(typeof(RendererValueTypeTooltipAttribute), false).FirstOrDefault() as RendererValueTypeTooltipAttribute;
                rendererValueTooltips.Add(type.Name, new GUIContent($"{name} Settings:", tooltipAttr?.Tooltip));
            }
        }

        internal RendererPropertyListView(SerializedObject serializedObject, UnityEngine.Object target)
        {
            this.serializedObject = serializedObject;
            this.target = target as IRendererProperties;
            rendererPropertiesProp = serializedObject.FindProperty("rendererProperties");
            CreateList();
        }

        internal void DoLayoutList()
        {
            if (sum != null)
                EditorGUILayout.HelpBox($"{sum} / 32 bits.", sum.Value <= 32 ? MessageType.Info : MessageType.Warning, true);
            list.DoLayoutList();
            if (EditorGUILayout.Foldout(true, propertySettingsFoldoutLabel))
            {
                EditorGUI.indentLevel++;
                if (selectedProperty != null)
                {
                    SerializedProperty name = selectedProperty.FindPropertyRelative(RendererPropertyBase.nameFieldName);
                    SerializedProperty settings = selectedProperty.FindPropertyRelative(RendererProperty<int, uint>.settingsFieldName);
                    EditorGUILayout.DelayedTextField(name, propertyNameFieldLabel);
                    if (settings != null)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(settings, rendererValueTooltips[selectedProperty.managedReferenceValue.GetType().Name]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            UpdateSum();
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(rendererValueTooltips[selectedProperty.managedReferenceValue.GetType().Name].tooltip, MessageType.None);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox(emptySelectionMessage, MessageType.Info, true);
                }
                EditorGUI.indentLevel--;
            }
        }

        void CreateList()
        {
            list = new ReorderableList(serializedObject, rendererPropertiesProp, true, true, true, true);

            list.drawHeaderCallback = DrawHeader;
            list.drawElementCallback = DrawListItems;
            list.drawElementBackgroundCallback = DrawListItemsBackground;
            list.onCanAddCallback = CanAdd;
            list.onAddDropdownCallback = AddDropdown;
            list.onSelectCallback = SelectionChanged;
            list.onChangedCallback = OnChange;

            UpdateSum();

            Undo.undoRedoEvent += (in UndoRedoInfo info) =>
            {
                UpdateSum();
            };
        }

        void SelectionChanged(ReorderableList list)
        {
            selectedProperty = null;
            if (list.count > 0)
                selectedProperty = list.serializedProperty.GetArrayElementAtIndex(list.index);
        }

        void DrawListItemsBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var previousColor = GUI.color;
                if (index > lastItemFittingIndex)
                    GUI.color = Color.red;
                ReorderableList.defaultBehaviours.elementBackground.Draw(rect, isHover: false, isActive, true, isFocused);
                GUI.color = previousColor;
            }
        }

        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty name = element.FindPropertyRelative(RendererPropertyBase.nameFieldName);
            EditorGUI.PropertyField(rect, element, new GUIContent(string.IsNullOrWhiteSpace(name.stringValue) ? "<no name>" : name.stringValue));
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
            EditorGUI.LabelField(rect, headerLabel);
        }

        bool CanAdd(ReorderableList list)
        {
            return true;
        }

        void OnChange(ReorderableList list)
        {
            SelectionChanged(list);
            serializedObject.ApplyModifiedProperties();
            OnChangeCallback?.Invoke();
            UpdateSum();
        }

        void UpdateSum()
        {
            sum = 0;
            lastItemFittingIndex = -1;
            foreach (var prop in target.RendererProperties)
            {
                sum += (int)prop.Length;
                if (sum <= 32)
                    lastItemFittingIndex++;
            }
        }
    }
}
