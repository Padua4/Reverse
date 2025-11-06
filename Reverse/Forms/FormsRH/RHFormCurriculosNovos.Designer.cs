namespace Reverse.Forms.FormsRH
{
    partial class FormCurriculosNovos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCurriculosNovos));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSalvar = new System.Windows.Forms.Label();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.lblCat = new System.Windows.Forms.Label();
            this.cbbCat = new System.Windows.Forms.ComboBox();
            this.lblNome = new System.Windows.Forms.Label();
            this.btnAnexar = new System.Windows.Forms.Button();
            this.lblAnexar = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnInapto = new System.Windows.Forms.Button();
            this.btnApto = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgvParticipantes = new System.Windows.Forms.DataGridView();
            this.pnlHeader.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParticipantes)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.pnlHeader.Controls.Add(this.lblInfo);
            this.pnlHeader.Controls.Add(this.label1);
            this.pnlHeader.Controls.Add(this.lblSalvar);
            this.pnlHeader.Controls.Add(this.btnSalvar);
            this.pnlHeader.Controls.Add(this.txtNome);
            this.pnlHeader.Controls.Add(this.lblCat);
            this.pnlHeader.Controls.Add(this.cbbCat);
            this.pnlHeader.Controls.Add(this.lblNome);
            this.pnlHeader.Controls.Add(this.btnAnexar);
            this.pnlHeader.Controls.Add(this.lblAnexar);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1920, 56);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(1311, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(243, 25);
            this.lblInfo.TabIndex = 9;
            this.lblInfo.Text = "Total de candidatos novos";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(235, 37);
            this.label1.TabIndex = 8;
            this.label1.Text = "Curriculos Novos";
            // 
            // lblSalvar
            // 
            this.lblSalvar.Location = new System.Drawing.Point(1081, 0);
            this.lblSalvar.Name = "lblSalvar";
            this.lblSalvar.Size = new System.Drawing.Size(164, 25);
            this.lblSalvar.TabIndex = 7;
            this.lblSalvar.Text = "Salvar";
            this.lblSalvar.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnSalvar
            // 
            this.btnSalvar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalvar.ForeColor = System.Drawing.Color.Black;
            this.btnSalvar.Location = new System.Drawing.Point(1081, 28);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(159, 28);
            this.btnSalvar.TabIndex = 6;
            this.btnSalvar.Text = "Ok";
            this.btnSalvar.UseVisualStyleBackColor = true;
            // 
            // txtNome
            // 
            this.txtNome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtNome.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNome.Location = new System.Drawing.Point(288, 30);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(371, 26);
            this.txtNome.TabIndex = 1;
            // 
            // lblCat
            // 
            this.lblCat.Location = new System.Drawing.Point(665, 0);
            this.lblCat.Name = "lblCat";
            this.lblCat.Size = new System.Drawing.Size(240, 25);
            this.lblCat.TabIndex = 4;
            this.lblCat.Text = "Categoria";
            this.lblCat.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cbbCat
            // 
            this.cbbCat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbCat.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbCat.FormattingEnabled = true;
            this.cbbCat.Location = new System.Drawing.Point(670, 30);
            this.cbbCat.Name = "cbbCat";
            this.cbbCat.Size = new System.Drawing.Size(235, 28);
            this.cbbCat.TabIndex = 5;
            // 
            // lblNome
            // 
            this.lblNome.Location = new System.Drawing.Point(283, 0);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(376, 25);
            this.lblNome.TabIndex = 0;
            this.lblNome.Text = "Nome do candidato";
            this.lblNome.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnAnexar
            // 
            this.btnAnexar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnexar.ForeColor = System.Drawing.Color.Black;
            this.btnAnexar.Location = new System.Drawing.Point(916, 28);
            this.btnAnexar.Name = "btnAnexar";
            this.btnAnexar.Size = new System.Drawing.Size(159, 28);
            this.btnAnexar.TabIndex = 2;
            this.btnAnexar.Text = "ANEXAR";
            this.btnAnexar.UseVisualStyleBackColor = true;
            // 
            // lblAnexar
            // 
            this.lblAnexar.Location = new System.Drawing.Point(911, 0);
            this.lblAnexar.Name = "lblAnexar";
            this.lblAnexar.Size = new System.Drawing.Size(164, 25);
            this.lblAnexar.TabIndex = 3;
            this.lblAnexar.Text = "Curriculo";
            this.lblAnexar.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.panel1.Controls.Add(this.btnInapto);
            this.panel1.Controls.Add(this.btnApto);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 891);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1920, 75);
            this.panel1.TabIndex = 1;
            // 
            // btnInapto
            // 
            this.btnInapto.BackColor = System.Drawing.Color.Red;
            this.btnInapto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInapto.Location = new System.Drawing.Point(1152, 6);
            this.btnInapto.Name = "btnInapto";
            this.btnInapto.Size = new System.Drawing.Size(375, 64);
            this.btnInapto.TabIndex = 1;
            this.btnInapto.Text = "Inapto";
            this.btnInapto.UseVisualStyleBackColor = false;
            // 
            // btnApto
            // 
            this.btnApto.BackColor = System.Drawing.Color.Green;
            this.btnApto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApto.Location = new System.Drawing.Point(1542, 6);
            this.btnApto.Name = "btnApto";
            this.btnApto.Size = new System.Drawing.Size(375, 64);
            this.btnApto.TabIndex = 0;
            this.btnApto.Text = "Apto";
            this.btnApto.UseVisualStyleBackColor = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvParticipantes);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 56);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1920, 835);
            this.panel2.TabIndex = 2;
            // 
            // dgvParticipantes
            // 
            this.dgvParticipantes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.dgvParticipantes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParticipantes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvParticipantes.Location = new System.Drawing.Point(0, 0);
            this.dgvParticipantes.Name = "dgvParticipantes";
            this.dgvParticipantes.ReadOnly = true;
            this.dgvParticipantes.Size = new System.Drawing.Size(1920, 835);
            this.dgvParticipantes.TabIndex = 0;
            // 
            // FormCurriculosNovos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(1920, 966);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "FormCurriculosNovos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormCurriculosNovos";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParticipantes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnAnexar;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.ComboBox cbbCat;
        private System.Windows.Forms.Label lblCat;
        private System.Windows.Forms.Label lblAnexar;
        private System.Windows.Forms.Label lblSalvar;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgvParticipantes;
        private System.Windows.Forms.Button btnInapto;
        private System.Windows.Forms.Button btnApto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblInfo;
    }
}