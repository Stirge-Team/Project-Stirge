using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

using EGL = UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.CustomEditors
{
    using Combat;
    using GenericBlackboard;
    using Tools;

    [CustomEditor(typeof(SerializedCondition))]
    public class SerializedConditionEditor : Editor
    {
        #region Static Setup
        static SerializedConditionEditor()
        {
            ConstantTypes.UnionWith(StirgeTypeHelper.NumericTypes);
        }
        public static readonly HashSet<Type> ConstantTypes = new HashSet<Type>
        {
            typeof(Boolean), typeof(String), typeof(Vector2), typeof(Vector3), typeof(Color), typeof(void)
        };
        #endregion

        #region Properties
        private const string s_scorableTypePropertyName = "m_scorableType";
        private const string s_operationPropertyName = "m_operation";
        private const string s_firstValueTypePropertyName = "m_firstValueType";
        private const string s_secondValueTypePropertyName = "m_secondValueType";
        private const string s_firstConstantPropertyName = "m_firstConstantObject";
        private const string s_secondConstantPropertyName = "m_secondConstantObject";
        private const string s_firstReferencePropertyName = "m_firstReferenceObject";
        private const string s_secondReferencePropertyName = "m_secondReferenceObject";
        private const string s_firstPropertyPropertyName = "m_firstPropertyName";
        private const string s_secondPropertyPropertyName = "m_secondPropertyName";
        private const string s_firstPropertyTargetPropertyName = "m_firstPropertyTarget";
        private const string s_secondPropertyTargetPropertyName = "m_secondPropertyTarget";
        private const string s_firstTypePropertyName = "m_firstTypeAssemblyQualifiedName";
        private const string s_secondTypePropertyName = "m_secondTypeAssemblyQualifiedName";
        private const string s_isValidPropertyName = "m_isValid";

        private SerializedScorableType m_scorableType;
        private SerializedProperty m_operationProperty;
        private SerializedProperty m_firstValueTypeProperty;
        private SerializedProperty m_secondValueTypeProperty;
        private SerializedProperty m_firstConstantProperty;
        private SerializedProperty m_secondConstantProperty;
        private SerializedProperty m_firstReferenceProperty;
        private SerializedProperty m_secondReferenceProperty;
        private SerializedProperty m_firstPropertyNameProperty;
        private SerializedProperty m_secondPropertyNameProperty;
        private SerializedProperty m_firstPropertyTargetProperty;
        private SerializedProperty m_secondPropertyTargetProperty;
        private SerializedProperty m_firstTypeProperty;
        private SerializedProperty m_secondTypeProperty;
        private SerializedProperty m_isValidProperty;
        #endregion

        #region Labels
        private static readonly GUIContent s_firstObjectLabel = new("First Object");
        private static readonly GUIContent s_secondObjectLabel = new("Second Object");

        private static GUIStyle s_middleStyle;
        private static bool s_middleStyleInitialised = false;
        #endregion

        private SerializedConditionObject m_firstObject;
        private SerializedConditionObject m_secondObject;

        private void OnEnable()
        {
            try
            {
                m_scorableType = (SerializedScorableType)serializedObject.FindProperty(s_scorableTypePropertyName).intValue;

                // init objects
                m_operationProperty = serializedObject.FindProperty(s_operationPropertyName);
                m_firstValueTypeProperty = serializedObject.FindProperty(s_firstValueTypePropertyName);
                m_secondValueTypeProperty = serializedObject.FindProperty(s_secondValueTypePropertyName);
                m_firstConstantProperty = serializedObject.FindProperty(s_firstConstantPropertyName);
                m_secondConstantProperty = serializedObject.FindProperty(s_secondConstantPropertyName);
                m_firstReferenceProperty = serializedObject.FindProperty(s_firstReferencePropertyName);
                m_secondReferenceProperty = serializedObject.FindProperty(s_secondReferencePropertyName);
                m_firstPropertyNameProperty = serializedObject.FindProperty(s_firstPropertyPropertyName);
                m_secondPropertyNameProperty = serializedObject.FindProperty(s_secondPropertyPropertyName);
                m_firstPropertyTargetProperty = serializedObject.FindProperty(s_firstPropertyTargetPropertyName);
                m_secondPropertyTargetProperty = serializedObject.FindProperty(s_secondPropertyTargetPropertyName);
                m_firstTypeProperty = serializedObject.FindProperty(s_firstTypePropertyName);
                m_secondTypeProperty = serializedObject.FindProperty(s_secondTypePropertyName);
                m_isValidProperty = serializedObject.FindProperty(s_isValidPropertyName);

                m_firstObject ??= InitialiseObject(m_firstValueTypeProperty, m_firstConstantProperty, m_firstReferenceProperty, m_firstPropertyNameProperty, m_firstPropertyTargetProperty, m_firstTypeProperty);
                m_secondObject ??= InitialiseObject(m_secondValueTypeProperty, m_secondConstantProperty, m_secondReferenceProperty, m_secondPropertyNameProperty, m_secondPropertyTargetProperty, m_secondTypeProperty);
            }
            catch
            {
                // this try-catch block stops a bunch of errors from being thrown on Script re-compilation.
                // Post from PigletPants explains it but it doesn't appear to cause any functional errors sooo ignore it is.
                // https://discussions.unity.com/t/im-getting-now-error-serializedobjectnotcreatableexception-object-at-index-0-is-null-how-to-fix/788961/3
            }
        }

        public override void OnInspectorGUI()
        {
            if (!s_middleStyleInitialised)
            {
                s_middleStyleInitialised = true;
                s_middleStyle = new(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
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
            DrawObject(ref m_firstObject);

            // Draw field for second object
            EGL.LabelField(s_secondObjectLabel, EditorStyles.boldLabel);
            DrawObject(ref m_secondObject);

            EGL.Space();

            // Draw preview expression
            DrawPreview();
            // Check validity of Condition and display any necessary warnings
            CheckValidity();

            EGL.Separator();

            // Apply changes
            if (ObjectChangeCheck(m_firstObject, ref m_firstValueTypeProperty, ref m_firstConstantProperty, ref m_firstReferenceProperty, ref m_firstPropertyNameProperty, ref m_firstPropertyTargetProperty, ref m_firstTypeProperty)
             || ObjectChangeCheck(m_secondObject, ref m_secondValueTypeProperty, ref m_secondConstantProperty, ref m_secondReferenceProperty, ref m_secondPropertyNameProperty, ref m_secondPropertyTargetProperty, ref m_secondTypeProperty)
             || EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private SerializedConditionObject InitialiseObject(SerializedProperty valueTypeProperty, SerializedProperty constantProperty, SerializedProperty referenceProperty, SerializedProperty propertyNameProperty, SerializedProperty propertyTargetProperty, SerializedProperty typeProperty)
        {
            SerializedConditionObject obj = new()
            {
                valueType = (ConditionValueType)valueTypeProperty.intValue,
                constantValue = constantProperty.managedReferenceValue,
                referenceValue = referenceProperty.objectReferenceValue,
                propertyValue = (BlackboardPropertyName)propertyNameProperty.boxedValue,
                propertyTarget = (ConditionPropertyTarget)propertyTargetProperty.intValue,
            };

            obj.Init(typeProperty.stringValue);

            return obj;
        }

        /// <summary>
        /// Returns if the Clear Data button was pressed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private void DrawObject(ref SerializedConditionObject obj)
        {
            EGL.BeginHorizontal();

            // ValueType enum field
            obj.valueType = (ConditionValueType)EGL.EnumPopup(new GUIContent("Value Type"), obj.valueType);

            switch (obj.valueType)
            {
                case ConditionValueType.Constant:
                    // Type field
                    if (GUILayout.Button("Select Type"))
                    {
                        SelectConstantType(obj);
                    }
            EGL.EndHorizontal();
                    if (obj.type != null)
                    {
                        // ensure constantValue is never null going into the Switch block
                        obj.constantValue ??= string.Empty;

                        GUIContent label = new(obj.type.Name + " Value");

                        // Draw property field dependent on selected Type
                        switch (obj.type.Name)
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
                                    colorValue = new(vector2ToColorValue.x, vector2ToColorValue.y, 0, 1);
                                }
                                else if (obj.constantValue is Vector3 vector3ToColorValue)
                                {
                                    colorValue = new(vector3ToColorValue.x, vector3ToColorValue.y, vector3ToColorValue.z, 1);
                                }
                                else
                                {
                                    if (!TryCast(obj.constantValue, out colorValue))
                                    {
                                        colorValue = new(0, 0, 0, 1);
                                    }
                                }
                                obj.constantValue = EGL.ColorField(label, colorValue);
                                break;
                            case nameof(Byte):
                                byte byteValue;
                                if (obj.constantValue is sbyte sByteToByteValue)
                                {
                                    byteValue = (byte)sByteToByteValue;
                                }
                                else
                                {
                                    if (!TryCast(obj.constantValue, out byteValue))
                                    {
                                        byteValue = new();
                                    }
                                }
                                TryCast(EGL.IntField(label, byteValue), out byteValue);
                                obj.constantValue = byteValue;
                                break;
                            case nameof(SByte):
                                sbyte sByteValue;
                                if (obj.constantValue is byte byteToSByteValue)
                                {
                                    sByteValue = (sbyte)byteToSByteValue;
                                }
                                else
                                {
                                    if (!TryCast(obj.constantValue, out sByteValue))
                                    {
                                        sByteValue = new();
                                    }
                                }
                                TryCast(EGL.IntField(label, sByteValue), out sByteValue);
                                obj.constantValue = sByteValue;
                                break;
                            case var n when n == typeof(void).Name:
                                EGL.LabelField("null", EditorStyles.textField);
                                obj.constantValue = null;
                                break;
                            default:
                                // check if type is an Enum type
                                if (obj.type.IsEnum)
                                {
                                    // Ensure value contains an enum value. If the cast returns null, create a default value
                                    Enum enumValue = (Enum)obj.constantValue;
                                    enumValue ??= Activator.CreateInstance(obj.type) as Enum;

                                    // check if Enum type is Flags type
                                    if (obj.type.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
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
                    break;
                case ConditionValueType.Reference:
                    EditorGUI.BeginDisabledGroup(true);
                    EGL.LabelField(obj.type != null ? obj.type.Name : "null", EditorStyles.textField, GUILayout.MaxWidth(140f));
                    EditorGUI.EndDisabledGroup();

            EGL.EndHorizontal();

                    string labelText = obj.type != null ? obj.type.Name : "Object";
                    obj.referenceValue = EGL.ObjectField(new GUIContent(labelText + " Value"), obj.referenceValue, typeof(Object), false);
                    if (obj.referenceValue != null)
                    {
                        obj.type = obj.referenceValue.GetType();
                    }
                    break;
                case ConditionValueType.Property:
                    // Property field
                    if (GUILayout.Button("Select Property"))
                    {
                        switch (obj.propertyTarget)
                        {
                            case ConditionPropertyTarget.User:
                                SelectProperty<UtilityEnemy>(ref obj);
                                break;
                            case ConditionPropertyTarget.Target:
                                SelectProperty<CombatEntity>(ref obj);
                                break;
                            case ConditionPropertyTarget.Scorable:
                                switch (m_scorableType)
                                {
                                    case SerializedScorableType.Action:
                                        SelectProperty<Action>(ref obj);
                                        break;
                                    case SerializedScorableType.MovementGoal:
                                        SelectProperty<MovementGoal>(ref obj);
                                        break;
                                    case SerializedScorableType.Status:
                                        SelectProperty<Status>(ref obj);
                                        break;
                                }
                                break;
                        }
                    }
            EGL.EndHorizontal();

                    EGL.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EGL.LabelField(new GUIContent("Property Target", "Where the property value should be retrieved from."), GUILayout.MaxWidth(110f));
                    ConditionPropertyTarget newPropertyTarget = (ConditionPropertyTarget)EGL.EnumPopup(GUIContent.none, obj.propertyTarget, GUILayout.MinWidth(110f));
                    if (newPropertyTarget != obj.propertyTarget)
                    {
                        // Clear the Property Value if the new Property Target is different
                        obj.propertyValue = new();
                        obj.propertyTarget = newPropertyTarget;
                    }

                    EGL.LabelField(obj.propertyValue.IsNull ? "null" : obj.propertyValue.Name + " : " + GetUIName(obj.type), EditorStyles.textField, GUILayout.MinWidth(Screen.width - 300f));
                    EGL.EndHorizontal();
                    break;
            }
        }

        private bool ObjectChangeCheck(SerializedConditionObject obj, ref SerializedProperty valueTypeProperty, ref SerializedProperty constantProperty, ref SerializedProperty referenceProperty, ref SerializedProperty propertyNameProperty, ref SerializedProperty propertyTargetProperty, ref SerializedProperty typeProperty)
        {
            if (obj.changed)
            {
                obj.changed = false;

                valueTypeProperty.intValue = (int)obj.valueType;
                switch (obj.valueType)
                {
                    case ConditionValueType.Constant:
                        constantProperty.managedReferenceValue = obj.constantValue;
                        referenceProperty.objectReferenceValue = null;
                        propertyNameProperty.boxedValue = new BlackboardPropertyName();
                        break;
                    case ConditionValueType.Reference:
                        referenceProperty.objectReferenceValue = obj.referenceValue;
                        constantProperty.managedReferenceValue = null;
                        propertyNameProperty.boxedValue = new BlackboardPropertyName();
                        break;
                    case ConditionValueType.Property:
                        propertyNameProperty.boxedValue = obj.propertyValue;
                        constantProperty.managedReferenceValue = null;
                        referenceProperty.objectReferenceValue = null;
                        propertyTargetProperty.intValue = (int)obj.propertyTarget;
                        break;
                }
                typeProperty.stringValue = obj.type?.AssemblyQualifiedName;

                return true;
            }
            return false;
        }

        #region Preview
        /// <summary>
        /// Returns if the Condition is valid.
        /// </summary>
        /// <returns></returns>
        private void DrawPreview()
        {
            // Draw preview
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

            EGL.LabelField("Preview", s_middleStyle);
            EGL.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawObjectPreview(m_firstObject);
            EGL.LabelField(operationString, s_middleStyle, GUILayout.MaxWidth(50f));
            DrawObjectPreview(m_secondObject);
            EGL.EndHorizontal();
        }

        private void CheckValidity()
        {
            // Display validity message
            bool isValid = true;
            if (m_firstObject.type == null || m_secondObject.type == null ||
               (m_firstObject.valueType == ConditionValueType.Property && m_firstObject.propertyValue.IsNull) || (m_secondObject.valueType == ConditionValueType.Property && m_secondObject.propertyValue.IsNull))
            {
                isValid = false;
                EGL.HelpBox("Please select values for this Condition.", MessageType.Warning);
            }
            else
            {
                bool bothNumeric = StirgeTypeHelper.IsNumericType(m_firstObject.type) && StirgeTypeHelper.IsNumericType(m_secondObject.type);

                switch ((Operation)m_operationProperty.intValue)
                {
                    case Operation.Equal:
                    case Operation.NotEqual:
                        bool firstIsNull = m_firstObject.type == typeof(void);
                        bool secondIsNull = m_secondObject.type == typeof(void);

                        // if both are numeric, it's cool
                        if (bothNumeric)
                            break;
                        // if the types match, it's cool
                        if (m_firstObject.type == m_secondObject.type)
                            break;
                        // If both are null, it's fine I guess?
                        if (firstIsNull && secondIsNull)
                            break;
                        // If one value is null and the other can be null
                        if (firstIsNull && StirgeTypeHelper.CanBeNull(m_secondObject.type) || secondIsNull && StirgeTypeHelper.CanBeNull(m_firstObject.type))
                            break;
                        // If both types implement IEquatable<> for the other Type, it's cool
                        if (!secondIsNull && m_firstObject.type.GetInterfaces().Contains(typeof(IEquatable<>).MakeGenericType(m_secondObject.type)) &&
                            !firstIsNull && m_secondObject.type.GetInterfaces().Contains(typeof(IEquatable<>).MakeGenericType(m_firstObject.type)))
                            break;

                        EGL.HelpBox("Condition is invalid as these types are not Equatable.", MessageType.Error);
                        isValid = false;
                        break;
                    default:
                        if (!bothNumeric)
                        {
                            EGL.HelpBox("Condition is invalid as these types are not Comparable.", MessageType.Error);
                            isValid = false;
                        }
                        break;
                }
            }

            // Apply is valid
            if (isValid != m_isValidProperty.boolValue)
            {
                m_isValidProperty.boolValue = isValid;
            }
        }

        private void DrawObjectPreview(SerializedConditionObject obj)
        {
            float minWidth = (Screen.width - 50f) * 0.35f;
            switch (obj.valueType)
            {
                case ConditionValueType.Constant:
                    EGL.LabelField(obj.constantValue != null ? obj.constantValue.ToString() : "null", EditorStyles.textField, GUILayout.MinWidth(minWidth));
                    break;
                case ConditionValueType.Reference:
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EGL.ObjectField(obj.referenceValue, typeof(Object), false, GUILayout.MinWidth(minWidth));
                    }
                    break;
                case ConditionValueType.Property:
                    string propertyTargetLabel = obj.propertyTarget switch
                    {
                        ConditionPropertyTarget.User => "User.",
                        ConditionPropertyTarget.Target => "Target.",
                        ConditionPropertyTarget.Scorable => m_scorableType switch
                        {
                            SerializedScorableType.Action => "Action.",
                            SerializedScorableType.MovementGoal => "MovementGoal.",
                            SerializedScorableType.Status => "Status.",
                            _ => string.Empty
                        },
                        _ => string.Empty
                    };
                    EGL.LabelField(propertyTargetLabel + (!obj.propertyValue.IsNull ? obj.propertyValue.Name : "null"), EditorStyles.textField, GUILayout.MinWidth(minWidth));
                    break;
            }
            EditorGUI.EndDisabledGroup();
        }
        #endregion

        private void SelectConstantType(SerializedConditionObject obj)
        {
            var genericMenu = new GenericMenu();
            IReadOnlyList<Type> validTypes = ConstantTypes.ToList();

            for (int i = 0, count = validTypes.Count; i < count; i++)
            {
                Type type = validTypes[i];
                string uiName = GetUIName(type);
                genericMenu.AddItem(new GUIContent(uiName), false, () =>
                {
                    obj.type = type;
                });
            }

            genericMenu.ShowAsContext();
        }

        private void SelectProperty<TBase>(ref SerializedConditionObject obj)
        {
            bool objectIsFirst = obj == m_firstObject;

            var genericMenu = new GenericMenu();
            IReadOnlyList<PropertyInfo> propertyInfos = GenericBlackboard<TBase>.CachedPropertyInfosArray;
            for (int i = 0, count = propertyInfos.Count; i < count; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                string name = propertyInfo.Name;
                Type type = propertyInfo.PropertyType;
                string typeName = GetUIName(type);
                genericMenu.AddItem(new GUIContent(name + " : " + typeName), false, () =>
                {
                    if (objectIsFirst)
                    {
                        m_firstObject.propertyValue = new(propertyInfo.Name);
                        m_firstObject.type = type;
                    }
                    else
                    {
                        m_secondObject.propertyValue = new(propertyInfo.Name);
                        m_secondObject.type = type;
                    }
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

        public static string GetUIName(Type type)
        {
            string typeName = type.Name;
            if (typeName.Length >= 11 && typeName[..10] == "Serialized")
                return Regex.Replace(type.Name[10..], "(\\B[A-Z])", " $1");
            return Regex.Replace(type.Name, "(\\B[A-Z])", " $1");
        }
    }
}
