using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;
using DataDefines;
using UnityEngine.Video;

public class ResourceManager
{
    Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, Object>();
    
    private bool loadDone = false;
    //데이터 순수성을 위해 읽기전용 프로퍼티 작성
    public bool isResourceLoadDone { get { return loadDone; } }
    #region 리소스등록
    public void RegistAllResource(DataDefines.ResourceDefine[] ResourceDefines,Action<string,int,int> CB,Action<bool,bool> isDone)
    {
        //FLOW : isDone의 첫번째 = 일러스트,두번째 = 모든 리소스
        if (!isResourceLoadDone)
        {
            string loadingName = string.Empty;
            int loadCount = 0;
            int totalCount = 0;

            var OpHandle = Addressables.LoadResourceLocationsAsync(string.Empty);
            LoadAllAsync<Sprite>("LoadingIlusts", (iluName,ilusTotal) =>
            {
                totalCount = ilusTotal;
                loadCount++;
                CB.Invoke(iluName, loadCount, ilusTotal);
                if (loadCount >= totalCount)
                {
                    totalCount = 0;
                    loadCount = 0;
                    isDone.Invoke(true, false);
                    for (int i = 0; i < ResourceDefines.Length; i++)
                    {
                        switch (ResourceDefines[i].Type)
                        {
                            case ResourceType.GameObject:
                                OpHandle = Addressables.LoadResourceLocationsAsync(ResourceDefines[i].LabelName, typeof(GameObject));
                                totalCount += OpHandle.Result.Count;
                                LoadAllAsync<GameObject>(ResourceDefines[i].LabelName, (loadResource, total) =>
                                {
                                    loadingName = loadResource;
                                    loadCount++;
                                    CB.Invoke(loadResource, loadCount, totalCount);
                                    if (loadCount == totalCount)
                                    {
                                        loadDone = true;
                                        isDone.Invoke(true,true);
                                    }
                                });
                                break;
                            case ResourceType.Sprites:
                                OpHandle = Addressables.LoadResourceLocationsAsync(ResourceDefines[i].LabelName, typeof(Sprite));
                                totalCount += OpHandle.Result.Count;
                                LoadAllAsync<Sprite>(ResourceDefines[i].LabelName, (loadResource, total) =>
                                {
                                    loadingName = loadResource;
                                    loadCount++;
                                    CB.Invoke(loadResource, loadCount, totalCount);
                                    if (loadCount == totalCount)
                                    {
                                        loadDone = true;
                                        isDone.Invoke(true,true);
                                    }
                                });
                                break;
                            case ResourceType.DataSheets:
                                OpHandle = Addressables.LoadResourceLocationsAsync(ResourceDefines[i].LabelName, typeof(TextAsset));
                                totalCount += OpHandle.Result.Count;
                                LoadAllAsync<TextAsset>(ResourceDefines[i].LabelName, (loadResource, total) =>
                                {
                                    loadingName = loadResource;
                                    loadCount++;
                                    CB.Invoke(loadResource, loadCount, totalCount);
                                    if (loadCount == totalCount)
                                    {
                                        loadDone = true;
                                        isDone.Invoke(true,true);
                                    }
                                });
                                break;
                            case ResourceType.Fonts:
                                OpHandle = Addressables.LoadResourceLocationsAsync(ResourceDefines[i].LabelName, typeof(Font));
                                totalCount += OpHandle.Result.Count;
                                LoadAllAsync<Font>(ResourceDefines[i].LabelName, (loadResource, total) =>
                                {
                                    loadingName = loadResource;
                                    loadCount++;
                                    CB.Invoke(loadResource, loadCount, totalCount);
                                    if (loadCount == totalCount)
                                    {
                                        loadDone = true;
                                        isDone.Invoke(true, true);
                                    }
                                });
                                break;
                            case ResourceType.RenderTexture:
                                OpHandle = Addressables.LoadResourceLocationsAsync(ResourceDefines[i].LabelName, typeof(RenderTexture));
                                totalCount += OpHandle.Result.Count;
                                LoadAllAsync<RenderTexture>(ResourceDefines[i].LabelName, (loadResource, total) =>
                                {
                                    loadingName = loadResource;
                                    loadCount++;
                                    CB.Invoke(loadResource, loadCount, totalCount);
                                    if (loadCount == totalCount)
                                    {
                                        loadDone = true;
                                        isDone.Invoke(true, true);
                                    }
                                });
                                break;
                            case ResourceType.Video:
                                OpHandle = Addressables.LoadResourceLocationsAsync(ResourceDefines[i].LabelName, typeof(VideoClip));
                                totalCount += OpHandle.Result.Count;
                                LoadAllAsync<VideoClip>(ResourceDefines[i].LabelName, (loadResource, total) =>
                                {
                                    loadingName = loadResource;
                                    loadCount++;
                                    CB.Invoke(loadResource, loadCount, totalCount);
                                    if (loadCount == totalCount)
                                    {
                                        loadDone = true;
                                        isDone.Invoke(true, true);
                                    }
                                });
                                break;
                            case ResourceType.Texture2D:
                                OpHandle = Addressables.LoadResourceLocationsAsync(ResourceDefines[i].LabelName, typeof(Texture2D));
                                totalCount += OpHandle.Result.Count;
                                LoadAllAsync<Texture2D>(ResourceDefines[i].LabelName, (loadResource, total) =>
                                {
                                    loadingName = loadResource;
                                    loadCount++;
                                    CB.Invoke(loadResource, loadCount, totalCount);
                                    if (loadCount == totalCount)
                                    {
                                        loadDone = true;
                                        isDone.Invoke(true, true);
                                    }
                                });
                                break;
                        }
                    }
                }
            });
        }
    }
    #endregion
    #region 리소스로딩함수
    public T Load<T>(string key) where T : Object
    {
        if (_resources.TryGetValue(key, out Object resource))
        {
            return resource as T;
        }
        if (typeof(T) == typeof(Sprite))
        {
            key = key + ".sprite";
            if (_resources.TryGetValue(key, out Object temp))
            {
                return temp as T;
            }
            // FLOW : 어드레서블의 키값(태그 아닌 단일객체)를 가져와서 지정한 타입으로 반환
        }
        if (typeof(T) == typeof(Font))
        {
            key = key + ".font";
            if (_resources.TryGetValue(key, out Object temp))
            {
                return temp as T;
            }
            // FLOW : 어드레서블의 키값(태그 아닌 단일객체)를 가져와서 지정한 타입으로 반환
        }
        return null;
    }
    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>($"{key}");
        if (prefab == null)
        {
            Debug.LogError($"Failed to load prefab : {key}");
            return null;
        }
        if (pooling)
        {
            return Managers.instance.Pool.Pop(prefab);
        }
        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
        //FLOW : Transform parent와 pooling이 null,false가 아니면 씬에 생성해줌
    }
    public void Destroy(GameObject go)
    {
        if (go == null) return;
        if (Managers.instance.Pool.Push(go)) return;

        Object.Destroy(go);

        //FLOW : 해당 오브젝트의 pool이 false이면 그냥 오브젝트를 디스트로이,그게 아니면 풀로 넣어줌
    }
    public void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        //프라이트인 경우 하위객체의 이름으로 로드하면 스프라이트로 로딩이 됨
        //FLOW : key와 같은 키값을 가진 자료들을 반환해줌
        string loadKey = key;
        if (key.Contains(".sprite"))
        {
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";
        }
        var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
        asyncOperation.Completed += (op) =>
        {
            //캐시 확인
            if (_resources.TryGetValue(key, out Object resource))
            {
                callback?.Invoke(op.Result);
                return;
            }
            _resources.Add(key, op.Result);
            callback?.Invoke(op.Result);
        };
    }
    public void LoadAllAsync<T>(string label, Action<string, int> callback) where T : UnityEngine.Object
    {
        //FLOW : label와 같은 라벨값을 가진 resource들을 op로 반환하여 딕셔너리에 자료를 등록해줌
        var OpHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));

        OpHandle.Completed += (op) =>
        {
            int totalCount = op.Result.Count;
            if (op.Result.Count == 0)
            {
                Debug.LogError( label+" : "+"해당 라벨에 이름이 없음");
                
                callback?.Invoke("해당 라벨에 값이 없습니다", 0);
                
                return;
            }
            foreach (var result in op.Result)
            {
                if (result.PrimaryKey.Contains(".sprite"))
                {
                    LoadAsync<Sprite>(result.PrimaryKey, (obj) =>
                    {
                        callback?.Invoke(result.PrimaryKey, totalCount);
                    });
                }
                else
                {
                    LoadAsync<T>(result.PrimaryKey, (obj) =>
                    {
                        callback?.Invoke(result.PrimaryKey, totalCount);
                    });
                }
            }
        };
    }
    #endregion 관련 함수
}
