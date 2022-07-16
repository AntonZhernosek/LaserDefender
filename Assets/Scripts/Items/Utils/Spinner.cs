using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] bool randomizeRotationDirection = true;
    [SerializeField] float minAngleSpeed;
    [SerializeField] float maxAngleSpeed;
    float angleIncreaseSpeed;
    Vector3 spinRotation;

    private void Start()
    {
        angleIncreaseSpeed = Random.Range(minAngleSpeed, maxAngleSpeed);

        RandomizeRotationDirection();

        ChangeRotation(Random.Range(0f, 180f));
    }

    void Update()
    {
        ChangeRotation(Time.deltaTime * angleIncreaseSpeed);
    }

    private void RandomizeRotationDirection()
    {
        if (randomizeRotationDirection)
        {
            angleIncreaseSpeed *= Random.Range(0, 2) * 2 - 1;
        }
    }

    private void ChangeRotation(float rotationAmount)
    {
        spinRotation.z += rotationAmount;
        Quaternion rotation = Quaternion.Euler(spinRotation);
        transform.rotation = rotation;
    }


}
