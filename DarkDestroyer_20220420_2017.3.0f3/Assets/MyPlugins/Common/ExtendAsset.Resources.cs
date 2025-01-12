/****************************************************

	文件：
	作者：WWS
	日期：2022/10/31 15:25:09
	功能：追要对Unity的Componetn组件的拓展方法(this大法)
        静态类不能有实例构造器。
        静态类不能有任何实例成员。
        静态类不能使用abstract或sealed修饰符。 
        静态类默认继承自System.Object根类，不能显式指定任何其他基类。

        Resources是sealed类

 *****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Object= UnityEngine.Object;


/// <summary>
/// 有的只属于这个工程好用，所以partial分权 .
/// 但是分不了，命名空间不同，只认最近的
/// 所以外部新建一个InternalResources用来处理该工程的类，做到 ExtendResources与外部工程的隔离
/// </summary>
public static partial    class ExtendResources
{

    [Obsolete("已过时:这种写死的不好用,用ILoader LoaderUtil")]
    class ResMgr
    { 
    #region 常变部分



    #endregion  

    private static Dictionary<string, Object> resDict;
    private static Dictionary<string, Object[]> resArrayDict;


    public static void Init()
    {
        resDict = new Dictionary<string, Object>();
        resArrayDict = new Dictionary<string, Object[]>();
    }

    /// <summary>
    /// 文件路径
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resPath"></param>
    /// <returns></returns>
    public static T Get<T>( string resPath) where T : Object
    {
        if (resDict.ContainsKey(resPath))
        {
            return resDict[resPath] as T;
        }
        else
        {
            var res = Resources.Load(resPath);
            resDict.Add(resPath, res);
            return res as T;
        }
    }


    /// <summary>
    /// 文件夹路径
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resPath">文件夹下所有</param>
    /// <returns></returns>
    public static T[] GetAll<T>( string resPath) where T : Object
    {
        Object[] objArray;
        if (resDict.ContainsKey(resPath))
        {
            objArray = resArrayDict[resPath];
        }
        else
        {
            var res = Resources.LoadAll(resPath);
            resArrayDict.Add(resPath, res);
            objArray = res;
        }
        T[] TArray = new T[objArray.Length];
        for (int i = 0; i < TArray.Length; i++)
        {
            TArray[i] = objArray[i] as T;
        }
        return TArray;
    }    
    }

}


public static partial class ExtendResources
{
    #region ResourcesLoader


    /// <summary>比较原生</summary>
    public class ResourcesLoader : IUObjectLoader ,IUObjectAsyncLoaderByIEnumerator
    {


        #region pub ILoader

        public T Load<T>(string path) where T : UnityEngine.Object
        {
            var uObj = Resources.Load<T>(path);
            if (uObj == null)
            {
                Debug.LogError("未找到对应Object，路径：" + path);
                return null;
            }

            return uObj;
        }

        public T[] LoadAll<T>(string path) where T : UnityEngine.Object
        {
            var arr = Resources.LoadAll<T>(path);
            if (arr == null || arr.Length == 0)
            {
                Debug.LogError($"ResourceLoader当前路径下未找到对应资源，路径：{path}");
                return null;
            }

            return arr;
        }
        public   IEnumerator LoadAsync<T>(string name,Action<T> cb) where T:UnityEngine.Object
        {
            ResourceRequest req = Resources.LoadAsync<T>(name);//类型，名字
            yield return req;


            T t = (req.asset) as T;
            cb(t);
        }

        #endregion

    }

    #endregion

}

