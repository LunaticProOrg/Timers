using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class TextViewer : IViewable
{
    protected readonly TextMeshProUGUI text;

    public TextViewer(TextMeshProUGUI _text)
    {
        text = _text;
    }

    public virtual void Viewing(string message)
    {
        try
        {
            text.text = message;
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
