using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetreDetecter : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public BoxCollider BoxCollider;
    public float Metre;
    public Color Color;
    public Text TextFrontMetre;
    public Text TextBackMetre;
    public Image ImageBackMetre;

    private void Start()
    {
        TextFrontMetre.text = "X"+Metre.ToString();
        TextBackMetre.text = "X"+Metre.ToString();
        ImageBackMetre.color = Color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Constants.TAG_STACK)
        {
            GameManager.instance.ScoreController.FurtherStackMetre = Metre;
            GameManager.instance.SmothFollow.target = transform;
            TextFrontMetre.gameObject.SetActive(false);
            BoxCollider.enabled = false;
            MeshRenderer.enabled = false;
        }
    }
}
