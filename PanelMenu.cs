using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace RestoranSiparisSistemi
{
    public class PanelMenu : Panel
    {
        private DataGridView dgv;
        private TextBox txtAd;
        private ComboBox cmbKategori;
        private NumericUpDown nudFiyat;
        private int seciliID = -1;

        public PanelMenu()
        {
            BackColor = Color.FromArgb(15, 15, 15);
            Build();
        }

        private void Build()
        {
            bool isAdmin = Program.AktifRol == "Admin";

            var lblBaslik = new Label
            {
                Text = isAdmin ? "ADMİN PANELİ - MENÜ YÖNETİMİ" : "MENÜ",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 10)
            };

            if (isAdmin)
            {
                // Form paneli
                var pForm = new Panel { Location = new Point(10, 55), Size = new Size(280, 280), BackColor = Color.FromArgb(26,26,26) };
                var lForm = new Label { Text = "Ürün Bilgileri", ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 10), Font = new Font("Segoe UI", 8, FontStyle.Bold) };

                var lAd = new Label { Text = "Ürün Adı", ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 38), Font = new Font("Segoe UI", 8) };
                txtAd = new TextBox { Location = new Point(10, 55), Size = new Size(250, 26), BackColor = Color.FromArgb(36,36,36), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) };

                var lKat = new Label { Text = "Kategori", ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 92), Font = new Font("Segoe UI", 8) };
                cmbKategori = new ComboBox { Location = new Point(10, 109), Size = new Size(250, 26), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(36,36,36), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                cmbKategori.Items.AddRange(new[] { "Yiyecek", "İçecek", "Tatlı", "Diğer" });

                var lFiyat = new Label { Text = "Fiyat (₺)", ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 146), Font = new Font("Segoe UI", 8) };
                nudFiyat = new NumericUpDown { Location = new Point(10, 163), Size = new Size(250, 26), Minimum = 0, Maximum = 99999, DecimalPlaces = 2, BackColor = Color.FromArgb(36,36,36), ForeColor = Color.White };

                var btnYeni = new Button { Text = "Yeni Ürün", Location = new Point(10, 210), Size = new Size(75, 32), BackColor = Color.FromArgb(27,94,32), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
                btnYeni.FlatAppearance.BorderSize = 0;
                btnYeni.Click += BtnYeni_Click;

                var btnGuncelle = new Button { Text = "Güncelle", Location = new Point(92, 210), Size = new Size(75, 32), BackColor = Color.FromArgb(230,81,0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
                btnGuncelle.FlatAppearance.BorderSize = 0;
                btnGuncelle.Click += BtnGuncelle_Click;

                var btnSil = new Button { Text = "Sil", Location = new Point(174, 210), Size = new Size(60, 32), BackColor = Color.FromArgb(183,28,28), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
                btnSil.FlatAppearance.BorderSize = 0;
                btnSil.Click += BtnSil_Click;

                pForm.Controls.AddRange(new Control[] { lForm, lAd, txtAd, lKat, cmbKategori, lFiyat, nudFiyat, btnYeni, btnGuncelle, btnSil });
                this.Controls.Add(pForm);
            }

            int tableX = isAdmin ? 310 : 10;
            int tableW = isAdmin ? 620 : 900;

            dgv = new DataGridView
            {
                Location = new Point(tableX, 70),
                Size = new Size(tableW, 430),
                BackgroundColor = Color.FromArgb(26,26,26),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(50,50,50),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                Font = new Font("Segoe UI", 9),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36,36,36);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gray;
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(26,26,26);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(50,50,50);
            if (isAdmin) dgv.SelectionChanged += Dgv_SelectionChanged;

            this.Controls.AddRange(new Control[] { lblBaslik, dgv });
        }

        public void Yukle()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            dgv.Columns.Add("ID", "Ürün ID");
            dgv.Columns.Add("Ad", "Ürün Adı");
            dgv.Columns.Add("Kategori", "Kategori");
            dgv.Columns.Add("Fiyat", "Fiyat");

            var dt = Database.ExecuteQuery("SELECT UrunID, UrunAdi, Kategori, Fiyat FROM Urunler ORDER BY Kategori, UrunAdi");
            foreach (System.Data.DataRow row in dt.Rows)
                dgv.Rows.Add(row["UrunID"], row["UrunAdi"], row["Kategori"], Convert.ToDecimal(row["Fiyat"]).ToString("N2") + " ₺");

            seciliID = -1;
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0 || txtAd == null) return;
            var row = dgv.SelectedRows[0];
            seciliID = Convert.ToInt32(row.Cells["ID"].Value);
            txtAd.Text = row.Cells["Ad"].Value.ToString();
            cmbKategori.Text = row.Cells["Kategori"].Value.ToString();
            string fiyatStr = row.Cells["Fiyat"].Value.ToString().Replace(" ₺","").Replace(",",".");
            if (decimal.TryParse(fiyatStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal f))
                nudFiyat.Value = f;
        }

        private void BtnYeni_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text)) { MessageBox.Show("Ürün adı boş olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbKategori.SelectedIndex < 0) { MessageBox.Show("Kategori seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            using (var conn = Database.GetConnection())
            {
             

                var kontrolCmd = new SQLiteCommand(
                    "SELECT COUNT(*) FROM Urunler WHERE UrunAdi=@a",
                    conn);

                kontrolCmd.Parameters.AddWithValue("@a", txtAd.Text.Trim());

                int kontrol = Convert.ToInt32(kontrolCmd.ExecuteScalar());

                if (kontrol > 0)
                {
                    MessageBox.Show("Bu ürün zaten mevcut!");
                    return;
                }
            }
            Database.ExecuteNonQuery("INSERT INTO Urunler (UrunAdi, Kategori, Fiyat) VALUES (@a,@k,@f)",
                new[] { new SQLiteParameter("@a", txtAd.Text.Trim()), new SQLiteParameter("@k", cmbKategori.Text), new SQLiteParameter("@f", nudFiyat.Value) });
            MessageBox.Show("Ürün eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Yukle();
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliID < 0) { MessageBox.Show("Güncellemek için ürün seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(txtAd.Text)) { MessageBox.Show("Ürün adı boş olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            Database.ExecuteNonQuery("UPDATE Urunler SET UrunAdi=@a, Kategori=@k, Fiyat=@f WHERE UrunID=@id",
                new[] { new SQLiteParameter("@a", txtAd.Text.Trim()), new SQLiteParameter("@k", cmbKategori.SelectedItem), new SQLiteParameter("@f", nudFiyat.Value), new SQLiteParameter("@id", seciliID) });
            MessageBox.Show("Ürün güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Yukle();
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (seciliID < 0) { MessageBox.Show("Silmek için ürün seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (MessageBox.Show("Bu ürünü silmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Database.ExecuteNonQuery("DELETE FROM Urunler WHERE UrunID=@id", new[] { new SQLiteParameter("@id", seciliID) });
                Yukle();
            }
        }
    }
}
