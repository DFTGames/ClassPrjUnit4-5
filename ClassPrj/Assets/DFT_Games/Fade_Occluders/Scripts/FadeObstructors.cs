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
    [RequireComponent(typeof(Camera))]
    public class FadeObstructors : FadeObstructorsBaseClass
    {
        // This function is called every fixed framerate frame, if the MonoBehaviour is enabled
        private void FixedUpdate()
        {
            if (playerTransform == null) // Do nothing if we have no target
                return;
            // Let's retrieve all the objects in the way of the camera
#if UNITY_5
            RaycastHit[] hit = Physics.RaycastAll(myTransform.position, myTransform.forward, (playerTransform.position - myTransform.position).magnitude + offset, layersToFade, ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
#else
            RaycastHit[] hit = Physics.RaycastAll(myTransform.position, myTransform.forward, (playerTransform.position - myTransform.position).magnitude + offset, layersToFade);
#endif
            List<int> renderersIdsHitInThisFrame = new List<int>();
            if (hit != null)
            {
                // Parse all objects we hit
                for (int i = 0; i < hit.Length; i++)
                {
#if !UNITY_5
                    if (hit[i].collider.isTrigger && ignoreTriggers)
                        continue;
#endif
                    // Ignore the player :)
                    if (!hit[i].collider.CompareTag(playerTag))
                    {
                        // Retrieve all the renderers
                        Renderer[] rendererWeHit = hit[i].collider.gameObject.GetComponentsInChildren<Renderer>();
                        // Loop through the renderers
                        for (int idx = 0; idx < rendererWeHit.Length; idx++)
                        {
                            if (rendererWeHit[idx] != null) // just to be on the safe side :)
                            {
                                // Store the render's unique Id among those hit during the current frame
                                renderersIdsHitInThisFrame.Add(rendererWeHit[idx].GetInstanceID());
                                // If we changed this alreadu we skip it, otherwise we proceed with
                                // the change
                                if (!modifiedShaders.ContainsKey(rendererWeHit[idx].GetInstanceID()))
                                {
                                    ShaderData shaderData = new ShaderData();
                                    shaderData.renderer = rendererWeHit[idx];
                                    shaderData.shader = new Shader[rendererWeHit[idx].materials.Length];
                                    shaderData.color = new Color[rendererWeHit[idx].materials.Length];
                                    for (int j = 0; j < rendererWeHit[idx].materials.Length; j++)
                                    {
                                        shaderData.shader[j] = rendererWeHit[idx].materials[j].shader;
                                        if (rendererWeHit[idx].materials[j].HasProperty("_Color"))
                                            shaderData.color[j] = rendererWeHit[idx].materials[j].color;
                                        rendererWeHit[idx].materials[j].shader = transparentShader;
                                        rendererWeHit[idx].materials[j].color = fadingColorToUse;
                                    }
                                    // Add the shader to the list of those that have been changed
                                    modifiedShaders.Add(rendererWeHit[idx].GetInstanceID(), shaderData);
                                }
                            }
                        }
                    }
                }
            }
            // Now let's restore those shaders that we changed but now they are no longer in the way
            List<int> renderersToRestore = new List<int>();
            foreach (KeyValuePair<int, ShaderData> elemento in modifiedShaders)
            {
                if (!renderersIdsHitInThisFrame.Contains(elemento.Key))
                    renderersToRestore.Add(elemento.Key);
            }
            for (int i = 0; i < renderersToRestore.Count; i++)
            {
                ShaderData sd = modifiedShaders[renderersToRestore[i]];
                modifiedShaders.Remove(renderersToRestore[i]);
                for (int j = 0; j < sd.renderer.materials.Length; j++)
                {
                    sd.renderer.materials[j].shader = sd.shader[j];
                    if (sd.renderer.materials[j].HasProperty("_Color"))
                        sd.renderer.materials[j].color = sd.color[j];
                }
            }
        }
    }
}