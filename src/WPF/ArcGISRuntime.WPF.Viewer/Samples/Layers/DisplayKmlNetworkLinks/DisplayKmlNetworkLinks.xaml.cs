// Copyright 2018 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using ArcGISRuntime.Samples.Managers;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Ogc;
using System;
using Esri.ArcGISRuntime.Geometry;

namespace ArcGISRuntime.WPF.Samples.DisplayKmlNetworkLinks
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Display KML network links",
        "Layers",
        "Display a KML file that loads content from a network resource.",
        "")]
    [ArcGISRuntime.Samples.Shared.Attributes.OfflineData("600748d4464442288f6db8a4ba27dc95")]
    public partial class DisplayKmlNetworkLinks
    {
        public DisplayKmlNetworkLinks()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            // Set up the basemap.
            MySceneView.Scene = new Scene(Basemap.CreateImageryWithLabels());

            // Get the path to the downloaded KML file.
            string filePath = DataManager.GetDataFolder("600748d4464442288f6db8a4ba27dc95", "Radar.kmz");

            // Create the dataset.
            KmlDataset dataset = new KmlDataset(new Uri(filePath));

            // Create the layer from the dataset.
            KmlLayer fileLayer = new KmlLayer(dataset);

            // Add the layer to the map.
            MySceneView.Scene.OperationalLayers.Add(fileLayer);

            // Zoom in to center the map on Germany.
            MySceneView.SetViewpointAsync(new Viewpoint(new MapPoint(8.150526, 50.472421, SpatialReferences.Wgs84), 2000000000));
        }
    }
}