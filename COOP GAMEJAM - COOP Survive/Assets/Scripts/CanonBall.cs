using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBall : MonoBehaviour
{
    public float damage = 15f;
    public GameObject explosionPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Hittable>().TakeHit(damage);
        }

        Destroy(gameObject);
        GameObject expo = Instantiate(explosionPrefab, transform.position, transform.rotation);
        FindObjectOfType<AudioManager>().Play("Explosion");
        Destroy(expo, 0.75f);
        
    }
   
    

    
}
