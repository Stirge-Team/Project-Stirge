using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneRestarter : MonoBehaviour
{
    public void ReloadCurrentScene(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log($"Loading scene {SceneManager.GetActiveScene().name}...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
