namespace Reverse.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.PanelHeader = new System.Windows.Forms.Panel();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblGreeting = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnConfiguracao = new System.Windows.Forms.Button();
            this.picTriagem = new System.Windows.Forms.PictureBox();
            this.picFinanceiro = new System.Windows.Forms.PictureBox();
            this.picRH = new System.Windows.Forms.PictureBox();
            this.picExp = new System.Windows.Forms.PictureBox();
            this.picComercial = new System.Windows.Forms.PictureBox();
            this.PanelHeader.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTriagem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFinanceiro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picComercial)).BeginInit();
            this.SuspendLayout();
            // 
            // PanelHeader
            // 
            this.PanelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.PanelHeader.Controls.Add(this.btnConfiguracao);
            this.PanelHeader.Controls.Add(this.btnSair);
            this.PanelHeader.Controls.Add(this.lblGreeting);
            this.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelHeader.Location = new System.Drawing.Point(0, 0);
            this.PanelHeader.Name = "PanelHeader";
            this.PanelHeader.Size = new System.Drawing.Size(1890, 67);
            this.PanelHeader.TabIndex = 1;
            this.PanelHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitulo_MouseDown);
            // 
            // btnSair
            // 
            this.btnSair.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSair.ForeColor = System.Drawing.Color.White;
            this.btnSair.Location = new System.Drawing.Point(1815, 0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(75, 67);
            this.btnSair.TabIndex = 1;
            this.btnSair.Text = "X";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // lblGreeting
            // 
            this.lblGreeting.AutoSize = true;
            this.lblGreeting.Font = new System.Drawing.Font("Segoe UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGreeting.ForeColor = System.Drawing.Color.White;
            this.lblGreeting.Location = new System.Drawing.Point(3, 9);
            this.lblGreeting.Name = "lblGreeting";
            this.lblGreeting.Size = new System.Drawing.Size(348, 47);
            this.lblGreeting.TabIndex = 0;
            this.lblGreeting.Text = "Texto de bem vindo";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.picComercial, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 67);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1890, 793);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.picTriagem);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(372, 787);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.picFinanceiro);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(381, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(372, 787);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.picRH);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(759, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(372, 787);
            this.panel3.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.picExp);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(1137, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(372, 787);
            this.panel4.TabIndex = 3;
            // 
            // btnConfiguracao
            // 
            this.btnConfiguracao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfiguracao.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfiguracao.ForeColor = System.Drawing.Color.White;
            this.btnConfiguracao.Location = new System.Drawing.Point(1650, 12);
            this.btnConfiguracao.Name = "btnConfiguracao";
            this.btnConfiguracao.Size = new System.Drawing.Size(145, 44);
            this.btnConfiguracao.TabIndex = 2;
            this.btnConfiguracao.Text = "C.D.U";
            this.btnConfiguracao.UseVisualStyleBackColor = true;
            this.btnConfiguracao.Click += new System.EventHandler(this.btnConfiguracao_Click);
            // 
            // picTriagem
            // 
            this.picTriagem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picTriagem.Image = ((System.Drawing.Image)(resources.GetObject("picTriagem.Image")));
            this.picTriagem.Location = new System.Drawing.Point(0, 0);
            this.picTriagem.Name = "picTriagem";
            this.picTriagem.Size = new System.Drawing.Size(372, 787);
            this.picTriagem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picTriagem.TabIndex = 0;
            this.picTriagem.TabStop = false;
            this.picTriagem.Click += new System.EventHandler(this.picTriagem_Click);
            // 
            // picFinanceiro
            // 
            this.picFinanceiro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picFinanceiro.Image = ((System.Drawing.Image)(resources.GetObject("picFinanceiro.Image")));
            this.picFinanceiro.Location = new System.Drawing.Point(0, 0);
            this.picFinanceiro.Name = "picFinanceiro";
            this.picFinanceiro.Size = new System.Drawing.Size(372, 787);
            this.picFinanceiro.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFinanceiro.TabIndex = 0;
            this.picFinanceiro.TabStop = false;
            this.picFinanceiro.Click += new System.EventHandler(this.picFinanceiro_Click);
            // 
            // picRH
            // 
            this.picRH.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picRH.Image = ((System.Drawing.Image)(resources.GetObject("picRH.Image")));
            this.picRH.Location = new System.Drawing.Point(0, 0);
            this.picRH.Name = "picRH";
            this.picRH.Size = new System.Drawing.Size(372, 787);
            this.picRH.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRH.TabIndex = 0;
            this.picRH.TabStop = false;
            this.picRH.Click += new System.EventHandler(this.picRH_Click);
            // 
            // picExp
            // 
            this.picExp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picExp.Image = ((System.Drawing.Image)(resources.GetObject("picExp.Image")));
            this.picExp.Location = new System.Drawing.Point(0, 0);
            this.picExp.Name = "picExp";
            this.picExp.Size = new System.Drawing.Size(372, 787);
            this.picExp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picExp.TabIndex = 0;
            this.picExp.TabStop = false;
            this.picExp.Click += new System.EventHandler(this.picExp_Click);
            // 
            // picComercial
            // 
            this.picComercial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picComercial.Image = ((System.Drawing.Image)(resources.GetObject("picComercial.Image")));
            this.picComercial.Location = new System.Drawing.Point(1515, 3);
            this.picComercial.Name = "picComercial";
            this.picComercial.Size = new System.Drawing.Size(372, 787);
            this.picComercial.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picComercial.TabIndex = 4;
            this.picComercial.TabStop = false;
            this.picComercial.Click += new System.EventHandler(this.picComercial_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(1890, 860);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.PanelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hub";
            this.PanelHeader.ResumeLayout(false);
            this.PanelHeader.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTriagem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFinanceiro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picComercial)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel PanelHeader;
        private System.Windows.Forms.Label lblGreeting;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picTriagem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.PictureBox picFinanceiro;
        private System.Windows.Forms.PictureBox picRH;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.PictureBox picExp;
        private System.Windows.Forms.PictureBox picComercial;
        private System.Windows.Forms.Button btnConfiguracao;
    }
}