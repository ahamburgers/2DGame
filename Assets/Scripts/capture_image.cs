using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

//用于捕获场景的截图并将其保存为PNG格式
public class capture_image : MonoBehaviour
{
    // Start is called before the first frame update

    public int spawncount;//Sets the number of generated objects
    public GameObject[] fruitPrefabs;
    private GameObject[] spawnedObjects;
    public KeyCode spawnKey = KeyCode.Space; // Trigger the generated key
    public float minSpawnHeight = 1.0f; // Minimum value of random height
    public float maxSpawnHeight = 3.0f; // The maximum of the random height
    private int positioncount = 0;
    // Defines a counter to name an image each time it is saved
    private int captureCounter = 0;
    public int numberOfPhotos = 10; // The number of photos taken

    // Define up to 32 colors and assign one color to each instance
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

    private Material instanceMat; // Materials are used for instance rendering
    //Main camera
    public Camera main_camera;
    //depth camera
    public Camera depth_camera;
    public Material mMat;
    private RenderTexture currentRT;

    void Start()
    {
        instanceMat = new Material(Shader.Find("Unlit/Color"));
        main_camera = Camera.main;
        
        depth_camera = gameObject.GetComponent<Camera>();
        // Set the camera mode to depth mode
        depth_camera.depthTextureMode = DepthTextureMode.Depth;
        // Set a new target render texture for the camera, the size of the camera's pixel width and height, 24 bits deep, using the ARGBFloat format
        depth_camera.targetTexture = new RenderTexture(depth_camera.pixelWidth, depth_camera.pixelHeight, 24, RenderTextureFormat.ARGBFloat);
    }


    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            //Execute the photo coroutine
            StartCoroutine(TakePhotos());
        }
    }
    IEnumerator TakePhotos()
    {
        for (int photoIndex = 0; photoIndex < numberOfPhotos; photoIndex++)
        {
            StartCoroutine(spawn()); 
            yield return new WaitForSeconds(4f); // wait for 4s
            StartCoroutine(CaptureProcess());                                  // 
            yield return new WaitForSeconds(3f); //wait for 3s
            ClearFruits(); //clear fruits
            yield return new WaitForSeconds(1f); // wait for 1s
        }
    }
    //深度图像的方法
    public void CaptureDepth()
    {
        // Saves the currently active render texture reference
        currentRT = RenderTexture.active;
        // Sets the camera's target texture to the currently active render texture
        RenderTexture.active = depth_camera.targetTexture;
        // Render the camera's view
        depth_camera.Render();
        // Create a new 2D texture, the same size as the camera's target texture
        Texture2D image = new Texture2D(depth_camera.targetTexture.width, depth_camera.targetTexture.height, TextureFormat.RGBAFloat, false);
        // Read pixels from the camera's target texture into a 2D texture
        image.ReadPixels(new Rect(0, 0, depth_camera.targetTexture.width, depth_camera.targetTexture.height), 0, 0);
        image.Apply();
        // Restores previously active render textures
        RenderTexture.active = currentRT;
        // Encode 2D textures into EXR format
        byte[] bytes = image.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
        // Save the EXR file to the specified path
        File.WriteAllBytes(Application.dataPath + "/images/depth/depth_" + captureCounter + ".exr", bytes);
        // Print a message on the console to indicate that the image has been saved
        Debug.Log("Screenshot depth_" + captureCounter + " has been saved.");
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // If the mMat material is set, use it to process the image
        if (null != mMat)
        {
            Graphics.Blit(source, destination, mMat);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
        
    private StringBuilder positionsString = new StringBuilder();
    //创建水果模型
    IEnumerator spawn()
    {
        spawnedObjects = new GameObject[spawncount];
        for (int i = 0; i < spawncount; i++)
        {
            // Random generating height
            float randomHeight = Random.Range(minSpawnHeight, maxSpawnHeight);
            // Randomly generated position
            Vector3 randomPosition = new Vector3(
                Random.Range(-1.0f, 1.0f), // X-axis range
                randomHeight, // Random height
                Random.Range(-1.0f, 1.0f) // Z-axis range
            );
            Quaternion randomRotation = Quaternion.Euler(
                Random.Range(0, 360), // X-axis rotation
                Random.Range(0, 360), // Y-axis rotation
                Random.Range(0, 360)  // Z-axis rotation
            );
            GameObject selectedFruitPrefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            // Generate a new object at a random location
            GameObject spawnedObject = Instantiate(selectedFruitPrefab, randomPosition, randomRotation);
            spawnedObjects[i] = spawnedObject;
        }
        yield return new WaitForSeconds(3f);//wait 3s
        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            positionsString.Append(spawnedObjects[i].name + "-");
            positionsString.Append("[" + spawnedObjects[i].transform.position + "]");
            positionsString.Append(", ");
        }
        Debug.Log(positionsString.ToString());
        WritePositionsToFile(positionsString.ToString());

    }

    void WritePositionsToFile(string positions)
    {
        // Specify folder path
        string folderPath = Application.dataPath + "/position";
        // If the folder does not exist, create the folder
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string filePath = Path.Combine(folderPath, "position_" + captureCounter + ".txt");

        // Writes location information to a text file
        File.WriteAllText(filePath, positions);

        // Print a message on the console indicating that the file has been created
        Debug.Log("Positions file created at: " + filePath);
        positioncount++;

    }

    void ClearFruits()
    {
        // Destroy all fruit on the field
        foreach (GameObject spawnedobject in spawnedObjects)
        {
            Destroy(spawnedobject);
        }
    }


    //拍摄脚本
    IEnumerator CaptureProcess()
    {
        // Capture common screenshots
        ScreenCapture.CaptureScreenshot("Assets/images/image/image_" + captureCounter + ".png");
        Debug.Log("Screenshot image_" + captureCounter + ".png" + " has been saved.");
        // Wait 0.1 seconds for the normal screenshot operation to complete
        yield return new WaitForSeconds(0.1f);
        // Capture semantic tag
        CaptureLanguageLabel();
        yield return new WaitForSeconds(1f);
        //Capture instance tag
        CaptureInstanceLabel();
        yield return new WaitForSeconds(1f);
        //Capture depth information
        CaptureDepth();
        captureCounter++;
    }
    //实例标签
    // 在这个方法中，为spawnedObjects中的每个物体分配颜色
    void CaptureInstanceLabel()
    {
        Material[] originalMats = new Material[spawnedObjects.Length];
        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            // Gets the renderer component of the object
            originalMats[i] = spawnedObjects[i].GetComponent<Renderer>().material;
            // Sets the color of the object
            Material newMatInstance = new Material(instanceMat);
            newMatInstance.color = instanceColors[i];
            spawnedObjects[i].GetComponent<Renderer>().material = newMatInstance;
        }
        ScreenCapture.CaptureScreenshot("Assets/images/instance/instance_label_" + captureCounter + ".png");
        Debug.Log("Instance label image_" + captureCounter + ".png" + " has been saved.");
        // Use coroutines to delay recovery of the original material
        StartCoroutine(RestoreMaterials(originalMats));
    }
    // 拍摄语义标签的方法
    void CaptureLanguageLabel()
    {
        Material[] originalMats = new Material[spawnedObjects.Length];
        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            // Save the original material
            originalMats[i] = spawnedObjects[i].GetComponent<Renderer>().material;
            Material newMatInstance = new Material(instanceMat);
                switch (spawnedObjects[i].tag)
            {                
                case "Apple":                 
                    newMatInstance.color = Color.red; 
                    spawnedObjects[i].GetComponent<Renderer>().material = newMatInstance;
                    break;
                case "Banana":
                    newMatInstance.color = Color.yellow; 
                    spawnedObjects[i].GetComponent<Renderer>().material = newMatInstance;
                    break;
            }
            
        }
        ScreenCapture.CaptureScreenshot("Assets/images/language/language_label_" + captureCounter + ".png");
        Debug.Log("Language label image_" + captureCounter + ".png" + " has been saved.");
        StartCoroutine(RestoreMaterials(originalMats));
    }

    //用于恢复材质的协程

    IEnumerator RestoreMaterials(Material[] originalMats)
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < originalMats.Length; i++)
        {
            if (i < spawnedObjects.Length && originalMats[i] != null)
            {
                spawnedObjects[i].GetComponent<Renderer>().material = originalMats[i];
            }
        }
    }

}
