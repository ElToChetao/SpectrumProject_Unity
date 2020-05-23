using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VisualizationController : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject bandCubePrefab;

    private Transform[] cubes;
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
        spectrumScript = FindObjectOfType<SpectrumController>();

        amount = spectrumScript.GetSpectrumSize();

        freqBands = new float[bandAmount];
        bandBuffer = new float[bandAmount];

        audioFreqBands = new float[bandAmount];
        audioBandBuffer = new float[bandAmount];

        bufferReducer = new float[bandAmount];
        highestFreqband = new float[bandAmount];

        cubes = new Transform[amount];
        bandCubes = new Transform[bandAmount];

        oriScale = cubePrefab.transform.localScale;
        bandOriScale = bandCubePrefab.transform.localScale;
        float offset = oriScale.x;

        //for (int i = 0; i < amount; i++)
        //{
        //    cubes[i] = Instantiate(cubePrefab).transform;
        //    cubes[i].SetParent(transform);
        //    cubes[i].position = Vector3.right * offset * i;
        //}
        offset = bandOriScale.x;

        bandMats = new Material[bandAmount];
        float colorOffset = (float)1 / 25;
        for (int i = 0; i < bandAmount; i++)
        {
            bandCubes[i] = Instantiate(bandCubePrefab).transform;
            bandCubes[i].SetParent(transform);
            bandCubes[i].position = Vector3.right * offset * i;

            bandMats[i] = bandCubes[i].GetComponentInChildren<MeshRenderer>().material;
            Color col = Color.HSVToRGB(colorOffset * i, 0.8f, 1);
            bandMats[i].color = col;
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
        //for (int i = 0; i < amount; i++)
        //{
        //    Vector3 newScale = oriScale + Vector3.up * spectrum[i] * maxHeight;
        //    cubes[i].localScale = newScale;
        //}
        ChangeBandColors();
        SetFrecBand();
        SetHightestFreq();

        if (useBuffer)
        {
            BandBuffer();
            for (int i = 0; i < bandCubes.Length; i++)
            {
                Vector3 newScale = bandOriScale + Vector3.up * audioBandBuffer[i] * bandMaxHeight;
                newScale.y = Mathf.Clamp(newScale.y, 0, float.MaxValue);
                bandCubes[i].localScale = newScale;
            }
        }
        else
        {
            for (int i = 0; i < bandCubes.Length; i++)
            {
                Vector3 newScale = bandOriScale + Vector3.up * audioFreqBands[i] * bandMaxHeight;
                newScale.y = Mathf.Clamp(newScale.y, 0, float.MaxValue);
                bandCubes[i].localScale = newScale;
            }
        }

        if (freqBands[0] > 4)
        {
            isBeat = true;
            if (!beatDetected)
            {
                beatDetected = true;
                //GameObject go = Instantiate(burstPrefab);
                //go.transform.position = new Vector3(4, 0, -1);
                //Destroy(go, 4);
            }
        }
        else if (freqBands[0] < 2)
        {
            beatDetected = false;
            isBeat = false;
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
