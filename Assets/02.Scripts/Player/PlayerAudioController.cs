using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] AudioClip footClip;

    [SerializeField] PlayerInputReader playerInputReader;
    [SerializeField] PlayerStamina playerStamina;
    Coroutine footstepRoutine;
    [SerializeField, Range(0f, 1f)] float walkVolume;
    private void Awake()
    {
        if(playerAudioSource == null) playerAudioSource = GetComponent<AudioSource>();
        if(playerInputReader == null) playerInputReader = GetComponentInParent<PlayerInputReader>();
        if(playerStamina == null) playerStamina = GetComponentInParent<PlayerStamina>();
    }

    private void Update()
    {
        if(playerInputReader.MoveDir.sqrMagnitude > 0.01f)
        {
            if (footstepRoutine == null && Keyboard.current.shiftKey.IsPressed() && !playerStamina.IsStaminaDepleted)
            {
                footstepRoutine = StartCoroutine(CoFootRunStepSFXPlay());
            }
            else if (footstepRoutine == null && !Keyboard.current.shiftKey.IsPressed() && !playerStamina.IsStaminaDepleted)
            {
                footstepRoutine = StartCoroutine(CoFootWalkStepSFXPlay());
            }
            else if (footstepRoutine == null && playerStamina.IsStaminaDepleted)
            {
                footstepRoutine = StartCoroutine(CoFootStaminaDepletedStepSFXPlay());
            }
                return;
        }
        else
        {
            if (footstepRoutine != null)
            {
                playerAudioSource.Stop();
                StopCoroutine(footstepRoutine);
                footstepRoutine = null;
            }
        }
    }
    IEnumerator CoFootStaminaDepletedStepSFXPlay()
    {
        playerAudioSource.clip = footClip;
        while (true)
        {
            if (!playerStamina.IsStaminaDepleted) break;
            playerAudioSource.Play();
            yield return new WaitForSeconds(0.4f);
        }
        footstepRoutine = null;
    }
    IEnumerator CoFootRunStepSFXPlay()
    {
        playerAudioSource.clip = footClip;
        playerAudioSource.volume = SoundManager.Instance.SfxVolume * (walkVolume * 1.4f) * SoundManager.Instance.MasterVolume;
        while (true)
        {
            if (!Keyboard.current.shiftKey.IsPressed() || playerStamina.IsStaminaDepleted)
            {
                break;
            }

            if(SoundManager.Instance.IsMasterMute || SoundManager.Instance.IsSfxMute)
            {
                continue;
            }
            playerAudioSource.Play();
            yield return new WaitForSeconds(0.23f);
        }
        footstepRoutine = null;
    }
    IEnumerator CoFootWalkStepSFXPlay()
    {
        playerAudioSource.clip = footClip;
        playerAudioSource.volume = SoundManager.Instance.SfxVolume * walkVolume * SoundManager.Instance.MasterVolume;
        while (true)
        {
            if (Keyboard.current.shiftKey.IsPressed()) break;
            if (SoundManager.Instance.IsMasterMute || SoundManager.Instance.IsSfxMute)
            {
                continue;
            }

            playerAudioSource.Play();
            yield return new WaitForSeconds(0.3f);
        }
        footstepRoutine = null;
    }
    private void OnValidate()
    {
        walkVolume = Mathf.Round(walkVolume * 100f) / 100f;
    }
}
