using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CarcassonneCraft
{
    public class Follow : MonoBehaviour
    {
        Transform parent;
        float offset = 1.1f;

        public void Init(Transform parent, string name)
        {
            this.parent = parent;

            GetComponent<Text>().text = name;

            SetPosition();
        }
        // Update is called once per frame
        void Update()
        {
            SetPosition();
        }

        void SetPosition()
        {
            Vector3 worldPos = parent.position;
            worldPos.y += offset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            transform.position = screenPos;
        }
    }
}
