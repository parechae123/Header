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
    //������ �������� ���� �б����� ������Ƽ �ۼ�
    public bool isResourceLoadDone { get { return loadDone; } }
    #region ���ҽ����
    public void RegistAllResource(DataDefines.ResourceDefine[] ResourceDefines,Action<string,int,int> CB,Action<bool,bool> isDone)
    {
        //FLOW : isDone�� ù��° = �Ϸ���Ʈ,�ι�° = ��� ���ҽ�
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
    #region ���ҽ��ε��Լ�
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
            // FLOW : ��巹������ Ű��(�±� �ƴ� ���ϰ�ü)�� �����ͼ� ������ Ÿ������ ��ȯ
        }
        if (typeof(T) == typeof(Font))
        {
            key = key + ".font";
            if (_resources.TryGetValue(key, out Object temp))
            {
                return temp as T;
            }
            // FLOW : ��巹������ Ű��(�±� �ƴ� ���ϰ�ü)�� �����ͼ� ������ Ÿ������ ��ȯ
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
        //FLOW : Transform parent�� pooling�� null,false�� �ƴϸ� ���� ��������
    }
    public void Destroy(GameObject go)
    {
        if (go == null) return;
        if (Managers.instance.Pool.Push(go)) return;

        Object.Destroy(go);

        //FLOW : �ش� ������Ʈ�� pool�� false�̸� �׳� ������Ʈ�� ��Ʈ����,�װ� �ƴϸ� Ǯ�� �־���
    }
    public void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        //������Ʈ�� ��� ������ü�� �̸����� �ε��ϸ� ��������Ʈ�� �ε��� ��
        //FLOW : key�� ���� Ű���� ���� �ڷ���� ��ȯ����
        string loadKey = key;
        if (key.Contains(".sprite"))
        {
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";
        }
        var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
        asyncOperation.Completed += (op) =>
        {
            //ĳ�� Ȯ��
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
        //FLOW : label�� ���� �󺧰��� ���� resource���� op�� ��ȯ�Ͽ� ��ųʸ��� �ڷḦ �������
        var OpHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));

        OpHandle.Completed += (op) =>
        {
            int totalCount = op.Result.Count;
            if (op.Result.Count == 0)
            {
                Debug.LogError( label+" : "+"�ش� �󺧿� �̸��� ����");
                
                callback?.Invoke("�ش� �󺧿� ���� �����ϴ�", 0);
                
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
    #endregion ���� �Լ�
}
