using UnityEngine;

/// <summary>
/// Um script bem simples s� para tocar a anima��o do texto de energia m�xima aparecendo
/// </summary>

public class HideText : MonoBehaviour
{
    public void HideGameObject()
    {
        this.gameObject.SetActive(false);
    }
}
