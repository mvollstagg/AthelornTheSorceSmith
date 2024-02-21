using UnityEngine;

[CreateAssetMenu(menuName = "MyAnimation")]
public class SoMyAnimation : ScriptableObject
{
    public AnimationClip start;
    public AnimationClip idle;
    public AnimationClip active;
    public AnimationClip finish;
}