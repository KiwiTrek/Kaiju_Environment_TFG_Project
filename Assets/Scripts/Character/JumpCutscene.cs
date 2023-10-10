using UnityEngine;

public class JumpCutscene : MonoBehaviour
{
    [SerializeField] private CharacterMov character = null;
    [SerializeField] private GameObject pressSpaceUI = null;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pressSpaceUI.SetActive(true);
            float distance = Mathf.Abs(Vector3.Distance(character.transform.position, character.jumpToTrunkFinalPos.transform.position));
            if (Input.GetButtonDown("Jump"))
            {
                pressSpaceUI.SetActive(false);
                float finalGravity = -(distance / (5.0f - (distance * 0.075f))) - 0.9f;
                character.verticalMov.y = 0.0f;
                character.verticalMov.y += Mathf.Sqrt(character.jumpHeight * finalGravity * character.gravity);
                character.PlaySound(SoundType.Jump);
                GameplayDirector.cutsceneMode = CutsceneType.JumpToBoss;
                Debug.Log("Jump to trunk mode: On!");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float distance = Mathf.Abs(Vector3.Distance(character.transform.position, character.jumpToTrunkFinalPos.transform.position));
            if (Input.GetButtonDown("Jump"))
            {
                pressSpaceUI.SetActive(false);
                float finalGravity = -(distance / (5.0f - (distance * 0.075f))) - 0.9f;
                character.verticalMov.y = 0.0f;
                character.verticalMov.y += Mathf.Sqrt(character.jumpHeight * finalGravity * character.gravity);
                character.PlaySound(SoundType.Jump);
                GameplayDirector.cutsceneMode = CutsceneType.JumpToBoss;
                Debug.Log("Jump to trunk mode: On!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pressSpaceUI.SetActive(false);
        }
    }
}
