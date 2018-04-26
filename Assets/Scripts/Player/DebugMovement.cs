using Assets.Scripts.Items;
using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovement : MonoBehaviour
{
    public bool Debug = false;

    private const float ROTATIONSPEED = 360;
    public float Sensitivity = 0.9f;
    public bool Inversed = false;
    public float Speed = 5;
    public float Accelaration = 5;

    public GameObject gun;
    private VR_Controller_Gun igun;
    private bool pressed;

    // Use this for initialization
    void Start()
    {
        if (!Debug) return;

        gun.transform.parent = transform;
        gun.transform.position.Set(0, 0, 0);
        gun.transform.rotation = Quaternion.Euler(45, 0, 0);
        gun.gameObject.SetActive(true);

        igun = gun.GetComponent<VR_Controller_Gun>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!Debug) return;

        float horAxis = Input.GetAxisRaw("Horizontal");
        float vertAxis = Input.GetAxisRaw("Vertical");

        Vector3 velocity = transform.rotation * (new Vector3(horAxis, 0, vertAxis) * Accelaration) * Time.deltaTime;
        velocity.y = 0;

        float vel = (float)Math.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z) * Time.deltaTime;

        if (vel > Speed)
        {
            float velx = velocity.x / vel * Speed * Time.deltaTime;
            float velz = velocity.z / vel * Speed * Time.deltaTime;

            Vector3 vecvel = new Vector3(velx, velocity.y, velz);
            velocity = velocity.normalized * Speed;

        }

        transform.position += velocity;

        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        Vector3 rot1 = new Vector3(mouseY, 0, 0) * -Sensitivity * ROTATIONSPEED * Time.deltaTime * (Inversed ? -1 : 1);
        transform.Rotate(rot1);
        transform.RotateAround(transform.position, Vector3.up, mouseX * Sensitivity * ROTATIONSPEED * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space) && !pressed)
        {
            igun.PlayerGunInterface.Shoot();
            pressed = true;
        }
        else if (!Input.GetKey(KeyCode.Space))
        {
            pressed = false;
        }

    }
}
