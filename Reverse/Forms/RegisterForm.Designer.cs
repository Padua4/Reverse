namespace Reverse
{
    partial class RegisterForm
    {
        private void initializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblNewUsuario = new System.Windows.Forms.Label();
            this.txtNewUsuario = new System.Windows.Forms.TextBox();
            this.lblNewSenha = new System.Windows.Forms.Label();
            this.txtNewSenha = new System.Windows.Forms.TextBox();
            this.lblConfirmSenha = new System.Windows.Forms.Label();
            this.txtConfirmSenha = new System.Windows.Forms.TextBox();
            this.lblMasterKey = new System.Windows.Forms.Label();
            this.txtMasterKey = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.SuspendLayout();
        }
        private System.ComponentModel.IContainer components = null;

       
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterForm));
            this.lblNewUsuario = new System.Windows.Forms.Label();
            this.txtNewUsuario = new System.Windows.Forms.TextBox();
            this.lblNewSenha = new System.Windows.Forms.Label();
            this.txtNewSenha = new System.Windows.Forms.TextBox();
            this.lblConfirmSenha = new System.Windows.Forms.Label();
            this.txtConfirmSenha = new System.Windows.Forms.TextBox();
            this.lblMasterKey = new System.Windows.Forms.Label();
            this.txtMasterKey = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblNewUsuario
            // 
            this.lblNewUsuario.AutoSize = true;
            this.lblNewUsuario.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewUsuario.Location = new System.Drawing.Point(117, 61);
            this.lblNewUsuario.Name = "lblNewUsuario";
            this.lblNewUsuario.Size = new System.Drawing.Size(146, 37);
            this.lblNewUsuario.TabIndex = 0;
            this.lblNewUsuario.Text = "Usuário:";
            // 
            // txtNewUsuario
            // 
            this.txtNewUsuario.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNewUsuario.Location = new System.Drawing.Point(65, 116);
            this.txtNewUsuario.Name = "txtNewUsuario";
            this.txtNewUsuario.Size = new System.Drawing.Size(235, 29);
            this.txtNewUsuario.TabIndex = 1;
            // 
            // lblNewSenha
            // 
            this.lblNewSenha.AutoSize = true;
            this.lblNewSenha.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewSenha.Location = new System.Drawing.Point(130, 191);
            this.lblNewSenha.Name = "lblNewSenha";
            this.lblNewSenha.Size = new System.Drawing.Size(122, 37);
            this.lblNewSenha.TabIndex = 2;
            this.lblNewSenha.Text = "Senha:";
            // 
            // txtNewSenha
            // 
            this.txtNewSenha.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNewSenha.Location = new System.Drawing.Point(65, 240);
            this.txtNewSenha.Name = "txtNewSenha";
            this.txtNewSenha.Size = new System.Drawing.Size(235, 29);
            this.txtNewSenha.TabIndex = 3;
            this.txtNewSenha.UseSystemPasswordChar = true;
            // 
            // lblConfirmSenha
            // 
            this.lblConfirmSenha.AutoSize = true;
            this.lblConfirmSenha.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfirmSenha.Location = new System.Drawing.Point(41, 309);
            this.lblConfirmSenha.Name = "lblConfirmSenha";
            this.lblConfirmSenha.Size = new System.Drawing.Size(285, 37);
            this.lblConfirmSenha.TabIndex = 4;
            this.lblConfirmSenha.Text = "Confirmar Senha:";
            // 
            // txtConfirmSenha
            // 
            this.txtConfirmSenha.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfirmSenha.Location = new System.Drawing.Point(65, 377);
            this.txtConfirmSenha.Name = "txtConfirmSenha";
            this.txtConfirmSenha.Size = new System.Drawing.Size(235, 29);
            this.txtConfirmSenha.TabIndex = 5;
            this.txtConfirmSenha.UseSystemPasswordChar = true;
            // 
            // lblMasterKey
            // 
            this.lblMasterKey.AutoSize = true;
            this.lblMasterKey.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMasterKey.Location = new System.Drawing.Point(73, 449);
            this.lblMasterKey.Name = "lblMasterKey";
            this.lblMasterKey.Size = new System.Drawing.Size(238, 37);
            this.lblMasterKey.TabIndex = 6;
            this.lblMasterKey.Text = "Chave-Mestre:\r\n";
            // 
            // txtMasterKey
            // 
            this.txtMasterKey.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMasterKey.Location = new System.Drawing.Point(65, 510);
            this.txtMasterKey.Name = "txtMasterKey";
            this.txtMasterKey.Size = new System.Drawing.Size(235, 29);
            this.txtMasterKey.TabIndex = 7;
            this.txtMasterKey.UseSystemPasswordChar = true;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Location = new System.Drawing.Point(93, 590);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(183, 54);
            this.btnSubmit.TabIndex = 8;
            this.btnSubmit.Text = "Cadastrar";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 688);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.txtMasterKey);
            this.Controls.Add(this.lblMasterKey);
            this.Controls.Add(this.txtConfirmSenha);
            this.Controls.Add(this.lblConfirmSenha);
            this.Controls.Add(this.txtNewSenha);
            this.Controls.Add(this.lblNewSenha);
            this.Controls.Add(this.txtNewUsuario);
            this.Controls.Add(this.lblNewUsuario);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cadastro";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNewUsuario;
        private System.Windows.Forms.TextBox txtNewUsuario;
        private System.Windows.Forms.Label lblNewSenha;
        private System.Windows.Forms.TextBox txtNewSenha;
        private System.Windows.Forms.Label lblConfirmSenha;
        private System.Windows.Forms.TextBox txtConfirmSenha;
        private System.Windows.Forms.Label lblMasterKey;
        private System.Windows.Forms.TextBox txtMasterKey;
        private System.Windows.Forms.Button btnSubmit;
    }
}