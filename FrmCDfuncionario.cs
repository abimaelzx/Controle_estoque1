using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Runtime.ConstrainedExecution;
using Mysqlx.Cursor;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;

namespace controle_de_estoque1
{
    public partial class FrmCDfuncionario : Form
    {
        private MySqlConnection conexao;
        string data_souce = "server=localhost;database=controle_estoque;Uid=root;";
        private readonly byte[] chave = GerarChave256Bits();

        private static byte[] GerarChave256Bits()
        {
            byte[] chave = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
            return chave;

        }

        Thread nt;
        public FrmCDfuncionario()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.Close();
            nt = new Thread(novofrmMenu);
            nt.SetApartmentState(ApartmentState.STA);
            nt.Start();
        }

        private void novofrmMenu()
        {
            Application.Run(new FrmMenu());
        }

        private void FrmCDfuncionario_Load(object sender, EventArgs e)
        {

        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            try
            {
                using (conexao = new MySqlConnection(data_souce))
                {
                    string sql = "INSERT INTO usuario (Nome, Cpf,Rg ,Contato,email ,pis ,cep ,bairro ,cidade,ctTrabalho,genero,estado_civil,uf ,nacionalidade,senha)" +
                        "VALUES( @Nome, @Cpf,@Rg ,@Contato,@email ,@pis ,@cep ,@bairro,@cidade,@ctTrabalho,@genero,@estado_civil,@uf ,@nacionalidade,@senha)";
                    using (MySqlCommand comando = new MySqlCommand(sql, conexao))
                    {
                     
                            string senha = txtSenha.Text;

                            if (!ValidarSenha(senha))
                            {
                                MessageBox.Show("A senha não atende aos critérios de segurança.");
                                MessageBox.Show("Pelo menos 8 caracteres de comprimento.\nPelo menos uma letra maiúscula (A-Z).\nPelo menos uma letra minúscula (a-z)." +
                                    "\nPelo menos um dígito numérico (0-9).\nPelo menos um caractere especial (!, @, #, $, %, etc.)." +
                                    "\nNão conter caracteres repetidos consecutivos.");
                                return;
                            }

                            string senhaCriptografada = CriptografarSenhaAES(senha);


                            comando.Parameters.AddWithValue("@Nome", txtNome.Text);
                        comando.Parameters.AddWithValue("@cpf", txtCpf.Text);
                        comando.Parameters.AddWithValue("@Rg", txtRg.Text);
                        comando.Parameters.AddWithValue("@Contato", txtContato.Text);
                        comando.Parameters.AddWithValue("@email", txtEmail.Text);
                        comando.Parameters.AddWithValue("@pis", txtPis.Text);
                        comando.Parameters.AddWithValue("@cep", txtCep.Text);
                        comando.Parameters.AddWithValue("@bairro", txtBairro.Text);
                        comando.Parameters.AddWithValue("@cidade", txtCidade.Text);
                        comando.Parameters.AddWithValue("@ctTrabalho", txtCarteira.Text);
                        comando.Parameters.AddWithValue("@genero", txtGenero.Text);
                        comando.Parameters.AddWithValue("@estado_civil", txtEstadocivil.Text);
                        comando.Parameters.AddWithValue("@uf", txtUf.Text);
                        comando.Parameters.AddWithValue("@nacionalidade", txtNacionalidade.Text);
                        comando.Parameters.AddWithValue("@senha", txtSenhaa.Text);

                        conexao.Open();

                        comando.ExecuteNonQuery(); 

                    }

                    MessageBox.Show("Dados cadastados com Sucesso!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro:{ex.Message}\n{ex.StackTrace}");

            }
        }

        private string CriptografarSenhaAES(string senha)
        {
            if (chave.Length != 32)
                throw new ArgumentException("A chave deve ter extamente 32 bytes)");

            byte[] iv = GerarIV();

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = chave;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(senha);

                        }
                        byte[] encryptedContent = msEncrypt.ToArray();
                        byte[] result = new byte[iv.Length + encryptedContent.Length];
                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(encryptedContent, 0, result, iv.Length, encryptedContent.Length);
                        return Convert.ToBase64String(result);


                    }

                }

            }

        }

        private byte[] GerarIV()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] iv = new byte[16];
                rng.GetBytes(iv);
                return iv;

            }

        }

        private bool ValidarSenha(string senha)
        {
            if (senha.Length < 8)
            {
                return false;
            }

            if (!Regex.IsMatch(senha, @"^(?=.[a-z])(?=.[A-Z])(?=.\d)(?=.[^\da-zA-Z]).{8,}$"))
            {
                return false;
            }

            for (int i = 0; i < senha.Length - 1; i++)
            {
                if (senha[i] == senha[i + 1])
                {
                    return false;
                }
            }

            return true;

        }


    }
}


