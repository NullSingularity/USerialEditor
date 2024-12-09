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
        Dictionary<TreeNode, UProperty> propertyNodes;
        Font nodeBaseFont, nodeModifiedFont, nodeInvalidFont;
        PropertyControl propertyControl;
        
        public ObjectFileControls(UProperty myObject, ClassLibrary classLibrary, PropertyFactory propertyFactory)
        {
            InitializeComponent();
            this.myObject = myObject;
            this.classLibrary = classLibrary;
            this.propertyFactory = propertyFactory;
            propertyNodes = new Dictionary<TreeNode, UProperty>();
            nodeBaseFont = new Font("Arial", 8);
            nodeModifiedFont = new Font(nodeBaseFont, FontStyle.Bold);
            nodeInvalidFont = new Font(nodeBaseFont, FontStyle.Strikeout);
            RefreshTree();
        }

        private void RefreshTree()
        {
            if(myObject == null) 
            {
                return;
            }
            treeView1.Nodes.Clear();
            propertyNodes.Clear();
            TreeNode rootNode = treeView1.Nodes.Add(myObject.GetName());
            BuildTreeWorker(rootNode, myObject);
        }

        private void BuildTreeWorker(TreeNode currentNode, UProperty currentProperty)
        {
            propertyNodes.Add(currentNode, currentProperty);
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
                TreeNode subNode = new TreeNode(newName);//currentNode.Nodes.Add(newName);
                subNode.NodeFont = nodeModifiedFont;
                currentNode.Nodes.Add(subNode);
                index++;
                BuildTreeWorker(subNode, subProperty);
            }
            StylizeNode(currentNode);
        }

        private void StylizeTree()
        {
           /* foreach (TreeNode node in treeView1.Nodes)
            {
                StylizeTreeWorker(node);
            }*/
           foreach(TreeNode node in propertyNodes.Keys)
           {
                StylizeNode(node);
           }
            // treeView1.BeginUpdate();
           // treeView1.Update();
           // treeView1.EndUpdate();
        }

        private void StylizeTreeWorker(TreeNode node)
        {
            foreach (TreeNode childNode in node.Nodes)
            {
                StylizeTreeWorker(childNode);
            }
            StylizeNode(node);
        }

        private void StylizeNode(TreeNode node)
        {
            if (node == null)
            { 
                return; 
            }
            if (!propertyNodes.ContainsKey(node)) 
            {
                return;
            }

            UProperty property = propertyNodes[node];
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

        private void propertyControl_PropertyModified(object sender, EventArgs e)
        {
            if (e is PropertyModifiedEventArgs)
            {
                Console.WriteLine("PROPERTY MODIFIED: " + ((PropertyModifiedEventArgs)e).Property.ToString());
                if(((PropertyModifiedEventArgs)e).Property is ArrayProperty)
                {
                    RefreshTree();
                }
            }
            StylizeTree();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UProperty property = propertyNodes[e.Node];
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
