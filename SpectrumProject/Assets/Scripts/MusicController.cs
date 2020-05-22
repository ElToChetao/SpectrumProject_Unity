using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public AudioSource source;
    private AudioClip[] loadedSongs;

    private Queue<AudioClip> queuedSongs = new Queue<AudioClip>();
    private List<AudioClip> songs = new List<AudioClip>();

    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI totalTimeText;
    public TextMeshProUGUI currentSongNameText;
    public RectTransform songBar;
    public Slider timeSlider;

    private float currentSongTotalSeconds;
    private float currentSongTime;
    private float oriXSize;

    private float timer = 0;
    private int seconds = 0;
    private int minutes = 0;

    private bool paused = false;
    private bool songPlaying = false;

    private int songIndex = 0;

    private void Awake()
    {
        loadedSongs = Resources.LoadAll<AudioClip>("Songs");
        foreach (AudioClip song in loadedSongs)
        {
            songs.Add(song);
        }

        oriXSize = songBar.sizeDelta.x;

        //timeSlider.onValueChanged.AddListener(delegate { StartCoroutine(SetSongSecond()); });
    }

    private void Update()
    {
        if (!paused)
        {
            if (!source.isPlaying && songs.Count > 0)
            {
                AudioClip song = songs[songIndex];
                source.PlayOneShot(song);

                int totalLength = (int)song.length;
                int totalMinutes = totalLength / 60;
                int restSeconds = totalLength - (totalMinutes * 60);

                totalTimeText.text = totalMinutes + ":" + restSeconds.ToString("00");

                seconds = 0;
                minutes = 0;

                currentSongTotalSeconds = song.length;
                currentSongTime = 0;

                currentTimeText.text = minutes + ":" + seconds.ToString("00");

                currentSongNameText.text = song.name;
            }

            if (source.isPlaying)
            {
                float dt = Time.deltaTime;
                timer += dt;
                currentSongTime += dt;
                if (timer >= 1)
                {
                    timer = 0;
                    seconds++;

                    if (seconds >= 60)
                    {
                        seconds = 0;
                        minutes++;
                    }

                    currentTimeText.text = minutes + ":" + seconds.ToString("00");
                }

                float perc = currentSongTime / currentSongTotalSeconds;
                Vector3 newSize = new Vector3(perc * oriXSize, songBar.sizeDelta.y);
                songBar.sizeDelta = newSize;

                timeSlider.value = perc;
            }
        }
    }

    private IEnumerator SetSongSecond()
    {
        paused = true;
        yield return new WaitForSeconds(0.5f);
        source.Stop();
        source.time = timeSlider.value * currentSongTotalSeconds;

        yield return new WaitForSeconds(0.5f);
        source.Play();
        yield return new WaitForSeconds(0.5f);
        paused = false;
    }

    public void OnNextSongButton()
    {
        NextSong();
    }
    private void NextSong()
    {
        if(songIndex < songs.Count)
        {
            songIndex++;
            source.Stop();
        }
    }

    public void OnPreviousSongButton()
    {
        PreviousSong();
    }
    private void PreviousSong()
    {
        if (songIndex > 0)
        {
            songIndex--;
            source.Stop();
        }
    }

    public void OnPauseSongButton()
    {
        PauseSong();
    }
    private void PauseSong()
    {
        source.Pause();
        paused = true;
    }
    public void OnUnPauseSongButton()
    {
        UnPauseSong();
    }
    private void UnPauseSong()
    {
        source.UnPause();
        paused = false;
    }
}