using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Renderer))]
public class BulletHoleDecal : MonoBehaviour
{
    static private Vector2[] quadUVs = new Vector2[]{new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};

    public float fadeDelay = 10f;
    public float fadeDuration = 2f;
    public Vector2 frames;
    public Vector2 randomSize = Vector2.one;
    public string materialName = "_MainColor";

    private Renderer _renderer;
    private Material _material;
    private Color baseColor;
    private float timer;

    void Start(){
        _renderer=GetComponent<Renderer>();
        _material=_renderer.material;
        baseColor=_material.GetColor(materialName);

        transform.localScale=transform.localScale*Random.Range(randomSize.x,randomSize.y);
        //Random UVs
		int random = Random.Range(0, (int)(frames.x*frames.y));
		int fx = (int)(random%frames.x);
		int fy = (int)(random/frames.y);
		// Set new UVs
		Vector2[] meshUvs = new Vector2[4];
		for(int i = 0; i < 4; i++)
		{
			meshUvs[i].x = (quadUVs[i].x + fx) * (1.0f/frames.x);
			meshUvs[i].y = (quadUVs[i].y + fy) * (1.0f/frames.y);
		}
		this.GetComponent<MeshFilter>().mesh.uv = meshUvs;
    }

    void Update(){
        timer+=Time.deltaTime;
        if(timer>fadeDelay){
            float alpha=Mathf.Clamp01((timer-fadeDelay)/fadeDuration);
            _material.SetColor(materialName,new Color(baseColor.r,baseColor.g,baseColor.b,baseColor.a*(1-alpha)));
            if(alpha<=0){
                Destroy(gameObject);
            }
        }
    }
}
