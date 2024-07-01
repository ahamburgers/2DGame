using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instance : MonoBehaviour
{
    // Start is called before the first frame update
    private int captureCounter = 0;

    // 定义最多32种颜色，为每个实例分配一个颜色
    private Color[] instanceColors = {
        Color.red, Color.green, Color.blue, Color.yellow, Color.magenta,
        Color.cyan, Color.white, Color.gray, Color.black, new Color(0.5f, 0.2f, 0.8f),
        new Color(0.2f, 0.5f, 0.2f), new Color(0.6f, 0.3f, 0.2f), new Color(0.9f, 0.6f, 0.1f),
        new Color(0.4f, 0.4f, 0.7f), new Color(0.8f, 0.5f, 0.3f), new Color(0.3f, 0.8f, 0.4f),
        new Color(0.7f, 0.2f, 0.6f), new Color(0.5f, 0.5f, 0.1f), new Color(0.1f, 0.7f, 0.9f),
        new Color(0.9f, 0.1f, 0.3f), new Color(0.3f, 0.3f, 0.3f), new Color(0.6f, 0.6f, 0.6f),
        new Color(0.1f, 0.1f, 0.5f), new Color(0.8f, 0.8f, 0.8f), new Color(0.2f, 0.9f, 0.2f),
        new Color(0.7f, 0.7f, 0.7f), new Color(0.4f, 0.1f, 0.4f), new Color(0.5f, 0.9f, 0.5f),
        new Color(0.6f, 0.4f, 0.8f), new Color(0.9f, 0.5f, 0.6f), new Color(0.3f, 0.6f, 0.3f),
        new Color(0.7f, 0.9f, 0.7f)
    };

    private Material instanceMat; // 材质用于实例渲染

    // Start函数会在脚本对象启动时调用一次
    void Start()
    {
        // 创建一个简单的材质用于实例渲染
        instanceMat = new Material(Shader.Find("Unlit/Color"));
    }

    // Update函数在每一帧都会被调用
    void Update()
    {
        // 当玩家按下空格键时
        if (Input.GetKeyDown(KeyCode.W))
        {
            //执行拍照协程
            StartCoroutine(CaptureProcess());
        }
    }

    //拍照协程
    IEnumerator CaptureProcess()
    {
        // 捕获普通截图
        ScreenCapture.CaptureScreenshot("Assets/images/image_" + captureCounter + ".png");
        Debug.Log("Screenshot image_" + captureCounter + ".png" + " has been saved.");

        // 等待0.1秒以确保普通截图操作完成
        yield return new WaitForSeconds(0.1f);

        // 捕获实例标签
        CaptureInstanceLabel();

        captureCounter++;
    }

    // 拍摄实例标签的方法
    void CaptureInstanceLabel()
    {
        GameObject[] Instances = GameObject.FindGameObjectsWithTag("Apple");
        // 保存当前每个苹果实例的材质
        Material[] originalMats = new Material[Instances.Length];

        for (int i = 0; i < Instances.Length; i++)
        {
            // 保存原始材质
            originalMats[i] = Instances[i].GetComponent<Renderer>().material;

            // 创建一个新的材质实例，并设置实例颜色
            Material newMatInstance = new Material(instanceMat);
            newMatInstance.color = instanceColors[i];
            Instances[i].GetComponent<Renderer>().material = newMatInstance;
        }

        // 捕获实例截图
        ScreenCapture.CaptureScreenshot("Assets/images/instance_label_" + captureCounter + ".png");
        Debug.Log("Instance label image_" + captureCounter + ".png" + " has been saved.");

        // 使用协程来延迟恢复原始材质
        StartCoroutine(RestoreMaterials(originalMats, Instances));
    }

    //用于恢复材质的协程
    IEnumerator RestoreMaterials(Material[] originalMats, GameObject[] Instance)
    {
        // 等待0.1秒以确保截图操作完成
        yield return new WaitForSeconds(0.1f);

        // 恢复原始材质
        for (int i = 0; i < originalMats.Length; i++)
        {
            if (i < Instance.Length && originalMats[i] != null)
            {
                Instance[i].GetComponent<Renderer>().material = originalMats[i];
            }
        }
    }
}
