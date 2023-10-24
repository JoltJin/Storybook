using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CharacterAnimator
{
    void Talking();
    void StopTalking();

    void FlipAnimation(float direction);
    void BasicAnimations(float horizontal, float vertical);
    
}
