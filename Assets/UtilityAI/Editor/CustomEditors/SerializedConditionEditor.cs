using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using EGL = UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.CustomEditors
{
    using NUnit.Framework.Internal;
    using System.Collections.Generic;
    using Tools;

    [CustomEditor(typeof(SerializedCondition))]
    public class SerializedConditionEditor : Editor
    {
        static SerializedConditionEditor()
        {
            ConstantTypes.UnionWith(StirgeTypeHelper.NumericTypes);
        }
        
        #region Valid Type Collections
        public static readonly HashSet<Type> ConstantTypes = new HashSet<Type>
        {
            typeof(Boolean), typeof(String), typeof(Vector2), typeof(Vector3), typeof(Color)
        };
        #endregion

        #region Property Names
        private const string OperationPropertyName = "m_operation";
        private const string FirstConstantPropertyName = "m_firstConstantObject";
        private const string SecondConstantPropertyName = "m_secondConstantObject";
        private const string FirstReferencePropertyName = "m_firstReferenceObject";
        private const string SecondReferencePropertyName = "m_secondReferenceObject";
        private const string IsValidPropertyName = "m_isValid";
        #endregion

        #region Labels
        private static readonly GUIContent s_firstObjectLabel = new("First Object");
        private static readonly GUIContent s_secondObjectLabel = new("Second Object");

        private static GUIStyle s_middleStyle;
        private static bool s_middleStyleInitialised = false;
        #endregion
         
        #region Properties
        private SerializedProperty m_operationProperty;
        private SerializedProperty m_firstConstantProperty;
        private SerializedProperty m_secondConstantProperty;
        private SerializedProperty m_firstReferenceProperty;
        private SerializedProperty m_secondReferenceProperty;
        private SerializedProperty m_isValidProperty;
        #endregion

        private SerializedConditionObject m_firstObject;
        private SerializedConditionObject m_secondObject;

        private void OnEnable()
        {
            m_operationProperty = serializedObject.FindProperty(OperationPropertyName);
            m_firstConstantProperty = serializedObject.FindProperty(FirstConstantPropertyName);
            m_secondConstantProperty = serializedObject.FindProperty(SecondConstantPropertyName);
            m_firstReferenceProperty = serializedObject.FindProperty(FirstReferencePropertyName);
            m_secondReferenceProperty = serializedObject.FindProperty(SecondReferencePropertyName);
            m_isValidProperty = serializedObject.FindProperty(IsValidPropertyName);

            // init objects
            m_firstObject ??= InitialiseObject(m_firstConstantProperty, m_firstReferenceProperty);
            m_secondObject ??= InitialiseObject(m_secondConstantProperty, m_firstReferenceProperty);
        }

        public override void OnInspectorGUI()
        {
            if (!s_middleStyleInitialised)
            {
                s_middleStyleInitialised = true;
                s_middleStyle = new(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            }
            
            if (GUILayout.Button("Re-Initialise"))
            {
                OnEnable();
            }

            EGL.Separator();
            EditorGUI.BeginChangeCheck();

            // Draw Operation Enum property field
            EGL.BeginHorizontal();
            EGL.LabelField("Operation", EditorStyles.boldLabel);
            EGL.PropertyField(m_operationProperty, GUIContent.none);
            EGL.EndHorizontal();

            // Draw field for first object
            EGL.LabelField(s_firstObjectLabel, EditorStyles.boldLabel);
            DrawObject(m_firstObject);

            // Draw field for second object
            EGL.LabelField(s_secondObjectLabel, EditorStyles.boldLabel);
            DrawObject(m_secondObject);

            // Check for changes
            ObjectChangeCheck(m_firstConstantProperty, m_firstReferenceProperty, m_firstObject);
            ObjectChangeCheck(m_secondConstantProperty, m_secondReferenceProperty, m_secondObject);

            EGL.Space();

            // Draw preview
            EGL.LabelField("Preview", s_middleStyle);
            EGL.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawObjectPreview(m_firstObject);
            Operation operation = (Operation)m_operationProperty.intValue;
            string operationString = operation switch
            {
                Operation.Equal => "==",
                Operation.NotEqual => "!=",
                Operation.LessThan => "<",
                Operation.GreaterThan => ">",
                Operation.LessThanOrEqual => "<=",
                Operation.GreaterThanOrEqual => ">=",
                _ => "?",
            };
            EGL.LabelField(operationString, s_middleStyle);
            DrawObjectPreview(m_secondObject);
            EGL.EndHorizontal();

            // Display validity message
            bool isValid = true;
            bool bothNumeric = StirgeTypeHelper.IsNumericType(m_firstObject.valueType) && StirgeTypeHelper.IsNumericType(m_secondObject.valueType);
            switch (operation)
            {
                case Operation.Equal:
                case Operation.NotEqual:
                    if (!bothNumeric && m_firstObject.valueType != m_secondObject.valueType &&
                        (m_firstObject.isNull && !m_secondObject.isNull && !m_secondObject.valueType.IsClass ||
                        m_secondObject.isNull && !m_firstObject.isNull && !m_firstObject.valueType.IsClass))
                    {
                        EGL.HelpBox("Condition is invalid as these types are not Equatable.", MessageType.Error);
                        isValid = false;
                    }
                    break;
                default:
                    if (!bothNumeric)
                    {
                        EGL.HelpBox("Condition is invalid as these types are not Comparable.", MessageType.Error);
                        isValid = false;
                    }
                    break;
            }

            // Apply is valid
            if (isValid != m_isValidProperty.boolValue)
            {
                m_isValidProperty.boolValue = isValid;
            }

            // Apply changes
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private SerializedConditionObject InitialiseObject(SerializedProperty constantProperty, SerializedProperty referenceProperty)
        {
            SerializedConditionObject obj = new() { constantValue = constantProperty.managedReferenceValue, referenceValue = referenceProperty.objectReferenceValue };
            if (obj.constantValue != null)
            {
                obj.isConstantValue = true;
                obj.valueType = obj.constantValue.GetType();
            }
            else if (obj.referenceValue != null)
            {
                obj.isConstantValue = false;
                obj.valueType = obj.referenceValue.GetType();
            }
            return obj;
        }

        private void DrawObject(SerializedConditionObject obj)
        {
            // Is Constant Value toggle
            obj.isConstantValue = EGL.Toggle(new GUIContent("Constant Value"), obj.isConstantValue);

            // if set to Constant value
            if (obj.isConstantValue)
            {
                // Type field
                if (GUILayout.Button("Select Type"))
                {
                    SelectType(obj);
                }
                if (obj.valueType != null)
                {
                    // ensure constantValue is never null going into the Switch block
                    obj.constantValue ??= string.Empty;

                    GUIContent label = new(obj.valueType.Name + " Value");

                    switch (obj.valueType.Name)
                    {
                        case nameof(UInt16):
                        case nameof(UInt32):
                        case nameof(Int32):
                        case nameof(Int16):
                            int int32Value;
                            if (obj.constantValue is string int32StringValue)
                            {
                                if (!int.TryParse(int32StringValue, out int32Value))
                                {
                                    int32Value = 0;
                                }
                            }
                            else
                            {
                                TryCast(obj.constantValue, out int32Value);
                            }
                            obj.constantValue = EGL.IntField(label, int32Value);
                            break;
                        case nameof(UInt64):
                        case nameof(Int64):
                            long longValue;
                            if (obj.constantValue is string longStringValue)
                            {
                                if (!long.TryParse(longStringValue, out longValue))
                                {
                                    longValue = 0;
                                }
                            }
                            else
                            {
                                TryCast(obj.constantValue, out longValue);
                            }
                            obj.constantValue = EGL.LongField(label, longValue);
                            break;
                        case nameof(Single):
                            float floatValue;
                            if (obj.constantValue is string floatStringValue)
                            {
                                if (!float.TryParse(floatStringValue, out floatValue))
                                {
                                    floatValue = 0;
                                }
                            }
                            else
                            {
                                TryCast(obj.constantValue, out floatValue);
                            }
                            obj.constantValue = EGL.FloatField(label, floatValue);
                            break;
                        case nameof(Decimal):
                        case nameof(Double):
                            double doubleValue;
                            if (obj.constantValue is string doubleStringValue)
                            {
                                if (!double.TryParse(doubleStringValue, out doubleValue))
                                {
                                    doubleValue = 0f;
                                }
                            }
                            else
                            {
                                TryCast(obj.constantValue, out doubleValue);
                            }
                            obj.constantValue = EGL.DoubleField(label, doubleValue);
                            break;
                        case nameof(Boolean):
                            TryCast(obj.constantValue, out bool boolValue);
                            obj.constantValue = EGL.Toggle(label, boolValue);
                            break;
                        case nameof(String):
                            string stringValue = obj.constantValue.ToString();
                            if (stringValue == null)
                            {
                                if (!TryCast(obj.constantValue, out stringValue))
                                    stringValue = string.Empty;
                            }
                            obj.constantValue = EGL.TextField(label, stringValue);
                            break;
                        case nameof(Vector2):
                            Vector2 vector2Value;
                            if (obj.constantValue is Vector3 vector2To3Value)
                            {
                                vector2Value = vector2To3Value;
                            }
                            else
                            {
                                TryCast(obj.constantValue, out vector2Value);
                            }
                            obj.constantValue = EGL.Vector2Field(label, vector2Value);
                            break;
                        case nameof(Vector3):
                            Vector3 vector3Value;
                            if (obj.constantValue is Vector2 vector3To2Value)
                            {
                                vector3Value = vector3To2Value;
                            }
                            else
                            {
                                TryCast(obj.constantValue, out vector3Value);
                            }
                            obj.constantValue = EGL.Vector3Field(label, vector3Value);
                            break;
                        case nameof(Color):
                            Color colorValue;
                            if (obj.constantValue is Vector2 vector2ToColorValue)
                            {
                                colorValue = new(vector2ToColorValue.x, vector2ToColorValue.y, 0);
                            }
                            else if (obj.constantValue is Vector3 vector3ToColorValue)
                            {
                                colorValue = new(vector3ToColorValue.x, vector3ToColorValue.y, vector3ToColorValue.z);
                            }
                            else
                            {
                                TryCast(obj.constantValue, out colorValue);
                            }
                            obj.constantValue = EGL.ColorField(label, colorValue);
                            break;
                        default:
                            // check if type is an Enum type
                            if (obj.valueType.IsEnum)
                            {
                                // Ensure value contains an enum value. If the cast returns null, create a default value
                                Enum enumValue = (Enum)obj.constantValue;
                                enumValue ??= Activator.CreateInstance(obj.valueType) as Enum;

                                // check if Enum type is Flags type
                                if (obj.valueType.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                                {
                                    obj.constantValue = EGL.EnumFlagsField(label, enumValue);
                                }
                                // if standard Enum
                                else
                                {
                                    obj.constantValue = EGL.EnumPopup(label, enumValue);
                                }
                            }
                            // If type is not valid somehow
                            else
                            {
                                EGL.HelpBox("Provided type is NOT valid for a Constant value.", MessageType.Warning);
                            }
                            break;
                    }
                }
            }
            // if set to Reference value
            else
            {
                string labelText = obj.valueType != null ? obj.valueType.Name : "Object";
                obj.referenceValue = EGL.ObjectField(new GUIContent(labelText + " Value"), obj.referenceValue, typeof(Object), false);
                if (obj.referenceValue != null)
                {
                    obj.valueType = obj.referenceValue.GetType();
                }
            }
        }

        private void ObjectChangeCheck(SerializedProperty constantProperty, SerializedProperty referenceProperty, SerializedConditionObject obj)
        {
            if (obj.changed)
            {
                obj.changed = false;
                if (obj.isConstantValue)
                {
                    constantProperty.managedReferenceValue = obj.constantValue;
                    referenceProperty.objectReferenceValue = null;
                }
                else
                {
                    referenceProperty.objectReferenceValue = obj.referenceValue;
                    constantProperty.managedReferenceValue = null;
                }
            }
        }

        private void DrawObjectPreview(SerializedConditionObject obj)
        {
            EditorGUI.BeginDisabledGroup(true);
            if (obj.isConstantValue)
            {
                EGL.TextField(obj.constantValue != null ? obj.constantValue.ToString() : "null", GUILayout.ExpandWidth(true));
            }
            else
            {
                EGL.ObjectField(obj.referenceValue, typeof(Object), false, GUILayout.ExpandWidth(true));
            }
            EditorGUI.EndDisabledGroup();
        }

        private void SelectType(SerializedConditionObject obj)
        {
            var genericMenu = new GenericMenu();
            IReadOnlyList<Type> validTypes = ConstantTypes.ToList();

            for (int i = 0, count = validTypes.Count; i < count; i++)
            {
                Type type = validTypes[i];
                string uiName = TypeHelper.GetDisplayName(type);
                genericMenu.AddItem(new GUIContent(uiName), false, () =>
                {
                    obj.valueType = type;
                });
            }

            genericMenu.ShowAsContext();
        }

        private bool TryCast<T>(object toCast, out T value)
        {
            try
            {
                value = (T)toCast;
                return true;
            }
            catch (InvalidCastException e)
            {
                value = default;
                return false;
            }
        }
    }
}
