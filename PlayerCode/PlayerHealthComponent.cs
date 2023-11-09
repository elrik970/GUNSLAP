using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthComponent : MonoBehaviour, IDamageable
{
    // Start is called before the first frame update
    public Animator animator;
    private bool Damageable = true;
    public float Health;
    [SerializeField] private float InvizTime;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0) {
            Destroy(gameObject);
            if (gameObject.tag == "Player") {
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
        }
    }
    public void Damage(float DamageAmount) {
        Debug.Log("DAMAGE");
        if (Damageable) {
            StartCoroutine(Inviz());
            Health-=DamageAmount;
            animator.Play("HitAnim");
        }
    }
    IEnumerator Inviz()
    {
        
        if (InvizTime != 0) {
            Damageable = false;
            yield return new WaitForSeconds(InvizTime);
            Damageable = true;
        }
        else {
            Damageable = true;
        }
        
    }
}
