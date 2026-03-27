//using UnityEngine;
//using UnityEngine.RSUVBitPacker;

//namespace UnityEditor.RSUVBitPacker
//{
//    [CustomPropertyDrawer(typeof(RendererSpaceProperty))]
//    public class RendererSpacePropertyDrawer : PropertyDrawer
//    {
//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            var nameProp = property.FindPropertyRelative(RendererPropertyBase.nameFieldName);
//            var labelStr = string.IsNullOrWhiteSpace(nameProp.stringValue) ? "<no name>" : nameProp.stringValue;
//        }
//    }
//}
