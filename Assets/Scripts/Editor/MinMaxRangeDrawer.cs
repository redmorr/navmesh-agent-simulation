using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer
{
    private const string VECTOR2_PROPERTY_X = "x";
    private const string VECTOR2_PROPERTY_Y = "y";
    private const string SPEARATOR_LABEL = " to ";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            MinMaxRangeAttribute minMaxAttribute = attribute as MinMaxRangeAttribute;

            SerializedProperty minProperty = property.FindPropertyRelative(VECTOR2_PROPERTY_X);
            SerializedProperty maxProperty = property.FindPropertyRelative(VECTOR2_PROPERTY_Y);

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            float min = minProperty.floatValue;
            float max = maxProperty.floatValue;

            Rect minValueBox = new Rect(position.x, position.y, position.width * 0.5f - 11f, position.height);
            Rect separatorBox = new Rect(minValueBox.xMax, position.y, 22f, position.height);
            Rect maxValueBox = new Rect(position.x + position.width - minValueBox.width, position.y, minValueBox.width, position.height);

            min = Mathf.Clamp(EditorGUI.DelayedFloatField(minValueBox, min), minMaxAttribute.min, max);
            EditorGUI.LabelField(separatorBox, SPEARATOR_LABEL);
            max = Mathf.Clamp(EditorGUI.DelayedFloatField(maxValueBox, max), min, minMaxAttribute.max);

            minProperty.floatValue = min;
            maxProperty.floatValue = max;
            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use this attribute only on Vector2 type.");
        }
    }
}
