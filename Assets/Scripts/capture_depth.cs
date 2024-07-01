using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

//这是一个Unity脚本，用于捕获场景中的深度图像并保存为EXR格式

public class capture_depth : MonoBehaviour
{
    // 定义一个计数器，用于每次保存图像时给图像命名
    private int captureCounter = 0;
    // 公开的材质变量，用于在OnRenderImage中处理图像
    public Material mMat;
    // 相机组件的引用
    private Camera mCamera;
    // 保存当前活动的渲染纹理的引用
    private RenderTexture currentRT;

    //数量
    public int numberOfPhotos;
    void Start()
    {
        // 获取游戏对象上的相机组件
    }

    void Update()
    {

    }
    
    public void CaptureDepth()
    {
        mCamera = gameObject.GetComponent<Camera>();
        // 设置相机模式为深度模式
        mCamera.depthTextureMode = DepthTextureMode.Depth;
        // 为相机设置一个新的目标渲染纹理，大小和相机的像素宽高一致，24位深度，使用ARGBFloat格式
        mCamera.targetTexture = new RenderTexture(mCamera.pixelWidth, mCamera.pixelHeight, 24, RenderTextureFormat.ARGBFloat);
        // 保存当前活动的渲染纹理引用
        currentRT = RenderTexture.active;

        // 将相机的目标纹理设置为当前活动的渲染纹理
        RenderTexture.active = mCamera.targetTexture;

        // 渲染相机的视图
        mCamera.Render();

        // 创建一个新的2D纹理，大小和相机的目标纹理一致
        Texture2D image = new Texture2D(mCamera.targetTexture.width, mCamera.targetTexture.height, TextureFormat.RGBAFloat, false);
        // 从相机的目标纹理中读取像素到2D纹理中
        image.ReadPixels(new Rect(0, 0, mCamera.targetTexture.width, mCamera.targetTexture.height), 0, 0);
        image.Apply();

        // 恢复之前的活动渲染纹理
        RenderTexture.active = currentRT;

        // 将2D纹理编码为EXR格式
        byte[] bytes = image.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
        // 保存EXR文件到指定路径
        File.WriteAllBytes(Application.dataPath + "/images/depth/depth_" + captureCounter + ".exr", bytes);
        // 在控制台打印消息，表示图像已被保存
        Debug.Log("Screenshot depth_" + captureCounter + " has been saved.");
        
        // 更新图像计数器
        captureCounter++;
    }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // 如果设置了mMat材质，使用该材质处理图像
            if (null != mMat)
            {
                Graphics.Blit(source, destination, mMat);
            }
            // 否则，直接将源纹理复制到目标纹理
            else
            {
                Graphics.Blit(source, destination);
            }
        }
}
