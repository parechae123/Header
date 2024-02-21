using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class HogisimInpecter : MonoBehaviour
{
    public float BounceCount;
    public float bounceForce;
    public Vector3 bombPosition;
    public Transform TargetPosition;
    public Transform ball;
    public float movementTime;

    public Vector3[] GetBounceVectors()
    {
        float tempY;
        float timeX = 0;
        int counter = 0;
        float tempPos = TargetPosition.position.x - transform.position.x;
        Debug.Log(tempPos);
        tempPos = tempPos / BounceCount;
        tempPos = tempPos / (314f / 30f);
        Debug.Log(tempPos);
        Vector3[] vectorArray = new Vector3[(int)((314f / 30f) * BounceCount) + ((int)BounceCount - 1)];
        Debug.Log(vectorArray.Length);
        for (int i = 0; i < BounceCount; i++)
        {
            for (float p = 0; p < 3.14f;)
            {
                timeX += tempPos;
                p += 0.3f;
                tempY = Mathf.Sin(p);
                Vector3 tempVec = new Vector3(timeX, (tempY * bounceForce) / (float)(i + 1), 0);
                if (counter >= vectorArray.Length)
                {
                    Debug.Log(counter);
                }
                vectorArray[counter] = transform.position + tempVec;
                counter++;
            }
        }
        return vectorArray;
    }
    IEnumerator MoveBalls(Vector3[] vectors)
    {
        float timeDelay = (movementTime / vectors.Length) / BounceCount;
        for (int i = 0; i < vectors.Length; i++)
        {
            yield return new WaitForSeconds(timeDelay);
            if (i == vectors.Length / BounceCount)
            {
                Debug.Log("감속");
                timeDelay += (movementTime / vectors.Length) / BounceCount;
            }
            ball.position = vectors[i];
        }
        yield return new WaitForSeconds(1.5f);
    }
    /*    // Enum 정의
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
    #endif*/
}
