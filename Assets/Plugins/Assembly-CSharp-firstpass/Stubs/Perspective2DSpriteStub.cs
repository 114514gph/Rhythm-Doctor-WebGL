using DG.Tweening;
using UnityEngine;

public partial class Perspective2DSprite : MonoBehaviour
{
    public Sprite sprite { get; set; }
    public Color color { get; set; }
    public virtual void SetSize(int width, int height) { }
}
