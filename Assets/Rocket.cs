﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 800f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        // TODO: stop sound on death
        if (state == State.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
	}

    private void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) {
            return; // ignore collisions when dead
        }

        switch(collision.gameObject.tag) {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success, 0.8f);
        Invoke("LoadNextScene", 1f); // TODO: parameterize
    }

    private void StartDeathSequence() {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death, 0.6f);
        Invoke("LoadFirstLevel", 1f); // TODO: parameterize
    }

    private void LoadFirstLevel() {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene() {
        SceneManager.LoadScene(1); // TODO: allow for more than two levels
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) { // can thrust while rotating
            ApplyThrust();
        } else {
            audioSource.Stop();
        }
    }

    private void ApplyThrust() {
        float thrustThisFrame = mainThrust * Time.deltaTime;

        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);

        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void RespondToRotateInput() {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physices control of rotation
    }
}
