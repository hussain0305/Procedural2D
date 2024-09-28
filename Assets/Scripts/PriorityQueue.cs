using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority>
{
    private List<KeyValuePair<TElement, TPriority>> elements = new List<KeyValuePair<TElement, TPriority>>();
    private Comparer<TPriority> comparer = Comparer<TPriority>.Default;

    // Add an element to the priority queue with its priority
    public void Enqueue(TElement element, TPriority priority)
    {
        elements.Add(new KeyValuePair<TElement, TPriority>(element, priority));
    }

    // Remove and return the element with the lowest priority
    public TElement Dequeue()
    {
        int bestIndex = 0;

        // Find the element with the lowest priority
        for (int i = 0; i < elements.Count; i++)
        {
            if (comparer.Compare(elements[i].Value, elements[bestIndex].Value) < 0)
            {
                bestIndex = i;
            }
        }

        TElement bestElement = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestElement;
    }

    // Check if the priority queue contains a specific element
    public bool Contains(TElement element)
    {
        foreach (var pair in elements)
        {
            if (EqualityComparer<TElement>.Default.Equals(pair.Key, element))
            {
                return true;
            }
        }
        return false;
    }

    // Return the number of elements in the queue
    public int Count
    {
        get { return elements.Count; }
    }
}