using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StackCollector : MonoBehaviour
{
    public float CollecterMaxScale;
    public float CollecterMinScale;
    public MeshRenderer Holder;
    public MeshRenderer Stick1;
    public MeshRenderer Stick2;
    public MeshRenderer Head;

    [HideInInspector]
    public List<Stack> CollectedStacks = new List<Stack>();
    private Material materialClone;

    private float perStackHeightDistance = 0.38f;

    private void Start()
    {
        SuperPowerController.OnSuperPowerActivated += OnSuperPowerActivated;
        GameManager.instance.OnGameStarted += OnGameStarted;
    }

    public void SetUpMaterial()
    {
        materialClone = new Material(GameManager.instance.PlayerManager.MaterialSource);
        Holder.material = materialClone;
        Stick1.material = materialClone;
        Stick2.material = materialClone;
        Head.material = materialClone;
    }

    public void ChangeColour(BaseColour colour)
    {
        materialClone.color = GameManager.instance.ColourController.GetColour(colour);

        foreach (var item in CollectedStacks)
            item.ChangeColour(colour);
    }

    public void ResetJointSettings()
    {
        Destroy(GameManager.instance.PlayerManager.Player.GetComponent<FixedJoint>());

        foreach (var item in CollectedStacks)
        {
            item.Rigidbody.isKinematic = false;
            item.EnableElastic(false);
            item.ThrowAway();
        }
        GameManager.instance.GameState = GameStates.GameFinished;

        Destroy(gameObject);
    }

    private void UseMaxScale()
    {
        Head.transform.localScale = new Vector3(CollecterMaxScale, transform.localScale.y, transform.localScale.z);
    }

    private void UseMinScale()
    {
        Head.transform.localScale = new Vector3(CollecterMinScale, transform.localScale.y, transform.localScale.z);
    }

    private void BalanceMassScale(float mass)
    {
        GameManager.instance.PlayerManager.Player.Rigidbody.mass += mass;
        GetComponent<Rigidbody>().mass += mass;

        GameManager.instance.PlayerManager.Player.Rigidbody.mass += mass;
    }

    private void PlaySound()
    {
        if (CollectedStacks.Last().IsBigStack)
        {
            GameManager.instance.AudioManager.PlayClip("CollectBigBrick");
        }
        else
        {
            GameManager.instance.AudioManager.PlayClip("CollectBrick");
        }
    }

    private void DoSomething()
    {
        for (int i = 0; i < CollectedStacks.Count; i++)
            CollectedStacks[i].EnableElastic(true, transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == Constants.TAG_STACK)
        {
            if (collision.collider.GetComponent<Stack>().CurrentColour == GameManager.instance.PlayerManager.CurrentColour)
            {
                CollectedStacks.Add(collision.collider.GetComponent<Stack>());

                if (CollectedStacks.Count == 1)
                {
                    Debug.Log("Calisti "+ CollectedStacks.Count);
                    CollectedStacks.Last().MoveOverCollecter(new Vector3(transform.position.x, 0.15f, transform.position.z), DoSomething);
                    CollectedStacks.Last().Elastic.AnimationSpeed = GameManager.instance.StackManager.MaxStackWaveStrength;
                }
                else
                {
                    //float perAnimationRange = CollectedStacks.Count / GameManager.instance.StackManager.MaxStackWaveStrength;

                    CollectedStacks.Last().MoveOverCollecter(new Vector3(transform.position.x, 0.15f, transform.position.z), DoSomething);
                    CollectedStacks.Last().Elastic.AnimationSpeed = GameManager.instance.StackManager.MaxStackWaveStrength;

                    for (int i = 0; i < CollectedStacks.Count - 1; i++)
                    {
                        CollectedStacks[i].MoveOverCollecter(new Vector3(
                            transform.position.x, CollectedStacks[i].transform.position.y + perStackHeightDistance / 2f,
                            transform.position.z),
                            DoSomething);

                        CollectedStacks[i].Elastic.AnimationSpeed -= GameManager.instance.StackManager.PerStackWaveReductionAmount;
                        BalanceMassScale(CollectedStacks[i].Rigidbody.mass);

                        if (CollectedStacks[i].Elastic.AnimationSpeed < GameManager.instance.StackManager.MinStackWaveStrength)
                            CollectedStacks[i].Elastic.AnimationSpeed = GameManager.instance.StackManager.MinStackWaveStrength;
                    }
                }

                PlaySound();

                if (collision.transform.name == "Stack")
                {
                    StartCoroutine(collision.transform.GetComponent<Stack>().CubeCreate());
                }
                else
                {

                }

                GameManager.instance.SuperPowerController.AddPower(CollectedStacks.Last().Point);
                GameManager.instance.ScoreController.CurrentCollectedStackNumber++;
                GameManager.instance.StackManager.Stacks.Remove(collision.collider.GetComponent<Stack>());
            }
            else
            {
                GameManager.instance.SuperPowerController.SubPower(collision.collider.GetComponent<Stack>().Point);
                GameManager.instance.StackManager.Stacks.Remove(collision.collider.GetComponent<Stack>());
                Destroy(collision.collider.gameObject);
            }
        }
    }

    #region Events

    private void OnSuperPowerActivated(bool IsActivated)
    {
        if (IsActivated)
            UseMaxScale();
        else
            UseMinScale();
    }

    private void OnGameStarted()
    {
      
    }

    private void OnDestroy()
    {
        SuperPowerController.OnSuperPowerActivated -= OnSuperPowerActivated;
        GameManager.instance.OnGameStarted -= OnGameStarted;
    }

    #endregion
}