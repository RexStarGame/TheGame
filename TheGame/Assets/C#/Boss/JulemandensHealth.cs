using UnityEngine;
using System.Collections;
public class JulemandensHealth : MonoBehaviour
{
    [Header("Julemandens Stats")]
    public int maxHealth = 100; // Maksimalt liv
    public int currentHealth { get; private set; }
    public int defense = 5; // Reducerer skade
    public float evasionChance = 0.2f; // Sandsynlighed for at undvige (20%)

    [Header("Bev�gelse")]
    public float flyveHastighed = 5f; // Hvor hurtigt julemanden flyver
    public float pauseTid = 2f; // Hvor l�nge julemanden holder pause for at kaste bomber
    public Transform venstreGr�nse, h�jreGr�nse; // Flyvegr�nser
    private Rigidbody2D rb; // Reference til Rigidbody2D
    private bool flyverModH�jre = true; // Retning for flyvning
    private bool erIPause = false; // Om julemanden holder pause

    [Header("Bombekastning")]
    public GameObject bombePrefab; // Prefab til bomben
    public Transform bombeSpawnPoint; // Hvor bomben spawner
    public float bombeInterval = 2f; // Tid mellem bomber
    public float case1BombeInterval = 5f; // Case 1: Bombeinterval
    public float case2BombeInterval = 3f; // Case 2: Bombeinterval
    public float case3BombeInterval = 1f; // Case 3: Bombeinterval
    private int currentCase = 1; // Starter i case 1

    public float landingsHastighed = 2f; // Hvor hurtigt julemanden lander/stiger
    public float flyveH�jde = 5f; // H�jde, julemanden flyver i
    public float landingsH�jde = 1f; // H�jde, julemanden lander i
    private bool erP�Jorden = false; // Om julemanden har landet

    [Header("Flyvetidsindstillinger")]
    public float minFlyvetid = 5f; // Minimum tid, bossen flyver
    public float maxFlyvetid = 10f; // Maksimum tid, bossen flyver
    private float nuv�rendeFlyvetid; // Den aktuelle tilf�ldige flyvetid
    private float flyvetidTimer; // T�ller til at spore flyvetiden
    private bool erLetning; // Om julemanden er ved at lette
    private bool erFlyvningGenstartet = false;

    [Header("Overgangsindstillinger")]
    public float letteTid = 2f; // Tid det tager at lette
    public float landingTid = 2f; // Tid det tager at lande
    private void Start()
    {
        currentHealth = maxHealth; // S�t julemandens liv til maks ved start
        rb = GetComponent<Rigidbody2D>(); // Find Rigidbody2D-komponenten
        nuv�rendeFlyvetid = Random.Range(minFlyvetid, maxFlyvetid); // Bestem f�rste flyvetid
        flyvetidTimer = nuv�rendeFlyvetid; // S�t timer til flyvetiden
        currentCase = 1; // Start i Case 1
        UpdateBombeInterval(); // S�t bombeinterval for Case 1
        // Start med at lette
        erLetning = true;
        StartCoroutine(StigOpTilFlyveH�jdeVedStart());
    }
    private IEnumerator StigOpTilFlyveH�jdeVedStart()
    {
       
        float startH�jde = transform.position.y;
        float tid = 0f; // T�ller tiden for transition
        float transitionTid = 2f; // Hvor lang tid letningen tager

        while (tid < transitionTid)
        {
            tid += Time.deltaTime;
            float nyH�jde = Mathf.Lerp(startH�jde, flyveH�jde, tid / transitionTid);
            transform.position = new Vector3(transform.position.x, nyH�jde, transform.position.z);
            yield return null;
        }

        // N�r letning er f�rdig
        erLetning = false;
        erP�Jorden = false;
        
        InvokeRepeating("KastBombe", 0f, bombeInterval); // Start bombekastning
    }
    private void UpdateCase()
    {
        float procentLiv = (float)currentHealth / maxHealth * 100f;
        int nyCase = 1;

        if (procentLiv <= 33f)
        {
            nyCase = 3; // Skift til Case 3
        }
        else if (procentLiv <= 66f)
        {
            nyCase = 2; // Skift til Case 2
        }
        else
        {
            nyCase = 1; // Bliv i Case 1
        }
        if (nyCase != currentCase)
        {
            currentCase = nyCase; // Opdater den aktuelle case
            UpdateBombeInterval(); // Opdater bombeinterval
            RestartBombekastning(); // S�rg for korrekt bombekastning
        }
    }

