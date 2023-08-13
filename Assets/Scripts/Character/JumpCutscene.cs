using UnityEngine;

public class JumpCutscene : MonoBehaviour
{
    [SerializeField] private CharacterMov character = null;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetButtonDown("Jump"))
            {
                character.verticalMov.y = 0.0f;
                character.verticalMov.y += Mathf.Sqrt(character.jumpHeight * -4.5f * character.gravity);
                character.PlaySound(SoundType.Jump);
                GameplayDirector.cutsceneMode = CutsceneType.JumpToBoss;
                Debug.Log("Jump to trunk mode: On!");
            }
            Debug.Log("It's a cutscene!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetButtonDown("Jump"))
            {
                character.verticalMov.y = 0.0f;
                character.verticalMov.y += Mathf.Sqrt(character.jumpHeight * -4.5f * character.gravity);
                character.PlaySound(SoundType.Jump);
                GameplayDirector.cutsceneMode = CutsceneType.JumpToBoss;
                Debug.Log("Jump to trunk mode: On!");
            }
            Debug.Log("It's a cutscene!");
        }
    }
}
