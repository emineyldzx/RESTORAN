using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace RestoranSiparisSistemi
{
    public class PanelSiparisler : Panel
    {
        private bool suppresseCellEvents = false;
        private DataGridView dgv;
        private ComboBox cmbDurum;
        private DateTimePicker dtpBaslangic, dtpBitis;
        private CheckBox chkTarihFiltre;

        public PanelSiparisler()
        {
            BackColor = Color.FromArgb(15, 15, 15);
            Build();
        }

        private void Build()
        {
            var lblBaslik = new Label
            {
                Text = "SİPARİŞLER",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(10, 20)
            };

            // Filtre bar
            var pFiltre = new Panel { Location = new Point(10, 50), Size = new Size(920, 60), BackColor = Color.FromArgb(26,26,26) };

            var lBaslangic = new Label { Text = "Başlangıç", ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 8), Font = new Font("Segoe UI", 8) };
            dtpBaslangic = new DateTimePicker { Location = new Point(10, 25), Size = new Size(160, 25), Format = DateTimePickerFormat.Short, BackColor = Color.FromArgb(36,36,36), ForeColor = Color.White };

            var lBitis = new Label { Text = "Bitiş", ForeColor = Color.Gray, AutoSize = true, Location = new Point(185, 8), Font = new Font("Segoe UI", 8) };
            dtpBitis = new DateTimePicker { Location = new Point(185, 25), Size = new Size(160, 25), Format = DateTimePickerFormat.Short, BackColor = Color.FromArgb(36,36,36), ForeColor = Color.White, Value = DateTime.Now };

            chkTarihFiltre = new CheckBox { Text = "Tarih filtrele", ForeColor = Color.Gray, Location = new Point(360, 28), AutoSize = true, Font = new Font("Segoe UI", 9) };

            var lDurum = new Label { Text = "Durum", ForeColor = Color.Gray, AutoSize = true, Location = new Point(490, 8), Font = new Font("Segoe UI", 8) };
            cmbDurum = new ComboBox { Location = new Point(490, 25), Size = new Size(160, 25), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(36,36,36), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            cmbDurum.Items.AddRange(new[] { "Tümü", "Hazırlanıyor", "Yolda", "Teslim Edildi" });
            cmbDurum.SelectedIndex = 0;

            var btnFiltrele = new Button { Text = "Filtrele", Location = new Point(665, 22), Size = new Size(90, 30), BackColor = Color.FromArgb(229,57,53), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFiltrele.FlatAppearance.BorderSize = 0;
            btnFiltrele.Click += (s, e) => Yukle();

            pFiltre.Controls.AddRange(new Control[] { lBaslangic, dtpBaslangic, lBitis, dtpBitis, chkTarihFiltre, lDurum, cmbDurum, btnFiltrele });

            dgv = new DataGridView
            {
                Location = new Point(10, 125),
                Size = new Size(920, 380),
                BackgroundColor = Color.FromArgb(26,26,26),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(50,50,50),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = false,
                Font = new Font("Segoe UI", 9),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.EditMode = DataGridViewEditMode.EditOnEnter;
           
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36,36,36);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gray;
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(26,26,26);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(50,50,50);

            if (Program.AktifRol == "Admin")
            {
                var colDurum = new DataGridViewComboBoxColumn
                {
                    Name = "DurumCol",
                    HeaderText = "Durum Değiştir",
                    Items = { "Hazırlanıyor", "Yolda", "Teslim Edildi" },
                    Width = 150,
                    ReadOnly = false,
                    FlatStyle = FlatStyle.Flat
                };
                dgv.CellValueChanged += Dgv_DurumDegistir;
                dgv.CurrentCellDirtyStateChanged += (s, e) => { if (dgv.IsCurrentCellDirty) dgv.CommitEdit(DataGridViewDataErrorContexts.Commit); };

                var colSil = new DataGridViewButtonColumn { Name = "SilCol", HeaderText = "Sil", Text = "Sil", UseColumnTextForButtonValue = true, Width = 60 };
                dgv.CellClick += Dgv_Sil;
                dgv.Columns.Add(colDurum);
                dgv.Columns.Add(colSil);
            }

            this.Controls.AddRange(new Control[] { lblBaslik, pFiltre, dgv });
        }

        public void Yukle()
        {
            string sql = @"SELECT SiparisID as 'No', MasaNo as 'Masa', 
                           strftime('%d.%m.%Y %H:%M', SiparisTarihi) as 'Tarih',
                           ToplamTutar || ' ₺' as 'Toplam',
                           Durum FROM Siparisler WHERE 1=1";

            if (chkTarihFiltre != null && chkTarihFiltre.Checked)
            {
                sql += $" AND date(SiparisTarihi) >= '{dtpBaslangic.Value:yyyy-MM-dd}'";
                sql += $" AND date(SiparisTarihi) <= '{dtpBitis.Value:yyyy-MM-dd}'";
            }

            if (cmbDurum.SelectedIndex > 0)
                sql += $" AND Durum='{cmbDurum.SelectedItem}'";

            sql += " ORDER BY SiparisID DESC";

            var dt = Database.ExecuteQuery(sql);

            // Mevcut özel sütunları koru
            dgv.Rows.Clear();
            if (dgv.Columns.Count == 0 || (dgv.Columns.Count > 0 && dgv.Columns[0].Name != "No"))
            {
                dgv.Columns.Clear();
                dgv.Columns.Add("No", "Sipariş No");
                dgv.Columns.Add("Masa", "Masa No");
                dgv.Columns.Add("Tarih", "Tarih");
                dgv.Columns.Add("Toplam", "Toplam Tutar");
                dgv.Columns.Add("Durum", "Durum");
                if (Program.AktifRol == "Admin")
                {
                    var colDurum = new DataGridViewComboBoxColumn { Name = "DurumCol", HeaderText = "Durum Değiştir", Width = 150, FlatStyle = FlatStyle.Flat };
                    colDurum.Items.AddRange(new[] { "Hazırlanıyor", "Yolda", "Teslim Edildi" });
                    dgv.Columns.Add(colDurum);
                    dgv.Columns.Add(new DataGridViewButtonColumn { Name = "SilCol", HeaderText = "Sil", Text = "Sil", UseColumnTextForButtonValue = true, Width = 60 });
                }
            }

            foreach (System.Data.DataRow row in dt.Rows)
            {
                int idx = dgv.Rows.Add(row["No"], row["Masa"], row["Tarih"], row["Toplam"], row["Durum"]);
                if (Program.AktifRol == "Admin")
                    dgv.Rows[idx].Cells["DurumCol"].Value = row["Durum"].ToString();

                string durum = row["Durum"]?.ToString() ?? "";

                Color c = durum == "Hazırlanıyor"
                    ? Color.FromArgb(245, 158, 11)
                    : durum == "Yolda"
                    ? Color.FromArgb(66, 165, 245)
                    : Color.FromArgb(76, 175, 80);

                if (Program.AktifRol == "Admin")
                {
                    try
                    {
                        dgv.Rows[idx].Cells["DurumCol"].Style.ForeColor = c;
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void Dgv_DurumDegistir(object sender, DataGridViewCellEventArgs e)
        {
            if (suppresseCellEvents) return;
            if (e.RowIndex < 0) return;
            int id = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["No"].Value);
            var cellValue = dgv.Rows[e.RowIndex].Cells["DurumCol"].Value;

            if (cellValue == null)
                return;

            string yeniDurum = cellValue.ToString();
            if (string.IsNullOrEmpty(yeniDurum)) return;
            Database.ExecuteNonQuery("UPDATE Siparisler SET Durum=@d WHERE SiparisID=@id",
                new[] { new SQLiteParameter("@d", yeniDurum), new SQLiteParameter("@id", id) });
            suppresseCellEvents = true;
            Yukle();
            suppresseCellEvents = false;
        }

        private void Dgv_Sil(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || dgv.Columns[e.ColumnIndex].Name != "SilCol") return;
            if (e.RowIndex < 0) return;
            int id = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["No"].Value);
            if (MessageBox.Show($"Sipariş #{id} silinsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Database.ExecuteNonQuery("DELETE FROM SiparisDetay WHERE SiparisID=@id", new[] { new SQLiteParameter("@id", id) });
                Database.ExecuteNonQuery("DELETE FROM Siparisler WHERE SiparisID=@id", new[] { new SQLiteParameter("@id", id) });
                Yukle();
            }
        }
    }
}
