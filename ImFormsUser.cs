﻿using System.Windows.Forms;
using ImForms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ImFormsUser
{
    public partial class ImFormsUser : Form
    {
        public int y = 0;
        public int t = 0;
        ImFormsMgr rightPanelMgr;

        public ImFormsUser()
        {
            InitializeComponent();
            Load += async (o, e) => await Main(Panel1);
            Load += async (o, e) => await Main2(Panel2);

            Task.Run(async () =>
            {
                while (true)
                {
                    t++;
                    await Task.Delay(1);
                }
            });
        }

        public async Task Main(Panel panel)
        {
            ImFormsMgr mgr = new ImFormsMgr(panel);

            IList<int> list = new List<int> { 1, 2, 3 };
            
            int x = 0;
            bool displayList = false;
            bool reverseList = false;
            float f = 0;
            string teststr = "";
            while (true)
            {
                mgr.Label("This ImForms panel refreshes only when there is user interaction");
                mgr.Space(CompileTime.ID());
                mgr.Label("ImForms makes it easy to display and modify one value with multiple controls");
                mgr.Label("x ="+x);
                mgr.Label("f ="+f,"f =");
                mgr.TreeView(new string[] { "bdfihdf", "dshsdiusdh" });
                string res = "";
                if (mgr.ComboBox("text box",ref res,new string[] { "t","f","c" }))
                {
                   mgr.Label(res);
                }
                mgr.InputMultilineText("Text box:",ref teststr);
                mgr.Label(teststr);
                mgr.RadioButton("0", ref x, 0);
                mgr.RadioButton("1", ref x, 1);
                mgr.SliderFloat("slider flt val:", ref f);
                mgr.ProgressFloat("progress:", ref f);
                int valueToAssignX = (x == 1) ? 0 : 1;
                if (mgr.Button("x <- " + valueToAssignX))
                {
                    x = valueToAssignX;
                }

                bool xIs1 = (x == 1);
                mgr.Checkbox("X == 1", ref xIs1);

                mgr.Space(CompileTime.ID());
                mgr.Label("Just like with other ImGui implementations, if a function isn't called for it," +
                    " a control isn't displayed.");
                mgr.Checkbox("Show list", ref displayList);

                if (displayList)
                {
                    var seq = reverseList ? list.Reverse() : list;

                    if (mgr.Button("Add to end")) { list.Add(list.LastOrDefault() + 1); }

                    if (list.Any() && mgr.Button("Remove from front"))
                    {
                        list.RemoveAt(0);
                    }

                    mgr.Checkbox("Display reversed", ref reverseList);

                    foreach (var n in seq) { mgr.Label("[" + n + "]"); }
                }

                mgr.Space(CompileTime.ID());
                mgr.Label("Values from other threads can be displayed when a panel refreshes.");
                mgr.LinkLabel("Try it!");
                mgr.Label("y = " + y, CompileTime.ID());

                await mgr.NextFrame();
            }
        }

        public async Task Main2(Panel panel)
        {
            rightPanelMgr = new ImFormsMgr(panel);

            var timer = new Timer();
            timer.Tick += (o, e) => rightPanelMgr.Refresh();
            int updateRate = 1000;

            while (true)
            {
                rightPanelMgr.Label("This ImForms panel auto-updates once every:");
                rightPanelMgr.RadioButton("Second", ref updateRate, 1000);
                rightPanelMgr.RadioButton("100ms", ref updateRate, 100);
                rightPanelMgr.RadioButton("10ms", ref updateRate, 10);
                rightPanelMgr.RadioButton("Never", ref updateRate, -1);
                timer.Interval = (updateRate > 0) ? updateRate : int.MaxValue;
                timer.Enabled = (updateRate > 0);
                rightPanelMgr.Space(CompileTime.ID());
                rightPanelMgr.Label("Auto-updating is an easy way to display values from other threads");
                rightPanelMgr.Label("y = " + y, CompileTime.ID());
                rightPanelMgr.Label("t = " + t, CompileTime.ID());
                await rightPanelMgr.NextFrame();
            }
        }

        private void yIncrBtn_Click(object sender, System.EventArgs e)
        {
            y++;
        }

        private void refreshBtn_Click(object sender, System.EventArgs e)
        {
            rightPanelMgr.Refresh();
        }
    }
}
