using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    private Text clockTimeText; 
    public float time = 01;

    private BallDropper ballDropper;
    private EnemyDropper enemyDropper;
    private bool increase = true;

    // Start is called before the first frame update
    void Start()
    {
        clockTimeText = GetComponent<Text>();
        ballDropper = FindObjectOfType<BallDropper>();
        enemyDropper = FindObjectOfType<EnemyDropper>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        TimeShow();

        if (Mathf.FloorToInt(time) % 30 == 0 && increase)
        {
            ballDropper.IncreaseDrops();
            enemyDropper.IncreaseDrops();
            increase = false;
            StartCoroutine(DelayBySecond(5));
        }
    }

    private void TimeShow()
    {
        int minutes = Mathf.FloorToInt(time/60);
        int seconds = Mathf.FloorToInt(time - minutes * 60);

        clockTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    IEnumerator DelayBySecond(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        increase = true;
    }
}
