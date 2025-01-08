using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValueChangedEvent : MonoBehaviour
{
    public Observable<int> intValue;
    private void OnEnable()
    {
        //intValue.onValueChanged.AddListener(OnValueChanged);
    }

    private void Start()
    {
        //intValue.Value = 5;
        //intValue.Value = 5;
        //intValue.Value = 10; 
    }

    private void OnValueChanged(int newValue)
    {
        //Debug.Log($"new Value : {newValue}");
    }
}

[System.Serializable]
public class Observable<T>
{
    [SerializeField] private T value;
    public UnityEvent<T> onValueChanged;
    public T Value
    {
        get => value;
        set
        {
            T oldValue = value;
            this.value = value;
            onValueChanged?.Invoke (value);
        }
        //set
        //{
        //    if (!Equals(this.value, value))
        //    {
        //        this.value = value;
        //        onValueChanged?.Invoke(value);
        //    }
        //}

    }
}
