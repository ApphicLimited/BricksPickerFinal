﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float ForwardSpeed;
    public float Speed;
    public float AnimatorSpeed;
    public BaseColour BaseColour;
    public SkinnedMeshRenderer SkinnedMeshRenderer;
    public StackCollector StackCollector;
    public ParticleSystem ColourChnagerParticleEffect;
    public Animator Animator;
    public Rigidbody Rigidbody;

    public BaseColour CurrentBaseColour { get; set; }

    private Material materialClone;
    private Vector3 nextPosition;
    private bool IsArrived;
    // Start is called before the first frame update
    void Start()
    {
        CurrentBaseColour = BaseColour;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.GameState != GameStates.GameOnGoing)
            return;

        if (Math.Abs(GameManager.instance.PlayerManager.EndTransform.position.z- transform.position.z) < 3f)
        {
            if (IsArrived == false)
                ArrivedDest();
            return;
        }

        StackCollector.transform.position = new Vector3(transform.position.x, StackCollector.transform.position.y, transform.position.z + 1.9f);

        transform.Translate(Vector3.forward * Time.deltaTime * ForwardSpeed);
        nextPosition.z = transform.position.z;

        Animator.speed = AnimatorSpeed;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.SmothFollow.CameraMovement.IsApproachedToEndPoint)
            return;

        float step = Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, step);
    }

    public void SetUpMaterial()
    {
        StackCollector.SetUpMaterial();

        materialClone = new Material(GameManager.instance.PlayerManager.MaterialSource);
        SkinnedMeshRenderer.material = materialClone;
        materialClone.color = GameManager.instance.ColourController.GetColour(CurrentBaseColour);
    }

    public void PlayKickAnim()
    {
        Animator.SetBool("Kicking", true);
        Animator.SetBool("Running", false);

        Animator.speed = 1;
    }

    public void MoveToSide(Vector3 position)
    {
        nextPosition = new Vector3(position.x, transform.position.y, transform.position.z);
    }

    public void ChangeColour(BaseColour colour,bool PlayParticle)
    {
        CurrentBaseColour = colour;
        materialClone.color = GameManager.instance.ColourController.GetColour(colour);
        StackCollector.ChangeColour(colour);

        if (PlayParticle)
        {
            ColourChnagerParticleEffect.gameObject.SetActive(true);
            ColourChnagerParticleEffect.startColor = GameManager.instance.ColourController.GetColour(colour);
            ColourChnagerParticleEffect.Play();
        }
    }

    private void ArrivedDest()
    {
        IsArrived = true;

        Destroy(GetComponent<FixedJoint>());
        Rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;

        StackCollector.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionZ;
        StackCollector.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 5f);
        PlayKickAnim();
        //Jump up
        Rigidbody.velocity = new Vector3(0f, 5f, 1f);
        GameManager.instance.TimeController.DoSlowMotion();
        GameManager.instance.GameState = GameStates.GamePaused;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag==Constants.TAG_COIN)
        {
            GameManager.instance.CoinController.CollectedCoins++;
            other.GetComponent<Coin>().DisAppear();
        }
    }

    #region Animation Event

    public void AtEndOfKickingAnim()
    {
        GameManager.instance.SmothFollow.GoForward();
        StackCollector.ResetJointSettings();
        GameManager.instance.TimeController.StopSlowMotion();
        GameManager.instance.SuperPowerController.KickPowerBar.gameObject.SetActive(false);
    }

    #endregion
}