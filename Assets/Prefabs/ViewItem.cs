using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ViewItem : MonoBehaviour, ISelectHandler
{
    void Start()
    {
    }

	public void OnSelect(BaseEventData eventData)
    {
        InteractionsScript.Instance.selected = this.gameObject.GetComponent<Text>().text;
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        InteractionsScript.Instance.selected = null;
        print("Deselected");
    }
}
