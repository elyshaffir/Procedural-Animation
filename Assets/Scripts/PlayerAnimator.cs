using UnityEngine;

class PlayerAnimator : MonoBehaviour
{
    Animator animator;
    float progress;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        progress += .05f;
        if (progress >= 1)
        {
            progress = 0;
            Debug.Log("RESET");
        }
        animator.SetFloat("RunProgress", progress);
    }
}