    private void UpdateBombeInterval()
    {
        // S�t bombeinterval baseret p� den aktuelle case
        switch (currentCase)
        {
            case 1:
                bombeInterval = case1BombeInterval;
                break;
            case 2:
                bombeInterval = case2BombeInterval;
                break;
            case 3:
                bombeInterval = case3BombeInterval;
                break;
        }
 
    }
    private void RestartBombekastning()
    {
        CancelInvoke("KastBombe"); // Stop alle tidligere kald
        InvokeRepeating("KastBombe", 0f, bombeInterval); // Start nye kald
    }
    private void Update()
    {
        // S�rg for, at velocity altid er nulstillet
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        // Kontroll�r, at julemanden holder sig inden for gr�nserne
        HoldIndenForGr�nser();
        if (!erIPause && !erP�Jorden)
        {

            FlyvetidMonitor();
            Flyv(); // Flyve-logik
        }
    }
    public void TagSkade(int skade)
    {
        // Tjek for evasion
        if (Random.value < evasionChance)
        {
 
            return;
        }

        // Reducer skade med forsvar
        int faktiskSkade = Mathf.Max(0, skade - defense);
        currentHealth -= faktiskSkade;
        // Tjek om julemanden er d�d
        if (currentHealth <= 0)
        {
            D�();
            return;
        }

        // Tjek og opdater case baseret p� liv
        UpdateCase();
    }
    private void Flyv()
    {
        if (flyverModH�jre)
        {
            transform.position += Vector3.right * flyveHastighed * Time.deltaTime;

            // Flip fjenden, hvis den ikke allerede kigger mod h�jre
            if (transform.localScale.x < 0)
            {
                Flip();
            }

            if (transform.position.x >= h�jreGr�nse.position.x)
            {
                flyverModH�jre = false;
            }
        }
        else
        {
            transform.position += Vector3.left * flyveHastighed * Time.deltaTime;

            // Flip fjenden, hvis den ikke allerede kigger mod venstre
            if (transform.localScale.x > 0)
            {
                Flip();
            }

            if (transform.position.x <= venstreGr�nse.position.x)
            {
                flyverModH�jre = true;
            }
        }
    }
    private void StartPause()
    {
        if (!erIPause)
        {
            erIPause = true;
            float randomPause = Random.Range(2f, pauseTid); // Tilf�ldig pause mellem 2 og 5 sekunder
     

            CancelInvoke("KastBombe"); // Stop bombekastning
            Invoke("FlyvIgen", randomPause); // Forts�t efter pause
        }
    }
    private void KastBombe()
    {
        if (bombePrefab != null && bombeSpawnPoint != null && !erP�Jorden)
        {
            Instantiate(bombePrefab, bombeSpawnPoint.position, Quaternion.identity);

        }
    }
    private void FlyvIgen()
    {
        if (!erFlyvningGenstartet)
        {
            erFlyvningGenstartet = true;
            nuv�rendeFlyvetid = Random.Range(minFlyvetid, maxFlyvetid);
            flyvetidTimer = nuv�rendeFlyvetid;
            StartCoroutine(StigOpTilFlyveH�jde());
            erFlyvningGenstartet = false;
        }
    }
    public void StartLanding()
    {
        if (!erP�Jorden && !erIPause)
        {
            StartCoroutine(LandOgPause());
        }
    }
    private IEnumerator StigOpTilFlyveH�jde()
    {
        float startH�jde = transform.position.y; // Startposition i Y-aksen
        float tid = 0f; // T�ller tiden for transition
        float transitionTid = letteTid; // Brug justerbar letteTid

        while (tid < transitionTid)
        {
            tid += Time.deltaTime;
            float nyH�jde = Mathf.Lerp(startH�jde, flyveH�jde, tid / transitionTid); // Beregn ny h�jde med Lerp
            transform.position = new Vector3(transform.position.x, nyH�jde, transform.position.z);
            yield return null;
        }

        // N�r letning er f�rdig
        erP�Jorden = false;
        erIPause = false;
        UpdateCase(); // Skift case
        InvokeRepeating("KastBombe", 0f, bombeInterval); // Genoptag bombekastning
    }

    private IEnumerator LandOgPause()
    {
        float startH�jde = transform.position.y;
        float tid = 0f; // T�ller tiden for transition
        float transitionTid = 2f; // Hvor lang tid landingen tager

        while (tid < transitionTid)
        {
            tid += Time.deltaTime;
            float nyH�jde = Mathf.Lerp(startH�jde, landingsH�jde, tid / transitionTid);
            transform.position = new Vector3(transform.position.x, nyH�jde, transform.position.z);
            yield return null;
        }

        // N�r landingen er f�rdig
        erP�Jorden = true;
        StartPause();
    }

    private void D�()
    {
        Destroy(gameObject);
    }
    private void HoldIndenForGr�nser()
    {
        // S�rg for, at julemanden holder sig inden for flyvegr�nserne
        float x = Mathf.Clamp(transform.position.x, venstreGr�nse.position.x, h�jreGr�nse.position.x);
        float y = transform.position.y; // Hold samme h�jde
        transform.position = new Vector3(x, y, transform.position.z);
    }
    private void FlyvetidMonitor()
    {
        if (!erIPause && !erP�Jorden)
        {
            flyvetidTimer -= Time.deltaTime;
            if (flyvetidTimer <= 0)
            {
                StartLanding();
            }
        }
    }
    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Vend X-aksen
        transform.localScale = localScale;
    }
}