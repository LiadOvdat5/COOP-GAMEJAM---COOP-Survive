using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxEffect : MonoBehaviour
{
    [SerializeField]
    private float parallaxSpeed;
    
    private Transform cameraTransform;
    private float startPosX;
    private float spriteSizeX;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        startPosX = transform.position.x;
        spriteSizeX = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float relativeDist = cameraTransform.position.x * parallaxSpeed;
        transform.position = new Vector3(startPosX + relativeDist, transform.position.y, transform.position.z);

        //Loop parallax effect
        float relativeCameraDist = cameraTransform.position.x * (1 - parallaxSpeed);
        if(relativeCameraDist > startPosX + spriteSizeX)
        {
            startPosX += spriteSizeX;
        } 
        else if (relativeCameraDist < startPosX - spriteSizeX)
        {
            startPosX -= spriteSizeX;
        }
    }
}
