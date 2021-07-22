using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TitanMove : MonoBehaviour
{
    [HideInInspector]
    public int stepIndex = 0;
    [HideInInspector]
    public bool InShootingRange = false;
    int Destination;
    string point = "WayPoint";
    NavMeshAgent agent;
    Animator anime;
    GameObject[] WayPoints = new GameObject[11];
    GameObject mainSkin, RedSkin;
    bool IsRight;
    bool IsHit=false;
    GameController game;
    int destMax,destDanger,titanIndex;
    
    void Start()
    {
        game = GameObject.Find("Base").GetComponent<GameController>();
        agent = GetComponent<NavMeshAgent>();
        anime = GetComponent<Animator>();
        mainSkin = transform.GetChild(7).gameObject;
        RedSkin = transform.GetChild(9).gameObject;
        RedSkin.SetActive(false);
        if (transform.GetSiblingIndex() == 0)
            IsRight = true;
        else
            IsRight = false;

        int counter = 0;
        Transform WayParent;
        if (IsRight)
            WayParent = transform.parent.GetChild(2).GetChild(0);
        else
            WayParent = transform.parent.GetChild(2).GetChild(1);
        for (int i = 0; i < 11; i++)
        {
            WayPoints[i] = WayParent.GetChild(i).gameObject;
            counter++;
        }
        if (game.Level == 1)
        {
            destMax = 10;
            destDanger = 8;
        }
        else if (game.Level == 2)
        {
            destMax = 9;
            destDanger = destMax - 1;
        }
        else
        {
            if (transform.parent.name == "North Titan" || transform.parent.name == "South Titan")
                destMax = 8;
            else
                destMax = 7;
            destDanger = destMax - 1;
        }
        for (int i = destMax; i < WayPoints.Length; i++)
        {
            Destroy(WayPoints[i].GetComponent<MeshRenderer>());
            Destroy(WayPoints[i].GetComponent<MeshFilter>());
        }
        int dangerRange = destMax - destDanger;
        for (int i = 1; i < dangerRange+1; i++)
            WayPoints[destMax - i].GetComponent<MeshRenderer>().material = game.plasmaGroundMat;
        titanIndex = transform.GetSiblingIndex();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsHit)
        {
            if (other.CompareTag(point))
            {
                if (other.transform.GetSiblingIndex() >= destDanger && !InShootingRange)
                {
                    InShootingRange = true;
                    game.BarFill.fillAmount += 0.125f;
                    GameController.TitansInRange++;
                    if (GameController.TitansInRange == 8)
                    {
                        game.PressSpace.SetActive(true);
                        game.Fire.Post(game.gameObject);
                    }
                    RedSkin.SetActive(true);
                    mainSkin.SetActive(false);
                }
                if (other.gameObject == WayPoints[Destination])
                {
                    if (titanIndex>0)
                    {
                        game.Tauntings[game.tauntingDire].SetActive(false);
                    }
                    
                    stepIndex = Destination;
                    agent.isStopped = true;
                    anime.SetTrigger("idle");
                    if (stepIndex == destMax)
                    {
                        attack();
                        if (GameController.TitansInRange < 8)
                            game.ActionUnlock();
                    }
                    else
                        if (titanIndex>0)
                        game.NextTurn();

                }
            }
            
        }
        
    }
    public void MoveTitanTurn(int stepsNumber)
    {
        if (titanIndex==0)
            StartCoroutine(diffrentMove(stepsNumber));
        else
        Move(stepsNumber);
    }
    IEnumerator diffrentMove(int move)
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        Move(move);
    }
    void Move(int move)
    {
        agent.isStopped = false;
        anime.SetTrigger("run");
        Destination = stepIndex + move;
        if (Destination > destMax)
            Destination = destMax;
        agent.SetDestination(WayPoints[Destination].transform.position);
    }
    public void Shoot()
    {
        IsHit = true;
        agent.isStopped = true;
        anime.SetTrigger("take_damage");
    }
    public void Die()
    {
        anime.SetTrigger("death");
    }
    public void attack()
    {
        IsHit = false;
        if (stepIndex==destMax)
        {
            if (Random.Range(0, 2) == 1)
                anime.SetTrigger("attack1");
            else
                anime.SetTrigger("attack2");
        }
        else
        {
            anime.SetTrigger("run");
            agent.isStopped = false;
            MoveTitanTurn(10);
            StartCoroutine(secondAttack());
        }    
        
    }
    public void Lose()
    {
        game.Lost();
    }
    IEnumerator  secondAttack()
    {
        yield return new WaitForSeconds(2);
        if (Random.Range(0, 2) == 1)
            anime.SetTrigger("attack1");
        else
            anime.SetTrigger("attack2");
    }
    public void GiantAttack()
    {
        if (!IsHit)
        game.GiantAttack.Post(game.gameObject);
    }
    public void GiantStep()
    {
        game.GiantStep.Post(game.gameObject);
    }
}
