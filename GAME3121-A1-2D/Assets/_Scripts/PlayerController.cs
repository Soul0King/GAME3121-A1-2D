using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
//using UnityEngine.UI;
using UnityEngine.UIElements;
//using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{

    public float thrustForce = 50.0f;

    Rigidbody2D rb;

    public float maxSpeed = 10.0f;
    public float scoreMultiplier = 10.0f;
    private float score = 0f;
    private float elapsedTime = 0f;

    public GameObject boosterFlame;

    [SerializeField]
    private PanelRenderer panel;

    [SerializeField]
    private VisualElementReference labelReference;
    [SerializeField]
    private VisualElementReference highLabelReference;

    [SerializeField]
    private VisualElementReference buttonReference;

    private Label scoreText;
    private Label highScoreText;
    private Button restartButton;

    public GameObject explosionPrefab;

    private const string HighScoreKey = "HighScore";


    private Soundmanager sm;

    [Header("Distance Settings")]
    [SerializeField] private float maxDistance = 10f;  
    [SerializeField] private float minDistance = 1f;   

    [Header("Pitch Settings")]
    [SerializeField] private float minPitch = 1.0f;
    [SerializeField] private float maxPitch = 2.5f;

    [Header("Speed Settings")]
    [SerializeField] private float maxRepeatDelay = 1.0f; 
    [SerializeField] private float minRepeatDelay = 0.1f;

    [SerializeField] private AudioSource audioSource;

    private float beepTimer;

    private void Awake()
    {

        panel.RegisterUIReloadCallback(OnUIReload);

    }

    private void OnEnable()
    {
        Debug.Log("On Enable called");
    }


    private void OnUIReload(PanelRenderer panel, VisualElement root, int version)
    {
        UnregisterButton();

        restartButton = root.Q<Button>("RestartButton");

        if (restartButton != null)
        {
            restartButton.clicked += OnStartClicked;
            restartButton.style.display = DisplayStyle.None;
        }

        labelReference.RegisterReferenceResolvedCallback(ve =>
        {
            if (ve is Label lbl) HandleLabelReferenceCallBack(lbl);
        });

        highLabelReference.RegisterReferenceResolvedCallback(ve =>
        {
            if (ve is Label lbl) HandleHighLabelReferenceCallBack(lbl);
        });



    }


    private void HandleLabelReferenceCallBack(VisualElement element)
    {
        if (element is Label lbl)

            scoreText = lbl;
    }

    private void HandleHighLabelReferenceCallBack(VisualElement element)
    {
        if (element is Label lbl)
        {
            highScoreText = lbl;
            highScoreText.text = $"High Score: {PlayerPrefs.GetFloat(HighScoreKey, 0f)}";
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

        sm = FindAnyObjectByType<Soundmanager>();
    }


    private void OnDestroy()
    {

        panel.UnregisterUIReloadCallback(OnUIReload);

    }


    // Update is called once per framevoid Update()
    void Update()
    {
        elapsedTime += Time.deltaTime;

        score = Mathf.Floor(elapsedTime * scoreMultiplier);

        if (scoreText != null)
        {

            scoreText.text = $"Score: {score}";

        }

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Vector2 direction = (mousePos - transform.position).normalized;

            transform.up = direction;

            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {

                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

            }

        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {

            boosterFlame.SetActive(true);

        }

        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {

            boosterFlame.SetActive(false);

        }


    }

    private void OnStartClicked()
    {
        Debug.Log("The UI Toolkit Button was clicked!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UnregisterButton()
    {
        if (restartButton != null)
        {
            restartButton.clicked -= OnStartClicked;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            sm.PlayLoseSFX();
            Debug.Log("Game Over!");

            float savedHighScore = PlayerPrefs.GetFloat(HighScoreKey, 0f);
            if (score > savedHighScore)
            {
                PlayerPrefs.SetFloat(HighScoreKey, score);
                PlayerPrefs.Save();
                savedHighScore = score;
            }

            if (highScoreText != null)
            {
                highScoreText.text = $"High Score: {savedHighScore}";
            }

            if (restartButton != null)
            {
                restartButton.style.display = DisplayStyle.Flex;
            }

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            float distance = Vector2.Distance(transform.position, collision.transform.position);

            float proximity = 1f - ((distance - minDistance) / (maxDistance - minDistance));

            audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, proximity);

            float currentDelay = Mathf.Lerp(maxRepeatDelay, minRepeatDelay, proximity);

            beepTimer += Time.deltaTime;
            if (beepTimer >= currentDelay)
            {
                audioSource.Play();
                beepTimer = 0f;
            }
        }
    }
}