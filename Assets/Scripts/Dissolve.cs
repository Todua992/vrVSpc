using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private float timeTillDissolve;
    private float currentTime;
    List<Material> materials = new List<Material>();

    void Start()
    {
        currentTime = -15f;
        var renders = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renders.Length; i++)
        {
            materials.AddRange(renders[i].materials);
        }
    }


    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > 0f)
        {
            var value = Mathf.Lerp(0f, 1f, currentTime / timeTillDissolve);
            SetValue(value);
            Destroy(gameObject, 3.5f );
        }
       
    }


    public void SetValue(float value)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].SetFloat("_Dissolve", value);
        }
    }
}