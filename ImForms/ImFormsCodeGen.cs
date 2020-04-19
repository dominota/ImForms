﻿
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Interop = System.Runtime.InteropServices;
using CmplTime = System.Runtime.CompilerServices;
using WForms = System.Windows.Forms;
using WFControlList = System.Windows.Forms.Control.ControlCollection;
using System.Diagnostics;
namespace ImForms
{
  
    public partial class ImFormsMgr
    {
	    public void Space([CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            Label("",callerfilepath, callerlinenumber, callermembername);
        }
        
        public void Label(string text, [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, id1 => new WForms.Label { Name = id1?.ToString(),Tag = id, AutoSize = true });
            ctrl.WfControl.Text = text;
        }

        
        public bool Button(string text, [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id,x => InitControlForClicking(new WForms.Button(),x));
            ctrl.WfControl.Text = text;
            return InteractedElementId == ctrl.ID;
        }

        
        public bool LinkLabel(string text,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.LinkLabel(), x));
            ctrl.WfControl.Text = text;
            return InteractedElementId == ctrl.ID;
        }


        
        public bool Checkbox(string text, ref bool checkBoxChecked,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.CheckBox(), x));
            var checkBox = ctrl.WfControl as WForms.CheckBox;
            checkBox.Text = text;
            checkBox.AutoCheck = false;
            var wasInteracted = InteractedElementId == ctrl.ID;

            if (wasInteracted) { checkBox.Checked = ! checkBox.Checked; checkBoxChecked = checkBox.Checked;  }
            else { checkBox.Checked = checkBoxChecked; }

            return wasInteracted;
        }


        
        public bool RadioButton(string text, ref int value, int checkAgainst,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.RadioButton(), x));
            var radioButton = ctrl.WfControl as WForms.RadioButton;
            radioButton.Text = text;
            radioButton.AutoCheck = false;
            var wasInteracted = InteractedElementId == ctrl.ID ;

            if (wasInteracted) { value = checkAgainst; }
            else { radioButton.Checked = (value == checkAgainst); }

            return wasInteracted ;
        }


        
        public bool SliderInt(string text,ref int value ,int minval = 0, int maxval = 1, [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.TrackBar(), x));
            var trackbar = ctrl.WfControl as WForms.TrackBar;
            trackbar.Text = text;
            trackbar.Minimum = minval;
            trackbar.Maximum = maxval;
            trackbar.Orientation = WForms.Orientation.Horizontal;
            if(FirstPass) trackbar.ValueChanged += LetImGuiHandleIt;
            var wasInteracted = InteractedElementId == ctrl.ID;
            if (wasInteracted) { value = trackbar.Value; }
            else { trackbar.Value = value; }
            return wasInteracted;
        }


        
        public bool SliderFloat(string text, ref float value, float minval = 0.0f, float maxval = 1.0f,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id , x => InitControlForClicking(new WForms.TrackBar(), x));
            var trackbar = ctrl.WfControl as WForms.TrackBar;
            trackbar.Text = text;
            var unitscale = (maxval - minval)*100; 
            trackbar.Minimum = (int)(minval*unitscale);
            trackbar.Maximum = (int)(maxval*unitscale);
            trackbar.Orientation = WForms.Orientation.Horizontal;
            if (FirstPass) trackbar.ValueChanged += LetImGuiHandleIt;
            var wasInteracted = InteractedElementId == ctrl.ID;
            if (wasInteracted) { value = trackbar.Value/100.0f; }
            else { trackbar.Value = (int)(value*unitscale); }
            return wasInteracted;
        }


        
        public bool ProgressInt(string text, ref int value, int minval = 0, int maxval = 1,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id , x => InitControlForClicking(new WForms.ProgressBar(), x));
            var trackbar = ctrl.WfControl as WForms.ProgressBar;
            trackbar.Text = text;
            trackbar.Minimum = minval;
            trackbar.Maximum = maxval;
            trackbar.Value = value;
            var wasInteracted = InteractedElementId == ctrl.ID || PrevInteractedElementId == ctrl.ID;
            return wasInteracted;
        }


        
        public bool ProgressFloat(string text, ref float value, float minval = 0.0f, float maxval = 1.0f,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.ProgressBar(), x));
            var trackbar = ctrl.WfControl as WForms.ProgressBar;
            trackbar.Text = text;
            var unitscale = (maxval - minval) * 100;
            trackbar.Minimum = (int)(minval * unitscale);
            trackbar.Maximum = (int)(maxval * unitscale);
            trackbar.Value = (int)(value * unitscale);
            var wasInteracted = InteractedElementId == ctrl.ID || PrevInteractedElementId == ctrl.ID;
            return wasInteracted;
        }


        
        public bool InputText(string text,ref string output,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.TextBox(), x));
            var textbox = ctrl.WfControl as WForms.TextBox;
            textbox.Text = text;
            textbox.Multiline = false;
            output = textbox.Text;
            var wasInteracted = InteractedElementId == ctrl.ID;
            return wasInteracted;
        }


        
        public bool InputMultilineText(string text, ref string output,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.TextBox(), x));
            var multilinetextbox = ctrl.WfControl as WForms.TextBox;
            if (FirstPass)
            {
                multilinetextbox.Text = text;
                multilinetextbox.WordWrap = false;
                multilinetextbox.Multiline = true;
                multilinetextbox.ScrollBars = WForms.ScrollBars.Both;
                multilinetextbox.Size = new System.Drawing.Size(multilinetextbox.Size.Width, multilinetextbox.Size.Height * 3);
                multilinetextbox.TextChanged += (o,e) => {
                    multilinetextbox.SelectionStart = multilinetextbox.Text.Length;
                    multilinetextbox.ScrollToCaret();
                };
            }
            output = multilinetextbox.Text;
            var wasInteracted = InteractedElementId == ctrl.ID;
            return wasInteracted;
        }


        
        public bool TreeView(IList<string> texts, ref int selectedIndex,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var text = texts[0];
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.TreeView(), x));
            var treeview = ctrl.WfControl as WForms.TreeView;
            treeview.Text = texts[0];
            if (FirstPass)
            {
                foreach (var text0 in texts)
                {
                    treeview.Nodes.Add(id.ToString(), text0);
                }
            }
            var wasInteracted = InteractedElementId == ctrl.ID;
            if (wasInteracted)
            {
                selectedIndex = treeview.Nodes.IndexOf(treeview.SelectedNode);
            }
            return wasInteracted;
        }

        
        public bool TreeView(IList<string> texts,   [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var text = texts[0];
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.TreeView(), x));
            var treeview = ctrl.WfControl as WForms.TreeView;
            treeview.Text = texts[0];
            if (FirstPass)
            {
                foreach (var text0 in texts)
                {
                    treeview.Nodes.Add(id.ToString(), text0);
                }
            }
            var wasInteracted = InteractedElementId == ctrl.ID;
            return wasInteracted;
        }



        
        public bool ComboBox(string text,ref string selecteditem ,string[] items , [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.ComboBox(), x));
            var combobox = ctrl.WfControl as WForms.ComboBox;
            combobox.Text = text;
            if (FirstPass) {
                combobox.SelectedIndexChanged += LetImGuiHandleIt;
                combobox.Click -= LetImGuiHandleIt;
                combobox.Items.AddRange(items);
                combobox.DropDownStyle = WForms.ComboBoxStyle.DropDownList;
            }
            var wasInteracted = InteractedElementId == ctrl.ID || PrevInteractedElementId == ctrl.ID;
            selecteditem = combobox.SelectedItem as string;
            selecteditem = selecteditem == null ? "" : selecteditem;
            return wasInteracted;
        }




        
        public bool ListBox(string text, ref string selecteditem,string[] items,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.ListBox(), x));
            var listbox = ctrl.WfControl as WForms.ListBox;
            listbox.Text = text;
            if (FirstPass)
            {
                listbox.Click -= LetImGuiHandleIt;
                listbox.SelectedValueChanged += LetImGuiHandleIt;
                listbox.Items.AddRange(items);
            }
            var wasInteracted = InteractedElementId == ctrl.ID || PrevInteractedElementId == ctrl.ID;
            selecteditem = listbox.SelectedItem as string;
            selecteditem = selecteditem == null ? "" : selecteditem;
            return wasInteracted;
        }


        
        public bool CheckedListBox(string text, ref string selecteditem, string[] items, [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.CheckedListBox(), x));
            var checkedlistbox = ctrl.WfControl as WForms.CheckedListBox;
            checkedlistbox.Text = text;
            if (FirstPass)
            {
                checkedlistbox.Click -= LetImGuiHandleIt;
                checkedlistbox.SelectedValueChanged += LetImGuiHandleIt;
                checkedlistbox.Items.AddRange(items);
            }
            var wasInteracted = InteractedElementId == ctrl.ID || PrevInteractedElementId == ctrl.ID;
            selecteditem = checkedlistbox.SelectedItem as string;
            selecteditem = selecteditem == null ? "" : selecteditem;
            return wasInteracted;
        }

        
        public bool Spinner(string text, ref int value, [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, x => InitControlForClicking(new WForms.NumericUpDown(), x));
            var spinner = ctrl.WfControl as WForms.NumericUpDown;
            spinner.Text = text;
            var wasInteracted = InteractedElementId == ctrl.ID || PrevInteractedElementId == ctrl.ID;
            if (wasInteracted) { value = (int)spinner.Value; }
            else { spinner.Value = value; }
            return wasInteracted;
        }

        
        public bool Image(string text,System.Drawing.Image image ,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, id1 => new WForms.PictureBox { Name = id1?.ToString(), AutoSize = true });
            var picturebox = ctrl.WfControl as WForms.PictureBox;
            picturebox.Text = text;
            picturebox.Image = image;
            var wasInteracted = InteractedElementId == ctrl.ID || PrevInteractedElementId == ctrl.ID;
            return wasInteracted;
        }

        
        public bool DateTime(string text,  ref DateTime value,  [CmplTime.CallerFilePath] string callerfilepath ="",[CmplTime.CallerLineNumber] int callerlinenumber= 0,[CmplTime.CallerMemberName] string callermembername = "")
        {
            var id = (ulong?) (callerfilepath,callerlinenumber,callermembername).GetHashCode();
            bool FirstPass = !ControlExists(id);
            var ctrl = ProcureControl(id, id1 => new WForms.DateTimePicker { Name = id1?.ToString(), AutoSize = true });
            var spinner = ctrl.WfControl as WForms.DateTimePicker;
            spinner.Text = text;
            spinner.Value = value;
            var wasInteracted = InteractedElementId == ctrl.ID || PrevInteractedElementId == ctrl.ID;
            return wasInteracted;
        }

	}

}