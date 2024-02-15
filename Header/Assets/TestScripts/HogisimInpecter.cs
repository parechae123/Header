using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HogisimInpecter : MonoBehaviour
{
    // Enum ����
    public enum MyEnum
    {
        DefaultValue,
        SpecialValue
    }

    // Inspector�� �����ų ������
    public MyEnum myEnum;
    public int[] myArray;

#if UNITY_EDITOR
    // ������ ���� Ŭ����
    [CustomEditor(typeof(HogisimInpecter))]
    public class MyComponentEditor : Editor
    {
        // SerializedProperty�� ����Ͽ� Inspector ������ ����
        SerializedProperty myEnumProp;
        SerializedProperty myArrayProp;
        SerializedObject aa;

        // �����Ͱ� Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
        void OnEnable()
        {
            // SerializedProperty �ʱ�ȭ
            myEnumProp = serializedObject.FindProperty("myEnum");

            myArrayProp = serializedObject.FindProperty("myArray");
        }

        // Inspector Ŀ���͸���¡ �޼���
        public override void OnInspectorGUI()
        {
            // serializedObject ������Ʈ
            serializedObject.Update();

            // Enum ������ Inspector�� ǥ��
            EditorGUILayout.PropertyField(myEnumProp);

            // Enum ���� SpecialValue�� ���� myArray�� Inspector�� ǥ��
            HogisimInpecter.MyEnum enumValue = (HogisimInpecter.MyEnum)myEnumProp.enumValueIndex;

            if (enumValue == HogisimInpecter.MyEnum.SpecialValue)
            {
                EditorGUILayout.PropertyField(myArrayProp, true);
            }

            // ����� ������ ����
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
