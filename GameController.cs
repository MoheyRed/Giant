using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    bool loseLock;
    public GameObject StepBG;
    public int Level;
    GameObject[] Numbers=new GameObject[6];
    Transform titanFathers;
    GameObject[] NorthTitans = new GameObject[2];
    GameObject[] SouthTitans = new GameObject[2];
    GameObject[] EastTitans = new GameObject[2];
    GameObject[] WestTitans = new GameObject[2];
    public static int TitansInRange;
    int random=-1;
    bool ActionLock, QuickLock, endStart = false;
    WaitForSeconds waitMove, waitQuick;
    WaitForSeconds wait1 = new WaitForSeconds(1);
    GameObject[] tempTitans;
    GameObject[] titans = new GameObject[8];
    public GameObject[] soldiers;
    int Univiersalcounter = 0;
    public string[] Keys;
    public int[] numbers;
    public Sprite[] sprites;
    public GameObject[] quicksUI;
    public GameObject[] Tauntings;
    [HideInInspector]
    public int tauntingDire;
    public Image BarFill;
    public GameObject PressSpace;
    public ParticleSystem smoke;
    public GameObject gameOver;
    public GameObject winScreen;
    public GameObject DarkBack;
    public Material plasmaGroundMat;
    public AK.Wwise.Event Music,Fire,click,lose,Miss,QuickSound,randomSound,shoot,smokeLaunch,Taunt,WinSound,GiantAttack,GiantStep,stopAll;
    private void Awake()
    {
        titanFathers = GameObject.Find("Titans").transform;
        for (int i = 0; i < 6; i++)
        {
            Numbers[i] = StepBG.transform.GetChild(i).gameObject;
            Numbers[i].SetActive(false);
        }
        StepBG.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        TitansFill(NorthTitans, 0);
        TitansFill(EastTitans, 1);
        TitansFill(SouthTitans, 2);
        TitansFill(WestTitans, 3);
        TitansInRange = 0;
        if (Level == 1)
        {
            waitMove = new WaitForSeconds(3);
            waitQuick = new WaitForSeconds(2);
        }
        else if (Level == 2)
        {
            waitMove = new WaitForSeconds(1.5f);
            waitQuick = new WaitForSeconds(1);
        }
        else
        {
            waitMove = new WaitForSeconds(1);
            waitQuick = new WaitForSeconds(1);
        }
        keysRandomizer(Keys,sprites);
        keysRandomizer(numbers);
        Music.Post(gameObject);
        loseLock = false;
    }
    void Update()
    {
        if (ActionLock)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                TitanRegularMove(0, NorthTitans);
            if (Input.GetKeyDown(KeyCode.DownArrow))
                TitanRegularMove(2, SouthTitans);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                TitanRegularMove(1, EastTitans);
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                TitanRegularMove(3, WestTitans);
        }
        if (Input.GetKeyDown(KeyCode.Space) && !endStart)
        {
            
            if (TitansInRange == 8)
            {
                Tauntings[0].transform.parent.gameObject.SetActive(false);
                smoke.Play();
                smokeLaunch.Post(gameObject);
                DarkBack.SetActive(false);
                PressSpace.SetActive(false);
                StopAllCoroutines();
                ActionLock = false;
                Numbers[numbers[random]-1].SetActive(false);
                StepBG.SetActive(false);
                foreach (GameObject titan in titans)
                    titan.GetComponent<TitanMove>().Shoot();
                foreach (GameObject sold in soldiers)
                    sold.GetComponent<Actions>().Aiming();
                endStart = true;
                random = 0;
                StartCoroutine(FirstQuick());
            }
        }
        if (QuickLock)
        {
            if (Input.GetKeyDown(Keys[random]))
            {
                shoot.Post(gameObject);
                quicksUI[random].SetActive(false);
                soldiers[random].GetComponent<Actions>().Attack();
                titans[random].GetComponent<TitanMove>().Die();
                random++;
                if (random == 8)
                {
                    QuickLock = false;
                    StartCoroutine(Win());
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(QuickTimeEvent());
                }

            }
            else if (Input.anyKeyDown&&!Input.GetKeyDown(Keys[random]))
                    quickFail();
        }
    }
    void TitansFill(GameObject[] Titans, int Direction)
    {
        titans[Univiersalcounter] = Titans[0] = titanFathers.GetChild(Direction).GetChild(0).gameObject;
        titans[Univiersalcounter + 1] = Titans[1] = titanFathers.GetChild(Direction).GetChild(1).gameObject;
        Univiersalcounter += 2;
    }
    void TitanMove()
    {
        tempTitans = null;
        int titanRandom = UnityEngine.Random.Range(0, 3);
        if (titanRandom == 0)
            tempTitans = NorthTitans;
        else if (titanRandom == 1)
            tempTitans = EastTitans;
        else if (titanRandom == 2)
            tempTitans = SouthTitans;
        else
            tempTitans = WestTitans;
        for (int i = 0; i < 2; i++)
            tempTitans[i].GetComponent<TitanMove>().MoveTitanTurn(numbers[random]);
    }
    public void NextTurn()
    {
        StopAllCoroutines();
        StartCoroutine(NumberRandomizer());
    }
    IEnumerator NumberRandomizer()
    {
        yield return wait1;
        randomSound.Post(gameObject);
        random++;
        Numbers[numbers[random]-1].SetActive(true);
        StepBG.SetActive(true);
        ActionLock = true;
        yield return waitMove;
        if (ActionLock)
        {
            ActionLock = false;
            Miss.Post(gameObject);
            Numbers[numbers[random] - 1].SetActive(false);
            StepBG.SetActive(false);
            TitanMove();
        }
    }
    IEnumerator FirstQuick()
    {
        yield return new WaitForSeconds(3);
        StartCoroutine(QuickTimeEvent());
    }
    IEnumerator QuickTimeEvent()
    {
        yield return wait1;
        QuickSound.Post(gameObject);
        quicksUI[random].GetComponent<Image>().sprite = sprites[random];
        quicksUI[random].SetActive(true);
        QuickLock = true;
        yield return waitQuick;
        if (QuickLock)
            quickFail();
    }
    void quickFail()
    {
        smoke.Stop(gameObject);
        QuickLock = false;
        quicksUI[random].SetActive(false);
        for (int i = random; i < 8; i++)
            titans[i].GetComponent<TitanMove>().attack();
    }
    void keysRandomizer(string[] Array,Sprite[] Arr2)
    {
        int n = Array.Length;
        int tempRand;
        System.Random rand = new System.Random();
        for (int i = 0; i < n; i++)
        {
            tempRand = i + rand.Next(n - i);
            swap(Array, i, tempRand);
            swap(Arr2, i, tempRand);
        }
    }
    void keysRandomizer(int[] Array)
    {
        int n = Array.Length;
        System.Random rand = new System.Random();
        for (int i = 0; i < n; i++)
        {
            swap(Array, i, i + rand.Next(n - i));
        }
    }
    void swap(string[] array, int a, int b)
    {
        string temp = array[a];
        array[a] = array[b];
        array[b] = temp;
    }
    void swap(int[] array, int a, int b)
    {
        int temp = array[a];
        array[a] = array[b];
        array[b] = temp;
    }
    void swap(Sprite[] array, int a, int b)
    {
        Sprite temp = array[a];
        array[a] = array[b];
        array[b] = temp;
    }
    void Taunting(int dir)
    {
        Tauntings[dir].SetActive(true);
        tauntingDire = dir;
        Taunt.Post(gameObject);
    }
    public void ActionUnlock()
    {
        ActionLock = false;
    }
     IEnumerator Win()
    {
        yield return waitMove;
        winScreen.SetActive(true);
        stopAll.Post(gameObject);
        WinSound.Post(gameObject);
        Time.timeScale = 0;
    }
    public void Lost()
    {
        if (!loseLock)
        {
            loseLock = true;
            gameOver.SetActive(true);
            stopAll.Post(gameObject);
            lose.Post(gameObject);
            Time.timeScale = 0;
        }
    }
    void TitanRegularMove(int dire,GameObject[]Titans)
    {
        ActionLock = false;
        Numbers[numbers[random]-1].SetActive(false);
        Taunting(dire);
        StepBG.SetActive(false);
        for (int i = 0; i < 2; i++)
        {
            Titans[i].GetComponent<TitanMove>().MoveTitanTurn(numbers[random]);
        }
        soldiers[dire*2].GetComponent<Actions>().Jump();
        soldiers[(dire*2)+1].GetComponent<Actions>().Jump();
    }
}
