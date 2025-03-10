﻿//-----------------------------------------------------------------
//  LinkedSpriteManager v0.64 (21-10-2012)
//  Copyright 2012 Brady Wright and Above and Beyond Software
//  All rights reserved
//-----------------------------------------------------------------
// A class to allow the drawing of multiple "quads" as part of a
// single aggregated mesh so as to achieve multiple, independently
// moving objects using a single draw call.
//-----------------------------------------------------------------


using UnityEngine;

// A variation on the SpriteManager that automatically links all
// translations and rotations of the client GameObjects to the
// associated sprite - meaning the client need not worry about
// micromanaging all transformations:
public class LinkedSpriteManager : SpriteManager
{
    private Transform t;
    private Vector3 pos;
    private SMSprite s;


    // Use this for initialization
    private void Start()
    {

    }

    // Transforms all sprites by their associated GameObject's
    // transforms:
    private void TransformSprites()
    {
        for (int i = 0; i < activeBlocks.Count; ++i)
        {
            ((SMSprite)activeBlocks[i]).Transform();
        }

        // Handle any billboarded sprites:
        if (activeBillboards.Count > 0)
        {
            t = Camera.main.transform;

            for (int i = 0; i < activeBillboards.Count; ++i)
            {
                s = (SMSprite)activeBillboards[i];
                pos = s.clientTransform.position;

                vertices[s.mv1] = pos + t.TransformDirection(s.v1);
                vertices[s.mv2] = pos + t.TransformDirection(s.v2);
                vertices[s.mv3] = pos + t.TransformDirection(s.v3);
                vertices[s.mv4] = pos + t.TransformDirection(s.v4);
            }
        }
    }

    // LateUpdate is called once per frame
    private new void LateUpdate()
    {
        // Transform all sprites according to their
        // client GameObject's transforms:
        TransformSprites();

        // Copy over the changes:
        mesh.vertices = vertices;

        if (vertCountChanged)
        {
            mesh.uv = UVs;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.triangles = triIndices;

            vertCountChanged = false;
            uvsChanged = false;
            colorsChanged = false;
        }
        else
        {
            if (uvsChanged)
            {
                mesh.uv = UVs;
                uvsChanged = false;
            }

            if (colorsChanged)
            {
                colorsChanged = false;

                mesh.colors = colors;
            }

            // Explicitly recalculate bounds since
            // we didn't assign new triangles (which
            // implicitly recalculates bounds)
            if (updateBounds || autoUpdateBounds)
            {
                mesh.RecalculateBounds();
                updateBounds = false;
            }
        }
    }
}