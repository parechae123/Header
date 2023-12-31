using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers s_instance;
    public static Managers instance { get { Init(); return s_instance; } }



    private GameManager game = new GameManager();
    public GameManager Game { get { return instance?.game; } }


    private ResourceManager resource = new ResourceManager();
    public ResourceManager Resource { get { return instance?.resource; } }


    private PoolManager pool = new PoolManager();
    public PoolManager Pool { get { return instance?.pool; } }


    private UIManager ui = new UIManager();
    public UIManager UI { get { return instance?.ui; } }


    private GridManager grid = new GridManager();
    public GridManager Grid { get { return instance?.grid; } }


    public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
            Debug.Log("게임 메니저 생성");
        }
    }
}
