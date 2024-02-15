using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HogisimInpecter : MonoBehaviour
{
    // Enum 정의
    public enum MyEnum
    {
        DefaultValue,
        SpecialValue
    }

    // Inspector에 노출시킬 변수들
    public MyEnum myEnum;
    public int[] myArray;

#if UNITY_EDITOR
    // 에디터 전용 클래스
    [CustomEditor(typeof(HogisimInpecter))]
    public class MyComponentEditor : Editor
    {
        // SerializedProperty를 사용하여 Inspector 변수에 접근
        SerializedProperty myEnumProp;
        SerializedProperty myArrayProp;
        SerializedObject aa;

        // 에디터가 활성화될 때 호출되는 메서드
        void OnEnable()
        {
            // SerializedProperty 초기화
            myEnumProp = serializedObject.FindProperty("myEnum");

            myArrayProp = serializedObject.FindProperty("myArray");
        }

        // Inspector 커스터마이징 메서드
        public override void OnInspectorGUI()
        {
            // serializedObject 업데이트
            serializedObject.Update();

            // Enum 변수를 Inspector에 표시
            EditorGUILayout.PropertyField(myEnumProp);

            // Enum 값이 SpecialValue일 때만 myArray를 Inspector에 표시
            HogisimInpecter.MyEnum enumValue = (HogisimInpecter.MyEnum)myEnumProp.enumValueIndex;

            if (enumValue == HogisimInpecter.MyEnum.SpecialValue)
            {
                EditorGUILayout.PropertyField(myArrayProp, true);
            }

            // 변경된 내용을 적용
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
