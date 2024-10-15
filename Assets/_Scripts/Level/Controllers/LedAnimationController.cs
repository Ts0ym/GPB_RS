using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AwakeComponents.ArtNet
{
    public class LedAnimationController : MonoBehaviour
    {
        public LedController ledController;
        private int activeThemeIndex = 1;

        public enum AnimationState
        {
            Idle,
            Active,
            ReturnToIdle
        }

        public AnimationState currentState = AnimationState.Idle;

        void Start()
        {
            if (ledController == null)
            {
                Debug.LogError("LedController не установлен!");
                return;
            }

            StartCoroutine(IdleAnimation());
        }

        public void SetState(AnimationState newState, int themeIndex = 1)
        {
            if (newState == AnimationState.Active)
            {
                activeThemeIndex = themeIndex;
            }

            if (currentState != newState)
            {
                currentState = newState;
                StopAllCoroutines();

                switch (newState)
                {
                    case AnimationState.Idle:
                        StartCoroutine(IdleAnimation());
                        break;
                    case AnimationState.Active:
                        StartCoroutine(ActiveAnimation());
                        break;
                    case AnimationState.ReturnToIdle:
                        StartCoroutine(ReturnToIdleAnimation());
                        break;
                }
            }
        }

        private IEnumerator IdleAnimation()
        {
            yield return null;
            while (currentState == AnimationState.Idle)
            {
                SetAllPedestalsColor(Color.white);
                ledController.ShowStrip();
                yield return new WaitForSeconds(2f);
                yield return StartCoroutine(PedestalWaveAnimation());
                yield return StartCoroutine(ReaderPulseAnimation());
            }
        }

        private void SetAllPedestalsColor(Color color)
        {
            for (int i = 1; i <= 6; i++)
            {
                SetPedestalColor(i, color);
            }
        }

        private void SetPedestalColor(int pedestalIndex, Color color)
        {
            int startPixel = pedestalIndex * 35;
            for (int j = 0; j < 35; j++)
            {
                ledController.SetPixel(startPixel + j, (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255));
            }
        }

        private IEnumerator PedestalWaveAnimation()
        {
            float duration = 0.3f;
            float interval = 0.1f;

            for (int i = 1; i <= 6; i++)
            {
                yield return StartCoroutine(FadePedestal(i, 1f, 0.7f, duration));
                yield return new WaitForSeconds(interval);
            }

            for (int i = 1; i <= 6; i++)
            {
                SetPedestalColor(i, Color.white);
            }
            ledController.ShowStrip();
        }

        private IEnumerator FadePedestal(int pedestalIndex, float fromBrightness, float toBrightness, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float brightness = Mathf.Lerp(fromBrightness, toBrightness, t);
                Color color = Color.white * brightness;
                SetPedestalColor(pedestalIndex, color);
                ledController.ShowStrip();

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator ReaderPulseAnimation()
        {
            int readerIndex = 0;
            int startPixel = readerIndex * 35;

            for (int pulse = 0; pulse < 2; pulse++)
            {
                SetReaderColor(Color.white);
                ledController.ShowStrip();
                yield return new WaitForSeconds(0.1f);
                yield return StartCoroutine(FadeReader(1f, 0f, 0.5f));
            }

            yield return new WaitForSeconds(1f);
        }

        private void SetReaderColor(Color color)
        {
            int startPixel = 0;
            for (int j = 0; j < 35; j++)
            {
                ledController.SetPixel(startPixel + j, (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255));
            }
        }

        private IEnumerator FadeReader(float fromBrightness, float toBrightness, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float brightness = Mathf.Lerp(fromBrightness, toBrightness, t);
                Color color = Color.white * brightness;
                SetReaderColor(color);
                ledController.ShowStrip();

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator ActiveAnimation()
        {
            int startingIndex = activeThemeIndex;
            yield return StartCoroutine(PedestalFadeOutWave(startingIndex));

            while (currentState == AnimationState.Active)
            {
                yield return StartCoroutine(ReaderRotationAnimation());
            }
        }

        private IEnumerator PedestalFadeOutWave(int startingIndex)
        {
            float duration = 0.1f;
            float interval = 0.1f;

            List<int> pedestalOrder = new List<int>();
            for (int i = startingIndex; i <= 6; i++)
            {
                pedestalOrder.Add(i);
            }
            for (int i = 1; i < startingIndex; i++)
            {
                pedestalOrder.Add(i);
            }

            foreach (int i in pedestalOrder)
            {
                yield return StartCoroutine(FadePedestal(i, 1f, 0f, duration));
                yield return new WaitForSeconds(interval);
            }
        }

        private IEnumerator ReaderRotationAnimation()
        {
            int readerIndex = 0;
            int startPixel = readerIndex * 35;
            int segmentLength = 35 / 4;

            while (currentState == AnimationState.Active)
            {
                for (int step = 0; step < 35; step++)
                {
                    ClearReader();
                    int segment1Start = (step) % 35;
                    int segment2Start = (step + 17) % 35;

                    SetReaderSegment(segment1Start, segmentLength, Color.white);
                    SetReaderSegment(segment2Start, segmentLength, Color.white);

                    ledController.ShowStrip();
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        private void ClearReader()
        {
            int startPixel = 0;
            for (int j = 0; j < 35; j++)
            {
                ledController.SetPixel(startPixel + j, 0, 0, 0);
            }
        }

        private void SetReaderSegment(int startPixel, int length, Color color)
        {
            int readerStart = 0;
            for (int j = 0; j < length; j++)
            {
                int pixelIndex = (startPixel + j) % 35;
                ledController.SetPixel(readerStart + pixelIndex, (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255));
            }
        }

        private IEnumerator ReturnToIdleAnimation()
        {
            float duration = 1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float brightness = Mathf.Lerp(0f, 1f, t);
                Color color = Color.white * brightness;
                SetAllPedestalsColor(color);
                ledController.ShowStrip();

                elapsed += Time.deltaTime;
                yield return null;
            }

            SetState(AnimationState.Idle);
        }
    }
}
