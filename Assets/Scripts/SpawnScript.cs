using UnityEngine;
using System.Collections.Generic;

public class SpawnScript : MonoBehaviour {

    public Transform prefab;
    public int numberOfObjects;

    public float recycleOffset;     //how far behind the player should stuff be recycled
    public Vector2 startPosition;
    public Vector2 minSize, maxSize, minGap, maxGap;
    public float minY, maxY;

    private Vector2 nextPosition;
    private Queue<Transform> objectQueue;

    void Start() {

        //A queue is just a collection of objects (like an array or List). It just stores a bunch of objects. 
        //The difference is that you can only enqueue (put something in the queue) at the end and dequeue (get something out) at the beginning of the queue. 
        //It's a FIFO(FirstIn-FirstOut).
        objectQueue = new Queue<Transform>(numberOfObjects);

        // add the number of objects defined by Designer into objectQueue
        for (int i = 0; i < numberOfObjects; i++) {
            objectQueue.Enqueue((Transform)Instantiate(prefab));
        }

        // public Vector 3's
        nextPosition = startPosition;

        for (int i = 0; i < numberOfObjects; i++) {
            Recycle();
        }
    }

    void Update() {
    
        /* if (objectQueue.Peek().localPosition.x + recycleOffset < Player.distanceTraveled) {
             Recycle();
         }*/
    }

    private void Recycle() {

        //Create a Vector 3 that cordinates within a public im an max size
        Vector2 scale = new Vector2((Random.Range(minSize.x, maxSize.x)), (Random.Range(minSize.y, maxSize.y)));

        //Store a vector 3 from the start of the game
        Vector2 position = nextPosition;

        // bumps the positions up to account for the objects thickness.
        position.x += scale.x * 0.5f;
        position.y += scale.y * 0.5f;


        //take the first thing that comes out of objectQueue and create it
        Transform spawnObject = objectQueue.Dequeue();
        //assign it's scale
        spawnObject.localScale = scale;
        //asign where it will spawn
        spawnObject.localPosition = position;
        //add it to the back of the queue
        objectQueue.Enqueue(spawnObject);

        //generate the next position
        nextPosition += new Vector2((Random.Range(minGap.x, maxGap.x) + scale.x), (Random.Range(minGap.y, maxGap.y)));

        //if Y postion is too small, add the smallest ammount you can
        if (nextPosition.y < minY) {
            nextPosition.y = minY + maxGap.y;
        } // if its to big, subtract the biggset amount you can
        else if (nextPosition.y > maxY) {
            nextPosition.y = maxY - maxGap.y;
        }
    }
}