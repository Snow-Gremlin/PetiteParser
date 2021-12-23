
namespace ExamplesRunner {
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.calcPage = new System.Windows.Forms.TabPage();
            this.calcResultBox = new System.Windows.Forms.TextBox();
            this.calcInputBox = new System.Windows.Forms.TextBox();
            this.btnCalcSolve = new System.Windows.Forms.Button();
            this.coloringPage = new System.Windows.Forms.TabPage();
            this.codeColoringBox = new System.Windows.Forms.RichTextBox();
            this.colorLangBox = new System.Windows.Forms.ComboBox();
            this.btnColorExample = new System.Windows.Forms.Button();
            this.diffPage = new System.Windows.Forms.TabPage();
            this.diffContainer = new System.Windows.Forms.SplitContainer();
            this.diffInputs = new System.Windows.Forms.SplitContainer();
            this.textBoxAdded = new System.Windows.Forms.TextBox();
            this.textBoxRemoved = new System.Windows.Forms.TextBox();
            this.textBoxDiff = new System.Windows.Forms.TextBox();
            this.colorDebouncer = new System.Windows.Forms.Timer(this.components);
            this.diffDebouncer = new System.Windows.Forms.Timer(this.components);
            this.tabControl.SuspendLayout();
            this.calcPage.SuspendLayout();
            this.coloringPage.SuspendLayout();
            this.diffPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.diffContainer)).BeginInit();
            this.diffContainer.Panel1.SuspendLayout();
            this.diffContainer.Panel2.SuspendLayout();
            this.diffContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.diffInputs)).BeginInit();
            this.diffInputs.Panel1.SuspendLayout();
            this.diffInputs.Panel2.SuspendLayout();
            this.diffInputs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.calcPage);
            this.tabControl.Controls.Add(this.coloringPage);
            this.tabControl.Controls.Add(this.diffPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(800, 450);
            this.tabControl.TabIndex = 0;
            // 
            // calcPage
            // 
            this.calcPage.Controls.Add(this.calcResultBox);
            this.calcPage.Controls.Add(this.calcInputBox);
            this.calcPage.Controls.Add(this.btnCalcSolve);
            this.calcPage.Location = new System.Drawing.Point(4, 24);
            this.calcPage.Name = "calcPage";
            this.calcPage.Padding = new System.Windows.Forms.Padding(3);
            this.calcPage.Size = new System.Drawing.Size(792, 422);
            this.calcPage.TabIndex = 0;
            this.calcPage.Text = "Calculator";
            this.calcPage.UseVisualStyleBackColor = true;
            // 
            // calcResultBox
            // 
            this.calcResultBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.calcResultBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.calcResultBox.Location = new System.Drawing.Point(8, 32);
            this.calcResultBox.Multiline = true;
            this.calcResultBox.Name = "calcResultBox";
            this.calcResultBox.ReadOnly = true;
            this.calcResultBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.calcResultBox.Size = new System.Drawing.Size(776, 382);
            this.calcResultBox.TabIndex = 2;
            this.calcResultBox.WordWrap = false;
            // 
            // calcInputBox
            // 
            this.calcInputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.calcInputBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.calcInputBox.Location = new System.Drawing.Point(8, 3);
            this.calcInputBox.Name = "calcInputBox";
            this.calcInputBox.Size = new System.Drawing.Size(695, 22);
            this.calcInputBox.TabIndex = 1;
            this.calcInputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.calcInputBox_KeyDown);
            // 
            // btnCalcSolve
            // 
            this.btnCalcSolve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalcSolve.Location = new System.Drawing.Point(709, 3);
            this.btnCalcSolve.Name = "btnCalcSolve";
            this.btnCalcSolve.Size = new System.Drawing.Size(75, 23);
            this.btnCalcSolve.TabIndex = 0;
            this.btnCalcSolve.Text = "Solve";
            this.btnCalcSolve.UseVisualStyleBackColor = true;
            this.btnCalcSolve.Click += new System.EventHandler(this.btnCalcSolve_Click);
            // 
            // coloringPage
            // 
            this.coloringPage.Controls.Add(this.codeColoringBox);
            this.coloringPage.Controls.Add(this.colorLangBox);
            this.coloringPage.Controls.Add(this.btnColorExample);
            this.coloringPage.Location = new System.Drawing.Point(4, 24);
            this.coloringPage.Name = "coloringPage";
            this.coloringPage.Padding = new System.Windows.Forms.Padding(3);
            this.coloringPage.Size = new System.Drawing.Size(792, 422);
            this.coloringPage.TabIndex = 1;
            this.coloringPage.Text = "Coloring";
            this.coloringPage.UseVisualStyleBackColor = true;
            // 
            // codeColoringBox
            // 
            this.codeColoringBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.codeColoringBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.codeColoringBox.Location = new System.Drawing.Point(8, 33);
            this.codeColoringBox.Name = "codeColoringBox";
            this.codeColoringBox.Size = new System.Drawing.Size(776, 381);
            this.codeColoringBox.TabIndex = 3;
            this.codeColoringBox.Text = "";
            this.codeColoringBox.TextChanged += new System.EventHandler(this.codeColoringBox_TextChanged);
            // 
            // colorLangBox
            // 
            this.colorLangBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorLangBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorLangBox.FormattingEnabled = true;
            this.colorLangBox.Location = new System.Drawing.Point(8, 4);
            this.colorLangBox.Name = "colorLangBox";
            this.colorLangBox.Size = new System.Drawing.Size(695, 23);
            this.colorLangBox.TabIndex = 2;
            this.colorLangBox.SelectedIndexChanged += new System.EventHandler(this.colorLangBox_SelectedIndexChanged);
            // 
            // btnColorExample
            // 
            this.btnColorExample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColorExample.Location = new System.Drawing.Point(709, 3);
            this.btnColorExample.Name = "btnColorExample";
            this.btnColorExample.Size = new System.Drawing.Size(75, 23);
            this.btnColorExample.TabIndex = 1;
            this.btnColorExample.Text = "Example";
            this.btnColorExample.UseVisualStyleBackColor = true;
            this.btnColorExample.Click += new System.EventHandler(this.btnColorExample_Click);
            // 
            // diffPage
            // 
            this.diffPage.Controls.Add(this.diffContainer);
            this.diffPage.Location = new System.Drawing.Point(4, 24);
            this.diffPage.Name = "diffPage";
            this.diffPage.Padding = new System.Windows.Forms.Padding(3);
            this.diffPage.Size = new System.Drawing.Size(792, 422);
            this.diffPage.TabIndex = 2;
            this.diffPage.Text = "Diff";
            this.diffPage.UseVisualStyleBackColor = true;
            // 
            // diffContainer
            // 
            this.diffContainer.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.diffContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffContainer.Location = new System.Drawing.Point(3, 3);
            this.diffContainer.Name = "diffContainer";
            // 
            // diffContainer.Panel1
            // 
            this.diffContainer.Panel1.Controls.Add(this.diffInputs);
            // 
            // diffContainer.Panel2
            // 
            this.diffContainer.Panel2.Controls.Add(this.textBoxDiff);
            this.diffContainer.Size = new System.Drawing.Size(786, 416);
            this.diffContainer.SplitterDistance = 543;
            this.diffContainer.TabIndex = 0;
            // 
            // diffInputs
            // 
            this.diffInputs.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.diffInputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffInputs.Location = new System.Drawing.Point(0, 0);
            this.diffInputs.Name = "diffInputs";
            // 
            // diffInputs.Panel1
            // 
            this.diffInputs.Panel1.Controls.Add(this.textBoxAdded);
            // 
            // diffInputs.Panel2
            // 
            this.diffInputs.Panel2.Controls.Add(this.textBoxRemoved);
            this.diffInputs.Size = new System.Drawing.Size(543, 416);
            this.diffInputs.SplitterDistance = 270;
            this.diffInputs.TabIndex = 0;
            // 
            // textBoxAdded
            // 
            this.textBoxAdded.AcceptsReturn = true;
            this.textBoxAdded.AcceptsTab = true;
            this.textBoxAdded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAdded.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxAdded.Location = new System.Drawing.Point(0, 0);
            this.textBoxAdded.Multiline = true;
            this.textBoxAdded.Name = "textBoxAdded";
            this.textBoxAdded.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxAdded.Size = new System.Drawing.Size(270, 416);
            this.textBoxAdded.TabIndex = 0;
            this.textBoxAdded.TextChanged += new System.EventHandler(this.inputDiff_TextChanged);
            // 
            // textBoxRemoved
            // 
            this.textBoxRemoved.AcceptsReturn = true;
            this.textBoxRemoved.AcceptsTab = true;
            this.textBoxRemoved.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxRemoved.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxRemoved.Location = new System.Drawing.Point(0, 0);
            this.textBoxRemoved.Multiline = true;
            this.textBoxRemoved.Name = "textBoxRemoved";
            this.textBoxRemoved.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxRemoved.Size = new System.Drawing.Size(269, 416);
            this.textBoxRemoved.TabIndex = 1;
            this.textBoxRemoved.TextChanged += new System.EventHandler(this.inputDiff_TextChanged);
            // 
            // textBoxDiff
            // 
            this.textBoxDiff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDiff.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxDiff.Location = new System.Drawing.Point(0, 0);
            this.textBoxDiff.Multiline = true;
            this.textBoxDiff.Name = "textBoxDiff";
            this.textBoxDiff.ReadOnly = true;
            this.textBoxDiff.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDiff.Size = new System.Drawing.Size(239, 416);
            this.textBoxDiff.TabIndex = 1;
            // 
            // colorDebouncer
            // 
            this.colorDebouncer.Interval = 250;
            this.colorDebouncer.Tick += new System.EventHandler(this.colorDebouncer_Tick);
            // 
            // diffDebouncer
            // 
            this.diffDebouncer.Interval = 250;
            this.diffDebouncer.Tick += new System.EventHandler(this.diffDebouncer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.Text = "Petite Parser Examples";
            this.tabControl.ResumeLayout(false);
            this.calcPage.ResumeLayout(false);
            this.calcPage.PerformLayout();
            this.coloringPage.ResumeLayout(false);
            this.diffPage.ResumeLayout(false);
            this.diffContainer.Panel1.ResumeLayout(false);
            this.diffContainer.Panel2.ResumeLayout(false);
            this.diffContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.diffContainer)).EndInit();
            this.diffContainer.ResumeLayout(false);
            this.diffInputs.Panel1.ResumeLayout(false);
            this.diffInputs.Panel1.PerformLayout();
            this.diffInputs.Panel2.ResumeLayout(false);
            this.diffInputs.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.diffInputs)).EndInit();
            this.diffInputs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage calcPage;
        private System.Windows.Forms.TabPage coloringPage;
        private System.Windows.Forms.TextBox calcResultBox;
        private System.Windows.Forms.Button btnCalcSolve;
        private System.Windows.Forms.TextBox calcInputBox;
        private System.Windows.Forms.Button btnColorExample;
        private System.Windows.Forms.ComboBox colorLangBox;
        private System.Windows.Forms.RichTextBox codeColoringBox;
        private System.Windows.Forms.Timer colorDebouncer;
        private System.Windows.Forms.TabPage diffPage;
        private System.Windows.Forms.SplitContainer diffContainer;
        private System.Windows.Forms.SplitContainer diffInputs;
        private System.Windows.Forms.TextBox textBoxAdded;
        private System.Windows.Forms.TextBox textBoxRemoved;
        private System.Windows.Forms.TextBox textBoxDiff;
        private System.Windows.Forms.Timer diffDebouncer;
    }
}

