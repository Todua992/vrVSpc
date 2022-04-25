using UnityEngine;

public class CannonLaunch : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private GameObject Cannonball;
    [SerializeField] private float ballSpeed;
    [SerializeField] private AudioSource _audiosource;
    [SerializeField] private GameObject particle;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Shoot();
            _audiosource.Play();
        }
    }

    private void Shoot()
    {
        
        GameObject Cannonball = Instantiate(this.Cannonball, start.position, start.rotation);
        Rigidbody rb = Cannonball.GetComponent<Rigidbody>();
        rb.AddForce(start.forward * ballSpeed, ForceMode.Impulse);

        // Afspiller partikler
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();

        ps.Play();

    }
}