using Contentful.Importer.Library;
using System;
using System.Windows.Forms;

namespace Contentful.Importer.App
{
    public partial class CapturePermaTokenForm : Form
    {

        internal string ResultCode { get; set; }
        internal string Username { get; set; }
        private TokenData TokenInfo { get; set; }
        public CapturePermaTokenForm()
        {
            Program.SplashScreen.Show();
            this.Hide();
            InitializeComponent();
            Load += CapturePermaTokenForm_Load;
            
        }

        private void CapturePermaTokenForm_Load(object sender, EventArgs e)
        {
            TokenData token;
            try
            {
                token = TokenData.Load();
            }
            catch (Exception)
            {
                Program.SplashScreen.Hide();
                this.Show();
                token = new TokenData();
            }
            TokenInfo = token;
            txtToken.Text = token.Token;
            if (!string.IsNullOrEmpty(token.Token))
            {
                txtToken.PasswordChar = '*';
                txtToken.ReadOnly = true;
                txtSpaceID.ReadOnly = true;
                txtUsername.Text = token.Username;
                this.txtSpaceID.Text = token.SpaceID;
                txtUsername.ReadOnly = true;
                this.ActiveControl = txtPassword;
                this.lblPassword.Text = "Password:";
               


            }
            else
            {
                btnConfirm.Text = "Save";
                this.ActiveControl = txtSpaceID;
                btnChangePassword.Visible = false;
                
            }
            System.Threading.Thread.Sleep(1500);
            Program.SplashScreen.Hide();
            this.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSpaceID.Text) && !string.IsNullOrEmpty(txtToken.Text) && !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtUsername.Text))
            {
                if (!string.IsNullOrEmpty(TokenInfo.Token))
                { //existing data
                    if (TokenInfo.Password.Equals(txtPassword.Text))
                    {
                        Library.ContentFul.Client.Instance = new Library.ContentFul.Client(txtToken.Text,txtSpaceID.Text);
                        string validationMessage = "";
                        var validate = Library.ContentFul.Client.Instance.ValidateUser(ref validationMessage, txtUsername.Text);
                        if (validate)
                        {
                            this.ResultCode = txtToken.Text;
                            this.Username = txtUsername.Text;
                            Program.SplashScreen.Show();
                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            MessageBox.Show(this, validationMessage, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show(this, "Invalid password please try again", "Validation",  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPassword.Text = "";
                        this.ActiveControl = txtPassword;
                    }
                }
                else
                { //new
                    TokenInfo.Password = txtPassword.Text;
                    TokenInfo.Token = txtToken.Text;
                    TokenInfo.SpaceID = txtSpaceID.Text;
                    TokenInfo.Username = txtUsername.Text;
                    string validationMessage = "";
                    Library.ContentFul.Client.Instance = new Library.ContentFul.Client(txtToken.Text,txtSpaceID.Text);
                    var validate = Library.ContentFul.Client.Instance.ValidateUser(ref validationMessage, txtUsername.Text);
                    if (validate)
                    {
                        TokenInfo.Save();
                        this.ResultCode = txtToken.Text;
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show(this, validationMessage, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    TokenInfo.Save();
                    this.ResultCode = txtToken.Text;
                    Program.SplashScreen.Show();
                    
                    this.DialogResult = DialogResult.OK;
                }
            }            
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this, "If you wish to change the existing password you will be required to enter the token again: \n Proceed ?", "Change Password",  MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                txtToken.PasswordChar = '\0';
                this.txtToken.ReadOnly = false;
                this.txtSpaceID.ReadOnly = false;
                this.txtToken.Text = "";
                this.txtSpaceID.Text = "";
                this.txtUsername.Text = "";
                this.lblPassword.Text = "Create Password:";
                this.ActiveControl = txtSpaceID;
                this.txtUsername.ReadOnly = false;
                TokenInfo = new TokenData();

                btnChangePassword.Visible = false;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConfirm.PerformClick();
            }
        }
    }
}
