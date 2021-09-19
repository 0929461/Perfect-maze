using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private int _walkSpeed = 3;

    void Start()
    {
        transform.position = new Vector3(0, 2, 0);
    }

    void Update()
    {
        PlayerMovement();
    }
    private void PlayerMovement()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 userInput = new Vector3(horizontalMove, 0, verticalMove);
        transform.Translate(userInput * _walkSpeed * Time.deltaTime);
    }
}
