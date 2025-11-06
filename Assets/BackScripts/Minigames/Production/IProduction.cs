using UnityEngine;
using System.Collections;
public interface IProduction
{
    public bool canInput { get; set; }
    public bool canOutput { get; set; }
    public productionItem inputItem { get; set; }
    public productionItem outputItem { get; set; }

    public IEnumerator produce();
}
