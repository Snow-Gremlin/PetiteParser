namespace LanguageTestingTool {
    partial class MainForm {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.tabInput = new System.Windows.Forms.TabPage();
            this.splitInput = new System.Windows.Forms.SplitContainer();
            this.boxInput = new System.Windows.Forms.TextBox();
            this.boxResultTree = new System.Windows.Forms.TextBox();
            this.tabStates = new System.Windows.Forms.TabPage();
            this.stateSplit = new System.Windows.Forms.SplitContainer();
            this.boxStateFrags = new System.Windows.Forms.TextBox();
            this.boxStateActions = new System.Windows.Forms.TextBox();
            this.panelState = new System.Windows.Forms.Panel();
            this.maxLabel = new System.Windows.Forms.Label();
            this.numState = new System.Windows.Forms.NumericUpDown();
            this.stateLabel = new System.Windows.Forms.Label();
            this.tabNorm = new System.Windows.Forms.TabPage();
            this.boxNorm = new System.Windows.Forms.TextBox();
            this.tabLang = new System.Windows.Forms.TabPage();
            this.splitLang = new System.Windows.Forms.SplitContainer();
            this.boxLang = new System.Windows.Forms.TextBox();
            this.boxLangResult = new System.Windows.Forms.TextBox();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.debounceTimer = new System.Windows.Forms.Timer(this.components);
            this.tabInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitInput)).BeginInit();
            this.splitInput.Panel1.SuspendLayout();
            this.splitInput.Panel2.SuspendLayout();
            this.splitInput.SuspendLayout();
            this.tabStates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stateSplit)).BeginInit();
            this.stateSplit.Panel1.SuspendLayout();
            this.stateSplit.Panel2.SuspendLayout();
            this.stateSplit.SuspendLayout();
            this.panelState.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numState)).BeginInit();
            this.tabNorm.SuspendLayout();
            this.tabLang.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitLang)).BeginInit();
            this.splitLang.Panel1.SuspendLayout();
            this.splitLang.Panel2.SuspendLayout();
            this.splitLang.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabInput
            // 
            this.tabInput.Controls.Add(this.splitInput);
            this.tabInput.Location = new System.Drawing.Point(4, 24);
            this.tabInput.Name = "tabInput";
            this.tabInput.Padding = new System.Windows.Forms.Padding(3);
            this.tabInput.Size = new System.Drawing.Size(716, 624);
            this.tabInput.TabIndex = 1;
            this.tabInput.Text = "Input";
            this.tabInput.UseVisualStyleBackColor = true;
            // 
            // splitInput
            // 
            this.splitInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitInput.Location = new System.Drawing.Point(3, 3);
            this.splitInput.Name = "splitInput";
            this.splitInput.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitInput.Panel1
            // 
            this.splitInput.Panel1.Controls.Add(this.boxInput);
            // 
            // splitInput.Panel2
            // 
            this.splitInput.Panel2.Controls.Add(this.boxResultTree);
            this.splitInput.Size = new System.Drawing.Size(710, 618);
            this.splitInput.SplitterDistance = 291;
            this.splitInput.TabIndex = 0;
            // 
            // boxInput
            // 
            this.boxInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxInput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.boxInput.Location = new System.Drawing.Point(0, 0);
            this.boxInput.Multiline = true;
            this.boxInput.Name = "boxInput";
            this.boxInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.boxInput.Size = new System.Drawing.Size(710, 291);
            this.boxInput.TabIndex = 0;
            this.boxInput.WordWrap = false;
            this.boxInput.TextChanged += new System.EventHandler(this.boxInput_TextChanged);
            // 
            // boxResultTree
            // 
            this.boxResultTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxResultTree.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.boxResultTree.Location = new System.Drawing.Point(0, 0);
            this.boxResultTree.Multiline = true;
            this.boxResultTree.Name = "boxResultTree";
            this.boxResultTree.ReadOnly = true;
            this.boxResultTree.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.boxResultTree.Size = new System.Drawing.Size(710, 323);
            this.boxResultTree.TabIndex = 0;
            this.boxResultTree.WordWrap = false;
            // 
            // tabStates
            // 
            this.tabStates.Controls.Add(this.stateSplit);
            this.tabStates.Controls.Add(this.panelState);
            this.tabStates.Location = new System.Drawing.Point(4, 24);
            this.tabStates.Name = "tabStates";
            this.tabStates.Size = new System.Drawing.Size(716, 624);
            this.tabStates.TabIndex = 3;
            this.tabStates.Text = "States";
            this.tabStates.UseVisualStyleBackColor = true;
            // 
            // stateSplit
            // 
            this.stateSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateSplit.Location = new System.Drawing.Point(0, 28);
            this.stateSplit.Name = "stateSplit";
            this.stateSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // stateSplit.Panel1
            // 
            this.stateSplit.Panel1.Controls.Add(this.boxStateFrags);
            // 
            // stateSplit.Panel2
            // 
            this.stateSplit.Panel2.Controls.Add(this.boxStateActions);
            this.stateSplit.Size = new System.Drawing.Size(716, 596);
            this.stateSplit.SplitterDistance = 298;
            this.stateSplit.TabIndex = 2;
            // 
            // boxStateFrags
            // 
            this.boxStateFrags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxStateFrags.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.boxStateFrags.Location = new System.Drawing.Point(0, 0);
            this.boxStateFrags.Multiline = true;
            this.boxStateFrags.Name = "boxStateFrags";
            this.boxStateFrags.ReadOnly = true;
            this.boxStateFrags.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.boxStateFrags.Size = new System.Drawing.Size(716, 298);
            this.boxStateFrags.TabIndex = 1;
            this.boxStateFrags.WordWrap = false;
            // 
            // boxStateActions
            // 
            this.boxStateActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxStateActions.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.boxStateActions.Location = new System.Drawing.Point(0, 0);
            this.boxStateActions.Multiline = true;
            this.boxStateActions.Name = "boxStateActions";
            this.boxStateActions.ReadOnly = true;
            this.boxStateActions.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.boxStateActions.Size = new System.Drawing.Size(716, 294);
            this.boxStateActions.TabIndex = 0;
            this.boxStateActions.WordWrap = false;
            // 
            // panelState
            // 
            this.panelState.Controls.Add(this.maxLabel);
            this.panelState.Controls.Add(this.numState);
            this.panelState.Controls.Add(this.stateLabel);
            this.panelState.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelState.Location = new System.Drawing.Point(0, 0);
            this.panelState.Name = "panelState";
            this.panelState.Size = new System.Drawing.Size(716, 28);
            this.panelState.TabIndex = 1;
            // 
            // maxLabel
            // 
            this.maxLabel.Location = new System.Drawing.Point(130, 1);
            this.maxLabel.Name = "maxLabel";
            this.maxLabel.Size = new System.Drawing.Size(115, 23);
            this.maxLabel.TabIndex = 2;
            this.maxLabel.Text = "Max";
            this.maxLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numState
            // 
            this.numState.Location = new System.Drawing.Point(45, 3);
            this.numState.Name = "numState";
            this.numState.Size = new System.Drawing.Size(79, 23);
            this.numState.TabIndex = 1;
            this.numState.ValueChanged += new System.EventHandler(this.numState_ValueChanged);
            // 
            // stateLabel
            // 
            this.stateLabel.Location = new System.Drawing.Point(2, 1);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(37, 23);
            this.stateLabel.TabIndex = 0;
            this.stateLabel.Text = "State";
            this.stateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabNorm
            // 
            this.tabNorm.Controls.Add(this.boxNorm);
            this.tabNorm.Location = new System.Drawing.Point(4, 24);
            this.tabNorm.Name = "tabNorm";
            this.tabNorm.Size = new System.Drawing.Size(716, 624);
            this.tabNorm.TabIndex = 2;
            this.tabNorm.Text = "Normalized";
            this.tabNorm.UseVisualStyleBackColor = true;
            // 
            // boxNorm
            // 
            this.boxNorm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxNorm.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.boxNorm.Location = new System.Drawing.Point(0, 0);
            this.boxNorm.Multiline = true;
            this.boxNorm.Name = "boxNorm";
            this.boxNorm.ReadOnly = true;
            this.boxNorm.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.boxNorm.Size = new System.Drawing.Size(716, 624);
            this.boxNorm.TabIndex = 0;
            this.boxNorm.WordWrap = false;
            // 
            // tabLang
            // 
            this.tabLang.Controls.Add(this.splitLang);
            this.tabLang.Location = new System.Drawing.Point(4, 24);
            this.tabLang.Name = "tabLang";
            this.tabLang.Padding = new System.Windows.Forms.Padding(3);
            this.tabLang.Size = new System.Drawing.Size(1125, 1167);
            this.tabLang.TabIndex = 0;
            this.tabLang.Text = "Language";
            this.tabLang.UseVisualStyleBackColor = true;
            // 
            // splitLang
            // 
            this.splitLang.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitLang.Location = new System.Drawing.Point(3, 3);
            this.splitLang.Name = "splitLang";
            this.splitLang.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitLang.Panel1
            // 
            this.splitLang.Panel1.Controls.Add(this.boxLang);
            // 
            // splitLang.Panel2
            // 
            this.splitLang.Panel2.Controls.Add(this.boxLangResult);
            this.splitLang.Size = new System.Drawing.Size(1119, 1161);
            this.splitLang.SplitterDistance = 813;
            this.splitLang.TabIndex = 0;
            // 
            // boxLang
            // 
            this.boxLang.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxLang.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.boxLang.Location = new System.Drawing.Point(0, 0);
            this.boxLang.Multiline = true;
            this.boxLang.Name = "boxLang";
            this.boxLang.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.boxLang.Size = new System.Drawing.Size(1119, 813);
            this.boxLang.TabIndex = 0;
            this.boxLang.WordWrap = false;
            this.boxLang.TextChanged += new System.EventHandler(this.boxLang_TextChanged);
            // 
            // boxLangResult
            // 
            this.boxLangResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxLangResult.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.boxLangResult.Location = new System.Drawing.Point(0, 0);
            this.boxLangResult.Multiline = true;
            this.boxLangResult.Name = "boxLangResult";
            this.boxLangResult.ReadOnly = true;
            this.boxLangResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.boxLangResult.Size = new System.Drawing.Size(1119, 344);
            this.boxLangResult.TabIndex = 0;
            this.boxLangResult.WordWrap = false;
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.tabLang);
            this.mainTabControl.Controls.Add(this.tabNorm);
            this.mainTabControl.Controls.Add(this.tabStates);
            this.mainTabControl.Controls.Add(this.tabInput);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(1133, 1195);
            this.mainTabControl.TabIndex = 0;
            // 
            // debounceTimer
            // 
            this.debounceTimer.Interval = 500;
            this.debounceTimer.Tick += new System.EventHandler(this.debounceTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1133, 1195);
            this.Controls.Add(this.mainTabControl);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Petite Language Testing Tool";
            this.tabInput.ResumeLayout(false);
            this.splitInput.Panel1.ResumeLayout(false);
            this.splitInput.Panel1.PerformLayout();
            this.splitInput.Panel2.ResumeLayout(false);
            this.splitInput.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitInput)).EndInit();
            this.splitInput.ResumeLayout(false);
            this.tabStates.ResumeLayout(false);
            this.stateSplit.Panel1.ResumeLayout(false);
            this.stateSplit.Panel1.PerformLayout();
            this.stateSplit.Panel2.ResumeLayout(false);
            this.stateSplit.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stateSplit)).EndInit();
            this.stateSplit.ResumeLayout(false);
            this.panelState.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numState)).EndInit();
            this.tabNorm.ResumeLayout(false);
            this.tabNorm.PerformLayout();
            this.tabLang.ResumeLayout(false);
            this.splitLang.Panel1.ResumeLayout(false);
            this.splitLang.Panel1.PerformLayout();
            this.splitLang.Panel2.ResumeLayout(false);
            this.splitLang.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitLang)).EndInit();
            this.splitLang.ResumeLayout(false);
            this.mainTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TabPage tabInput;
        private TabPage tabStates;
        private TabPage tabNorm;
        private TextBox boxNorm;
        private TabPage tabLang;
        private SplitContainer splitLang;
        private TextBox boxLang;
        private TextBox boxLangResult;
        private TabControl mainTabControl;
        private TextBox boxStateActions;
        private SplitContainer splitInput;
        private TextBox boxInput;
        private TextBox boxResultTree;
        private System.Windows.Forms.Timer debounceTimer;
        private Panel panelState;
        private Label maxLabel;
        private NumericUpDown numState;
        private Label stateLabel;
        private SplitContainer stateSplit;
        private TextBox boxStateFrags;
    }
}