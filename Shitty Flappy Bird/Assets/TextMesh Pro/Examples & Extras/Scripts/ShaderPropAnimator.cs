using System.Collections;

using UnityEngine;


namespace TMPro.Examples
{
  public class ShaderPropAnimator : MonoBehaviour
  {
    public AnimationCurve GlowCurve;

    public float m_frame;

    private Material m_Material;

    private Renderer m_Renderer;

    private void Awake()
    {
      // Cache a reference to object's renderer
      this.m_Renderer = this.GetComponent<Renderer>();

      // Cache a reference to object's material and create an instance by doing so.
      this.m_Material = this.m_Renderer.material;
    }

    private void Start()
    {
      this.StartCoroutine(this.AnimateProperties());
    }

    private IEnumerator AnimateProperties()
    {
      //float lightAngle;
      float glowPower;
      this.m_frame = Random.Range(0f, 1f);

      while (true)
      {
        //lightAngle = (m_Material.GetFloat(ShaderPropertyIDs.ID_LightAngle) + Time.deltaTime) % 6.2831853f;
        //m_Material.SetFloat(ShaderPropertyIDs.ID_LightAngle, lightAngle);

        glowPower = this.GlowCurve.Evaluate(this.m_frame);
        this.m_Material.SetFloat(ShaderUtilities.ID_GlowPower, glowPower);

        this.m_frame += Time.deltaTime * Random.Range(0.2f, 0.3f);

        yield return new WaitForEndOfFrame();
      }
    }
  }
}
