using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Score : MonoBehaviour

{
    public TMPro.TextMeshProUGUI scoreText;
    public Transform player;
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
    public float volume = 1.0f;
    public int score;



    // Start is called before the first frame update
    void Start(){
    }
    void Update()
    {
        scoreText.SetText(player.position.z.ToString("0"));
        addScore();

        audioSource1 = GetComponent<AudioSource>();
        audioSource2 = GetComponent<AudioSource>();
        audioSource3 = GetComponent<AudioSource>();


        void addScore()
        {
            score = int.Parse(scoreText.text);
            checkscore();
        }

        void checkscore()
        {
            if (score == 0)
            {
                scoreEquals0();


            }
            if (score > 10)
            {
                scoreEqualsorLargerThan10();
            }
        }
        void scoreEquals0()
        {

            audioSource1.Play();
            audioSource1.mute = false;


        }

        void scoreEqualsorLargerThan10()
        {

            audioSource2.Play();

        }

    }
}
