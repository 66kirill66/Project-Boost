using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100f;  //[SerializeField] change in inspector (yes)  public (not)
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;
    bool isTransitioning = false;   // died  or not

    bool collisionsDisabled = false;

    // Use this for initialization
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (!isTransitioning)
        {
            RespondToThrustInput();  //  status takes off
            RespondToRotateInput();   // rotation right left 
        }
        if(Debug.isDebugBuild)
        {
            RespondToDebugKeeys();  //collision On/Of
        }
    }

    private void RespondToDebugKeeys()   //collision On/Of
    {
       if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
       else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)    // objects collision
    {
        if (isTransitioning || collisionsDisabled) { return; }     //ignore Collisions 
       
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }        
    }
    private void StartSuccessSequence()   // win
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);   
    }

    private void StartDeathSequence()   //status died
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirslevel", levelLoadDelay); 
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;  
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex); // more than 2 levels 
    }

    private void LoadFirslevel()
    {
        SceneManager.LoadScene(0); 
    }

    private void RespondToThrustInput() // status takes off  (true)
    {
        
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyTrust();
        }

        else
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust() // status takes off (false)
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyTrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (audioSource.isPlaying == false)  // if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }   

    private void RespondToRotateInput()   // rotation right left 
    {
        rigidBody.angularVelocity = Vector3.zero; //  remuve physics rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }
}

