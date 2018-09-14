// Copyright 2018 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ArcGISRuntime.Samples.Managers;
using Esri.ArcGISRuntime.Ogc;
using Esri.ArcGISRuntime.Portal;

namespace ArcGISRuntime.WPF.Samples.DisplayKml
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Display KML",
        "Layers",
        "Display a KML file from URL, a local file, or a portal item.",
        "")]
    [ArcGISRuntime.Samples.Shared.Attributes.OfflineData("324e4742820e46cfbe5029ff2c32cb1f")]
    public partial class DisplayKml
    {
        // Dictionary associates labels (for use in UI) with layers.
        private readonly Dictionary<string, KmlLayer> _dataSources = new Dictionary<string, KmlLayer>();

        public DisplayKml()
        {
            InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            // Set up the basemap.
            MySceneView.Scene = new Scene(Basemap.CreateImageryWithLabels());

            // Configure the layers - from URL.
            KmlLayer urlLayer = new KmlLayer(new Uri("https://www.arcgis.com/sharing/rest/content/items/324e4742820e46cfbe5029ff2c32cb1f/data"));

            // Configure the layers - from file.
            string filePath = DataManager.GetDataFolder("324e4742820e46cfbe5029ff2c32cb1f", "US_State_Capitals.kml");
            KmlLayer fileLayer = new KmlLayer(new Uri(filePath));

            // Configure the layers - from portal.
            ArcGISPortal portal = await ArcGISPortal.CreateAsync();
            PortalItem item = await PortalItem.CreateAsync(portal, "9fe0b1bfdcd64c83bd77ea0452c76253");
            KmlLayer portalItemLayer = new KmlLayer(item);

            // Add the layers to the collection.
            _dataSources["URL"] = urlLayer;
            _dataSources["Local file"] = fileLayer;
            _dataSources["Portal item"] = portalItemLayer;

            // Update the UI.
            LayerPicker.IsEnabled = true;
            LayerPicker.ItemsSource = _dataSources.Keys;
            LayerPicker.SelectionChanged += LayerPicker_SelectionChanged;
            LayerPicker.SelectedIndex = 0;
        }

        private async void LayerPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear existing layers.
            MySceneView.Scene.OperationalLayers.Clear();

            // Get the name of the selected layer.
            string name = e.AddedItems[0].ToString();

            // Verify that the selected layer is in the dictionary.
            if (_dataSources.ContainsKey(name))
            {
                // Retrieve the layer from the dictionary.
                KmlLayer layer = _dataSources[name];

                // Add the selected layer to the map.
                MySceneView.Scene.OperationalLayers.Add(layer);

                // Zoom to the extent of the layer.
                await layer.LoadAsync();
                await MySceneView.SetViewpointAsync(new Viewpoint(layer.FullExtent));
            }
        }
    }
}