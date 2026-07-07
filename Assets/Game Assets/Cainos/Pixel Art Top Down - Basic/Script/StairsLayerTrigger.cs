using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //when object enter or exit the trigger, put it to the assigned layer and sorting layers base on the direction
    //used in the stairs objects for player to travel between layers

    public class StairsLayerTrigger : MonoBehaviour
    {
        public Direction direction;                                 //direction of the stairs
        [Space]
        public string layerUpper;
        public string sortingLayerUpper;
        [Space]
        public string layerLower;
        public string sortingLayerLower;

        private static float lastLayerChangeTime;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (Time.time - lastLayerChangeTime < 0.3f) return;

            GameObject target = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;

            if (direction == Direction.South && other.bounds.center.y < transform.position.y)
            {
                SetLayerAndSortingLayer(target, layerUpper, sortingLayerUpper);
                lastLayerChangeTime = Time.time;
            }
            else
            if (direction == Direction.West && other.bounds.center.x < transform.position.x)
            {
                SetLayerAndSortingLayer(target, layerUpper, sortingLayerUpper);
                lastLayerChangeTime = Time.time;
            }
            else
            if (direction == Direction.East && other.bounds.center.x > transform.position.x)
            {
                SetLayerAndSortingLayer(target, layerUpper, sortingLayerUpper);
                lastLayerChangeTime = Time.time;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (Time.time - lastLayerChangeTime < 0.3f) return;

            GameObject target = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;

            if (direction == Direction.South && other.bounds.center.y < transform.position.y)
            {
                SetLayerAndSortingLayer(target, layerLower, sortingLayerLower);
                lastLayerChangeTime = Time.time;
            }
            else
            if (direction == Direction.West && other.bounds.center.x < transform.position.x)
            {
                SetLayerAndSortingLayer(target, layerLower, sortingLayerLower);
                lastLayerChangeTime = Time.time;
            }
            else
            if (direction == Direction.East && other.bounds.center.x > transform.position.x)
            {
                SetLayerAndSortingLayer(target, layerLower, sortingLayerLower);
                lastLayerChangeTime = Time.time;
            }
        }

        private void SetLayerAndSortingLayer( GameObject target, string layer, string sortingLayer )
        {
            target.layer = LayerMask.NameToLayer(layer);

            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = sortingLayer;
            }

            SpriteRenderer[] srs = target.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer childSr in srs)
            {
                childSr.sortingLayerName = sortingLayer;
            }
        }

        public enum Direction
        {
            North,
            South,
            West,
            East
        }    
    }
}
