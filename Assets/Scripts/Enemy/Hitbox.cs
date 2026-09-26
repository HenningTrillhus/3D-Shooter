using UnityEngine;

public enum BodyPart
{
    Head,
    Torso
}

public class Hitbox : MonoBehaviour
{
    public BodyPart bodyPart;
    public float damageMultiplier = 1f; // f.eks. hode = 2x skade, bein = 0.75x

    [Tooltip("Referanse til hoved-helse-scriptet på roten av karakteren")]
    public EnemyHealth ownerHealth; // dra inn root-objektets helse-script her
}