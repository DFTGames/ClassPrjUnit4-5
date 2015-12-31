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
 * Copyright 2010-2015 - DFT Games Ltd. - Version 3.0 (30 Oct. 2015)
 *
 * *******************************************************************
 */

using System.Collections.Generic;
using UnityEngine;

namespace DFTGames.Tools
{
    public class ShaderData
    {
        public Renderer renderer { get; set; }
        public Shader[] shader { get; set; }
        public Color[] color { get; set; }
    }

    [RequireComponent(typeof(Camera))]
    public abstract class FadeObstructorsBaseClass : MonoBehaviour
    {
        public bool ignoreTriggers = true;
        public Color fadingColorToUse = new Color(1f, 1f, 1f, 0.3f);
        public LayerMask layersToFade = (LayerMask)(-1);
        public Transform playerTransform;
        public float offset = -0.5f;
        public string playerTag = "Player";


        protected Transform myTransform;
        protected Dictionary<int, ShaderData> modifiedShaders = new Dictionary<int, ShaderData>();
        protected Shader transparentShader;

        // Use this for initialization
        public virtual void Start()
        {
  
            myTransform = transform;
            transparentShader = Shader.Find("Transparent/Diffuse");
            // Find the player if the target has not been assigned
            if (playerTransform == null)
                playerTransform = GameObject.FindGameObjectWithTag(playerTag).GetComponent<Transform>();
            if (playerTransform == null)
            {
                Debug.LogError("Player's transform not set and can't find any object in the scene with tag " + playerTag);
                return;
            }
        }
    }
}