using UnityEditor;
using UnityEngine;

public class FireLineTest : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private int numPoints = 50;
    private float timeInterval = 0.1f;
    private float initialVelocity = 10f;
    private float gravity = -9.8f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numPoints;
    }

    private void Update()
    {
        
        for (int i = 0; i < numPoints; i++)
        {
            float time = i * timeInterval;
            Vector2 tempVec = pos() * initialVelocity;
            float x = tempVec.x * time;
            float y = (tempVec.y * time) + (0.5f * gravity * time * time);

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
    Vector2 pos()
    {
        Vector2 tempVec = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        tempVec.x = tempVec.x - transform.position.x;
        tempVec.y = tempVec .y - transform.position.y;
        Debug.Log(tempVec.normalized);
        return tempVec.normalized;
    }
}