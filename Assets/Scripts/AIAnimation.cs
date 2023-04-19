using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimation : MonoBehaviour
{
    // I want to control the pace of the scale and rotation outside this class
    public float _rotationSpeed = 5f;
    public Vector3 _vectorScale = new Vector3(0.01f, 0.01f, 0.01f);

    // All the layers of that one tiny ball... yep, a lot of layers
    [SerializeField] private GameObject _outterlayer;
    [SerializeField] private GameObject _webLayer;
    [SerializeField] private GameObject _fringeLayer;
    [SerializeField] private GameObject _coreLayer;

    // things that I can manipulate with rotation, scales, and color 
    [SerializeField] private int _colorRange;
    [SerializeField] private float _rotationDegree = 2.0f;
    // the biggest size within the current cube size without clipping is 150
    [SerializeField] private float _maxScaleSize = 150f;

    // things that I don't want to be set default from the start
    private bool scaleIncrease = true;
    private float _originalSize;

    private void Start()
    {
        // any direction is fine cause they are all the same size
        _originalSize = _fringeLayer.transform.localScale.x;
    }

    private void Update()
    {
        RandomRotate();
        VoiceResponseScale();
    }

    // turn the cube in random rocation, in my case, starting from zero to 360 with 2 degree increment
    void RandomRotate()
    {
        // rotate the vector 360 degrees
        Vector3 rotationDegrees = _rotationDegree * new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
        _outterlayer.transform.Rotate(rotationDegrees, _rotationSpeed * Time.deltaTime); // _rotationSpeed makes it sping slower
        _webLayer.transform.Rotate(-rotationDegrees, _rotationSpeed * Time.deltaTime);
    }

    // Create a pulsing effect when "talking"
    void VoiceResponseScale()
    {
        if (scaleIncrease)
        {
            if (_coreLayer.transform.localScale.x >= _maxScaleSize) scaleIncrease = false;
            _coreLayer.transform.localScale += _vectorScale;
            _fringeLayer.transform.localScale += _vectorScale;
        }
        else
        {
            if (_coreLayer.transform.localScale.x <= _originalSize) scaleIncrease = true;
            _coreLayer.transform.localScale -= _vectorScale;
            _fringeLayer.transform.localScale -= _vectorScale;
        }
    }

    public void setAIAnimationResponse(float rotation, Vector3 scale)
    {
        _rotationSpeed = rotation;
        _vectorScale = scale;
    }

}