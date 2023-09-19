using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HurdleController : MonoBehaviour
{
    private float speed = 15f;
    private bool canRun = false;
    private bool isMarked = false;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (canRun)
        {
            // move the hurdle to the left
            transform.Translate(Vector3.left * Time.deltaTime * speed);

            if (transform.position.x < 8)
            {
                if (!isMarked)
                {
                    isMarked = true;
                    gameManager.IncreaseScore();
                }
            }


            if (transform.position.x <= 0)
            {
                int randomZ = Random.Range(-2, 2) > 0 ? 4 : -4;
                transform.position = new Vector3(150, transform.position.y, randomZ);
                isMarked = false;
            }

        }
    }
    public void StartRun()
    {
        canRun = true;
    }
    public void StopRun()
    {
        canRun = false;
    }
}
