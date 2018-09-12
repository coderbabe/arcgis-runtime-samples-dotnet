// Copyright 2018 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Ogc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ArcGISRuntime.Samples.Managers;

namespace ArcGISRuntime.WPF.Samples.ListKmlContents
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "List KML contents",
        "Layers",
        "List the contents of a KML file.",
        "")]
    [ArcGISRuntime.Samples.Shared.Attributes.OfflineData("da301cb122874d5497f8a8f6c81eb36e")]
    public partial class ListKmlContents
    {
        // Hold a list of LayerDisplayVM; this is the ViewModel.
        private ObservableCollection<LayerDisplayVM> _viewModelList = new ObservableCollection<LayerDisplayVM>();

        public ListKmlContents()
        {
            InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            // Add a basemap.
            MySceneView.Scene = new Scene(Basemap.CreateImageryWithLabels());

            // Get the URL to the data
            Uri kmlUrl = new Uri(DataManager.GetDataFolder("da301cb122874d5497f8a8f6c81eb36e", "esri_test_data.kmz"));

            // Create the KML dataset and layer.
            KmlDataset dataset = new KmlDataset(kmlUrl);
            KmlLayer layer = new KmlLayer(dataset);

            // Add the layer to the map.
            MySceneView.Scene.OperationalLayers.Add(layer);

            await dataset.LoadAsync();
            
            // Build the ViewModel from the expanded list of layer infos
            foreach (KmlNode node in dataset.RootNodes)
            {
                // LayerDisplayVM is a custom type made for this sample to serve as the ViewModel; it is not a part of ArcGIS Runtime.
                LayerDisplayVM nodeVm = new LayerDisplayVM(node, null);
                _viewModelList.Add(nodeVm);
                LayerDisplayVM.BuildLayerInfoList(nodeVm, _viewModelList);
            }

            // Update the list of layers, using the root node from the list.
            LayerTreeView.ItemsSource = _viewModelList;
        }

        private void LayerTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Get the KML node.
            LayerDisplayVM selectedItem = (LayerDisplayVM)e.NewValue;

            // Get the extent of the node.
            Envelope geometry = selectedItem.Node.Extent;

            // If the extent is valid, zoom to it.
            if (geometry != null && !geometry.IsEmpty)
            {
                MySceneView.SetViewpointAsync(new Viewpoint(geometry));
            }
        }
    }

    public class LayerDisplayVM
    {
        public KmlNode Node { get; set; }

        public List<LayerDisplayVM> Children { get; set; }

        private LayerDisplayVM Parent { get; set; }

        public LayerDisplayVM(KmlNode info, LayerDisplayVM parent)
        {
            Node = info;
            Parent = parent;
        }

        // Override ToString to enhance display formatting.
        public override string ToString()
        {
            return Node.Name + " - " + Node.GetType().Name;
        }

        public static void BuildLayerInfoList(LayerDisplayVM root, IList<LayerDisplayVM> result)
        {
            // Add the root node to the result list.
            //result.Add(root);

            // Make the node visible.
            root.Node.IsVisible = true;

            // Initialize the child collection for the root.
            root.Children = new List<LayerDisplayVM>();

            // Recursively add children. KmlContainers and KmlNetworkLinks can both have children.
            var containerNode = root.Node as KmlContainer;
            var networkLinkNode = root.Node as KmlNetworkLink;

            List<KmlNode> children = new List<KmlNode>();
            if (containerNode != null)
            {
                children.AddRange(containerNode.ChildNodes);
            }

            if (networkLinkNode != null)
            {
                children.AddRange(networkLinkNode.ChildNodes);
            }

            foreach (KmlNode node in children)
            {
                // Create the view model for the sublayer.
                LayerDisplayVM layerVM = new LayerDisplayVM(node, root);

                // Add the sublayer to the root's sublayer collection.
                root.Children.Add(layerVM);

                // Recursively add children.
                BuildLayerInfoList(layerVM, result);
            }
        }
    }
}