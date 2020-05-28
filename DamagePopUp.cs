using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public interface IText
{
    void SetText(string text, float Value);
    void LevelUpText();
}
public class DamagePopUp : MonoBehaviour, IText
{
    private Animator TextAnim;
    private Text Text;
    public bool DontDeParent;
    private void Awake()
    {
        TextAnim = GetComponent<Animator>();
        Text = GetComponent<Text>();
    }
    private void OnEnable()
    {
        StartCoroutine(WaitForAnimation());
    }
    private IEnumerator WaitForAnimation()
    {
        if (TextAnim.isActiveAndEnabled)
        {
            AnimatorClipInfo[] ClipInfo = TextAnim.GetCurrentAnimatorClipInfo(0);

            if (ClipInfo.Length > 0)
                yield return new WaitForSeconds(ClipInfo[0].clip.length);
        }

        gameObject.SetActive(false);
        if(!DontDeParent)
        transform.SetParent(ObjectPooler.Instance.transform);
    }

    public void SetText(string text, float Value)
    {
        Text.text = Value.ToString("F0") + text;
    }
    public void LevelUpText()
    {
        Text.text = "LEVEL UP";
    }
}
