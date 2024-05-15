# leikjaforritun2024

# Skilaverkefni 1 [20%]
- [Skriptur]()
- [Myndband]()
<br><br>
**Útskýra í stuttu**
- Scene view : <br>
  Scene viewer er notað skil þess að skoða senuna eða vinnusvæðið, þar sem þú smellir, færir og breytit hlutum
- Game view : <br>
  Svipað "Scene Viewer" nema það sínir PoV frá leiknum
- Project : <br>
  
- Hierarchy
- Inspector

  
# Skilaverkefni 2 [20%]

- [Allar Skriptur](https://github.com/Belistov/leikjaforritun2024/tree/main/Skil%202%20Skript)
- [Myndbandi](https://www.youtube.com/watch?v=XcpUH3Ep_9A)
- [Spilaðu Leikinn](https://bakupyronew.itch.io/forritun-skil-2)

# Skilaverkefni 3 [20%]
## Sktiptur
Skripta til að labba um
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 6f; // Hraði hreyfingar
    public float sprintSpeed = 12f; // Hraði hlaupa
    public float jumpHeight = 3f; // Hæð hoppa
    public float gravity = -9.81f; // Þyngdarkraftur

    public Transform groundCheck; // Staðsetning fyrir að athuga hvort spilarinn sé á jörðinni
    public float groundDistance = 0.4f; // Fjarlægð frá jörðinni sem athugunin er gerð á
    public LayerMask groundMask; // Lag sem jörðin tilheyrir

    Vector3 velocity; // Hraði spilarans
    bool isGrounded; // Athuga hvort spilarinn sé á jörðinni
    bool isSprinting; // Athuga hvort spilarinn sé að hlaupa

    // Update er kallað einu sinni á hverju frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal"); // Hreyfing á x-ás
        float z = Input.GetAxis("Vertical"); // Hreyfing á z-ás

        Vector3 move = transform.right * x + transform.forward * z; // Hreyfing spilara í samhengi við áttirnar

        // Hlaupa
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }

        // Setja hraða eftir hvort er verið að hlaupa eða ekki
        float currentSpeed;
        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = speed;
        }

        // Hreyfa spilarann
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Athuga hvort spilarinn sé á jörðinni
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Hoppa
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Bæta við þyngdarkrafti
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
```
Bissu Kóðinn sem ég gerði sjálfur<br>
[Hlekkur að kóðanum mínum](https://github.com/Belistov/Gun-Script)
```cs
using UnityEngine;
using System.Collections;

public class ShooterCode : MonoBehaviour
{
    [Header("< Aðalmyndavélin / Leikmaður >")]
    [SerializeField] private mouseLook cam; // Myndavélarskoðari
    private float originalSensitivity; // Upprunalegur næmni

    [SerializeField] private PlayerMovement _speed; // Hreyfing leikmanns
    private float originalSpeed; // Upprunalegur hraði

    [SerializeField] private PlayerMovement _SprintSpeed; // Hlaupahraði leikmanns
    private float originalSprintSpeed; // Upprunalegur hlaupahraði

    [Header("< Stillingar byssu >")]
    public bool is_auto; // Sjálfvirkt skjót
    [Header("")]
    public float damage = 10f; // Meðalhármi
    public float fireRate = 15f; // Eldhafi
    public float impactForce = 10f; // Áhrifskraftur
    [Header("")]
    public float aimDist = 30f; // Skotfjarlægð
    public float aimSens = 10f; // Næmni við miðjuð skot
    public float aimWalkSpeed = 12f; // Hraði gangandi með miðjuð skot
    public float aimSprintSpeed = 12f; // Hraði hlaupandi með miðjuð skot
    public float range = 100f; // Skammtur
    [Header("")]
    public float maxAmmo = 10f; // Hámark skotafjöldi
    public float currentAmmo; // Núverandi skotafjöldi
    public float reloadTime = 1f; // Tími á að hlaða
    private bool isReloading = false; // Er að hlaða

    [Header("< Hlutar byssu >")]
    public GameObject muzzle; // Hverfandi
    public Camera fpsCam; // FPS myndavél
    public AudioSource shoot_SFX; // Hljóðspor
    public LineRenderer bulletTrail; // Nota LineRenderer fyrir skotaleiðslu
    private Transform gunTransform; // Hversu byssan snýr
    private Quaternion originalRotation; // Upprunaleg snúningur
    private float nextTimeToFire = 0f; // Tími á næsta skoti

    void Start()
    {
        shoot_SFX = GetComponent<AudioSource>();
        shoot_SFX.Pause();

        originalSensitivity = cam.mouseSensitivity;
        originalSpeed = _speed.speed;
        originalSprintSpeed = _SprintSpeed.sprintSpeed;
        currentAmmo = maxAmmo;

        gunTransform = transform;
        originalRotation = gunTransform.localRotation;
    }

    void OnEnable()
    {
        isReloading = false;
    }

    void Update()
    {
        if (isReloading)
        {
            StartCoroutine(Reload());
            return;
        }
        if (currentAmmo <= 0 || Input.GetKeyDown("r"))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Aim();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            noAim();
        }

        if (is_auto == true)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
                shoot_SFX.Play(0);
            }
        }
        if (is_auto == false)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
                shoot_SFX.Play(0);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Endurhleðsla...");

        float elapsedTime = 0f;
        Quaternion startRotation = gunTransform.localRotation;
        Quaternion targetRotation = originalRotation * Quaternion.Euler(-30f, 0f, 0f);

        while (elapsedTime < reloadTime)
        {
            gunTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / reloadTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        noAim();

        gunTransform.localRotation = originalRotation;

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            healthCode target = hit.transform.GetComponent<healthCode>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            // Setja staðsetningu fyrir LineRenderer (skotaleiðslu)
            bulletTrail.SetPosition(0, muzzle.transform.position);
            bulletTrail.SetPosition(1, hit.point);

            // Aukahreyfing til rigidbody eftir þörfum
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
        }
        else
        {
            // Ef geislinn hittir ekkert, stilla LineRenderer á sjálfgefna lengd
            bulletTrail.SetPosition(0, muzzle.transform.position);
            bulletTrail.SetPosition(1, muzzle.transform.position + fpsCam.transform.forward * range);
        }
        Debug.Log("Skot eftir :" + currentAmmo);
    }

    void Aim()
    {
        fpsCam.fieldOfView = aimDist;
        cam.mouseSensitivity = aimSens;
        _speed.speed = aimWalkSpeed;
        _SprintSpeed.sprintSpeed = aimSprintSpeed;
    }

    void noAim()
    {
        fpsCam.fieldOfView = 60;
        cam.mouseSensitivity = originalSensitivity;
        _speed.speed = originalSpeed;
        _SprintSpeed.sprintSpeed = originalSprintSpeed;
    }
}
```
Hreyfa músina
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook : MonoBehaviour
{
    public float mouseSensitivity = 1000f; // Næmni músar

    public Transform playerBody; // Transform fyrir líkamann

    float XRotation = 0f; // Snúningur um x-ás

    // Start er kallað þegar leikurinn byrjar
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Læsa músarboltanum
    }

    // Update er kallað einu sinni á hverjum frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // Hreyfing músar til hægri/vinstri
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; // Hreyfing músar upp/niður

        XRotation -= mouseY; // Lækka X snúning
        XRotation = Mathf.Clamp(XRotation, -90f, 90f); // Takmarka X snúning á milli -90 og 90 gráður

        transform.localRotation = Quaternion.Euler(XRotation, 0f, 0f); // Snúa myndavélinni um X snúning
        playerBody.Rotate(Vector3.up * mouseX); // Snúa leikmanni í kringum Y ás
    }
}
```
Heldur utan um líf, það þarf varla comments hérna
```cs
using UnityEngine;

public class healthCode : MonoBehaviour
{
    public float health = 10f;

    public void TakeDamage (float amount)
    {
        health -= amount;
        Debug.Log("Health : "+health);

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die ()
    {
        Destroy(gameObject);
    }
}
```
Óvinir sem elta player
```cs
using UnityEngine;

public class EnemyCode : MonoBehaviour
{
    public string playerTag = "Player"; // Merki leikmannsins GameObject
    public float followSpeed = 5f; // Hraði sem hluturinn fylgir leikmanninum með
    public float rotationSpeed = 5f; // Hraði sem hluturinn snýst til að horfa á leikmanninn

    private Transform player; // Tilvísun í transform leikmannsins

    void Start()
    {
        // Finna leikmann GameObject-ið með því að nota merkið
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Leikmaður með merkið '" + playerTag + "' fannst ekki!");
        }
    }

    void Update()
    {
        // Athuga hvort leikmaður er fundinn
        if (player != null)
        {
            // Reikna átt til að hreyfa sig í átt að leikmanninum
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0f; // Hunsa láréttar hreyfingar

            // Hreyfa sig í átt að leikmanninum
            transform.position += directionToPlayer.normalized * followSpeed * Time.deltaTime;

            // Snúa til að horfa á leikmanninn
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
```
Fæðir Óvini
```cs
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Forsnið fyrir óvinina sem á að framkalla
    public Transform spawnPoint; // Staðsetningin þar sem óvinirnar munu framkallast
    public float spawnInterval = 2f; // Tími milli framkalls óvinanna
    public int maxEnemies = 10; // Hámarksfjöldi óvinna sem á að framkalla
    public float spawnRadius = 10f; // Geisli kringum framkallarstað þar sem óvinir geta framkallast

    private int currentEnemyCount = 0; // Núverandi fjöldi framkallaðra óvinna
    private float spawnTimer = 0f; // Tími til að fylgjast með tíma milli framkalls

    void Update()
    {
        // Athuga hvort við höfum ekki náð hámarksfjölda óvinna
        if (currentEnemyCount < maxEnemies)
        {
            // Uppfæra tímamælið
            spawnTimer += Time.deltaTime;

            // Athuga hvort sé komið tími til að framkalla nýjan óvin
            if (spawnTimer >= spawnInterval)
            {
                SpawnEnemy();
                spawnTimer = 0f; // Endurstilla tímamælið
            }
        }
    }

    void SpawnEnemy()
    {
        // Athuga hvort forsnitt óvinarins og framkallarstaður séu skilgreindir
        if (enemyPrefab != null && spawnPoint != null)
        {
            // Reikna slembistaðsetningu innan geisla framkallarstaðarins
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0f; // Tryggja að óvinir framkalli á sömu hæð og landslagið

            // Reikna framkallarstaðinn miðað við staðsetningu framkallarstaðarins
            Vector3 spawnPosition = spawnPoint.position + randomOffset;

            // Framkalla nýjan óvin á reiknaðri staðsetningu
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemyCount++; // Auka fjölda óvinna
        }
        else
        {
            Debug.LogWarning("Forsnið óvinar eða framkallarstaður ekki skilgreindur í EnemySpawner!");
        }
    }
}
```
## Hlekkir
[Youtube Myndband](https://www.youtube.com/watch?v=JT0UTBF1z-E)
[Spilaðu Leikinn](https://bakupyronew.itch.io/skil4)
## Athugasemdir
Ég var ekki að nenna að downloada vatni og setja allar týpur af shitti, er of þreyttur, búinn að vera erfiður mánuður og hef varla tíma til neins, <br>
g´´oða nótt og bless <3

# Skilaverkefni 4 [20%]
## Athugasemdir
Ég og Elvar unnum verkefnið saman, notuðum sama asset pakka og ég lánaði honum kóðann minn, ég neita að gera meira víst leiðbeningar eru svo illa settar upp.
##
## Hlekkir
[Youtube Myndband](https://www.youtube.com/watch?v=H2gD_ZE-4wQ)
[Spilaðu Leikinn](https://bakupyronew.itch.io/forrd-skil-4)
## Kóðinn
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    Rigidbody2D rb; // Rigidbody sem stjórnar hreyfingum leikmanns

    public float walkS = 4f; // Hraði leikmanns þegar hann gengur
    public float speedLim = 0.7f; // Takkmarkaður hraði þegar leikmaður fer í ská
    public float inputHor; // Inntak frá notanda um láréttar hreyfingar
    public float inputVer; // Inntak frá notanda um lóðréttar hreyfingar

    Animator animator; // Animator sem stjórnar hreyfingum leikmanns
    string currentAnimState; // Núverandi stöðu hreyfingar leikmanns
    const string Player_Idle = "Player_Idle"; // Nafn stöðu þegar leikmaður er kyrr
    const string Player_Walk_Down = "Player_Run_Down"; // Nafn stöðu þegar leikmaður er að hreyfast niður
    const string Player_Walk_Up = "Player_Run_Up"; // Nafn stöðu þegar leikmaður er að hreyfast upp
    const string Player_Walk_Left = "Player_Run_Left"; // Nafn stöðu þegar leikmaður er að hreyfast til vinstri
    const string Player_Walk_Right = "Player_Run_Right"; // Nafn stöðu þegar leikmaður er að hreyfast til hægri

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>(); // Ná í Rigidbody tengt þessum hlut

        animator = gameObject.GetComponent<Animator>(); // Ná í Animator tengt þessum hlut
    }

    // Update is called once per frame
    void Update()
    {
        inputHor = Input.GetAxisRaw("Horizontal"); // Ná í láréttar hreyfingar frá notanda
        inputVer = Input.GetAxisRaw("Vertical"); // Ná í lóðréttar hreyfingar frá notanda
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        if (inputHor != 0 || inputVer != 0) // Ef notandi er að gefa inntak um hreyfingu
        {
            if (inputHor != 0 && inputVer != 0) // Ef notandi er að hreyfa leikmann í báðar áttir
            {
                inputHor *= speedLim; // Takmarka hraða í láréttu með `speedLim`
                inputVer *= speedLim; // Takmarka hraða í lóðréttu með `speedLim`
            }

            // Setja hreyfingu leikmanns miðað við inntakið
            rb.velocity = new Vector2(inputHor * walkS, inputVer * walkS);

            // Athuga stefnu hreyfingar og stilla viðeigandi hreyfingastöðu
            if (inputHor < 0) // Ef leikmaður er að hreyfast til vinstri
            {
                ChangeAnimState(Player_Walk_Right); // Setja hreyfingastöðu til hægri (og flip)
            }
            if (inputHor > 0) // Ef leikmaður er að hreyfast til hægri
            {
                ChangeAnimState(Player_Walk_Left); // Setja hreyfingastöðu til vinstri (án flips)
            }
            if (inputVer > 0) // Ef leikmaður er að hreyfast upp
            {
                ChangeAnimState(Player_Walk_Up); // Setja hreyfingastöðu upp
            }
            if (inputVer < 0) // Ef leikmaður er að hreyfast niður
            {
                ChangeAnimState(Player_Walk_Down); // Setja hreyfingastöðu niður
            }

        }
        else // Ef ekki er verið að gefa inntak um hreyfingu
        {
            rb.velocity = new Vector2(0f, 0f); // Núllstilla hreyfingu leikmanns
            ChangeAnimState(Player_Idle); // Setja hreyfingastöðu sem kyrrstöðu
        }
    }

    // Fall sem stýrir breytingu á hreyfingastöðu leikmanns
    void ChangeAnimState(string newState)
    {
        if (currentAnimState == newState) return; // Ef núverandi stöða er þegar þessi stöða, hætta

        animator.Play(newState); // Leika viðeigandi hreyfingastöðu í Animator

        currentAnimState = newState; // Uppfæra núverandi hreyfingastöðu
    }
}
```

# Skilaverkefni 5 [20%]
## Athugasemdir
Mér fynst asset pakkin "Ruby's adventure" vera svo leiðinlegur að vinna með, maður bara missir áhugann á leikjaforritun við að nota svona pakka, þannig ég valdi mér pakka sjálfur.

## Hlekkir

## Skriptur
Death Barrier / KillZone
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    // Þetta fall er kallað þegar einhver fer inn í svæðið
    void OnTriggerEnter2D(Collider2D col)
    {
        // Athuga hvort hluturinn sem fór inn í svæðið sé merktur sem "Player"
        if (col.gameObject.tag == "Player")
        {
            // Endurhlaða núverandi sviðsmynd ef hluturinn er leikmaðurinn
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // Eyða hlutnum ef hann er ekki leikmaðurinn
            Destroy(col.gameObject);
        }
    }
}
```

Stigakerfi
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Points : MonoBehaviour
{
    public string pointTag; // Merkimiði fyrir stigahluti
    public string enemyTag; // Merkimiði fyrir óvinahluti
    public float pointCount; // Núverandi stiga fjöldi

    // Start er kallað áður en fyrsta ramminn er uppfærður
    void Start()
    {
        pointCount = 0;
        Debug.Log("Stig sett á byrjunarstöðu: " + pointCount);
        Debug.Log("Búist við að rekast á hluti með merki: " + pointTag);
    }

    // OnTriggerEnter2D er kallað þegar Collider2D annar fer inn í kveikjuna
    void OnTriggerEnter2D(Collider2D other)
    {
        // Athuga hvort þessi hlutur sé leikmaðurinn og rekst á hluti með stigamerkið
        if (gameObject.CompareTag("Player") && other.gameObject.CompareTag(pointTag))
        {
            // Hækka stiga fjöldann
            pointCount++;
            Debug.Log("Stig aukin: " + pointCount);
            Destroy(other.gameObject); // Eyða hlutnum sem var rekst á

            // Athuga hvort nóg stig hafi verið safnað til að vinna
            if (pointCount >= 15)
            {
                SceneManager.LoadScene("WinScene");
            }
        }

        // Athuga hvort þessi hlutur sé óvinur og rekst á hluti með óvinamerkið eða stigamerkið og óvinamerkið
        if (gameObject.CompareTag(enemyTag) && other.gameObject.CompareTag(enemyTag) || gameObject.CompareTag(pointTag) && other.gameObject.CompareTag(enemyTag))
        {
            // Hunsa árekstra milli óvina, framkvæma ekki neinar aðgerðir
            return;
        }

        // Athuga hvort þessi hlutur sé leikmaðurinn og rekst á hluti með óvinamerkið
        if (gameObject.CompareTag("Player") && other.gameObject.CompareTag(enemyTag))
        {
            // Lækka stiga fjöldann
            pointCount--;
            Debug.Log("Stig lækkuð: " + pointCount);

            // Athuga hvort stig hafi farið niður fyrir núll til að tapa
            if (pointCount < 0)
            {
                SceneManager.LoadScene("LoseScene");
            }
        }
    }
}
```

Senu Hlaðari / SceneLoader
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    void Awake()
    {
        // Útfærsla á Singleton munstri
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Valfrjálst, halda þessum hlut á milli sviðsmynda
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GoToGameplay()
    {
        SceneManager.LoadScene("Scene1");
    }
}
```

Attack Script
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float dmgValue = 4; // Skemmdargildi árásarinnar
    public GameObject throwableObject; // Hlutir sem hægt er að kasta
    public Transform attackCheck; // Staðsetning til að athuga hvort óvinur sé nálægt fyrir árás
    private Rigidbody2D m_Rigidbody2D; // Stýring á stífleika
    public Animator animator; // Stýring á hreyfimyndum
    public bool canAttack = true; // Athuga hvort leikmaðurinn getur ráðist á
    public bool isTimeToCheck = false; // Athuga hvort tími sé kominn til að athuga

    public GameObject cam; // Myndavélin

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start er kallað áður en fyrsta ramminn er uppfærður
    void Start()
    {
        
    }

    // Update er kallað einu sinni í hverjum ramma
    void Update()
    {
        // Athuga hvort leikmaðurinn ýti á X eða Fire1 takkann og getur ráðist á
        if (Input.GetKeyDown(KeyCode.X) && canAttack || Input.GetButtonDown("Fire1") && canAttack)
        {
            canAttack = false;
            animator.SetBool("IsAttacking", true);
            StartCoroutine(AttackCooldown());
        }

        // Athuga hvort leikmaðurinn ýti á V eða Fire2 takkann
        if (Input.GetKeyDown(KeyCode.V) || Input.GetButtonDown("Fire2") && canAttack)
        {
            // Búa til kastvopn og stilla stefnu þess
            GameObject throwableWeapon = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f,-0.2f), Quaternion.identity) as GameObject; 
            Vector2 direction = new Vector2(transform.localScale.x, 0);
            throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction; 
            throwableWeapon.name = "ThrowableWeapon";
        }
    }

    // Tímabil þar sem ekki er hægt að ráðast á eftir árás
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        canAttack = true;
    }

    // Gerir skaða á óvini þegar leikmaður gerir skyndiárás
    public void DoDashDamage()
    {
        dmgValue = Mathf.Abs(dmgValue);
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.9f);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.tag == "Enemy")
            {
                if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
                {
                    dmgValue = -dmgValue;
                }
                collidersEnemies[i].gameObject.SendMessage("ApplyDamage", dmgValue);
                cam.GetComponent<CameraFollow>().ShakeCamera();
            }
        }
    }
}
```
PlayerMovement
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public CharacterController2D controller; // Stýring fyrir persónu
    public Animator animator; // Stýring á hreyfimyndum

    public float runSpeed = 40f; // Hraði persónunnar

    float horizontalMove = 0f; // Lárétt hreyfing
    bool jump = false; // Athuga hvort persónan á að stökkva
    bool dash = false; // Athuga hvort persónan á að gera skyndiárás

    // Uppfært einu sinni í hverjum ramma
    void Update () {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown("space"))
        {
            jump = true;
        }

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown("left shift"))
        {
            dash = true;
        }
    }

    // Kallað þegar persónan fellur
    public void OnFall()
    {
        animator.SetBool("IsJumping", true);
    }

    // Kallað þegar persónan lendir
    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    // Uppfært á föstum tímabilum
    void FixedUpdate ()
    {
        // Hreyfa persónuna
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);
        jump = false;
        dash = false;
    }
}
```
CameraFollow
````cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f; // Hraði sem myndavélin fylgir markinu
    public Transform Target; // Markið sem myndavélin á að fylgja

    // Transform af myndavélinni til að hrista. Nær í transform af gameObjectinu
    // ef það er null.
    private Transform camTransform;

    // Hversu lengi myndavélin á að hristast.
    public float shakeDuration = 0f;

    // Styrkur hristingsins. Hærra gildi hristir myndavélina meira.
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        Cursor.visible = false;
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    private void Update()
    {
        // Færa myndavélina að nýrri staðsetningu í átt að markinu
        Vector3 newPosition = Target.position;
        newPosition.z = -10;
        transform.position = Vector3.Slerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);

        // Ef hristingur er enn í gangi, hrista myndavélina
        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
    }

    // Kallað til að hrista myndavélina
    public void ShakeCamera()
    {
        originalPos = camTransform.localPosition;
        shakeDuration = 0.2f;
    }
}
```
Character Controller Script
```cs
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                            // Krafan sem er beitt þegar leikmaðurinn stekkur.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;    // Hversu mikið á að slétta út hreyfinguna
    [SerializeField] private bool m_AirControl = false;                           // Hvort leikmaðurinn geti stjórnað í loftinu
    [SerializeField] private LayerMask m_WhatIsGround;                            // Maski sem ákvarðar hvað er jörð fyrir persónuna
    [SerializeField] private Transform m_GroundCheck;                             // Staðsetning sem merkir hvar á að athuga hvort persónan sé á jörðinni
    [SerializeField] private Transform m_WallCheck;                               // Staðsetning sem athugar hvort persónan snerti vegg

    const float k_GroundedRadius = .2f; // Radíus yfirborðshringings til að ákvarða hvort sé á jörðinni
    private bool m_Grounded;            // Hvort leikmaðurinn sé á jörðinni
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // Til að ákvarða í hvaða átt leikmaðurinn snýr
    private Vector3 velocity = Vector3.zero;
    private float limitFallSpeed = 25f; // Hámarkshraði í falli

    public bool canDoubleJump = true; // Hvort leikmaðurinn geti tvístökk
    [SerializeField] private float m_DashForce = 25f;
    private bool canDash = true;
    private bool isDashing = false; // Hvort leikmaðurinn sé að skyndiárás
    private bool m_IsWall = false; // Hvort það sé veggur fyrir framan leikmanninn
    private bool isWallSliding = false; // Hvort leikmaðurinn sé að renna á vegg
    private bool oldWallSlidding = false; // Hvort leikmaðurinn hafi verið að renna á vegg í fyrri ramma
    private float prevVelocityX = 0f;
    private bool canCheck = false; // Til að athuga hvort leikmaðurinn renni á vegg

    public float life = 10f; // Líf leikmannsins
    public bool invincible = false; // Hvort leikmaðurinn sé ósigrandi
    private bool canMove = true; // Hvort leikmaðurinn geti hreyft sig

    private Animator animator;
    public ParticleSystem particleJumpUp; // Agnaáhrif þegar stekkur upp
    public ParticleSystem particleJumpDown; // Agnaáhrif þegar lendir

    private float jumpWallStartX = 0;
    private float jumpWallDistX = 0; // Fjarlægð milli leikmanns og veggs
    private bool limitVelOnWallJump = false; // Til að takmarka veggstökksfjarlægð með lágum fps

    [Header("Events")]
    [Space]

    public UnityEvent OnFallEvent;
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (OnFallEvent == null)
            OnFallEvent = new UnityEvent();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // Leikmaðurinn er á jörðinni ef hringing til staðsetningar á groundcheck hittir eitthvað sem er merkt sem jörð
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                m_Grounded = true;
                if (!wasGrounded )
                {
                    OnLandEvent.Invoke();
                    if (!m_IsWall && !isDashing) 
                        particleJumpDown.Play();
                    canDoubleJump = true;
                    if (m_Rigidbody2D.velocity.y < 0f)
                        limitVelOnWallJump = false;
                }
        }

        m_IsWall = false;

        if (!m_Grounded)
        {
            OnFallEvent.Invoke();
            Collider2D[] collidersWall = Physics2D.OverlapCircleAll(m_WallCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < collidersWall.Length; i++)
            {
                if (collidersWall[i].gameObject != null)
                {
                    isDashing = false;
                    m_IsWall = true;
                }
            }
            prevVelocityX = m_Rigidbody2D.velocity.x;
        }

        if (limitVelOnWallJump)
        {
            if (m_Rigidbody2D.velocity.y < -0.5f)
                limitVelOnWallJump = false;
            jumpWallDistX = (jumpWallStartX - transform.position.x) * transform.localScale.x;
            if (jumpWallDistX < -0.5f && jumpWallDistX > -1f) 
            {
                canMove = true;
            }
            else if (jumpWallDistX < -1f && jumpWallDistX >= -2f) 
            {
                canMove = true;
                m_Rigidbody2D.velocity = new Vector2(10f * transform.localScale.x, m_Rigidbody2D.velocity.y);
            }
            else if (jumpWallDistX < -2f) 
            {
                limitVelOnWallJump = false;
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
            }
            else if (jumpWallDistX > 0) 
            {
                limitVelOnWallJump = false;
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
            }
        }
    }

    public void Move(float move, bool jump, bool dash)
    {
        if (canMove) {
            if (dash && canDash && !isWallSliding)
            {
                StartCoroutine(DashCooldown());
            }
            if (isDashing)
            {
                m_Rigidbody2D.velocity = new Vector2(transform.localScale.x * m_DashForce, 0);
            }
            else if (m_Grounded || m_AirControl)
            {
                if (m_Rigidbody2D.velocity.y < -limitFallSpeed)
                    m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -limitFallSpeed);
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

                if (move > 0 && !m_FacingRight && !isWallSliding)
                {
                    Flip();
                }
                else if (move < 0 && m_FacingRight && !isWallSliding)
                {
                    Flip();
                }
            }
            if (m_Grounded && jump)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("JumpUp", true);
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                canDoubleJump = true;
                particleJumpDown.Play();
                particleJumpUp.Play();
            }
            else if (!m_Grounded && jump && canDoubleJump && !isWallSliding)
            {
                canDoubleJump = false;
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 1.2f));
                animator.SetBool("IsDoubleJumping", true);
            }
            else if (m_IsWall && !m_Grounded)
            {
                if (!oldWallSlidding && m_Rigidbody2D.velocity.y < 0 || isDashing)
                {
                    isWallSliding = true;
                    m_WallCheck.localPosition = new Vector3(-m_WallCheck.localPosition.x, m_WallCheck.localPosition.y, 0);
                    Flip();
                    StartCoroutine(WaitToCheck(0.1f));
                    canDoubleJump = true;
                    animator.SetBool("IsWallSliding", true);
                }
                isDashing = false;

                if (isWallSliding)
                {
                    if (move * transform.localScale.x > 0.1f)
                    {
                        StartCoroutine(WaitToEndSliding());
                    }
                    else 
                    {
                        oldWallSlidding = true;
                        m_Rigidbody2D.velocity = new Vector2(-transform.localScale.x * 2, -5);
                    }
                }

                if (jump && isWallSliding)
                {
                    animator.SetBool("IsJumping", true);
                    animator.SetBool("JumpUp", true); 
                    m_Rigidbody2D.velocity = new Vector2(0f, 0f);
                    m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * m_JumpForce * 1.2f, m_JumpForce));
                    jumpWallStartX = transform.position.x;
                    limitVelOnWallJump = true;
                    canDoubleJump = true;
                    isWallSliding = false;
                    animator.SetBool("IsWallSliding", false);
                    oldWallSlidding = false;
                    m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                    canMove = false;
                }
                else if (dash && canDash)
                {
                    isWallSliding = false;
                    animator.SetBool("IsWallSliding", false);
                    oldWallSlidding = false;
                    m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                    canDoubleJump = true;
                    StartCoroutine(DashCooldown());
                }
            }
            else if (isWallSliding && !m_IsWall && canCheck) 
            {
                isWallSliding = false;
                animator.SetBool("IsWallSliding", false);
                oldWallSlidding = false;
                m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                canDoubleJump = true;
            }
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void ApplyDamage(float damage, Vector3 position) 
    {
        if (!invincible)
        {
            animator.SetBool("Hit", true);
            life -= damage;
            Vector2 damageDir = Vector3.Normalize(transform.position - position) * 40f ;
            m_Rigidbody2D.velocity = Vector2.zero;
            m_Rigidbody2D.AddForce(damageDir * 10);
            if (life <= 0)
            {
                StartCoroutine(WaitToDead());
            }
            else
            {
                StartCoroutine(Stun(0.25f));
                StartCoroutine(MakeInvincible(1f));
            }
        }
    }

    IEnumerator DashCooldown()
    {
        animator.SetBool("IsDashing", true);
        isDashing = true;
        canDash = false;
        yield return new WaitForSeconds(0.1f);
        isDashing = false;
        yield return new WaitForSeconds(0.5f);
        canDash = true;
    }

    IEnumerator Stun(float time) 
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator MakeInvincible(float time) 
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }

    IEnumerator WaitToMove(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator WaitToCheck(float time)
    {
        canCheck = false;
        yield return new WaitForSeconds(time);
        canCheck = true;
    }

    IEnumerator WaitToEndSliding()
    {
        yield return new WaitForSeconds(0.1f);
        canDoubleJump = true;
        isWallSliding = false;
        animator.SetBool("IsWallSliding", false);
        oldWallSlidding = false;
        m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
    }

    IEnumerator WaitToDead()
    {
        animator.SetBool("IsDead", true);
        canMove = false;
        invincible = true;
        GetComponent<Attack>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
```

Throwable Weapon Script
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeapon : MonoBehaviour
{
    public Vector2 direction; // The direction in which the weapon will be thrown
    public bool hasHit = false; // A flag to check if the weapon has hit something
    public float speed = 10f; // Speed at which the weapon will travel

    // Start is called before the first frame update
    void Start()
    {
        // Initialization if needed
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If the weapon hasn't hit anything, set its velocity in the specified direction
        if (!hasHit)
        {
            GetComponent<Rigidbody2D>().velocity = direction * speed;
        }
    }

    // This method is called when the weapon collides with another collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        // If the weapon collides with an enemy
        if (collision.gameObject.tag == "Enemy")
        {
            // Send a message to the enemy to apply damage
            collision.gameObject.SendMessage("ApplyDamage", Mathf.Sign(direction.x) * 2f);
            // Destroy the weapon after it hits an enemy
            Destroy(gameObject);
        }
        // If the weapon collides with something that is not the player
        else if (collision.gameObject.tag != "Player")
        {
            // Destroy the weapon upon collision
            Destroy(gameObject);
        }
    }
}
```
