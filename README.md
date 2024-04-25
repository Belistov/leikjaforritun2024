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
[Spilaðu Leikinn]()
## Athugasemdir

# Skilaverkefni 4 [20%]

# Skilaverkefni 5 [20%]
