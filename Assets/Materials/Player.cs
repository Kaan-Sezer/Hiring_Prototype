using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject road;
    [SerializeField] GameObject startScreen;
    [SerializeField] List<GameObject> cubes = new List<GameObject>();
    [SerializeField] Ease easeType = Ease.InBounce;  
    [Range(0, 5), SerializeField] float forwardMoveSpeed;
    [Range(0, 5), SerializeField] float sideMoveSpeed;
    [Range(0.1f, 0.5f), SerializeField] float animDuration;


    void Start()
    {
        Time.timeScale = 0;
        cubes.Add(transform.GetChild(1).gameObject);                
    }

    void Update()
    {
        ForwardMove();
        SideMove();
        RearrangeCubes();
    }

    private void RearrangeCubes()
    {
        foreach (GameObject gameObject in cubes)
        {
            gameObject.transform.position = new Vector3
                (
                Mathf.Lerp(gameObject.transform.position.x, transform.position.x, Time.deltaTime * 5),
                gameObject.transform.position.y,
                Mathf.Lerp(gameObject.transform.position.z, transform.position.z, Time.deltaTime * 5)
                );
        }
    }

    private void SideMove()
    {
        float roadWidth = road.transform.localScale.x;
        float limit = roadWidth / 2 - 0.5f;

        if (transform.position.x >= -limit)
        {
            if (Input.GetKey("d"))
            {
                transform.position = Vector3.Lerp(transform.position, transform.position - Vector3.right * Time.deltaTime * sideMoveSpeed, 1);
            }
        }

        if (transform.position.x <= limit)
        {
            if (Input.GetKey("a"))
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right * Time.deltaTime * sideMoveSpeed, 1);
            }
        }
    }

    private void ForwardMove()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position - Vector3.forward * Time.deltaTime * forwardMoveSpeed, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Green"))
        {
            Jump();
            AddCube(other);
        }

        if (other.transform.CompareTag("Red"))
        {
            RemoveCube(other);
            StartCoroutine(Drop());
        }
    }

    private void Jump()
    {
        foreach (GameObject gameObject in cubes)
        {
            gameObject.transform.DOMoveY(gameObject.transform.position.y + 2.1f, animDuration).SetEase(easeType).OnComplete(() => gameObject.transform.DOMoveY(gameObject.transform.position.y - 1, animDuration));
        }
    }

    private void AddCube(Collider other)
    {
        other.GetComponent<BoxCollider>().enabled = false;
        other.transform.parent = transform;
        cubes.Add(other.gameObject);
    }

    private void RemoveCube(Collider other)
    {
        other.GetComponent<BoxCollider>().enabled = false;
        cubes[cubes.Count - 1].transform.parent = null;
        cubes.Remove(cubes[cubes.Count - 1]);
    }    

    IEnumerator Drop()
    {
        yield return new WaitForSeconds(0.2f);

        foreach (GameObject gameObject in cubes)
        {
            gameObject.transform.DOMoveY(gameObject.transform.position.y - 1.1f, animDuration).SetEase(easeType);
        }
    }

    public void StartButton()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        startScreen.SetActive(false);
    }
}
