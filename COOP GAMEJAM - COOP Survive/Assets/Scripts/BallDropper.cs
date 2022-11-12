using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDropper : MonoBehaviour
{
    public GameObject canonBall;
    [SerializeField] private float minTimeBetweenShots = 15;
    [SerializeField] private float maxTimeBetweenShots = 20;
    private bool shoot = false;
    private Transform heroKnight;
    private float nextShootTime = 5;
    


   
    void Start()
    {
        heroKnight = FindObjectOfType<HeroKnightController>().GetComponent<Transform>();
        
        StartCoroutine(DelayBySecond(nextShootTime));
    }

    // Update is called once per frame
    void Update()
    {
        if (shoot)
        {
            Drop();
        }
        
    }



    private void Drop()
    {
        Vector2 target = new Vector2(heroKnight.position.x, transform.position.y);
        GameObject ball = Instantiate(canonBall, target, transform.rotation);
        shoot = false;
        nextShootTime = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        StartCoroutine(DelayBySecond(nextShootTime));
    }

    IEnumerator DelayBySecond(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        shoot = true;
    }

    public void IncreaseDrops()
    {
        minTimeBetweenShots /= 1.5f;
        maxTimeBetweenShots /= 1.5f;
    }

}
