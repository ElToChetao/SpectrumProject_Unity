using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VisualizationController : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject bandCubePrefab;
    public GameObject ringCubePrefab;

    private Transform[] cubes;
    private Transform[] ringCubes;
    private Transform[] bandCubes;

    private int amount;
    public int bandAmount;

    private SpectrumController spectrumScript;

    private Vector3 oriScale;
    private Vector3 bandOriScale;

    public float maxHeight;
    public float bandMaxHeight;

    private float[] spectrum;
    private float[] freqBands;
    private float[] bandBuffer;

    private float[] audioFreqBands;
    private float[] audioBandBuffer;

    private float[] bufferReducer;
    private float[] highestFreqband;

    private Material[] bandMats;
    private float colorSpeed;
    public bool useBuffer = true;

    private bool isBeat;
    private bool beatDetected;

    public Material vfxMaterial;
    public Material vfxBurstMat;

    public GameObject burstPrefab;

    private void Start()
    {
        InitVariables();
        CreateSimpleCubes();
        CreateBandCubes();
        //CreateRing();
    }

    private void InitVariables()
    {
        spectrumScript = FindObjectOfType<SpectrumController>();
        amount = spectrumScript.GetSpectrumSize();

        freqBands = new float[bandAmount];
        bandBuffer = new float[bandAmount];

        audioFreqBands = new float[bandAmount];
        audioBandBuffer = new float[bandAmount];

        bufferReducer = new float[bandAmount];
        highestFreqband = new float[bandAmount];
    }

    private void CreateBandCubes()
    {
        bandOriScale = bandCubePrefab.transform.localScale;
        float offset = bandOriScale.x;
        bandCubes = new Transform[bandAmount];
        bandMats = new Material[bandAmount];
        Transform parent = new GameObject("BandCubes").transform;

        float colorOffset = (float)1 / 25;
        for (int i = 0; i < bandAmount; i++)
        {
            bandCubes[i] = Instantiate(bandCubePrefab).transform;
            bandCubes[i].SetParent(parent);
            bandCubes[i].position = Vector3.right * offset * i;

            bandMats[i] = bandCubes[i].GetComponentInChildren<MeshRenderer>().material;
            Color col = Color.HSVToRGB(colorOffset * i, 0.8f, 1);
            bandMats[i].color = col;
        }
    }

    private void CreateSimpleCubes()
    {
        float offset = oriScale.x;
        oriScale = cubePrefab.transform.localScale;
        cubes = new Transform[amount];
        Transform parent = new GameObject("SimpleCubes").transform;
        for (int i = 0; i < amount; i++)
        {
            cubes[i] = Instantiate(cubePrefab).transform;
            cubes[i].SetParent(parent);
            cubes[i].position = Vector3.right * offset * i;
        }
    }

    private void CreateRing()
    {
        oriScale = cubePrefab.transform.localScale;
        ringCubes = new Transform[amount];

        Transform parent = new GameObject("RingParent").transform;
        float angleOffset = (float)360 / 512;
        float radius = 3;
        for (int i = 0; i < amount; i++)
        {
            float currentAngle = angleOffset * i;

            Vector3 pos;
            pos.x = radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            pos.y = radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            pos.z = 0;

            Vector3 rot;
            rot.x = 0;
            rot.y = 0;
            rot.z = currentAngle - 90;

            ringCubes[i] = Instantiate(ringCubePrefab).transform;
            ringCubes[i].SetParent(parent);
            ringCubes[i].position = pos;
            ringCubes[i].localEulerAngles = rot;
        }
    }

    private void ChangeBandColors()
    {
        colorSpeed += Time.deltaTime * 0.1f;
        float colorOffset = (float)1 / 25;
        for (int i = 0; i < bandAmount; i++)
        {
            Color col = Color.HSVToRGB((colorOffset * i + colorSpeed) % 1, 0.8f, 1);
            bandMats[i].color = col;
            if(beatDetected)
            {
                col = Color.HSVToRGB((colorOffset * i + colorSpeed + 0.38f) % 1, 0.8f, 1);
                vfxBurstMat.color = col;
                bandMats[i].color = col;
            }
            if(i == bandAmount / 2)
            {
                //col = Color.HSVToRGB((colorOffset * i + colorSpeed + 0.38f) % 1, 0.8f, 1);
                vfxMaterial.color = col;
            }
        }
    }

    private void Update()
    {
        spectrum = spectrumScript.GetSpectrum();
        //UpdateSimpleCubes();
        //UpdateRingCubes();
        ChangeBandColors();
        SetFrecBand();
        SetHightestFreq();

        if (useBuffer)
        {
            BandBuffer();
            UpdateBandCubes(audioBandBuffer);
        }
        else
        {
            UpdateBandCubes(audioFreqBands);
        }

        DetectBeat();
    }

    private void UpdateBandCubes(float[] data)
    {
        for (int i = 0; i < bandCubes.Length; i++)
        {
            Vector3 newScale = bandOriScale + Vector3.up * data[i] * bandMaxHeight;
            newScale.y = Mathf.Clamp(newScale.y, 0, float.MaxValue);
            bandCubes[i].localScale = newScale;
        }
    }

    private void DetectBeat()
    {
        if (freqBands[0] > 4)
        {
            isBeat = true;
            if (!beatDetected)
            {
                beatDetected = true;
            }
        }
        else if (freqBands[0] < 2)
        {
            beatDetected = false;
            isBeat = false;
        }
    }

    private void UpdateSimpleCubes()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 newScale = oriScale + Vector3.up * spectrum[i] * maxHeight;
            cubes[i].localScale = newScale;
        }
    }

    private void UpdateRingCubes()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 newScale = oriScale + Vector3.up * spectrum[i] * maxHeight;
            ringCubes[i].localScale = newScale;
        }
    }

    private void BandBuffer()
    {
        for (int i = 0; i < bandAmount; i++)
        {
            if (freqBands[i] > bandBuffer[i])
            {
                bandBuffer[i] = freqBands[i];
                bufferReducer[i] = 0.005f;
            }

            if (freqBands[i] < bandBuffer[i])
            {
                bandBuffer[i] -= bufferReducer[i];
                bufferReducer[i] *= 1.2f;
            }
        }
    }

    private void SetHightestFreq()
    {
        for (int i = 0; i < bandAmount; i++)
        {
            if(freqBands[i] > highestFreqband[i])
            {
                highestFreqband[i] = freqBands[i];
            }
            audioFreqBands[i] = freqBands[i] / highestFreqband[i];
            audioBandBuffer[i] = bandBuffer[i] / highestFreqband[i];
        }
    }
    private void SetFrecBand()
    {
        int count = 0;
        for (int i = 0; i < freqBands.Length; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if(i == freqBands.Length - 1)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += spectrum[count] * (count + 1);
                count++;
            }
            average /= count;

            freqBands[i] = average * 10;
        }
    }
}
