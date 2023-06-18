﻿using Consul;
using CrmBl.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrmUi
{
    public partial class Main : Form
    {
        CrmContext db;
        Cart cart;
        Customer customer;
        CashDesk cashDesk;
        

        public Main()
        {
            InitializeComponent();
            db = new CrmContext();

            cart = new Cart(customer);
            cashDesk = new CashDesk(1, db.Sellers.FirstOrDefault(), db);
            cashDesk.IsModel = false;
        }

        private void ProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var CatalogProduct = new Catalog<Product>(db.Products, db);
            CatalogProduct.Show();
        }

        private void SellerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var CatalogSeller = new Catalog<Seller>(db.Sellers, db);
            CatalogSeller.Show();
        }

        private void CustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var CatalogCustomer = new Catalog<Customer>(db.Customers, db);
            CatalogCustomer.Show();
        }

        private void CheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var CheckCustomer = new Catalog<Check>(db.Checks, db);
            CheckCustomer.Show();
        }

        private void CustomerAddToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var form = new CustomerForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                db.Customers.Add(form.Customer);
                db.SaveChanges();
            }
        }

        private void SellerAddToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var form = new SellerForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                db.Sellers.Add(form.Seller);
                db.SaveChanges();
            }
        }

        private void ProductAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ProductForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                db.Products.Add(form.Product);
                db.SaveChanges();
            }
        }

        private void ModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ModelForm();
            form.Show();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Task.Run(() => 
            {
                listBox1.Invoke((Action)delegate
                {
                    listBox1.Items.AddRange(db.Products.ToArray());
                    UpdateLists();
                });
            });
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem is Product product)
            {
                cart.Add(product);
                listBox2.Items.Add(product);
                UpdateLists();
            } 
        }
        private void UpdateLists()
        {            
            listBox2.Items.Clear();
            listBox2.Items.AddRange(cart.GetAll().ToArray());
            label1.Text = "Итого: " + cart.Price;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var form = new Login();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                var tempCustomer = db.Customers.FirstOrDefault(c => c.Name.Equals(form.Customer.Name));
                if (tempCustomer != null)
                {
                    customer = tempCustomer;                   
                }
                else
                {
                    db.Customers.Add(form.Customer);
                    db.SaveChanges();
                    customer = form.Customer;
                }
                cart.Customer = customer;
            }

            linkLabel1.Text = $"Здравствуй, {customer.Name}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (customer != null)
            {
                cashDesk.Enqueue(cart);
                var price = cashDesk.Dequeue();
                listBox2.Items.Clear();
                cart = new Cart(customer);

                MessageBox.Show("Покупка выполнена успешно. Сумма: " + price, "Покупка выполнена", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Авторизуйтесь, пожалуйста!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
