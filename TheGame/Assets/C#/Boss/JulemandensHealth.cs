using UnityEngine;
using System.Collections;
public class JulemandensHealth : MonoBehaviour
{
    [Header("Julemandens Stats")]
    public int maxHealth = 100; // Maksimalt liv
    public int currentHealth { get; private set; }
    public int defense = 5; // Reducerer skade
    public float evasionChance = 0.2f; // Sandsynlighed for at undvige (20%)

    [Header("Bevægelse")]
    public float flyveHastighed = 5f; // Hvor hurtigt julemanden flyver
    public float pauseTid = 2f; // Hvor længe julemanden holder pause for at kaste bomber
    public Transform venstreGrænse, højreGrænse; // Flyvegrænser
    private Rigidbody2D rb; // Reference til Rigidbody2D
    private bool flyverModHøjre = true; // Retning for flyvning
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
    public float flyveHøjde = 5f; // Højde, julemanden flyver i
    public float landingsHøjde = 1f; // Højde, julemanden lander i
    private bool erPåJorden = false; // Om julemanden har landet

    [Header("Flyvetidsindstillinger")]
    public float minFlyvetid = 5f; // Minimum tid, bossen flyver
    public float maxFlyvetid = 10f; // Maksimum tid, bossen flyver
    private float nuværendeFlyvetid; // Den aktuelle tilfældige flyvetid
    private float flyvetidTimer; // Tæller til at spore flyvetiden
    private bool erLetning; // Om julemanden er ved at lette
    private bool erFlyvningGenstartet = false;

    [Header("Overgangsindstillinger")]
    public float letteTid = 2f; // Tid det tager at lette
    public float landingTid = 2f; // Tid det tager at lande
    private void Start()
    {
        currentHealth = maxHealth; // Sæt julemandens liv til maks ved start
        rb = GetComponent<Rigidbody2D>(); // Find Rigidbody2D-komponenten
        nuværendeFlyvetid = Random.Range(minFlyvetid, maxFlyvetid); // Bestem første flyvetid
        flyvetidTimer = nuværendeFlyvetid; // Sæt timer til flyvetiden
        currentCase = 1; // Start i Case 1
        UpdateBombeInterval(); // Sæt bombeinterval for Case 1
        // Start med at lette
        erLetning = true;
        StartCoroutine(StigOpTilFlyveHøjdeVedStart());
    }
    private IEnumerator StigOpTilFlyveHøjdeVedStart()
    {
       
        float startHøjde = transform.position.y;
        float tid = 0f; // Tæller tiden for transition
        float transitionTid = 2f; // Hvor lang tid letningen tager

        while (tid < transitionTid)
        {
            tid += Time.deltaTime;
            float nyHøjde = Mathf.Lerp(startHøjde, flyveHøjde, tid / transitionTid);
            transform.position = new Vector3(transform.position.x, nyHøjde, transform.position.z);
            yield return null;
        }

        // Når letning er færdig
        erLetning = false;
        erPåJorden = false;
        
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
            RestartBombekastning(); // Sørg for korrekt bombekastning
        }
    }

    private void UpdateBombeInterval()
    {
        // Sæt bombeinterval baseret på den aktuelle case
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
        // Sørg for, at velocity altid er nulstillet
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        // Kontrollér, at julemanden holder sig inden for grænserne
        HoldIndenForGrænser();
        if (!erIPause && !erPåJorden)
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
        // Tjek om julemanden er død
        if (currentHealth <= 0)
        {
            Dø();
            return;
        }

        // Tjek og opdater case baseret på liv
        UpdateCase();
    }
    private void Flyv()
    {
        if (flyverModHøjre)
        {
            transform.position += Vector3.right * flyveHastighed * Time.deltaTime;

            // Flip fjenden, hvis den ikke allerede kigger mod højre
            if (transform.localScale.x < 0)
            {
                Flip();
            }

            if (transform.position.x >= højreGrænse.position.x)
            {
                flyverModHøjre = false;
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

            if (transform.position.x <= venstreGrænse.position.x)
            {
                flyverModHøjre = true;
            }
        }
    }
    private void StartPause()
    {
        if (!erIPause)
        {
            erIPause = true;
            float randomPause = Random.Range(2f, pauseTid); // Tilfældig pause mellem 2 og 5 sekunder
     

            CancelInvoke("KastBombe"); // Stop bombekastning
            Invoke("FlyvIgen", randomPause); // Fortsæt efter pause
        }
    }
    private void KastBombe()
    {
        if (bombePrefab != null && bombeSpawnPoint != null && !erPåJorden)
        {
            Instantiate(bombePrefab, bombeSpawnPoint.position, Quaternion.identity);

        }
    }
    private void FlyvIgen()
    {
        if (!erFlyvningGenstartet)
        {
            erFlyvningGenstartet = true;
            nuværendeFlyvetid = Random.Range(minFlyvetid, maxFlyvetid);
            flyvetidTimer = nuværendeFlyvetid;
            StartCoroutine(StigOpTilFlyveHøjde());
            erFlyvningGenstartet = false;
        }
    }
    public void StartLanding()
    {
        if (!erPåJorden && !erIPause)
        {
            StartCoroutine(LandOgPause());
        }
    }
    private IEnumerator StigOpTilFlyveHøjde()
    {
        float startHøjde = transform.position.y; // Startposition i Y-aksen
        float tid = 0f; // Tæller tiden for transition
        float transitionTid = letteTid; // Brug justerbar letteTid

        while (tid < transitionTid)
        {
            tid += Time.deltaTime;
            float nyHøjde = Mathf.Lerp(startHøjde, flyveHøjde, tid / transitionTid); // Beregn ny højde med Lerp
            transform.position = new Vector3(transform.position.x, nyHøjde, transform.position.z);
            yield return null;
        }

        // Når letning er færdig
        erPåJorden = false;
        erIPause = false;
        UpdateCase(); // Skift case
        InvokeRepeating("KastBombe", 0f, bombeInterval); // Genoptag bombekastning
    }

    private IEnumerator LandOgPause()
    {
        float startHøjde = transform.position.y;
        float tid = 0f; // Tæller tiden for transition
        float transitionTid = 2f; // Hvor lang tid landingen tager

        while (tid < transitionTid)
        {
            tid += Time.deltaTime;
            float nyHøjde = Mathf.Lerp(startHøjde, landingsHøjde, tid / transitionTid);
            transform.position = new Vector3(transform.position.x, nyHøjde, transform.position.z);
            yield return null;
        }

        // Når landingen er færdig
        erPåJorden = true;
        StartPause();
    }

    private void Dø()
    {
        Destroy(gameObject);
    }
    private void HoldIndenForGrænser()
    {
        // Sørg for, at julemanden holder sig inden for flyvegrænserne
        float x = Mathf.Clamp(transform.position.x, venstreGrænse.position.x, højreGrænse.position.x);
        float y = transform.position.y; // Hold samme højde
        transform.position = new Vector3(x, y, transform.position.z);
    }
    private void FlyvetidMonitor()
    {
        if (!erIPause && !erPåJorden)
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