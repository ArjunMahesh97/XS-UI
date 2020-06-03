using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class WordSearchTutorial : MonoBehaviour
{
    public List<RectTransform> Transforms;
    public RectTransform FingerSprite;

    public Transform lineConnectorParent;
    public GameObject lineConnectorPrefab;
    private UILineConnector uiLineConnector;

    public int currentTransform = 0;

    IEnumerator Work()
    {
        GameObject go = Instantiate(lineConnectorPrefab, lineConnectorParent);
        uiLineConnector = go.GetComponent<UILineConnector>();
        uiLineConnector.GetComponent<UILineRenderer>().color = new Color(.73f, .25f, .19f);

        yield return new WaitForSeconds(0.1f);

        FingerSprite.position = Transforms[0].position;

        while (true)
        {
            currentTransform = 0;
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < Transforms.Count; i++)
            {
                currentTransform = i;
                yield return new WaitForSeconds(0.1f);
                uiLineConnector.transforms.Add(Transforms[i]);
                yield return new WaitForSeconds(0.4f);
            }

            uiLineConnector.transforms.Clear();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(Work());
    }

    public void SetVisible(bool visible)
    {
        uiLineConnector.GetComponent<UILineRenderer>().enabled = visible;
        FingerSprite.GetComponent<Image>().enabled = visible;
    }

    void Update()
    {
        FingerSprite.position =
            Vector2.Lerp(FingerSprite.position, Transforms[currentTransform].position, Time.deltaTime*10);
    }
}
