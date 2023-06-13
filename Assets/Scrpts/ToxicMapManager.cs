using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;

public class ToxicMapManager : MonoBehaviour
{
    int damage = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<PlayerController>().TakenDamage(damage);
    }

    public void DontClick()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
