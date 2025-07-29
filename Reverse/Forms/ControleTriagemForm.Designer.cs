namespace Reverse
{
    partial class ControleTriagemForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControleTriagemForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblGreeting = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.dgvNotas = new System.Windows.Forms.DataGridView();
            this.Nf = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Em = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CdB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SKU = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.chkFiltroEmissao = new System.Windows.Forms.CheckBox();
            this.dtpFiltroEmissao = new System.Windows.Forms.DateTimePicker();
            this.btnLimparFiltro = new System.Windows.Forms.Button();
            this.btnImportXml = new System.Windows.Forms.Button();
            this.btnFiltrar = new System.Windows.Forms.Button();
            this.txtFiltroDescricao = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFiltroCodBarras = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlResultados = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnStartTriagem = new System.Windows.Forms.Button();
            this.dgvFiltrados = new System.Windows.Forms.DataGridView();
            this.NF2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DT2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CB2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SKU2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Desc2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Valor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotas)).BeginInit();
            this.pnlFiltros.SuspendLayout();
            this.pnlResultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiltrados)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(128)))));
            this.panel1.Controls.Add(this.btnBack);
            this.panel1.Controls.Add(this.lblGreeting);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1304, 60);
            this.panel1.TabIndex = 0;
            // 
            // btnBack
            // 
            this.btnBack.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnBack.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(1240, 0);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(64, 60);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "← Voltar";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // lblGreeting
            // 
            this.lblGreeting.AutoSize = true;
            this.lblGreeting.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblGreeting.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGreeting.ForeColor = System.Drawing.Color.White;
            this.lblGreeting.Location = new System.Drawing.Point(0, 0);
            this.lblGreeting.Name = "lblGreeting";
            this.lblGreeting.Size = new System.Drawing.Size(126, 24);
            this.lblGreeting.TabIndex = 0;
            this.lblGreeting.Text = "Texto Label";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tlpMain);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1304, 601);
            this.panel2.TabIndex = 1;
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 3;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31F));
            this.tlpMain.Controls.Add(this.dgvNotas, 0, 0);
            this.tlpMain.Controls.Add(this.pnlFiltros, 1, 0);
            this.tlpMain.Controls.Add(this.pnlResultados, 2, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(1304, 601);
            this.tlpMain.TabIndex = 3;
            // 
            // dgvNotas
            // 
            this.dgvNotas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNotas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nf,
            this.Em,
            this.CdB,
            this.SKU,
            this.Desc,
            this.VL});
            this.dgvNotas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvNotas.Location = new System.Drawing.Point(3, 3);
            this.dgvNotas.Name = "dgvNotas";
            this.dgvNotas.ReadOnly = true;
            this.dgvNotas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNotas.Size = new System.Drawing.Size(646, 595);
            this.dgvNotas.TabIndex = 2;
            this.dgvNotas.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNotas_CellContentClick);
            // 
            // Nf
            // 
            this.Nf.HeaderText = "NumeroNF";
            this.Nf.Name = "Nf";
            this.Nf.ReadOnly = true;
            // 
            // Em
            // 
            this.Em.HeaderText = "DataEmissao";
            this.Em.Name = "Em";
            this.Em.ReadOnly = true;
            this.Em.Width = 130;
            // 
            // CdB
            // 
            this.CdB.HeaderText = "CodigoBarras";
            this.CdB.Name = "CdB";
            this.CdB.ReadOnly = true;
            // 
            // SKU
            // 
            this.SKU.HeaderText = "SKU";
            this.SKU.Name = "SKU";
            this.SKU.ReadOnly = true;
            // 
            // Desc
            // 
            this.Desc.HeaderText = "Descrição";
            this.Desc.Name = "Desc";
            this.Desc.ReadOnly = true;
            // 
            // VL
            // 
            this.VL.HeaderText = "Valor";
            this.VL.Name = "VL";
            this.VL.ReadOnly = true;
            // 
            // pnlFiltros
            // 
            this.pnlFiltros.Controls.Add(this.chkFiltroEmissao);
            this.pnlFiltros.Controls.Add(this.dtpFiltroEmissao);
            this.pnlFiltros.Controls.Add(this.btnLimparFiltro);
            this.pnlFiltros.Controls.Add(this.btnImportXml);
            this.pnlFiltros.Controls.Add(this.btnFiltrar);
            this.pnlFiltros.Controls.Add(this.txtFiltroDescricao);
            this.pnlFiltros.Controls.Add(this.label3);
            this.pnlFiltros.Controls.Add(this.label2);
            this.pnlFiltros.Controls.Add(this.txtFiltroCodBarras);
            this.pnlFiltros.Controls.Add(this.label1);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFiltros.Location = new System.Drawing.Point(655, 3);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Padding = new System.Windows.Forms.Padding(10);
            this.pnlFiltros.Size = new System.Drawing.Size(241, 595);
            this.pnlFiltros.TabIndex = 3;
            // 
            // chkFiltroEmissao
            // 
            this.chkFiltroEmissao.AutoSize = true;
            this.chkFiltroEmissao.Location = new System.Drawing.Point(126, 115);
            this.chkFiltroEmissao.Name = "chkFiltroEmissao";
            this.chkFiltroEmissao.Size = new System.Drawing.Size(111, 17);
            this.chkFiltroEmissao.TabIndex = 9;
            this.chkFiltroEmissao.Text = "Filtrar por Emissão";
            this.chkFiltroEmissao.UseVisualStyleBackColor = true;
            // 
            // dtpFiltroEmissao
            // 
            this.dtpFiltroEmissao.CustomFormat = "dd/MM/yyyy";
            this.dtpFiltroEmissao.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFiltroEmissao.Location = new System.Drawing.Point(13, 113);
            this.dtpFiltroEmissao.Name = "dtpFiltroEmissao";
            this.dtpFiltroEmissao.Size = new System.Drawing.Size(106, 20);
            this.dtpFiltroEmissao.TabIndex = 8;
            this.dtpFiltroEmissao.ValueChanged += new System.EventHandler(this.dtpFiltroEmissao_ValueChanged);
            // 
            // btnLimparFiltro
            // 
            this.btnLimparFiltro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLimparFiltro.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimparFiltro.Location = new System.Drawing.Point(19, 318);
            this.btnLimparFiltro.Name = "btnLimparFiltro";
            this.btnLimparFiltro.Size = new System.Drawing.Size(134, 38);
            this.btnLimparFiltro.TabIndex = 7;
            this.btnLimparFiltro.Text = "Limpar Filtro";
            this.btnLimparFiltro.UseVisualStyleBackColor = true;
            this.btnLimparFiltro.Click += new System.EventHandler(this.btnLimparFiltro_Click);
            // 
            // btnImportXml
            // 
            this.btnImportXml.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportXml.Location = new System.Drawing.Point(13, 554);
            this.btnImportXml.Name = "btnImportXml";
            this.btnImportXml.Size = new System.Drawing.Size(180, 32);
            this.btnImportXml.TabIndex = 0;
            this.btnImportXml.Text = "Importar XML";
            this.btnImportXml.UseVisualStyleBackColor = true;
            this.btnImportXml.Click += new System.EventHandler(this.btnImportXml_Click);
            // 
            // btnFiltrar
            // 
            this.btnFiltrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFiltrar.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnFiltrar.Location = new System.Drawing.Point(19, 258);
            this.btnFiltrar.Name = "btnFiltrar";
            this.btnFiltrar.Size = new System.Drawing.Size(134, 38);
            this.btnFiltrar.TabIndex = 6;
            this.btnFiltrar.Text = "Filtrar";
            this.btnFiltrar.UseVisualStyleBackColor = true;
            this.btnFiltrar.Click += new System.EventHandler(this.btnFiltrar_Click);
            // 
            // txtFiltroDescricao
            // 
            this.txtFiltroDescricao.Location = new System.Drawing.Point(13, 177);
            this.txtFiltroDescricao.Name = "txtFiltroDescricao";
            this.txtFiltroDescricao.Size = new System.Drawing.Size(180, 20);
            this.txtFiltroDescricao.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label3.Location = new System.Drawing.Point(9, 153);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 21);
            this.label3.TabIndex = 4;
            this.label3.Text = "Descrição:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label2.Location = new System.Drawing.Point(13, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "Data Emissão:";
            // 
            // txtFiltroCodBarras
            // 
            this.txtFiltroCodBarras.Location = new System.Drawing.Point(13, 50);
            this.txtFiltroCodBarras.Name = "txtFiltroCodBarras";
            this.txtFiltroCodBarras.Size = new System.Drawing.Size(180, 20);
            this.txtFiltroCodBarras.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label1.Location = new System.Drawing.Point(9, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Código de Barras:";
            // 
            // pnlResultados
            // 
            this.pnlResultados.Controls.Add(this.lblTotal);
            this.pnlResultados.Controls.Add(this.btnStartTriagem);
            this.pnlResultados.Controls.Add(this.dgvFiltrados);
            this.pnlResultados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlResultados.Location = new System.Drawing.Point(902, 3);
            this.pnlResultados.Name = "pnlResultados";
            this.pnlResultados.Padding = new System.Windows.Forms.Padding(10);
            this.pnlResultados.Size = new System.Drawing.Size(399, 595);
            this.pnlResultados.TabIndex = 4;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(10, 564);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(110, 21);
            this.lblTotal.TabIndex = 1;
            this.lblTotal.Text = "Total: R$ 0,00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnStartTriagem
            // 
            this.btnStartTriagem.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartTriagem.Location = new System.Drawing.Point(206, 554);
            this.btnStartTriagem.Name = "btnStartTriagem";
            this.btnStartTriagem.Size = new System.Drawing.Size(180, 32);
            this.btnStartTriagem.TabIndex = 1;
            this.btnStartTriagem.Text = "Iniciar Triagem";
            this.btnStartTriagem.UseVisualStyleBackColor = true;
            this.btnStartTriagem.Click += new System.EventHandler(this.btnStartTriagem_Click);
            // 
            // dgvFiltrados
            // 
            this.dgvFiltrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFiltrados.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NF2,
            this.DT2,
            this.CB2,
            this.SKU2,
            this.Desc2,
            this.Valor});
            this.dgvFiltrados.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvFiltrados.Location = new System.Drawing.Point(10, 10);
            this.dgvFiltrados.Name = "dgvFiltrados";
            this.dgvFiltrados.Size = new System.Drawing.Size(379, 461);
            this.dgvFiltrados.TabIndex = 0;
            // 
            // NF2
            // 
            this.NF2.HeaderText = "NumeroNF";
            this.NF2.Name = "NF2";
            // 
            // DT2
            // 
            this.DT2.HeaderText = "DataEmissao";
            this.DT2.Name = "DT2";
            // 
            // CB2
            // 
            this.CB2.HeaderText = "CodigoBarras";
            this.CB2.Name = "CB2";
            // 
            // SKU2
            // 
            this.SKU2.HeaderText = "SKU\n\n";
            this.SKU2.Name = "SKU2";
            // 
            // Desc2
            // 
            this.Desc2.HeaderText = "Descrição";
            this.Desc2.Name = "Desc2";
            // 
            // Valor
            // 
            this.Valor.HeaderText = "Valor";
            this.Valor.Name = "Valor";
            // 
            // ControleTriagemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1304, 661);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ControleTriagemForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Controle de Triagem";
            this.Load += new System.EventHandler(this.ControleTriagemForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tlpMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotas)).EndInit();
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.pnlResultados.ResumeLayout(false);
            this.pnlResultados.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiltrados)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblGreeting;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnImportXml;
        private System.Windows.Forms.Button btnStartTriagem;
        private System.Windows.Forms.DataGridView dgvNotas;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.TextBox txtFiltroCodBarras;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFiltrar;
        private System.Windows.Forms.TextBox txtFiltroDescricao;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLimparFiltro;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nf;
        private System.Windows.Forms.DataGridViewTextBoxColumn Em;
        private System.Windows.Forms.DataGridViewTextBoxColumn CdB;
        private System.Windows.Forms.DataGridViewTextBoxColumn SKU;
        private System.Windows.Forms.DataGridViewTextBoxColumn Desc;
        private System.Windows.Forms.DataGridViewTextBoxColumn VL;
        private System.Windows.Forms.Panel pnlResultados;
        private System.Windows.Forms.DataGridView dgvFiltrados;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.DateTimePicker dtpFiltroEmissao;
        private System.Windows.Forms.DataGridViewTextBoxColumn NF2;
        private System.Windows.Forms.DataGridViewTextBoxColumn DT2;
        private System.Windows.Forms.DataGridViewTextBoxColumn CB2;
        private System.Windows.Forms.DataGridViewTextBoxColumn SKU2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Desc2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Valor;
        private System.Windows.Forms.CheckBox chkFiltroEmissao;
    }
}