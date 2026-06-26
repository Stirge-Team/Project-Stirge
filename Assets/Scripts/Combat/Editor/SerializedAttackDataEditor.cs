using Stirge.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.tvOS;
using EGL = UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;

namespace Stirge.Combat.Attacks.Serialization
{
    [CustomEditor(typeof(SerializedAttackData))]
    public class SerializedAttackDataEditor : Editor
    {
        private const string SerializedAttackNodesPropertyName = "m_serializedAttackNodes";
        private const string SerializedBindingsPropertyName = "m_nodeBindings";

        private readonly Dictionary<Object, Editor> m_editors = new();

        private static bool s_nodesFoldout = true;

        private int m_removeIndex = 0;

        private int m_removeLoopDepth = 0;

        public override void OnInspectorGUI()
        {
            SerializedProperty attackNodesProperty = serializedObject.FindProperty(SerializedAttackNodesPropertyName);
            SerializedProperty bindingsProperty = serializedObject.FindProperty(SerializedBindingsPropertyName);

            s_nodesFoldout = EGL.Foldout(s_nodesFoldout, "Attack Nodes");
            if (s_nodesFoldout)
            {
                EGL.BeginVertical(GUI.skin.window);

                // Draw the root node
                if (attackNodesProperty.arraySize > 0)
                {
                    SerializedProperty nodeProperty = attackNodesProperty.GetArrayElementAtIndex(0);
                    DrawAttackNode(attackNodesProperty, bindingsProperty, nodeProperty, 0);
                }

                EGL.Separator();

                if (GUILayout.Button("Set Root Attack Node"))
                {
                    AddAttackNode(attackNodesProperty, bindingsProperty, -1);
                }

                EGL.EndVertical();
            }
        }

