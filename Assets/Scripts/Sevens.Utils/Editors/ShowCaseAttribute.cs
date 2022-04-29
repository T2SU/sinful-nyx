// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Sevens.Utils.Editors
{
    public class ShowCaseAttribute : PropertyAttribute
    {
    }

    [CustomPropertyDrawer(typeof(ShowCaseAttribute))]
    public class ShowCaseDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string val = property.propertyType switch
            {
                SerializedPropertyType.Integer => property.intValue.ToString(),
                SerializedPropertyType.Boolean => property.boolValue.ToString(),
                SerializedPropertyType.Float => property.floatValue.ToString("0.00000"),
                SerializedPropertyType.String => property.stringValue,
                SerializedPropertyType.Enum => property.enumDisplayNames[property.enumValueIndex],
                SerializedPropertyType.ObjectReference => property.objectReferenceValue.name,
                _ => $"(not supported)",
            };
            EditorGUI.LabelField(position, label.text, val);
        }
    }
}
#endif