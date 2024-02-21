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
                Debug.Log("����");
                timeDelay += (movementTime / vectors.Length) / BounceCount;
            }
            ball.position = vectors[i];
        }
        yield return new WaitForSeconds(1.5f);
    }
    /*    // Enum ����
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
    #endif*/
}
