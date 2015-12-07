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

namespace DFTGames.Tools
{
    /// <summary>
    /// This is the class added automatically to the object by its manager 
    /// to let the object be aware of its "master", so that it can be easily
    /// dismissed without retrieving it.
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        /// <summary>
        /// Declaring the delegate to be called to dismiss this object
        /// </summary>
        public AutoReleaseElementDelegate myManager;

        /// <summary>
        /// Send a message or call this method to dismissi the object
        /// </summary>
        public void Dismiss()
        {
            myManager(gameObject);
        }
    }
}