using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableEvents/Void Event")]
public class VoidEventChannelSO : ScriptableObject {

    public readonly UnityEvent OnRaise = new UnityEvent();

}