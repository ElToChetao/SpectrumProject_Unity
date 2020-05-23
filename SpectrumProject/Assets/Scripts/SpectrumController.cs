using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpectrumController : MonoBehaviour
{
    private AudioSource source;
    public int spectrumSize;
    private float[] spectrum;
    public FFTWindow mode;
    private void Start()
    {
        source = GetComponent<AudioSource>();
        spectrum = new float[spectrumSize];
    }

    public int GetSpectrumSize() { return spectrumSize; }
    public float[] GetSpectrum() { return spectrum; }
    private void Update()
    {
        source.GetSpectrumData(spectrum, 0, mode);
    }
}
