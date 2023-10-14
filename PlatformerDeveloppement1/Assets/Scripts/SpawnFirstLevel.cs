using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFirstLevel : MonoBehaviour
{
    [SerializeField] private GameObject Background;
    Vector3 BackgroundStartPos;
    [SerializeField] private GameObject[] Decors;
    List<Vector3> DecorStartPos = new List<Vector3>();
    [SerializeField] private GameObject[] Obstacles;
    List<Vector3> ObstaclesStartPos = new List<Vector3>();
    [SerializeField] private float[] timeBtwCategories;
    [SerializeField] private float timeBtwObjects = 0.01f;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        SaveEveryStartPos();
        MoveObjectsOutOfScreen();
        player.SetActive(false);
        StartCoroutine(SlideObjectsToTheScreen());
    }
    void SaveEveryStartPos()
    {
        BackgroundStartPos = Background.transform.position;
        foreach (GameObject decor in Decors)
        {
            DecorStartPos.Add(decor.transform.position);
        }
        foreach (GameObject obstacle in Obstacles)
        {
            ObstaclesStartPos.Add(obstacle.transform.position);
        }
    }
    void MoveObjectsOutOfScreen()
    {
        Background.transform.position = new Vector3(Background.transform.position.x, Background.transform.position.y + 20, Background.transform.position.z);
        foreach (GameObject decor in Decors)
        {
            decor.transform.position = new Vector3(decor.transform.position.x, decor.transform.position.y + 20, decor.transform.position.z);
        }
        foreach (GameObject obstacle in Obstacles)
        {
            obstacle.transform.position = new Vector3(obstacle.transform.position.x, obstacle.transform.position.y + 20, obstacle.transform.position.z);
        }
    }
    IEnumerator SlideObjectsToTheScreen()
    {
        yield return new WaitForSeconds(timeBtwCategories[0]);
        StartCoroutine(SlideBackgroundToTheScreen());
        yield return new WaitForSeconds(timeBtwCategories[1]);

        for (int i = 0; i < Decors.Length; i++)
        {
            StartCoroutine(SlideADecorWithADelay(i));
        }

        yield return new WaitForSeconds(timeBtwCategories[2]);

        for (int i = 0; i < Obstacles.Length; i++)
        {
            StartCoroutine(SlideAnObstacleWithADelay(i));
        }

        yield return new WaitForSeconds(timeBtwCategories[3]);
        player.SetActive(true);
    }
    IEnumerator SlideBackgroundToTheScreen()
    {
        float time = 0;
        while (time < 1)
        {
            Background.transform.position = Vector3.Lerp(Background.transform.position, BackgroundStartPos, time);
            time += Time.deltaTime;
            yield return null;
        }
        Background.transform.position = BackgroundStartPos;
    }
    IEnumerator SlideADecorWithADelay(int i )
    {
        yield return new WaitForSeconds(timeBtwObjects * i);
        float time = 0;
        while (time < 1)
        {
            Decors[i].transform.position = Vector3.Lerp(Decors[i].transform.position, DecorStartPos[i], time);
            time += Time.deltaTime;
            yield return null;
        }
        Decors[i].transform.position = DecorStartPos[i];
    }
    IEnumerator SlideAnObstacleWithADelay(int i)
    {
        yield return new WaitForSeconds(timeBtwObjects * i);
        float time = 0;
        while (time < 1)
        {
            Obstacles[i].transform.position = Vector3.Lerp(Obstacles[i].transform.position, ObstaclesStartPos[i], time);
            time += Time.deltaTime;
            yield return null;
        }
        Obstacles[i].transform.position = ObstaclesStartPos[i];
    }
}