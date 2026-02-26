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
            this.picNotificador = new System.Windows.Forms.PictureBox();
            this.btnConfiguracao = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblGreeting = new System.Windows.Forms.Label();
            this.picTriagem = new System.Windows.Forms.PictureBox();
            this.picFinanceiro = new System.Windows.Forms.PictureBox();
            this.picRH = new System.Windows.Forms.PictureBox();
            this.picExp = new System.Windows.Forms.PictureBox();
            this.picComercial = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.picAtendimento = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.picFiscal = new System.Windows.Forms.PictureBox();
            this.picProducao = new System.Windows.Forms.PictureBox();
            this.btnMensagem = new System.Windows.Forms.Button();
            this.PanelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNotificador)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTriagem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFinanceiro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picComercial)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAtendimento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFiscal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picProducao)).BeginInit();
            this.SuspendLayout();
            // 
            // PanelHeader
            // 
            this.PanelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.PanelHeader.Controls.Add(this.btnMensagem);
            this.PanelHeader.Controls.Add(this.picNotificador);
            this.PanelHeader.Controls.Add(this.btnConfiguracao);
            this.PanelHeader.Controls.Add(this.btnSair);
            this.PanelHeader.Controls.Add(this.lblGreeting);
            this.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelHeader.Location = new System.Drawing.Point(0, 0);
            this.PanelHeader.Name = "PanelHeader";
            this.PanelHeader.Size = new System.Drawing.Size(1920, 40);
            this.PanelHeader.TabIndex = 1;
            this.PanelHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitulo_MouseDown);
            // 
            // picNotificador
            // 
            this.picNotificador.Dock = System.Windows.Forms.DockStyle.Right;
            this.picNotificador.Image = ((System.Drawing.Image)(resources.GetObject("picNotificador.Image")));
            this.picNotificador.Location = new System.Drawing.Point(1780, 0);
            this.picNotificador.Name = "picNotificador";
            this.picNotificador.Size = new System.Drawing.Size(71, 40);
            this.picNotificador.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picNotificador.TabIndex = 3;
            this.picNotificador.TabStop = false;
            // 
            // btnConfiguracao
            // 
            this.btnConfiguracao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfiguracao.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfiguracao.ForeColor = System.Drawing.Color.White;
            this.btnConfiguracao.Location = new System.Drawing.Point(1626, 3);
            this.btnConfiguracao.Name = "btnConfiguracao";
            this.btnConfiguracao.Size = new System.Drawing.Size(71, 34);
            this.btnConfiguracao.TabIndex = 2;
            this.btnConfiguracao.Text = "C.D.U";
            this.btnConfiguracao.UseVisualStyleBackColor = true;
            this.btnConfiguracao.Click += new System.EventHandler(this.btnConfiguracao_Click);
            // 
            // btnSair
            // 
            this.btnSair.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSair.ForeColor = System.Drawing.Color.White;
            this.btnSair.Location = new System.Drawing.Point(1851, 0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(69, 40);
            this.btnSair.TabIndex = 1;
            this.btnSair.Text = "X";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // lblGreeting
            // 
            this.lblGreeting.AutoSize = true;
            this.lblGreeting.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGreeting.ForeColor = System.Drawing.Color.White;
            this.lblGreeting.Location = new System.Drawing.Point(3, 3);
            this.lblGreeting.Name = "lblGreeting";
            this.lblGreeting.Size = new System.Drawing.Size(240, 32);
            this.lblGreeting.TabIndex = 0;
            this.lblGreeting.Text = "Texto de bem vindo";
            // 
            // picTriagem
            // 
            this.picTriagem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picTriagem.Image = ((System.Drawing.Image)(resources.GetObject("picTriagem.Image")));
            this.picTriagem.Location = new System.Drawing.Point(3, 3);
            this.picTriagem.Name = "picTriagem";
            this.picTriagem.Size = new System.Drawing.Size(314, 474);
            this.picTriagem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picTriagem.TabIndex = 0;
            this.picTriagem.TabStop = false;
            this.picTriagem.Click += new System.EventHandler(this.picTriagem_Click);
            // 
            // picFinanceiro
            // 
            this.picFinanceiro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picFinanceiro.Image = ((System.Drawing.Image)(resources.GetObject("picFinanceiro.Image")));
            this.picFinanceiro.Location = new System.Drawing.Point(963, 3);
            this.picFinanceiro.Name = "picFinanceiro";
            this.picFinanceiro.Size = new System.Drawing.Size(314, 474);
            this.picFinanceiro.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFinanceiro.TabIndex = 0;
            this.picFinanceiro.TabStop = false;
            this.picFinanceiro.Click += new System.EventHandler(this.picFinanceiro_Click);
            // 
            // picRH
            // 
            this.picRH.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picRH.Image = ((System.Drawing.Image)(resources.GetObject("picRH.Image")));
            this.picRH.Location = new System.Drawing.Point(643, 3);
            this.picRH.Name = "picRH";
            this.picRH.Size = new System.Drawing.Size(314, 474);
            this.picRH.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picRH.TabIndex = 0;
            this.picRH.TabStop = false;
            this.picRH.Click += new System.EventHandler(this.picRH_Click);
            // 
            // picExp
            // 
            this.picExp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picExp.Image = ((System.Drawing.Image)(resources.GetObject("picExp.Image")));
            this.picExp.Location = new System.Drawing.Point(323, 3);
            this.picExp.Name = "picExp";
            this.picExp.Size = new System.Drawing.Size(314, 474);
            this.picExp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picExp.TabIndex = 0;
            this.picExp.TabStop = false;
            this.picExp.Click += new System.EventHandler(this.picExp_Click);
            // 
            // picComercial
            // 
            this.picComercial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picComercial.Image = ((System.Drawing.Image)(resources.GetObject("picComercial.Image")));
            this.picComercial.Location = new System.Drawing.Point(1283, 3);
            this.picComercial.Name = "picComercial";
            this.picComercial.Size = new System.Drawing.Size(314, 474);
            this.picComercial.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picComercial.TabIndex = 4;
            this.picComercial.TabStop = false;
            this.picComercial.Click += new System.EventHandler(this.picComercial_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.Controls.Add(this.picAtendimento, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox6, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox5, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox4, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox3, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.picFinanceiro, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.picTriagem, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.picFiscal, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.picComercial, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.picExp, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.picRH, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.picProducao, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 40);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1920, 960);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // picAtendimento
            // 
            this.picAtendimento.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picAtendimento.Image = ((System.Drawing.Image)(resources.GetObject("picAtendimento.Image")));
            this.picAtendimento.Location = new System.Drawing.Point(1603, 483);
            this.picAtendimento.Name = "picAtendimento";
            this.picAtendimento.Size = new System.Drawing.Size(314, 474);
            this.picAtendimento.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picAtendimento.TabIndex = 11;
            this.picAtendimento.TabStop = false;
            this.picAtendimento.Click += new System.EventHandler(this.picAtendimento_Click);
            // 
            // pictureBox6
            // 
            this.pictureBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox6.Image")));
            this.pictureBox6.Location = new System.Drawing.Point(1283, 483);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(314, 474);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 10;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.Location = new System.Drawing.Point(963, 483);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(314, 474);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox5.TabIndex = 9;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(643, 483);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(314, 474);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 8;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(323, 483);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(314, 474);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 7;
            this.pictureBox3.TabStop = false;
            // 
            // picFiscal
            // 
            this.picFiscal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picFiscal.Image = ((System.Drawing.Image)(resources.GetObject("picFiscal.Image")));
            this.picFiscal.Location = new System.Drawing.Point(1603, 3);
            this.picFiscal.Name = "picFiscal";
            this.picFiscal.Size = new System.Drawing.Size(314, 474);
            this.picFiscal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFiscal.TabIndex = 5;
            this.picFiscal.TabStop = false;
            this.picFiscal.Click += new System.EventHandler(this.picFiscal_Click);
            // 
            // picProducao
            // 
            this.picProducao.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picProducao.Image = ((System.Drawing.Image)(resources.GetObject("picProducao.Image")));
            this.picProducao.Location = new System.Drawing.Point(3, 483);
            this.picProducao.Name = "picProducao";
            this.picProducao.Size = new System.Drawing.Size(314, 474);
            this.picProducao.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picProducao.TabIndex = 6;
            this.picProducao.TabStop = false;
            this.picProducao.Click += new System.EventHandler(this.picProducao_Click);
            // 
            // btnMensagem
            // 
            this.btnMensagem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMensagem.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMensagem.ForeColor = System.Drawing.Color.White;
            this.btnMensagem.Location = new System.Drawing.Point(1703, 3);
            this.btnMensagem.Name = "btnMensagem";
            this.btnMensagem.Size = new System.Drawing.Size(71, 34);
            this.btnMensagem.TabIndex = 4;
            this.btnMensagem.Text = "M.S";
            this.btnMensagem.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(1920, 1000);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.PanelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hub";
            this.PanelHeader.ResumeLayout(false);
            this.PanelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNotificador)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTriagem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFinanceiro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picComercial)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAtendimento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFiscal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picProducao)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel PanelHeader;
        private System.Windows.Forms.Label lblGreeting;
        private System.Windows.Forms.PictureBox picTriagem;
        private System.Windows.Forms.PictureBox picFinanceiro;
        private System.Windows.Forms.PictureBox picRH;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.PictureBox picExp;
        private System.Windows.Forms.PictureBox picComercial;
        private System.Windows.Forms.Button btnConfiguracao;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox picFiscal;
        private System.Windows.Forms.PictureBox picProducao;
        private System.Windows.Forms.PictureBox picAtendimento;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox picNotificador;
        private System.Windows.Forms.Button btnMensagem;
    }
}