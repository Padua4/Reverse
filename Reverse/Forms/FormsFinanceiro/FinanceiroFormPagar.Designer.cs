namespace Reverse.Forms.FormsFinanceiro
{
    partial class FormPagar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPagar));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblLoteAtual = new System.Windows.Forms.Label();
            this.btnContasSelecionar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvContasPagar = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPago = new System.Windows.Forms.Button();
            this.btnExcluir = new System.Windows.Forms.Button();
            this.btnCriar = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContasPagar)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.pnlHeader.Controls.Add(this.lblLoteAtual);
            this.pnlHeader.Controls.Add(this.btnContasSelecionar);
            this.pnlHeader.Controls.Add(this.label1);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1870, 40);
            this.pnlHeader.TabIndex = 2;
            // 
            // lblLoteAtual
            // 
            this.lblLoteAtual.AutoSize = true;
            this.lblLoteAtual.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoteAtual.ForeColor = System.Drawing.Color.White;
            this.lblLoteAtual.Location = new System.Drawing.Point(461, 5);
            this.lblLoteAtual.Name = "lblLoteAtual";
            this.lblLoteAtual.Size = new System.Drawing.Size(211, 32);
            this.lblLoteAtual.TabIndex = 3;
            this.lblLoteAtual.Text = "Lote: 00/00/0000";
            // 
            // btnContasSelecionar
            // 
            this.btnContasSelecionar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnContasSelecionar.ForeColor = System.Drawing.Color.White;
            this.btnContasSelecionar.Location = new System.Drawing.Point(247, 5);
            this.btnContasSelecionar.Name = "btnContasSelecionar";
            this.btnContasSelecionar.Size = new System.Drawing.Size(208, 32);
            this.btnContasSelecionar.TabIndex = 2;
            this.btnContasSelecionar.Text = "Selecionar Dia";
            this.btnContasSelecionar.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Contas a Pagar";
            // 
            // dgvContasPagar
            // 
            this.dgvContasPagar.AllowUserToAddRows = false;
            this.dgvContasPagar.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(244)))));
            this.dgvContasPagar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvContasPagar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvContasPagar.Location = new System.Drawing.Point(0, 40);
            this.dgvContasPagar.Name = "dgvContasPagar";
            this.dgvContasPagar.Size = new System.Drawing.Size(1870, 960);
            this.dgvContasPagar.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 960);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1870, 40);
            this.panel1.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.btnPago, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnExcluir, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCriar, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1005, 40);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnPago
            // 
            this.btnPago.BackColor = System.Drawing.Color.Green;
            this.btnPago.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPago.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPago.ForeColor = System.Drawing.Color.White;
            this.btnPago.Location = new System.Drawing.Point(673, 3);
            this.btnPago.Name = "btnPago";
            this.btnPago.Size = new System.Drawing.Size(329, 34);
            this.btnPago.TabIndex = 4;
            this.btnPago.Text = "PAGO";
            this.btnPago.UseVisualStyleBackColor = false;
            // 
            // btnExcluir
            // 
            this.btnExcluir.BackColor = System.Drawing.Color.Red;
            this.btnExcluir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExcluir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluir.ForeColor = System.Drawing.Color.White;
            this.btnExcluir.Location = new System.Drawing.Point(338, 3);
            this.btnExcluir.Name = "btnExcluir";
            this.btnExcluir.Size = new System.Drawing.Size(329, 34);
            this.btnExcluir.TabIndex = 3;
            this.btnExcluir.Text = "Excluir Linha";
            this.btnExcluir.UseVisualStyleBackColor = false;
            // 
            // btnCriar
            // 
            this.btnCriar.BackColor = System.Drawing.Color.White;
            this.btnCriar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCriar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCriar.ForeColor = System.Drawing.Color.Black;
            this.btnCriar.Location = new System.Drawing.Point(3, 3);
            this.btnCriar.Name = "btnCriar";
            this.btnCriar.Size = new System.Drawing.Size(329, 34);
            this.btnCriar.TabIndex = 2;
            this.btnCriar.Text = "Nova Linha";
            this.btnCriar.UseVisualStyleBackColor = false;
            // 
            // FormPagar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(1870, 1000);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvContasPagar);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "FormPagar";
            this.Text = "FormPagar";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContasPagar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnContasSelecionar;
        private System.Windows.Forms.DataGridView dgvContasPagar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCriar;
        private System.Windows.Forms.Button btnPago;
        private System.Windows.Forms.Button btnExcluir;
        private System.Windows.Forms.Label lblLoteAtual;
    }
}