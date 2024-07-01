using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Produce : MonoBehaviour
{
    // Start is called before the first frame update
    public int spawncount;
    void Start()
    {

    }

    // Update is called once per frame
    public GameObject objectToSpawn; // 要生成的物体
    public KeyCode spawnKey = KeyCode.Space; // 触发生成的按键
    public float minSpawnHeight = 1.0f; // 随机高度的最小值
    public float maxSpawnHeight = 3.0f; // 随机高度的最大值

    void Update()
    {
        // 检测是否按下指定按键
        if (Input.GetKeyDown(spawnKey))
        {
            for (int i = 0; i < spawncount; i++)
            {
                // 随机生成高度
                float randomHeight = Random.Range(minSpawnHeight, maxSpawnHeight);

                // 随机生成位置
                Vector3 randomPosition = new Vector3(
                    Random.Range(-1.0f, 1.0f), // X轴范围
                    randomHeight, // 随机高度
                    Random.Range(-1.0f, 1.0f) // Z轴范围
                );
                Quaternion randomRotation = Quaternion.Euler(
                    Random.Range(0, 360), // X轴旋转
                    Random.Range(0, 360), // Y轴旋转
                    Random.Range(0, 360)  // Z轴旋转
                );

                // 在随机位置生成新物体
                GameObject spawnedObject = Instantiate(objectToSpawn, randomPosition, randomRotation);
            }
        }
    }
}
