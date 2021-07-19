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
    string ShootingCollider = "SphereCollider";
    NavMeshAgent agent;
    Animator anime;
    GameObject[] WayPoints = new GameObject[11];
    GameObject mainSkin, RedSkin;
    bool IsRight;
    bool IsHit=false;
    GameController game;
    int destMax;
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
        }
        else if (game.Level == 2)
        {
            destMax = 9;
        }
        else
        {
            if (transform.parent.name == "North Titan" || transform.parent.name == "South Titan")
                destMax = 8;
            else
                destMax = 7;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsHit)
        {
            if (other.CompareTag(point))
            {
                if (other.gameObject == WayPoints[Destination])
                {
                    game.Tauntings[game.tauntingDire].SetActive(false);
                    stepIndex = Destination;
                    agent.isStopped = true;
                    anime.SetTrigger("idle");
                    GameController.MoveLock = false;
                    if (stepIndex == destMax)
                    {
                        attack();
                        if (GameController.TitansInRange < 8)
                        {
                            game.ActionUnlock();
                            return;
                        }
                    }
                    game.NextTurn();
                }
                

            }
            if (other.CompareTag(ShootingCollider))
            {
                game.BarFill.fillAmount +=0.125f;
                GameController.TitansInRange++;
                if (GameController.TitansInRange==8)
                {
                    game.PressSpace.SetActive(true);
                    game.Fire.Post(game.gameObject);
                }
                RedSkin.SetActive(true);
                mainSkin.SetActive(false);
            }
        }
        
    }
    public void MoveTitanTurn(int stepsNumber)
    {
        GameController.MoveLock = true;
        agent.isStopped = false;
        anime.SetTrigger("run");
        Destination = stepIndex + stepsNumber;
        if (Destination>destMax)
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
        if (stepIndex==destMax)
        {
            if (Random.Range(0, 2) == 1)
                anime.SetTrigger("attack1");
            else
                anime.SetTrigger("attack2");
        }
        else
        {
            anime.SetTrigger("Run");
            agent.isStopped = false;
            agent.SetDestination(WayPoints[destMax].transform.position);
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
        game.GiantAttack.Post(game.gameObject);
    }
    public void GiantStep()
    {
        game.GiantStep.Post(game.gameObject);
    }
}
