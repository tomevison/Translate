﻿using Google.Cloud.Translation.V2;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KUKA.Translate
{
    public partial class Form1 : Form
    {
        String inputFilename;
        String delimiter;
        static String previewTextField;

        BackgroundWorker worker = new BackgroundWorker();

        // options
        bool KeepOriginalComment = false;
        bool OverwriteOriginalFile = false;

        public Form1()
        {
            InitializeComponent();

            // google translate API key
            string credential_path = @"C:\Users\tomevo\Source\Repos\Translate\KUKA.Translate\googleApplicationCredentials.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void OptionKeepOriginalComment_CheckedChanged(object sender, EventArgs e)
        {
            KeepOriginalComment = OptionKeepOriginalComment.Checked;
            Console.WriteLine("Option: KeepOriginalComment = " + KeepOriginalComment);
        }

        private void OptionOverwriteOriginalFile_CheckedChanged(object sender, EventArgs e)
        {
            OverwriteOriginalFile = OptionOverwriteOriginalFile.Checked;
            Console.WriteLine("Option: OverwriteOriginalFile = " + OverwriteOriginalFile);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        // displays the 'About' window
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();
        }

        // Selects the file to be translated, saves path to "inputFileName"
        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            // Show the dialog and get result.
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)// Test result.
            {
                inputFilename = openFileDialog1.FileName;
                textBox1.Text = inputFilename;

                // update file size and info
                LabelFilename.Text = "Filename: ";
                LabelFileNameUpdate.Text = openFileDialog1.SafeFileName;
                labelFileSize.Text = "Size: ";
                LabelFileSizeUpdate.Text = new FileInfo(inputFilename).Length.ToString() + " b";

                // show file in preview window
                richTextBoxPreview.Text = File.ReadAllText(inputFilename);
            }
            Console.WriteLine("input file name: " + inputFilename); // <-- For debugging use.
        }

        private void buttonTranslate_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Translating...");
            StreamReader reader = File.OpenText(inputFilename);
            string line;
            richTextBoxPreview.Text = "";

            while ((line = reader.ReadLine()) != null)
            {
                ParseLine(line);

            }
            Console.WriteLine("Translation complete..");
        }

        private String ParseLine(String input)
        {
            String output = "output";

            if (input.Contains(";")) // if the line contains a comment
            {
                string[] items = input.Split(';');
                richTextBoxPreview.AppendText(items[0]);

                string translated = translate(items[1]);

                // translate in background thread
                //TranslationBackgroundWorker.RunWorkerAsync();

                if (KeepOriginalComment)
                {
                    richTextBoxPreview.AppendText("; " + items[1]);
                    richTextBoxPreview.SelectionBackColor = Color.Gold;
                    richTextBoxPreview.AppendText(" -> " + translated);
                }
                else
                {
                    richTextBoxPreview.SelectionBackColor = Color.Gold;
                    richTextBoxPreview.AppendText("; " + translated);
                }
                Console.WriteLine(items[1] + " -> " + translated);
            }
            else
            {
                richTextBoxPreview.AppendText(input); // default statement for non-comment lines
            }
            richTextBoxPreview.AppendText("\r\n");  // insert newline

            return output;
        }

        // returns a translated string
        private String translate(String input)
        {

            TranslationClient client = TranslationClient.Create();
            TranslationResult result = client.TranslateText(input, LanguageCodes.English, LanguageCodes.German);

            return result.TranslatedText;
        }

        private void TextBoxDelimiter_TextChanged(object sender, EventArgs e)
        {
            delimiter = TextBoxDelimiter.Text;
            Console.WriteLine("Delimiter changed to: "+delimiter);
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save your translated File";
            saveFileDialog1.ShowDialog();
            SaveTranslatedFile();
        }

        private void SaveTranslatedFile()
        {
            // create a new file

            
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            // get file name
            string name = saveFileDialog1.FileName;

            // create new file
            Console.WriteLine("Create new file");
            StreamWriter writer = new StreamWriter(name);

            // Write to the selected file name
            for (int i = 0; i < richTextBoxPreview.Lines.Length; i++)
            {
                writer.WriteLine(richTextBoxPreview.Lines[i]);
            }

            // close the file
            writer.Close();
        }

        private void TranslationBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("background translation task started..");
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void LabelFileNameUpdate_Click(object sender, EventArgs e)
        {

        }
    }
}
