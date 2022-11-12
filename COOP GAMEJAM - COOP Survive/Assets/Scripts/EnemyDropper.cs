using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropper : MonoBehaviour
{
    public GameObject enemy;
    [SerializeField] private float minTimeBetweenDrops = 15;
    [SerializeField] private float maxTimeBetweenDrops = 20;
    private bool drop = false;
    private float nextShootTime = 5;
    public Transform[] dropPoints;

    void Start()
    {
        StartCoroutine(DelayBySecond(nextShootTime));
    }

    // Update is called once per frame
    void Update()
    {
        if (drop)
        {
            Drop();
        }
    }

    private void Drop()
    {
        int ran = Random.Range(0, 2);
        Transform target = dropPoints[ran];
        GameObject ball = Instantiate(enemy, target.position, transform.rotation);
        drop = false;
        nextShootTime = Random.Range(minTimeBetweenDrops, maxTimeBetweenDrops);
        StartCoroutine(DelayBySecond(nextShootTime));
    }

    IEnumerator DelayBySecond(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        drop = true;
    }

    public void IncreaseDrops()
    {
        minTimeBetweenDrops /= 1.2f;
        maxTimeBetweenDrops /= 1.2f;
    }
}
