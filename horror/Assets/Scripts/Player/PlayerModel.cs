using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    private List<int> animList = new List<int>();
    private int animatorX;
    private int animatorY;
    private int jumpAnim;
    private int fallingAnim;

    // Start is called before the first frame update
    void Start()
    {
        animatorX = Animator.StringToHash("X_Velocity");
        animatorY = Animator.StringToHash("Y_Velocity");
        jumpAnim = Animator.StringToHash("Jump");
        fallingAnim = Animator.StringToHash("Falling");

        animList.Add(animatorX);
        animList.Add(animatorY);
        animList.Add(jumpAnim);
        animList.Add(fallingAnim);
    }

    public int GetAnimInt(int i)
    {
        return animList[i];
    }
}
