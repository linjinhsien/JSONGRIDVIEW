using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private const string JsonUrl = "https://openapi.twse.com.tw/v1/exchangeReport/BWIBBU_ALL";

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string relativePath = @"json\BWIBBU_ALL.json";
            string jsonFilePath = Path.Combine(currentDirectory, relativePath);

            await DownloadJsonFileAsync(jsonFilePath);
            LoadJsonToDataGridView(jsonFilePath);
            Form1_Resize(sender, e);
        }

        private async Task DownloadJsonFileAsync(string filePath)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(JsonUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        File.WriteAllText(filePath, json);
                        Console.WriteLine($"JSON 檔案下載成功：{filePath}");
                    }
                    else
                    {
                        Console.WriteLine($"下載 JSON 檔案失敗：{response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下載 JSON 檔案時發生錯誤：{ex.Message}");
            }
        }

        private void LoadJsonToDataGridView(string jsonFilePath)
        {
            // Read the JSON data from the file
            string json = File.ReadAllText(jsonFilePath);

            // Deserialize the JSON string into a List<Stock> object
            List<Stock> stocks = JsonConvert.DeserializeObject<List<Stock>>(json);

            // Convert the List<Stock> to a DataTable
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Code");
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("PEratio");
            dataTable.Columns.Add("DividendYield");
            dataTable.Columns.Add("PBratio");




            foreach (var stock in stocks)
            {
                dataTable.Rows.Add(stock.Code, stock.Name, stock.PEratio, stock.DividendYield, stock.PBratio);
            }

            dataTable.Columns["Code"].ColumnName = "股票代碼";
            dataTable.Columns["Name"].ColumnName = "股票名稱";
            dataTable.Columns["PEratio"].ColumnName = "本益比";
            dataTable.Columns["DividendYield"].ColumnName = "殖利率";
            dataTable.Columns["PBratio"].ColumnName = "股價淨值比";

            // Bind the DataTable to the DataGridView
            dataGridView1.DataSource = dataTable;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int Columncount = dataGridView1.ColumnCount;
            int Width = dataGridView1.Width - 70;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.Width = Width / Columncount;
            }
        }
    }

    public class Stock
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PEratio { get; set; }
        public string DividendYield { get; set; }
        public string PBratio { get; set; }
    }
}