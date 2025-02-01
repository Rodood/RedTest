using UnityEngine;

/// <summary>
/// Um script bem simples só para tocar a animação do texto de energia máxima aparecendo
/// </summary>

public class HideText : MonoBehaviour
{
    public void HideGameObject()
    {
        this.gameObject.SetActive(false);
    }
}
