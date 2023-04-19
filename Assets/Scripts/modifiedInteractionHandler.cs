/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Meta.WitAi;
using Meta.WitAi.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Voice.Demo
{
    public class modifiedInteractionHandler : MonoBehaviour
    {
        [Header("Default States"), Multiline]
        [SerializeField] private string freshStateText = "Try pressing the Activate button and saying \"Make the cube red\"";

        [Header("UI")]
        [SerializeField] private TMP_InputField _textArea;
        //[SerializeField] private TMP_Text userText;
        [SerializeField] private bool _showJson; // for testing purposes

        [Header("Voice")]
        [SerializeField] private AppVoiceExperience _appVoiceExperience;

        [Header("BallAI")]
        [SerializeField] private GameObject _ballAI;
        private float _originalRotationSpeed;
        private Vector3 _originalVectorScale;

        // Whether voice is activated
        public bool IsActive => _active;
        private bool _active = false;

        private void Start()
        {
            _originalRotationSpeed = _ballAI.GetComponent<AIAnimation>()._rotationSpeed;
            _originalVectorScale = _ballAI.GetComponent<AIAnimation>()._vectorScale;
        }

        // Add delegates
        private void OnEnable()
        {
            _textArea.text = freshStateText;
            _appVoiceExperience.VoiceEvents.OnRequestCreated.AddListener(OnRequestStarted);
            _appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnRequestTranscript);
            _appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnRequestTranscript);
            _appVoiceExperience.VoiceEvents.OnStartListening.AddListener(OnListenStart);
            _appVoiceExperience.VoiceEvents.OnStoppedListening.AddListener(OnListenStop);
            _appVoiceExperience.VoiceEvents.OnStoppedListeningDueToDeactivation.AddListener(OnListenForcedStop);
            _appVoiceExperience.VoiceEvents.OnStoppedListeningDueToInactivity.AddListener(OnListenForcedStop);
            _appVoiceExperience.VoiceEvents.OnResponse.AddListener(OnRequestResponse);
            _appVoiceExperience.VoiceEvents.OnError.AddListener(OnRequestError);
        }
        // Remove delegates
        private void OnDisable()
        {
            _appVoiceExperience.VoiceEvents.OnRequestCreated.RemoveListener(OnRequestStarted);
            _appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnRequestTranscript);
            _appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnRequestTranscript);
            _appVoiceExperience.VoiceEvents.OnStartListening.RemoveListener(OnListenStart);
            _appVoiceExperience.VoiceEvents.OnStoppedListening.RemoveListener(OnListenStop);
            _appVoiceExperience.VoiceEvents.OnStoppedListeningDueToDeactivation.RemoveListener(OnListenForcedStop);
            _appVoiceExperience.VoiceEvents.OnStoppedListeningDueToInactivity.RemoveListener(OnListenForcedStop);
            _appVoiceExperience.VoiceEvents.OnResponse.RemoveListener(OnRequestResponse);
            _appVoiceExperience.VoiceEvents.OnError.RemoveListener(OnRequestError);
        }

        // Request began
        private void OnRequestStarted(WitRequest r)
        {
            // Store json on completion
            if (_showJson) r.onRawResponse = (response) => _textArea.text = response;
            // Begin
            _active = true;
        }
        // Request transcript
        private void OnRequestTranscript(string transcript)
        {
            _textArea.text = transcript;
        }
        // Listen start
        private void OnListenStart()
        {
            _textArea.text = $"Listening...";
            _ballAI.GetComponent<AIAnimation>()._rotationSpeed *= 10;
            _ballAI.GetComponent<AIAnimation>()._vectorScale *= 10;
        }
        // Listen stop
        private void OnListenStop()
        {
            _textArea.text = "Processing...";
            //_ballAI.GetComponent<AIAnimation>()._rotationSpeed = _originalRotationSpeed;
        }
        // Listen stop
        private void OnListenForcedStop()
        {
            if (!_showJson)
            {
                _textArea.text = freshStateText;
            }
            OnRequestComplete();
        }
        // Request response
        private void OnRequestResponse(WitResponseNode response)
        {
            if (!_showJson)
            {
                if (!string.IsNullOrEmpty(response["text"]))
                {
                    _textArea.text = response["text"];
                }
                else
                {
                    _textArea.text = freshStateText;
                }
            }
            OnRequestComplete();
        }
        // Request error
        private void OnRequestError(string error, string message)
        {
            if (!_showJson)
            {
                _textArea.text = $"<color=\"red\">Error: {error}\n\n{message}</color>";
            }
            OnRequestComplete();
        }
        // Deactivate
        private void OnRequestComplete()
        {
            _active = false;
            _ballAI.GetComponent<AIAnimation>()._rotationSpeed = _originalRotationSpeed;
            _ballAI.GetComponent<AIAnimation>()._vectorScale = _originalVectorScale;
        }

        // Toggle activation
        public void ToggleActivation()
        {
            SetActivation(!_active);
        }
        // Set activation
        public void SetActivation(bool toActivated)
        {
            if (_active != toActivated)
            {
                _active = toActivated;
                if (_active)
                {
                    _appVoiceExperience.Activate();
                }
                else
                {
                    _appVoiceExperience.Deactivate();
                }
            }
        }
    }
}
