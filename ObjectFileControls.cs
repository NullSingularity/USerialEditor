using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USerialEditor
{
    public partial class ObjectFileControls : UserControl
    {
        ClassLibrary classLibrary;
        PropertyFactory propertyFactory;
        UProperty myObject;
        Dictionary<TreeNode, UProperty> propertiesByNode;
        Dictionary<UProperty, TreeNode> nodesByProperty;
        Font nodeBaseFont, nodeModifiedFont, nodeInvalidFont;
        PropertyControl propertyControl;
        
        public ObjectFileControls(UProperty myObject, ClassLibrary classLibrary, PropertyFactory propertyFactory)
        {
            InitializeComponent();
            this.myObject = myObject;
            this.classLibrary = classLibrary;
            this.propertyFactory = propertyFactory;
            propertiesByNode = new Dictionary<TreeNode, UProperty>();
            nodesByProperty = new Dictionary<UProperty, TreeNode>();
            nodeBaseFont = new Font("Arial", 8);
            nodeModifiedFont = new Font(nodeBaseFont, FontStyle.Bold);
            nodeInvalidFont = new Font(nodeBaseFont, FontStyle.Strikeout);
            InitializeTree();
        }

        private void InitializeTree()
        {
            if(myObject == null) 
            {
                return;
            }
            propertyTreeView.Nodes.Clear();
            propertiesByNode.Clear();
            nodesByProperty.Clear();
            TreeNode rootNode = propertyTreeView.Nodes.Add(myObject.GetName());
            BuildTreeWorker(rootNode, myObject);
        }

        private void BuildTreeWorker(TreeNode currentNode, UProperty currentProperty)
        {
            if(!propertiesByNode.ContainsKey(currentNode))
            {
                propertiesByNode.Add(currentNode, currentProperty);
            }
            if(!nodesByProperty.ContainsKey(currentProperty))
            {
                nodesByProperty.Add(currentProperty, currentNode);
            }
            int index = 0;
            foreach (UProperty subProperty in currentProperty.SubProperties)
            {
                string newName = subProperty.GetName();
                if (currentProperty is StaticArrayProperty)
                {
                    newName += "[" + subProperty.GetStaticArrayIndex() + "]";
                }
                else if (currentProperty is ArrayProperty)
                {
                    newName += "[" + index + "]";
                }
                TreeNode subNode = new TreeNode(newName);
                subNode.NodeFont = nodeModifiedFont;
                currentNode.Nodes.Add(subNode);
                index++;
                BuildTreeWorker(subNode, subProperty);
            }
            StylizeNode(currentNode);
        }

        private void StylizeTree()
        {
           foreach(TreeNode node in propertiesByNode.Keys)
           {
                StylizeNode(node);
           }
        }

        private void StylizeTreeWorker(TreeNode node)
        {
            foreach (TreeNode subNode in node.Nodes)
            {
                StylizeTreeWorker(subNode);
            }
            StylizeNode(node);
        }

        private void StylizeNode(TreeNode node)
        {
            if (node == null)
            { 
                return; 
            }
            if (!propertiesByNode.ContainsKey(node)) 
            {
                return;
            }

            UProperty property = propertiesByNode[node];
            if (property is UndefinedProperty)
            {
                node.ForeColor = Color.Red;
                node.NodeFont = nodeInvalidFont;
            }
            else if (property is PropertyGroup)
            {
                node.NodeFont = nodeBaseFont;
                node.ForeColor = Color.Black;
            }
            else if (!property.IsModified())
            {
                node.NodeFont = nodeBaseFont;
                node.ForeColor = Color.Gray;
            }
            else
            {
                node.NodeFont = nodeModifiedFont;
                node.ForeColor = Color.Black;
            }
            //node.Text = node.Text;

        }

        private void RemoveNode(TreeNode node)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                TreeNode subNode = node.Nodes[i];
                if(subNode != null)
                {
                    RemoveNode(subNode);
                    i--;
                }
            }
            node.Remove();
            RemoveNodeMapping(node);
        }

        private void RemoveNodeMapping(TreeNode node)
        {
            if(propertiesByNode.ContainsKey(node))
            {
                UProperty property = propertiesByNode[node];
                propertiesByNode.Remove(node);
                if(nodesByProperty.ContainsKey(property))
                {
                    nodesByProperty.Remove(property);
                }
            }

        }

        private void UpdateNode(UProperty property)
        {
            TreeNode baseNode = null;
            nodesByProperty.TryGetValue(property, out baseNode);
            if(baseNode == null)
            {
                return;
            }
            UProperty selectedProperty = null;
            propertiesByNode.TryGetValue(propertyTreeView.SelectedNode, out selectedProperty);

            propertyTreeView.BeginUpdate();
            for (int i = 0; i < baseNode.Nodes.Count; i++)
            {
                TreeNode subNode = baseNode.Nodes[i];
                if(subNode != null)
                {
                    RemoveNode(subNode);
                    i--;
                }
            }

            BuildTreeWorker(baseNode, property);
            if (selectedProperty != null)
            {
                TreeNode selectedNode = null;
                nodesByProperty.TryGetValue(selectedProperty, out selectedNode);
                if (selectedNode != null)
                {
                    propertyTreeView.SelectedNode = selectedNode;
                }
            }
            propertyTreeView.EndUpdate();
        }

        private void propertyControl_PropertyModified(object sender, EventArgs e)
        {
            if (e is PropertyModifiedEventArgs)
            {
                Console.WriteLine("PROPERTY MODIFIED: " + ((PropertyModifiedEventArgs)e).Property.ToString());
                if(((PropertyModifiedEventArgs)e).Property is ArrayProperty)
                {
                    UpdateNode(((PropertyModifiedEventArgs)e).Property);
                }
            }
            StylizeTree();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UProperty property = propertiesByNode[e.Node];
            if(property == null)
            {
                Console.WriteLine("Null selection");
            }
            else
            {
                Console.WriteLine("Selected property " + property.GetName() + "/" + property.GetPropertyType());
                UpdateControls(property);
            }
            
        }

        private void UpdateControls(UProperty property)
        {
            if (property == null)
            {
                return;
            }
            labelPropertyName.Text = "Name: " + property.GetName();
            labelPropertyType.Text = "Type: " + property.GetHumanReadableType();
            if(property.GetSubtype() == "")
            {
                labelPropertySubtype.Text = "Subtype: None";
            }
            else
            {
                labelPropertySubtype.Text = "Subtype: " + property.GetSubtype();
            }
            labelPropertyName.Show();
            labelPropertyType.Show();
            labelPropertySubtype.Show();


            if(propertyControl != null)
            {
                panelControl.Controls.Remove(propertyControl);
                propertyControl.Dispose();
            }

            if(property is BoolProperty)
            {
                propertyControl = new PropertyControlBoolean((BoolProperty)property);
            }
            else if(property is IntProperty)
            {
                propertyControl = new PropertyControlInt((IntProperty)property);
            }
            else if(property is FloatProperty) 
            {
                propertyControl = new PropertyControlFloat((FloatProperty)property);
            }
            else if(property is StringProperty)
            {
                propertyControl = new PropertyControlString((StringProperty)property);
            }
            else if(property is ByteProperty)
            {
                if(property.GetSubtype() == "" || property.GetSubtype() == null)
                {
                    propertyControl = new PropertyControlByte((ByteProperty)property);
                }
                else
                {
                    propertyControl = new PropertyControlEnum((ByteProperty)property);
                }
            }
            else if(property is ArrayProperty)
            {
                propertyControl = new PropertyControlArray((ArrayProperty)property, propertyFactory);
            }
            else if(property is StructProperty)
            {
                propertyControl = new PropertyControlStruct((StructProperty)property);
            }

            if(propertyControl != null)
            {
                propertyControl.Dock = DockStyle.Fill;
                panelControl.Controls.Add(propertyControl);
                propertyControl.PropertyModified += new EventHandler(this.propertyControl_PropertyModified);
            }
        }
    }
}
