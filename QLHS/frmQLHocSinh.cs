using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class frmQLHocSinh : Form
    {
        QLHS_dbDataContext db = new QLHS_dbDataContext();
        public frmQLHocSinh()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void frmQLHocSinh_Load(object sender, EventArgs e)
        {
            cboLop.DataSource = db.Lops;
            cboLop.DisplayMember = "TenLop";
            cboLop.ValueMember = "MaLop";

            fillHocSinhByLop();
        }


        private void fillHocSinhByLop()
        {
            string tenHS = txtTenHS.Text.Trim();

            var dsHocSinh = db.HocSinhs
                .Where(p => p.TenHS.Contains(tenHS))
                .Select(p => new
                {
                    p.MaHS,
                    p.TenHS,
                    TenLop = p.Lop.TenLop,
                    p.DTB
                })
                .ToList();

            dgvHocSinh.DataSource = dsHocSinh;
        }


        private void cboLop_SelectedIndexChanged(object sender, EventArgs e)
        {
    
                fillHocSinhByLop();
        
        }
        private void btnTim_Click(object sender, EventArgs e)
        {
            fillHocSinhByLop();
        } 

        private void btnThem_Click(object sender, EventArgs e)
        {
            string maHS = txtMaHS.Text.Trim();
            string tenHS = txtTenHS.Text.Trim();

            if (string.IsNullOrWhiteSpace(maHS) ||
                string.IsNullOrWhiteSpace(tenHS))
            {
                MessageBox.Show("Vui lòng nhập mã và tên học sinh!");
                return;
            }

            if (!(cboLop.SelectedValue is string))
            {
                MessageBox.Show("Vui lòng chọn lớp!");
                return;
            }

            float diem;
            if (!float.TryParse(txtDiemTB.Text, out diem) ||
                diem < 0 || diem > 10)
            {
                MessageBox.Show("Điểm trung bình phải từ 0 đến 10!");
                return;
            }

            if (db.HocSinhs.Any(p => p.MaHS == maHS))
            {
                MessageBox.Show("Mã học sinh đã tồn tại!");
                return;
            }

            HocSinh hs = new HocSinh
            {
                MaHS = maHS,
                TenHS = tenHS,
                NgaySinh = dtpNgaySinh.Value,
                DiaChi = txtDiaChi.Text.Trim(),
                MaLop = cboLop.SelectedValue.ToString(),
                DTB = diem
            };

            db.HocSinhs.InsertOnSubmit(hs);
            db.SubmitChanges();

            MessageBox.Show("Thêm học sinh thành công!");
            fillHocSinhByLop();
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maHS = txtMaHS.Text.Trim();

            if (string.IsNullOrWhiteSpace(maHS))
            {
                MessageBox.Show("Vui lòng chọn học sinh cần xóa!");
                return;
            }

            HocSinh hs = db.HocSinhs.SingleOrDefault(
                p => p.MaHS == maHS
            );

            if (hs == null)
            {
                MessageBox.Show("Không tìm thấy học sinh!");
                return;
            }

            DialogResult ketQua = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa học sinh này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (ketQua == DialogResult.Yes)
            {
                db.HocSinhs.DeleteOnSubmit(hs);
                db.SubmitChanges();

                MessageBox.Show("Xóa học sinh thành công!");

                txtMaHS.Clear();
                txtTenHS.Clear();
                txtDiaChi.Clear();
                txtDiemTB.Clear();

                fillHocSinhByLop();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maHS = txtMaHS.Text.Trim();

            HocSinh hs = db.HocSinhs.SingleOrDefault(
                p => p.MaHS == maHS
            );

            if (hs == null)
            {
                MessageBox.Show("Không tìm thấy học sinh!");
                return;
            }

            if (!(cboLop.SelectedValue is string))
            {
                MessageBox.Show("Vui lòng chọn lớp!");
                return;
            }

            float diem;
            if (!float.TryParse(txtDiemTB.Text, out diem) ||
                diem < 0 || diem > 10)
            {
                MessageBox.Show("Điểm trung bình phải từ 0 đến 10!");
                return;
            }

            hs.TenHS = txtTenHS.Text.Trim();
            hs.NgaySinh = dtpNgaySinh.Value;
            hs.DiaChi = txtDiaChi.Text.Trim();
            hs.MaLop = cboLop.SelectedValue.ToString();
            hs.DTB = diem;

            db.SubmitChanges();

            MessageBox.Show("Cập nhật học sinh thành công!");
            fillHocSinhByLop();
        }


        private void dgvHocSinh_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvHocSinh.Rows[e.RowIndex];

            txtMaHS.Text = Convert.ToString(row.Cells["MaHS"].Value);
            txtTenHS.Text = Convert.ToString(row.Cells["TenHS"].Value);

            string maHS = txtMaHS.Text;

            HocSinh hs = db.HocSinhs.SingleOrDefault(p => p.MaHS == maHS);

            if (hs != null)
            {
                dtpNgaySinh.Value = hs.NgaySinh ?? DateTime.Today;
                txtDiaChi.Text = hs.DiaChi;
                txtDiemTB.Text = hs.DTB.ToString();

                cboLop.SelectedValue = hs.MaLop;
            }
        }
    }
}
