using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100f;  //[SerializeField] изметить в инспекторе(да) не меняется в других кодах public (да)
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
    //Превосходя
    bool isTransitioning = false;   // умер или нет

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
            RespondToThrustInput();  //  толчёк
            RespondToRotateInput();   // повернуть
        }
        if(Debug.isDebugBuild)
        {
            RespondToDebugKeeys();

        }

    }

    private void RespondToDebugKeeys()
    {
       if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
       else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; //collision On/Of
        }
    }

    void OnCollisionEnter(Collision collision)    // столкновение обьектов
    {
        if (isTransitioning || collisionsDisabled) { return; }     //ignore Collisions игнор столкновения}
       
 
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
    private void StartSuccessSequence()   // статус прошел
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);   //Invoke задержка в 
    }

    private void StartDeathSequence()   //статус умер
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirslevel", levelLoadDelay);   //Invoke задержка в 
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;  // int потому что индекс целый номер.
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex); // больше чем для 2х уровней
    }

    private void LoadFirslevel()
    {
        SceneManager.LoadScene(0); // больше чем для 2х уровней
    }

    private void RespondToThrustInput() // Взлет
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

    private void StopApplyingThrust()
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
    }   //управление взлетом

    private void RespondToRotateInput()   // ротация право лево
    {
        rigidBody.angularVelocity = Vector3.zero; //  удалите вращение из-за физики

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

