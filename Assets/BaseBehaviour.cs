using UnityEngine;
using Chronos;

    // A base class that provides a shortcut 
    // for accessing the timeline component.
    [RequireComponent(typeof(Timeline))]
    public abstract class BaseBehaviour : MonoBehaviour
    {
        public Timeline time
        {
            get { return GetComponent<Timeline>(); }
        }
    }

