using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Base;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class Toy : Item
    {
        [SerializeField] private float velocity;
        [SerializeField] private float force;
        [SerializeField] private float torqueForce;
        [SerializeField] private Vector2 randomTimer;
        [SerializeField] private Rigidbody2D rb;

        [NaughtyAttributes.Button]
        private void Spawn()
        {
            if (!Application.isPlaying) return;

            var item = Instantiate(this, transform.parent);
            item.Fly();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Constant.TAG.DEATHZONE))
            {
                Destroy(gameObject);
            }
        }
        protected override void Init()
        {
        }
        private void Fly()
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddTorque(torqueForce);
            StartCoroutine("OnFlying");
        }
        private IEnumerator OnFlying()
        {
            rb.AddForce(Vector2.right * UnityEngine.Random.Range(-force, force));
            yield return new WaitForSeconds(UnityEngine.Random.Range(randomTimer.x, randomTimer.y));

            StartCoroutine("OnFlying");
        }
    }
}
