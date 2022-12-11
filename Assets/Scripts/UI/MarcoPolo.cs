using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarcoPolo : MonoBehaviour
{
    [SerializeField] private Button marcoPoloButton;
    [SerializeField] private TextMeshProUGUI resultTextField;

    private StringBuilder stringBuilder = new StringBuilder(900);

    private void OnEnable()
    {
        marcoPoloButton.GetComponent<Button>().onClick.AddListener(PrintMarcoPolo);
    }

    public void PrintMarcoPolo()
    {
        stringBuilder.Clear();

        for (int i = 1; i < 101; i++)
        {
            bool isDivisibleByAny = false;

            if (i % 3 == 0)
            {
                isDivisibleByAny = true;
                stringBuilder.Append("Marko");
            }

            if (i % 5 == 0)
            {
                isDivisibleByAny = true;
                stringBuilder.Append("Polo");
            }

            if(!isDivisibleByAny)
                stringBuilder.Append(i);

            stringBuilder.Append(" ");
        }

        resultTextField.SetText(stringBuilder);
    }
}
