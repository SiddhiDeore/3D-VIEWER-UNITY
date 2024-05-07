using UnityEngine;
using UnityEngine.UI;

public class UpDownMovement : MonoBehaviour
{
    public float upSpeed = 5f; 
    public float downSpeed = 2f; 
   
    private bool isMovingUp = false;
    private bool isMovingDown = false;

   

    void FixedUpdate()
    {
        if (isMovingUp)
        {
            transform.Translate(Vector3.up * upSpeed * Time.deltaTime);

        }
        
        if (isMovingDown)
        {
            transform.Translate(Vector3.down * downSpeed * Time.deltaTime);

        }
    }

   public void MoveUp()
    {
        isMovingUp = !isMovingUp;
    }

    public void MoveDown()
    {
        isMovingDown = !isMovingDown;
    }
}
