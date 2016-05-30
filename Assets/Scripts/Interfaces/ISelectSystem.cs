using System.Collections.Generic;

public interface ISelectSystem<T>
{
    void Select(T item);
    void Deselect(T item);
    List<T> ListSelected();
    bool IsSelected(T item);
    void Clear();
}