        /// <summary>
        /// Returns true if an <see cref="AttackNode"/> that was Bound to the <see cref="AttackNode"/> in <paramref name="nodeProperty"/> was removed.
        /// </summary>
        private bool DrawAttackNode(SerializedProperty attackNodesProperty, SerializedProperty bindingsProperty, SerializedProperty nodeProperty, int nodeIndex)
        {
            bool removed = false;

            var objectValue = (SerializedAttackNode_Base)nodeProperty.objectReferenceValue;

            if (!m_editors.TryGetValue(objectValue, out Editor editor))
            {
                editor = CreateEditorWithContext(new Object[] { objectValue }, target);
                m_editors.Add(objectValue, editor);
            }

            EGL.BeginVertical(GUI.skin.box);

            EGL.LabelField(GetUIName(objectValue.attackNodeType), EditorStyles.boldLabel);
            objectValue.name = EGL.TextField("Name", objectValue.name);

            // If there are bindings to this AttackNode, draw those nodes here.
            for (int attackNodeIndex = 0, attackNodeCount = attackNodesProperty.arraySize; attackNodeIndex < attackNodeCount; ++attackNodeIndex)
            {
                SerializedProperty bindingProperty = bindingsProperty.GetArrayElementAtIndex(attackNodeIndex);

                // Binding Index represents each existing AttackNode. If the value at that AttackNode's index is equal to the index of THIS Node,
                // that means that Attack Node is bound to THIS Attack Node and we want to draw it now.
                if (bindingProperty != null && bindingProperty.intValue == nodeIndex)
                {
                    SerializedProperty boundNodeProperty = attackNodesProperty.GetArrayElementAtIndex(attackNodeIndex);

                    // if the node is removed, then adjust the loop index and count to account for array resizing
                    if (DrawAttackNode(attackNodesProperty, bindingsProperty, boundNodeProperty, attackNodeIndex))
                    {
                        --attackNodeIndex;
                        attackNodeCount = attackNodesProperty.arraySize;
                    }
                }
            }

            editor.OnInspectorGUI();

            // If the AttackNode being drawn is a Decorator Attack Node, then we want to be able to add AttackNodes to it
            // In the case of Multi Decorators, we also need to be able to remove them.
            if (objectValue.attackNodeType.IsSubclassOf(typeof(DecoratorNodeSingle)))
            {
                EGL.Separator();

                // Because this is a Single Decorator, we need to remove the existing bound Attack Node before adding a new one
                // Find the existing bound AttackNode.
                // Ignore the Attack Node at index 0 as this is always the root Node
                int nodeIndexToBeRemoved = -1;
                for (int i = 1, count = bindingsProperty.arraySize; i < count; i++)
                {
                    SerializedProperty bindingPropertyAtThisIndex = bindingsProperty.GetArrayElementAtIndex(i);
                    if (bindingPropertyAtThisIndex.intValue == nodeIndex)
                    {
                        nodeIndexToBeRemoved = i;
                    }
                }

                if (nodeIndexToBeRemoved == -1)
                {
                    if (GUILayout.Button("Set Attack Node"))
                    {
                        // Add the new Attack Node, providing the index to overwrite
                        AddAttackNode(attackNodesProperty, bindingsProperty, nodeIndex);
                    }
                }
                // Add a prompt to remove the existing Node    
                else
                {
                    SerializedProperty nodePropertyToBeRemoved = attackNodesProperty.GetArrayElementAtIndex(nodeIndexToBeRemoved);
                    if (GUILayout.Button($"Remove {nodePropertyToBeRemoved.objectReferenceValue.name} from this Decorator"))
                    {
                        m_removeLoopDepth = 0;
                        RemoveAttackNode(attackNodesProperty, bindingsProperty, nodeIndexToBeRemoved);

                        removed = true;

                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                    }
                }
            }
            else if (objectValue.attackNodeType.IsSubclassOf(typeof(DecoratorNodeMulti)))
            {
                EGL.Separator();

                // Add to array
                if (GUILayout.Button("Add Attack Node"))
                {
                    AddAttackNode(attackNodesProperty, bindingsProperty, nodeIndex);
                }

                // Get the Nodes bound to this node
                var boundNodes = new List<(int nodeIndex, SerializedProperty nodeProperty)>(); // Tuple list!
                for (int i = 0, count = bindingsProperty.arraySize; i < count; i++)
                {
                    // If the binding at this Index is set to the Node Index of THIS Node, then it is bound
                    SerializedProperty bindingProperty = bindingsProperty.GetArrayElementAtIndex(i);
                    if (bindingProperty.intValue == nodeIndex)
                    {
                        boundNodes.Add(new(i, attackNodesProperty.GetArrayElementAtIndex(i)));
                    }
                }

                // If there are bound Nodes
                int bindingCount = boundNodes.Count;
                if (bindingCount > 0)
                {
                    // Remove at index
                    m_removeIndex = EGL.IntField(m_removeIndex);

                    // if the provided index is within bounds of array
                    if (m_removeIndex >= 0 && m_removeIndex < bindingCount)
                    {
                        int nodeIndexToBeRemoved = boundNodes[m_removeIndex].nodeIndex; // Access Tuple properties by name!
                        SerializedProperty nodePropertyToBeRemoved = boundNodes[m_removeIndex].nodeProperty;
                        if (GUILayout.Button($"Remove {nodePropertyToBeRemoved.objectReferenceValue.name} at index {m_removeIndex}"))
                        {
                            m_removeLoopDepth = 0;
                            RemoveAttackNode(attackNodesProperty, bindingsProperty, nodeIndexToBeRemoved);

                            removed = true;

                            serializedObject.ApplyModifiedProperties();
                            AssetDatabase.SaveAssets();
                        }
                    }
                    else
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EGL.TextArea("Index is outside the bounds of the array!");
                        }
                    }
                }
                else
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EGL.TextArea("No bound Attack Nodes!");
                    }
                }
            }

            EGL.EndVertical();

            return removed;
        }

        private void AddAttackNode(SerializedProperty attackNodesProperty, SerializedProperty bindingsProperty, int bindingIndex)
        {
            var genericMenu = new GenericMenu();
            IReadOnlyList<Type> attackNodeTypes = SerializedAttackNodeTypesCollection.attackNodeTypes;

            for (int i = 0, count = attackNodeTypes.Count; i < count; i++)
            {
                Type type = attackNodeTypes[i];
                string uiName = GetUIName(type);
                genericMenu.AddItem(new GUIContent(uiName), false, () =>
                {
                    Type serializedNodeType = SerializedAttackNodeTypesCollection.GetSerialisedAttackNodeType(type);
                    ScriptableObject instance = CreateInstance(serializedNodeType);
                    instance.name = uiName.Replace(" ", string.Empty);

                    AssetDatabase.AddObjectToAsset(instance, target);

                    // if no binding, then this is the Root node
                    if (bindingIndex == -1)
                    {
                        // Destroy all existing Nodes
                        while (attackNodesProperty.arraySize > 0)
                        {
                            int nodeIndex = attackNodesProperty.arraySize - 1;
                            m_removeLoopDepth = 0;
                            RemoveAttackNode(attackNodesProperty, bindingsProperty, nodeIndex);
                        }

                        // Create as Root Node
                        attackNodesProperty.arraySize = 1;
                        attackNodesProperty.GetArrayElementAtIndex(0).objectReferenceValue = instance;

                        bindingsProperty.arraySize = 1;
                        bindingsProperty.GetArrayElementAtIndex(0).intValue = -1;
                    }
                    else
                    {
                        // Add to end of array bound to specified node
                        int newAttackNodeIndex = attackNodesProperty.arraySize++;
                        SerializedProperty newAttackNodeProperty = attackNodesProperty.GetArrayElementAtIndex(newAttackNodeIndex);
                        newAttackNodeProperty.objectReferenceValue = instance;

                        int newAttackNodeBindingIndex = bindingsProperty.arraySize++;
                        SerializedProperty newAttackNodeBindingProperty = bindingsProperty.GetArrayElementAtIndex(newAttackNodeBindingIndex);
                        newAttackNodeBindingProperty.intValue = bindingIndex;
                    }

                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                });
            }

            genericMenu.ShowAsContext();
        }

        private void RemoveAttackNode(SerializedProperty attackNodesProperty, SerializedProperty bindingsProperty, int indexOfRemovedNode)
        {
            // Get the property of the Node to remove
            SerializedProperty nodeProperty = attackNodesProperty.GetArrayElementAtIndex(indexOfRemovedNode);

            // Before removeing, first remove any Nodes that are bound to the Node to be removed
            RemoveAttackNodesWithBinding(attackNodesProperty, bindingsProperty, indexOfRemovedNode);

            // Destroy SerializedAttackNode asset
            var objectValue = nodeProperty.objectReferenceValue;
            DestroyImmediate(objectValue, true);

            // Then, fix up the arrays by removing the now empty elements
            // This loop depth variable stops the array from being adjusted before all of the nested AttackNodes are readied to be removed
            if (m_removeLoopDepth == 0)
            {
                for (int attackNodeCount = attackNodesProperty.arraySize, attackNodeIndex = attackNodeCount - 1; attackNodeIndex >= 0; attackNodeIndex--)
                {
                    SerializedProperty attackNodePropertyAtThisIndex = attackNodesProperty.GetArrayElementAtIndex(attackNodeIndex);
                    if (attackNodePropertyAtThisIndex.objectReferenceValue == null)
                    {
                        SerializedPropertyHelper.CompletelyRemove(attackNodesProperty, attackNodeIndex);
                        SerializedPropertyHelper.CompletelyRemove(bindingsProperty, attackNodeIndex);
                    }
                }
            }
        }

        private void RemoveAttackNodesWithBinding(SerializedProperty attackNodesProperty, SerializedProperty bindingsProperty, int indexOfRemovedNode)
        {
            // Check each Attack Node. If they were bound to the removed Node, also remove them
            for (int attackNodeIndex = 0, attackNodeCount = attackNodesProperty.arraySize; attackNodeIndex < attackNodeCount; attackNodeIndex++)
            {
                int bindingValue = bindingsProperty.GetArrayElementAtIndex(attackNodeIndex).intValue;
                if (bindingValue == indexOfRemovedNode)
                {
                    m_removeLoopDepth++;
                    RemoveAttackNode(attackNodesProperty, bindingsProperty, attackNodeIndex);
                    m_removeLoopDepth--;
                }
            }
        }

        #region String Formatting
        public static string GetUIName(Type type)
        {
            if (!type.IsGenericType)
            {
                // "object" name not to confuse it with UnityEngine.Object.
                return type == typeof(object) ? "object" : GetNameWithSpaces(type.Name);
            }

            string genericName = type.Name;
            genericName = genericName[..genericName.IndexOf("`", StringComparison.Ordinal)];
            var stringBuilder = new StringBuilder(GetNameWithSpaces(genericName));
            stringBuilder.Append(" <");

            Type[] genericParameters = type.GetGenericArguments();

            for (int i = 0, count = genericParameters.Length; i < count; ++i)
            {
                stringBuilder.Append(GetUIName(genericParameters[i]));

                if (i < count - 1)
                {
                    stringBuilder.Append(", ");
                }
            }

            stringBuilder.Append('>');

            return stringBuilder.ToString();
        }
        private static string GetNameWithSpaces(string typeName)
        {
            return Regex.Replace(typeName, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
        }
        #endregion
    }
}
