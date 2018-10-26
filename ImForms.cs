﻿using System.Collections.Generic;
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
    public static class CompileTime
    {
        public static string ID(
            [CmplTime.CallerFilePath] string srcFilePath = "",
            [CmplTime.CallerLineNumber] int srcLineNumber = 0)
        {
            return srcFilePath + srcLineNumber;
        }
    }

    public enum ImDraw
    {
        NotDrawn,
        Drawn
    }

    public class ImControl
    {
        public readonly WForms.Control WfControl;
        public ImDraw State { get; set; }
        public int SortKey { get; set; }
        public string ID { get { return WfControl.Name; } }

        public ImControl(WForms.Control control) { WfControl = control; }
    }

    public class ImFormsMgr
    {
        [Interop.DllImport("user32.dll", CharSet = Interop.CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(Interop.HandleRef hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        private static void EnableRepaint(Interop.HandleRef handle, bool enable)
        {
            const int WM_SETREDRAW = 0x000B;
            SendMessage(handle, WM_SETREDRAW, new IntPtr(enable ? 1 : 0), IntPtr.Zero);
        }

        private int RemainingRedraws = 0;
        private TaskCompletionSource<bool> TCS;
        private Dictionary<string, ImControl> ImControls;
        public WFControlList DisplayedControls;
        private int CurrentSortKey;
        private string InteractedElementId;

        // OH NOTE This could be configurable by the user in the _distant_ future
        private  int RedrawsPerInteraction = 1;

        public ImFormsMgr(WForms.Panel panel)
        {
            InteractedElementId = null;
            ImControls = new Dictionary<string, ImControl>();
            TCS = new TaskCompletionSource<bool>();
            CurrentSortKey = 0;
            DisplayedControls = panel.Controls;
        }

        public void QueueRedraws(int numRedraws) { RemainingRedraws += numRedraws; }

        private void LetImGuiHandleIt(object sender, EventArgs args)
        {
            InteractedElementId = ((WForms.Control)sender).Name;
            QueueRedraws(RedrawsPerInteraction);
            Refresh();
        }

        public ImControl ProcureControl(string id, ImFormsCtrlMaker maker)
        {
            ImControl ctrl;
            if (!ImControls.TryGetValue(id, out ctrl))
            {
                ctrl = new ImControl(maker(id));
                ImControls.Add(id, ctrl);
            }
            
            ctrl.State = ImDraw.Drawn;
            ctrl.SortKey = CurrentSortKey;
            ctrl.WfControl.Visible = true;
            CurrentSortKey++;
            return ctrl;
        }

        public delegate WForms.Control ImFormsCtrlMaker(string id);

        // Generic maker 
        public WForms.Control ClickCtrlMaker<TCtrl>(string id) where TCtrl : WForms.Control, new()
        {
            var wfCtrl = new TCtrl() { Name = id };
            wfCtrl.Click += LetImGuiHandleIt;
            var tbar = wfCtrl as WForms.TrackBar;
            if (tbar != null)
            {
                tbar.ValueChanged += LetImGuiHandleIt;
            }
            wfCtrl.AutoSize = true;
            return wfCtrl;
        }

        public void Space(string id)
        {
            Label("", id);
        }

        public void Label(string text, string id = null)
        {
            var ctrl = ProcureControl(id ?? text, id1 => new WForms.Label { Name = id1, AutoSize = true });
            ctrl.WfControl.Text = text;
        }

        public bool Button(string text, string id = null)
        {
            var ctrl = ProcureControl(id ?? text, ClickCtrlMaker<WForms.Button>);
            ctrl.WfControl.Text = text;
            return InteractedElementId == ctrl.ID;
        }

        public bool LinkLabel(string text, string id = null)
        {
            var ctrl = ProcureControl(id ?? text, ClickCtrlMaker<WForms.LinkLabel>);
            ctrl.WfControl.Text = text;
            return InteractedElementId == ctrl.ID;
        }

        public bool Checkbox(string text, ref bool checkBoxChecked, string id = null)
        {
            var ctrl = ProcureControl(id ?? text, ClickCtrlMaker<WForms.CheckBox>);
            var checkBox = ctrl.WfControl as WForms.CheckBox;
            checkBox.Text = text;
            var wasInteracted = InteractedElementId == ctrl.ID;

            if (wasInteracted) { checkBoxChecked = checkBox.Checked; }
            else { checkBox.Checked = checkBoxChecked; }

            return wasInteracted;
        }

        public bool RadioButton(string text, ref int value, int checkAgainst, string id = null)
        {
            var ctrl = ProcureControl(id ?? text, ClickCtrlMaker<WForms.RadioButton>);
            var radioButton = ctrl.WfControl as WForms.RadioButton;
            radioButton.Text = text;
            var wasInteracted = InteractedElementId == ctrl.ID;

            if (wasInteracted) { value = checkAgainst; }
            else { radioButton.Checked = (value == checkAgainst); }

            return wasInteracted;
        }

        public bool SliderInt(string text,ref int value ,int minval = 0, int maxval = 1, string id = null)
        {
            var ctrl = ProcureControl(id ?? text, ClickCtrlMaker<WForms.TrackBar>);
            var trackbar = ctrl.WfControl as WForms.TrackBar;
            trackbar.Text = text;
            trackbar.Minimum = minval;
            trackbar.Maximum = maxval;
            var wasInteracted = InteractedElementId == ctrl.ID;
            if (wasInteracted) { value = trackbar.Value; }
            else { trackbar.Value = value; }
            return wasInteracted;
        }

        public bool SliderFloat(string text, ref float value, float minval = 0.0f, float maxval = 1.0f, string id = null)
        {
            var ctrl = ProcureControl(id ?? text, ClickCtrlMaker<WForms.TrackBar>);
            var trackbar = ctrl.WfControl as WForms.TrackBar;
            trackbar.Text = text;
            var unitscale = (maxval - minval)*100; 
            trackbar.Minimum = (int)(minval*unitscale);
            trackbar.Maximum = (int)(maxval*unitscale);
            var wasInteracted = InteractedElementId == ctrl.ID;
            if (wasInteracted) { value = trackbar.Value/100.0f; }
            else { trackbar.Value = (int)(value*unitscale); }
            return wasInteracted;
        }

        public void ProgressInt(string text, ref int value, int minval = 0, int maxval = 1, string id = null)
        {
            var ctrl = ProcureControl(id ?? text, ClickCtrlMaker<WForms.ProgressBar>);
            var trackbar = ctrl.WfControl as WForms.ProgressBar;
            trackbar.Text = text;
            trackbar.Minimum = minval;
            trackbar.Maximum = maxval;
            trackbar.Value = value; 
        }

        public void ProgressFloat(string text, ref float value, float minval = 0.0f, float maxval = 1.0f, string id = null)
        {
            var ctrl = ProcureControl(id ?? text, ClickCtrlMaker<WForms.ProgressBar>);
            var trackbar = ctrl.WfControl as WForms.ProgressBar;
            trackbar.Text = text;
            var unitscale = (maxval - minval) * 100;
            trackbar.Minimum = (int)(minval * unitscale);
            trackbar.Maximum = (int)(maxval * unitscale);
            trackbar.Value = (int)(value * unitscale);
        }

        private static IDictionary<string,int> ConvertEnumToDictionary(Enum e)
        {
            return Enum.GetValues(e.GetType()).Cast<int>().ToDictionary(x => Enum.GetName(e.GetType(), x));
        }

        public int ComboBox(string text,Enum Enumeration , string id = null)
        {
            var ctrl = ProcureControl(id ?? text, ClickCtrlMaker<WForms.ComboBox>);
            var trackbar = ctrl.WfControl as WForms.ComboBox;
            trackbar.Text = Enumeration.GetType().Name;
            var source = ConvertEnumToDictionary(Enumeration);
            if (!trackbar.Items.Contains(source.Keys.First())) {
                trackbar.Items.Clear();
                trackbar.Items.AddRange(source.Keys.ToArray());
            }
            Debug.WriteLine("tbi :" +trackbar.SelectedIndex.ToString());
            return trackbar.SelectedIndex;
        }

        public bool CheckedListBox(string text, ref bool checkBoxChecked, string id = null)
        {
            //TODO(shazan) : Implement this  
            var ctrl = ProcureControl(id ?? text, ClickCtrlMaker<WForms.CheckedListBox>);
            var checkedlistbox = ctrl.WfControl as WForms.CheckedListBox;
            return false;
        }
        
        public void Refresh()
        {
            if (!TCS.Task.IsCompleted)
            {
                TCS.SetResult(true);
            }
        }

        public async Task NextFrame()
        {
            // PrevInteractedElement = InteractedElement;
            const int ctrlsToTriggerCleanup = 100;
            const int ctrlsToRemoveForCleanup = 50;

            var undrawnControls = ImControls.Values.Where(ctrl => ctrl.State == ImDraw.NotDrawn)
                .Take(ctrlsToTriggerCleanup).ToList();

            if (undrawnControls.Count == ctrlsToTriggerCleanup)
            {
                foreach (var ctrl in undrawnControls.Take(ctrlsToRemoveForCleanup))
                {
                    ImControls.Remove(ctrl.ID);
                }
            }

            InteractedElementId = null;
            WForms.Control[] sortedControls = ImControls.Values.Where(x => x.State == ImDraw.Drawn)
                .OrderBy(c => c.SortKey).Select(c => c.WfControl).ToArray();
            var controlsChanged = DisplayedControls.Count != sortedControls.Length
                || !Enumerable.Zip(
                        DisplayedControls.OfType<WForms.Control>(),
                        sortedControls,
                        (c1, c2) => c1 == c2).All(b => b);
            
            if (controlsChanged)
            {
                var handle = new Interop.HandleRef(DisplayedControls.Owner, DisplayedControls.Owner.Handle);
                EnableRepaint(handle, false);
                DisplayedControls.Clear();
                DisplayedControls.AddRange(sortedControls);
                EnableRepaint(handle, true);
                DisplayedControls.Owner.Refresh();
            }

            // Automatically go to next frame for each requested redraw
            if (RemainingRedraws <= 0)
            {
                RemainingRedraws = 0;
                await TCS.Task;
                TCS = new TaskCompletionSource<bool>();
            }
            else
            {
                RemainingRedraws--;
            }

            foreach (var ctrl in ImControls.Values)
            {
                ctrl.State = ImDraw.NotDrawn;
                ctrl.SortKey = 999999;
            }
        }
    }
}