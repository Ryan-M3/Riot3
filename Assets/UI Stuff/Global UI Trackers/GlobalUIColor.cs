using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Elements which are not entirely diegetic are color coded. However,
/// this color is constantly shifting (this is the same sort of logic
/// as having powerups spinning or glowing). Basically, this object
/// just reports what the current color is.
/// </summary>
public class GlobalUIColor : MonoBehaviour {
    public float speed = 3f;
    protected Color curColor = new Color(1f, 0f, 0f);
    protected List<Color> colorHistory = new List<Color>();
    protected Color lastColor;
    protected Color nextColor;
    protected Gradient grad;

    public Color GetColor() {
        return curColor;
    }

    public Gradient GetGrandient() {
        return MakeGradient(GetColor(), lastColor);
    }

    protected void Start() {
        StartCoroutine("Shift");
    }

    protected IEnumerable<Color> InfiniteColors() {
        while (true) {
            float h, s, v;
            Color.RGBToHSV(new Color(1f, 0f, 0f), out h, out s, out v);
            yield return Color.HSVToRGB(Random.Range(0f, 1f), s, v);
        }
    }

    protected IEnumerator Shift() {
        float t;
        lastColor = curColor;
        foreach (Color nxt in InfiniteColors()) {
            nextColor = nxt;
            t = 0f;
            while (t < speed) {
                t += Time.deltaTime;
                curColor = Color.Lerp(lastColor, nextColor, t / speed);
                yield return null;
            }
            lastColor = nextColor;
        }
    }

    protected static Gradient MakeGradient(Color colorFrom, Color colorTo) {
        GradientColorKey[] gck = new GradientColorKey[2];
        GradientAlphaKey[] gak = new GradientAlphaKey[2];
        gck[0].color = colorFrom;
        gak[0].alpha = 1.0f;
        gak[0].time  = 0.0f;
        gck[1].color = colorTo;
        gak[1].alpha = 0.0f;
        gak[1].time  = 1.0f;
        Gradient g = new Gradient();
        g.SetKeys(gck, gak);
        return g;
    }
}
