using System.Collections.Generic;

// json won't save nested lists, so you have to write List<WrappedList> to save via Json, my main method
[System.Serializable]
public class WrappedList
{
    public List<float> values;

    public WrappedList()
    {

    }

    public WrappedList(List<float> values)
    {
        this.values = values;
    }
}
