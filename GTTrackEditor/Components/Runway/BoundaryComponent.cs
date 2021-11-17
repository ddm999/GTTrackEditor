﻿using GTTrackEditor.Readers;
using GTTrackEditor.Readers.Entities;

using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.SharpDX.Core;

using SharpDX;

using System;
using System.Collections.Generic;
using System.Windows.Media;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Matrix3D = System.Windows.Media.Media3D.Matrix3D;
using Quaternion = System.Windows.Media.Media3D.Quaternion;
using MatrixTransform3D = System.Windows.Media.Media3D.MatrixTransform3D;

using GTTrackEditor.ModelEntities;
using GTTrackEditor.Utils;

using PDTools.Files;
using PDTools.Files.Courses.Runway;

namespace GTTrackEditor.Components.Runway;

public class BoundaryComponent : TrackComponentBase
{
    public MeshGeometry3D BoundaryModel { get; set; } = new();
    public DiffuseMaterial BoundaryMaterial { get; set; } = new();

    public RunwayFile RunwayData { get; set; }

    public BoundaryComponent()
    {
        Name = "Boundary";
        BoundaryMaterial.DiffuseColor = new(1.0f, 1.0f, 0.0f, 1.0f);
    }

    public void Init(RunwayFile runwayData)
    {
        RunwayData = runwayData;
    }

    public override void RenderComponent()
    {
        int i = 0;
        List<List<Vector3>> boundaries = new();
        List<Vector3> boundary = new();
        while (i < RunwayData.BoundaryVerts.Count)
        {
            RunwayBoundaryVert vert = RunwayData.BoundaryVerts[i];
            boundary.Add(vert.Position.ToSharpDXVector());

            if (vert.counter < 0) // boundary end
            {
                boundaries.Add(boundary);
                boundary = new List<Vector3>();
            }
            i++;
        }

        MeshBuilder meshBuilder = new(false, false);
        for (int n = 0; n < boundaries.Count; n++)
        {
            List<Vector3> vec3s = boundaries[n];
            meshBuilder.AddTube(vec3s, 1f, 18, true);
        }

        MeshGeometry3D m = meshBuilder.ToMesh();
        m.AssignTo(BoundaryModel);
    }

    public override void Hide()
    {
        if (!IsVisible)
            return;

        BoundaryModel.ClearAllGeometryData();
        BoundaryModel.UpdateVertices();
        TreeViewItemColor = Brushes.Gray;
        IsVisible = false;
    }

    public override void Show()
    {
        if (IsVisible)
            return;

        RenderComponent();
        TreeViewItemColor = Brushes.White;
        IsVisible = true;
    }
}

