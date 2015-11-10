/*********************************************************************
 * This code is written by and belongs to DFT Games Ltd.
 * *******************************************************************
 * The license governing this asset id the Unity Asset Store EULA,
 * found here: http://unity3d.com/es/legal/as_terms hereby summarised:
 * *******************************************************************
 * Usage is granted to the licensee in conjunction with the 
 * package that has been licensed and also in other final products 
 * produced by the licensee and aimed to an end user. 
 * It's forbidden to use this code as part of packages or assets 
 * aimed to be used by developers other than the licensee 
 * of the original package.
 * *******************************************************************
 * 
 * Copyright 2010-2015 - DFT Games Ltd. - Version 6.3 (4 Oct. 2015)
 * 
 * *******************************************************************
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DFTGames.Tools
{
    public delegate void AutoReleaseElementDelegate(GameObject element);

    /// <summary>
    /// A simple, dirty and fast typed pooling system.
    /// </summary>
    public class PoolingManager<T> : System.IDisposable where T : class
    {

        /// <summary>
        /// The out of the way position. This is the position used
        /// to set the game object out of the camera. Feel free to change it
        /// if in your project this position is not really out of the way.
        /// </summary>
        private static Vector3 outOfTheWay = new Vector3(900000f, 900000f, 900000f);

        /// <summary>
        /// The available elements list.
        /// </summary>
        private List<GameObject> availableList = new List<GameObject>();

        /// <summary>
        /// The elements in use list.
        /// </summary>
        private List<GameObject> inUseList = new List<GameObject>();

        /// <summary>
        /// The original prefab reference.
        /// </summary>
        private GameObject original = null;

        /// <summary>
        /// Is this a GameObject pool?
        /// </summary>
        private bool isGameObject = false;

        /// <summary>
        /// We keep track of the average active objects over time to clean up memory
        /// </summary>
        private int average_active = 0;
        private int average_available = 0;
        private float optimizationCounter = 0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolingSystem`1"/> class.
        /// </summary>
        /// <param name='prefab'>
        /// Prefab.
        /// </param>
        /// <param name='initialSize'>
        /// Initial size of this pool. Defaults to 3 if omitted.
        /// </param>
        public PoolingManager(GameObject prefab, int initialSize = 3)
        {
            GameObject temp = null;
            original = prefab;

            // Remember if this is a GameObject type or not
            if (typeof(T) == typeof(GameObject))
                isGameObject = true;

            // Populate the initial pool
            for (int i = 0; i < initialSize; i++)
            {
                // Instantiate the object
                temp = GameObject.Instantiate(original, outOfTheWay, Quaternion.identity) as GameObject;
                PooledObject pooled = temp.AddComponent<PooledObject>();
                // Feed the delegate for dismissal
                pooled.myManager = AutoReleaseElement;
                // Set the object inactive
                temp.SetActive(false);
                // Add it to the list of the available elements
                availableList.Add(temp);
            }
        }

        /// <summary>
        /// Call this in your Update() to optimize the pool. 
        /// </summary>
        /// <param name="poolResizeTimeInSeconds">
        /// Time cycle to optimize the pool. Defaults to 10 if omitted.
        /// </param>
        /// <param name="minimumAvailableOverInUseRatio">
        /// Desired ratio of available over active objects. Defaults to 0.5f if omitted.
        /// </param>
        public void Optimize(float poolResizeTimeInSeconds = 10f, float minimumAvailableOverInUseRatio = 0.5f)
        {
            optimizationCounter += UnityEngine.Time.deltaTime;
            if (optimizationCounter >= poolResizeTimeInSeconds)
            {
                optimizationCounter = 0f;
                float ratio = (float)Available / (float)InUse;
                if (ratio < minimumAvailableOverInUseRatio)
                {
                    // Increase the pool if it makes sense to do so
                    int missing = (int)((minimumAvailableOverInUseRatio - ratio) * (float)InUse);
                    for (int i = 0; i < missing; i++)
                    {
                        GameObject temp = GameObject.Instantiate(original, outOfTheWay, Quaternion.identity) as GameObject;
                        PooledObject pooled = temp.AddComponent<PooledObject>();
                        // Feed the delegate for dismissal
                        pooled.myManager = AutoReleaseElement;
                        temp.SetActive(false);
                        availableList.Add(temp);
                    }
                }
                else if (ratio > minimumAvailableOverInUseRatio)
                {
                    while (ratio > minimumAvailableOverInUseRatio)
                    {
                        GameObject.Destroy(availableList[0]);
                        availableList.RemoveAt(0);
                        ratio = (float)Available / (float)InUse;
                    }
                }
            }
        }

        /// <summary>
        /// Cleans up the memory. This method isn't really
        /// something you want to call often, but from time to time
        /// it might help in some specific use case. DO NOT use it
        /// unless profiling your app you can actually see that
        /// there is need for it. This is just a temporary memory
        /// release of the excess allocation as the objects go back and forth 
        /// between the two lists. NEVER call this on every frame!!!
        /// If you really need this you can use it every few seconds.
        /// </summary>
        public void CleanUp()
        {
            inUseList.TrimExcess();
            availableList.TrimExcess();
        }

        /// <summary>
        /// Gets the in use size.
        /// </summary>
        /// <value>
        /// The in use size.
        /// </value>
        public int InUse
        {
            get
            {
                return inUseList.Count;
            }
        }

        /// <summary>
        /// Gets the available size.
        /// </summary>
        /// <value>
        /// The available size.
        /// </value>
        public int Available
        {
            get
            {
                return availableList.Count;
            }
        }

        /// <summary>
        /// This is functional to the PooledObject class but it can
        /// be used if you want a straight way to use just a
        /// GameObject to dismiss the pool element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="SetOutOfTheWay"></param>
        public void AutoReleaseElement(GameObject element)
        {
            if (isGameObject) // Are we dealing with GameObject?
            {
                ReleaseElement(element as T, true);
            }
            else // if not...
            {
                // Get the component to get its GameObject
                ReleaseElement(element.GetComponent(typeof(T)) as T, true);
            }
        }


        /// <summary>
        /// Releases the element. To be called instead of Destroy.
        /// </summary>
        /// <param name='element'>
        /// Element.
        /// </param>
        /// <param name='SetOutOfTheWay'>
        /// Set the object out of the way. True if omitted.
        /// </param>
        public void ReleaseElement(T element, bool SetOutOfTheWay = true)
        {
            GameObject temp;
            if (isGameObject) // Are we dealing with GameObject?
            {
                temp = element as GameObject;
            }
            else // if not...
            {
                // Get the component to get its GameObject
                Component cTemp = element as Component;
                temp = cTemp.gameObject;
            }

            // change the object position id the flag is true
            if (SetOutOfTheWay)
                temp.transform.position = outOfTheWay;
            // Set the object inactive
            temp.SetActive(false);
            inUseList.Remove(temp);
            availableList.Add(temp);
        }

        /// <summary>
        /// Gets the element. To be called instead of Instantiate.
        /// </summary>
        /// <returns>
        /// The element.
        /// </returns>
        public T GetElement()
        {
            GameObject temp = null;
            // Check if the pool contains an usable element
            if (availableList.Count == 0)
            {
                // No free elements, so we create a new one.
                temp = GameObject.Instantiate(original, outOfTheWay, Quaternion.identity) as GameObject;
                PooledObject pooled = temp.AddComponent<PooledObject>();
                // Feed the delegate for dismissal
                pooled.myManager = AutoReleaseElement;
                temp.SetActive(false);
                // add the new object to the in use list
                inUseList.Add(temp);
            }
            else // an element is available
            {
                // fetch the element
                temp = availableList[0];
                // remove it from the active list
                availableList.RemoveAt(0);
                // add the object to the in use list
                inUseList.Add(temp);
            }

            // Activate the object
            temp.SetActive(true);
            // Return the proper object
            if (isGameObject)
                return temp as T;
            else
            {
                return temp.GetComponent(typeof(T)) as T;
            }
        }

        /// <summary>
        /// Call this to make sure to release all the memory before destroying the pool.
        /// </summary>
        public void Dispose()
        {
            original = null;
            for (int i = 0; i < inUseList.Count; i++)
            {
                GameObject.Destroy(inUseList[i]);
            }
            for (int i = 0; i < availableList.Count; i++)
            {
                GameObject.Destroy(availableList[i]);
            }
            inUseList.Clear();
            inUseList = null;
            availableList.Clear();
            availableList = null;
        }
    }
}