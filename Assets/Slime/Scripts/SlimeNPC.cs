using UnityEngine;

public class SlimeNPC : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            collision.gameObject.GetComponent<Collider>().enabled = false;
            collision.gameObject.transform.SetParent(transform);
            collision.gameObject.transform.localPosition = Random.insideUnitSphere * 0.5f;
            collision.gameObject.transform.localScale *= 0.2f;

            collision.gameObject.AddComponent<FloatInSlime>();
            //transform.localScale += new Vector3(growthRate, growthRate, growthRate);
        }
    }
}

