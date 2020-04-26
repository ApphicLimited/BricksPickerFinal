﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    public BaseColour MainColour;
    public int Point;
    public bool IsBigStack;
    public Elastic Elastic;
    public MeshRenderer MeshRenderer;
    public Rigidbody Rigidbody;
    public Animation animation;

    public BaseColour CurrentColour { get; set; }

    private Material materialClone;

    public delegate void AfterMoved();
    public AfterMoved AfterMovedDoAction;

    private void Start()
    {
        animation = GetComponent<Animation>();
        SetUpMaterial();
        ChangeColour(MainColour);
    }

    private void Update()
    {
        if (GameManager.instance.GameState != GameStates.GameOnGoing)
            return;
    }

    private void SetUpMaterial()
    {
        materialClone = new Material(GameManager.instance.StackManager.MaterialSource);
        MeshRenderer.material = materialClone;
    }

    public void ChangeColour(BaseColour colour)
    {
        CurrentColour = colour;
        if (materialClone == null)
            SetUpMaterial();

        materialClone.color = GameManager.instance.ColourController.GetColour(colour);
    }

    public void ResetColour()
    {
        CurrentColour = MainColour;
        if (materialClone == null)
            SetUpMaterial();

        materialClone.color = GameManager.instance.ColourController.GetColour(MainColour);
    }

    public void EnableElastic(bool isEnable, Transform _transform = null)
    {
        if (isEnable)
        {
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;
            Rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
            GetComponent<BoxCollider>().enabled = true;

            if(animation.isPlaying)
            {
                animation.Stop();
            }
            if(transform.name == "Stack")
            {
                animation.Play("BrickScale");
            }
            else
            {
                animation.Play("BrickScale2");
            }
            


        }
        else
        {
            Rigidbody.constraints &= ~RigidbodyConstraints.FreezeAll;
            GetComponent<BoxCollider>().enabled = true;
        }

        if (_transform != null)
            Elastic.Target = _transform;

        Elastic.enabled = isEnable;
    }

    public void ThrowAway()
    {
        Rigidbody.AddForce(Vector3.forward * GameManager.instance.StackManager.StackThrowingForce, ForceMode.Impulse);
        Destroy(this);
    }

    public void MoveOverCollecter(Vector3 newPos, AfterMoved action = null)
    {
        transform.position = newPos;

        AfterMovedDoAction = action;
        DoAction();
    }

    private void DoAction()
    {
        AfterMovedDoAction?.Invoke();
        AfterMovedDoAction = null;
    }
}