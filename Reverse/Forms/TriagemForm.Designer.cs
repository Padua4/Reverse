namespace Reverse
{
    partial class TriagemForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriagemForm));
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblFormTitle = new System.Windows.Forms.Label();
            this.cmbSelecionarPalete = new System.Windows.Forms.ComboBox();
            this.btnCriarPalete = new System.Windows.Forms.Button();
            this.panelCommand = new System.Windows.Forms.Panel();
            this.btnNovoItem = new System.Windows.Forms.Button();
            this.btnExportarPDF = new System.Windows.Forms.Button();
            this.btnEditarItem = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.txtBusca = new System.Windows.Forms.TextBox();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.flpLegend = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.dgvProdutos = new System.Windows.Forms.DataGridView();
            this.SKU = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodigoBarras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descricao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Valor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Flag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvisoVencimento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flpPaleteActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAdicionarItem = new System.Windows.Forms.Button();
            this.btnRemoverItem = new System.Windows.Forms.Button();
            this.btnAtualizarItem = new System.Windows.Forms.Button();
            this.dgvItensPalete = new System.Windows.Forms.DataGridView();
            this.SKUP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodigoBarrasP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QuantidadeP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValorP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FlagP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblPaleteAtual = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            this.panelCommand.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).BeginInit();
            this.flpPaleteActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItensPalete)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(128)))));
            this.panelHeader.Controls.Add(this.lblFormTitle);
            this.panelHeader.Controls.Add(this.cmbSelecionarPalete);
            this.panelHeader.Controls.Add(this.btnCriarPalete);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1904, 80);
            this.panelHeader.TabIndex = 0;
            // 
            // lblFormTitle
            // 
            this.lblFormTitle.AutoSize = true;
            this.lblFormTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblFormTitle.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFormTitle.ForeColor = System.Drawing.Color.White;
            this.lblFormTitle.Location = new System.Drawing.Point(0, 0);
            this.lblFormTitle.Name = "lblFormTitle";
            this.lblFormTitle.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.lblFormTitle.Size = new System.Drawing.Size(269, 32);
            this.lblFormTitle.TabIndex = 0;
            this.lblFormTitle.Text = "Triagem e Paletes";
            // 
            // cmbSelecionarPalete
            // 
            this.cmbSelecionarPalete.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelecionarPalete.FormattingEnabled = true;
            this.cmbSelecionarPalete.Location = new System.Drawing.Point(137, 35);
            this.cmbSelecionarPalete.Margin = new System.Windows.Forms.Padding(6, 15, 15, 15);
            this.cmbSelecionarPalete.Name = "cmbSelecionarPalete";
            this.cmbSelecionarPalete.Size = new System.Drawing.Size(200, 21);
            this.cmbSelecionarPalete.TabIndex = 2;
            // 
            // btnCriarPalete
            // 
            this.btnCriarPalete.Location = new System.Drawing.Point(28, 35);
            this.btnCriarPalete.Name = "btnCriarPalete";
            this.btnCriarPalete.Size = new System.Drawing.Size(100, 40);
            this.btnCriarPalete.TabIndex = 1;
            this.btnCriarPalete.Text = "Criar Palete";
            this.btnCriarPalete.UseVisualStyleBackColor = true;
            // 
            // panelCommand
            // 
            this.panelCommand.Controls.Add(this.btnNovoItem);
            this.panelCommand.Controls.Add(this.btnExportarPDF);
            this.panelCommand.Controls.Add(this.btnEditarItem);
            this.panelCommand.Controls.Add(this.label1);
            this.panelCommand.Controls.Add(this.btnBuscar);
            this.panelCommand.Controls.Add(this.txtBusca);
            this.panelCommand.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCommand.Location = new System.Drawing.Point(0, 80);
            this.panelCommand.Name = "panelCommand";
            this.panelCommand.Size = new System.Drawing.Size(1904, 60);
            this.panelCommand.TabIndex = 1;
            // 
            // btnNovoItem
            // 
            this.btnNovoItem.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNovoItem.Location = new System.Drawing.Point(648, 11);
            this.btnNovoItem.Name = "btnNovoItem";
            this.btnNovoItem.Size = new System.Drawing.Size(250, 36);
            this.btnNovoItem.TabIndex = 3;
            this.btnNovoItem.Text = "Cadastrar Produto";
            this.btnNovoItem.UseVisualStyleBackColor = true;
            // 
            // btnExportarPDF
            // 
            this.btnExportarPDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportarPDF.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportarPDF.Location = new System.Drawing.Point(1642, 11);
            this.btnExportarPDF.Name = "btnExportarPDF";
            this.btnExportarPDF.Size = new System.Drawing.Size(250, 36);
            this.btnExportarPDF.TabIndex = 5;
            this.btnExportarPDF.Text = "Exportar PDF";
            this.btnExportarPDF.UseVisualStyleBackColor = true;
            // 
            // btnEditarItem
            // 
            this.btnEditarItem.Font = new System.Drawing.Font("Arial", 9.75F);
            this.btnEditarItem.Location = new System.Drawing.Point(975, 10);
            this.btnEditarItem.Name = "btnEditarItem";
            this.btnEditarItem.Size = new System.Drawing.Size(250, 36);
            this.btnEditarItem.TabIndex = 4;
            this.btnEditarItem.Text = "Editar Produto";
            this.btnEditarItem.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(70, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Buscar:";
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(28, 18);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(36, 36);
            this.btnBuscar.TabIndex = 1;
            this.btnBuscar.Text = "🔍";
            this.btnBuscar.UseVisualStyleBackColor = true;
            // 
            // txtBusca
            // 
            this.txtBusca.Location = new System.Drawing.Point(130, 27);
            this.txtBusca.Name = "txtBusca";
            this.txtBusca.Size = new System.Drawing.Size(400, 20);
            this.txtBusca.TabIndex = 0;
            // 
            // panelFooter
            // 
            this.panelFooter.Controls.Add(this.flpLegend);
            this.panelFooter.Controls.Add(this.toolStripContainer1);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 901);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.panelFooter.Size = new System.Drawing.Size(1904, 140);
            this.panelFooter.TabIndex = 2;
            // 
            // flpLegend
            // 
            this.flpLegend.AutoSize = true;
            this.flpLegend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpLegend.Location = new System.Drawing.Point(20, 10);
            this.flpLegend.Name = "flpLegend";
            this.flpLegend.Size = new System.Drawing.Size(1864, 120);
            this.flpLegend.TabIndex = 2;
            this.flpLegend.WrapContents = false;
            this.flpLegend.Paint += new System.Windows.Forms.PaintEventHandler(this.flpLegend_Paint);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(150, 150);
            this.toolStripContainer1.Location = new System.Drawing.Point(167, 134);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(150, 175);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 140);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.dgvProdutos);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.flpPaleteActions);
            this.splitContainerMain.Panel2.Controls.Add(this.dgvItensPalete);
            this.splitContainerMain.Panel2.Controls.Add(this.lblPaleteAtual);
            this.splitContainerMain.Size = new System.Drawing.Size(1904, 761);
            this.splitContainerMain.SplitterDistance = 634;
            this.splitContainerMain.TabIndex = 3;
            // 
            // dgvProdutos
            // 
            this.dgvProdutos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProdutos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SKU,
            this.CodigoBarras,
            this.Descricao,
            this.Valor,
            this.Flag,
            this.AvisoVencimento});
            this.dgvProdutos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProdutos.Location = new System.Drawing.Point(0, 0);
            this.dgvProdutos.Name = "dgvProdutos";
            this.dgvProdutos.Size = new System.Drawing.Size(634, 761);
            this.dgvProdutos.TabIndex = 0;
            // 
            // SKU
            // 
            this.SKU.HeaderText = "SKU";
            this.SKU.Name = "SKU";
            // 
            // CodigoBarras
            // 
            this.CodigoBarras.HeaderText = "CodigoBarras";
            this.CodigoBarras.Name = "CodigoBarras";
            // 
            // Descricao
            // 
            this.Descricao.HeaderText = "Descricao";
            this.Descricao.Name = "Descricao";
            // 
            // Valor
            // 
            this.Valor.HeaderText = "Valor";
            this.Valor.Name = "Valor";
            // 
            // Flag
            // 
            this.Flag.HeaderText = "Flag";
            this.Flag.Name = "Flag";
            // 
            // AvisoVencimento
            // 
            this.AvisoVencimento.HeaderText = "AvisoVencimento";
            this.AvisoVencimento.Name = "AvisoVencimento";
            // 
            // flpPaleteActions
            // 
            this.flpPaleteActions.Controls.Add(this.btnAdicionarItem);
            this.flpPaleteActions.Controls.Add(this.btnRemoverItem);
            this.flpPaleteActions.Controls.Add(this.btnAtualizarItem);
            this.flpPaleteActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flpPaleteActions.Location = new System.Drawing.Point(0, 711);
            this.flpPaleteActions.Name = "flpPaleteActions";
            this.flpPaleteActions.Size = new System.Drawing.Size(1266, 50);
            this.flpPaleteActions.TabIndex = 2;
            this.flpPaleteActions.WrapContents = false;
            // 
            // btnAdicionarItem
            // 
            this.btnAdicionarItem.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdicionarItem.Location = new System.Drawing.Point(3, 3);
            this.btnAdicionarItem.Name = "btnAdicionarItem";
            this.btnAdicionarItem.Size = new System.Drawing.Size(200, 36);
            this.btnAdicionarItem.TabIndex = 0;
            this.btnAdicionarItem.Text = "Adicionar →";
            this.btnAdicionarItem.UseVisualStyleBackColor = true;
            // 
            // btnRemoverItem
            // 
            this.btnRemoverItem.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoverItem.Location = new System.Drawing.Point(209, 3);
            this.btnRemoverItem.Name = "btnRemoverItem";
            this.btnRemoverItem.Size = new System.Drawing.Size(200, 36);
            this.btnRemoverItem.TabIndex = 1;
            this.btnRemoverItem.Text = "← Remover";
            this.btnRemoverItem.UseVisualStyleBackColor = true;
            // 
            // btnAtualizarItem
            // 
            this.btnAtualizarItem.AutoSize = true;
            this.btnAtualizarItem.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAtualizarItem.Location = new System.Drawing.Point(415, 3);
            this.btnAtualizarItem.Name = "btnAtualizarItem";
            this.btnAtualizarItem.Size = new System.Drawing.Size(200, 36);
            this.btnAtualizarItem.TabIndex = 2;
            this.btnAtualizarItem.Text = "Atualizar Qtde/Valor";
            this.btnAtualizarItem.UseVisualStyleBackColor = true;
            // 
            // dgvItensPalete
            // 
            this.dgvItensPalete.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItensPalete.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SKUP,
            this.CodigoBarrasP,
            this.QuantidadeP,
            this.ValorP,
            this.FlagP});
            this.dgvItensPalete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItensPalete.Location = new System.Drawing.Point(0, 42);
            this.dgvItensPalete.Name = "dgvItensPalete";
            this.dgvItensPalete.Size = new System.Drawing.Size(1266, 719);
            this.dgvItensPalete.TabIndex = 1;
            // 
            // SKUP
            // 
            this.SKUP.HeaderText = "SKU";
            this.SKUP.Name = "SKUP";
            // 
            // CodigoBarrasP
            // 
            this.CodigoBarrasP.HeaderText = "CodigoBarras";
            this.CodigoBarrasP.Name = "CodigoBarrasP";
            // 
            // QuantidadeP
            // 
            this.QuantidadeP.HeaderText = "Quantidade";
            this.QuantidadeP.Name = "QuantidadeP";
            // 
            // ValorP
            // 
            this.ValorP.HeaderText = "Valor";
            this.ValorP.Name = "ValorP";
            // 
            // FlagP
            // 
            this.FlagP.HeaderText = "Flag";
            this.FlagP.Name = "FlagP";
            // 
            // lblPaleteAtual
            // 
            this.lblPaleteAtual.AutoSize = true;
            this.lblPaleteAtual.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPaleteAtual.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPaleteAtual.Location = new System.Drawing.Point(0, 0);
            this.lblPaleteAtual.Name = "lblPaleteAtual";
            this.lblPaleteAtual.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.lblPaleteAtual.Size = new System.Drawing.Size(91, 42);
            this.lblPaleteAtual.TabIndex = 0;
            this.lblPaleteAtual.Text = "Palete —";
            // 
            // TriagemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelCommand);
            this.Controls.Add(this.panelHeader);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TriagemForm";
            this.Text = "Triagem";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.TriagemForm_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelCommand.ResumeLayout(false);
            this.panelCommand.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            this.panelFooter.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).EndInit();
            this.flpPaleteActions.ResumeLayout(false);
            this.flpPaleteActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItensPalete)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelCommand;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Label lblFormTitle;
        private System.Windows.Forms.ComboBox cmbSelecionarPalete;
        private System.Windows.Forms.Button btnCriarPalete;
        private System.Windows.Forms.TextBox txtBusca;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnNovoItem;
        private System.Windows.Forms.Button btnExportarPDF;
        private System.Windows.Forms.Button btnEditarItem;
        private System.Windows.Forms.DataGridView dgvProdutos;
        private System.Windows.Forms.DataGridViewTextBoxColumn SKU;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodigoBarras;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descricao;
        private System.Windows.Forms.DataGridViewTextBoxColumn Valor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Flag;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvisoVencimento;
        private System.Windows.Forms.DataGridView dgvItensPalete;
        private System.Windows.Forms.DataGridViewTextBoxColumn SKUP;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodigoBarrasP;
        private System.Windows.Forms.DataGridViewTextBoxColumn QuantidadeP;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValorP;
        private System.Windows.Forms.DataGridViewTextBoxColumn FlagP;
        private System.Windows.Forms.Label lblPaleteAtual;
        private System.Windows.Forms.FlowLayoutPanel flpPaleteActions;
        private System.Windows.Forms.Button btnAdicionarItem;
        private System.Windows.Forms.Button btnRemoverItem;
        private System.Windows.Forms.Button btnAtualizarItem;
        private System.Windows.Forms.FlowLayoutPanel flpLegend;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
    }
}