using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Text.RegularExpressions;

namespace MedicalLatinTranslator
{
    public partial class MainWindow : Window
    {
        public List<Translation> all_translations = new List<Translation>();

        public MainWindow()
        {
            InitializeComponent();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {      
            var dictionary = "";
            if(!File.Exists("dictionary.txt"))
            {
                MessageBox.Show("Отсутствует словарь dictionary.txt!");
                return;
            }
            
            dictionary = File.ReadAllText("dictionary.txt", Encoding.Default);            

            all_translations.AddRange(GetAllTranslationPairs(dictionary));
            

            //foreach(Translation translation in all_translations)
            //{
            //    if (translation.latin == "")
            //        continue;
            //    ComboBox_Search.Items.Add(translation.latin);
            //}

            //foreach (Translation translation in all_translations)
            //{
            //    foreach(string russian in translation.russian)
            //    {
            //        if (russian == "")
            //            continue;
            //        ComboBox_Search.Items.Add(russian);
            //    }
            //}

            //ComboBox_Search.Items.Clear();
        }

        private void ComboBox_Search_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox_Search.IsDropDownOpen = true;

            //var wordToTranslate = ComboBox_Search.Text;

            //if (wordToTranslate == "") return;

            ////if (ComboBox_Search.SelectedValue == null) return;

            ////var wordToTranslate = ComboBox_Search.SelectedValue.ToString();

            //LeaveComboBoxItemsThatStartsWith(ComboBox_Search.Text);
            
            //foreach(string )
            //    ComboBox_Search.Items.
        }



        Translation[] GetAllTranslationPairs(string dictionary)
        {
            MatchCollection m_steam_favorites = Regex.Matches(dictionary, @"(.*?)\s((?:[0-9]|[а-я])+.*)");

            var all_translations = new List<Translation>();
            foreach(Match m in m_steam_favorites)
            {
                string latin = m.Groups[1].Value;
                string[] translations = RemoveWordsNumeration(m.Groups[2].Value);

                all_translations.Add(new Translation() { latin = latin, russian = translations });
            }

            return all_translations.ToArray();
        }

        private string[] RemoveWordsNumeration(string words_translation)
        {          
            if (!words_translation.Contains("1"))
                return new string[] {words_translation.Replace("\r","")};

            var found_translations = new List<string>();

            bool is_in_prefix = false;
            int start_copy_pos = 0;
            int end_copy_pos = 0;
            for(int i = 0; i < words_translation.Length; i++)
            {
                if (i == words_translation.Length - 1)
                {
                    found_translations.Add(words_translation.Substring(start_copy_pos, end_copy_pos - start_copy_pos + 1).Trim().Replace("\r", ""));
                    break;
                }


                if (char.IsDigit(words_translation[i])
                || (char.IsDigit(words_translation[i - 1]) && words_translation[i] == ')'))//если текущий символ входит в префикс
                {
                    is_in_prefix = true;            //если дошли до префикса и до этого есть какой-то перевод (start_copy_pos не менялся с 0)
                    if (start_copy_pos != 0 && char.IsDigit(words_translation[i]))   //и только входим в префикс
                        found_translations.Add(words_translation.Substring(start_copy_pos, end_copy_pos - start_copy_pos + 1).Trim()); //добавляем подстроку в найденное
                }
                                        
                else
                {
                    if (!is_in_prefix) //иначе если до этого был не префикс, то увеличиваем индекс последней позиции копирования
                        end_copy_pos++;
                    else                //если до этого был префикс -> мы из него только что вышли, эта позиция - начало для копирования
                    {
                        start_copy_pos = i;
                        end_copy_pos = i;
                        is_in_prefix = false;
                    }                        
                }                
            }

            return found_translations.ToArray();
        }




        public class Translation
        {
            public string latin;
            public string[] russian;
        }

        private void ComboBox_Search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_Search.SelectedValue == null) return;

            var wordToTranslate = ComboBox_Search.SelectedValue.ToString();

            //LeaveComboBoxItemsThatStartsWith(wordToTranslate);

            //var found = false;
            
            if (IsCharLatin(wordToTranslate[0]))//если выбрано слово на латыне
            {
                foreach(Translation translation in all_translations)
                {
                    if (wordToTranslate == translation.latin)
                    {
                        TextBox_Translation.Text = "";

                        foreach(string russian in translation.russian)
                        {
                            if (translation.russian.Length == 1)
                            {
                                TextBox_Translation.Text = russian;
                                //found = true;
                                break;
                            }
                                
                            TextBox_Translation.Text += russian + (russian == translation.russian[translation.russian.Length - 1]? "" : ", "); //ставим замятую только после НЕ последних слов в вариантах перевода
                        }
                         
                        break;//нашли перевод
                    }
                }
            }
            else
            {
                foreach (Translation translation in all_translations)
                {
                    foreach(string russian in translation.russian)
                    {
                        if (wordToTranslate == russian)
                        {
                            TextBox_Translation.Text = translation.latin;
                        }
                    }
                }
            }
        }        


        void LeaveComboBoxItemsThatStartsWith(string startsWithText)
        {
            ComboBox_Search.Items.Clear();

            if(IsCharLatin(startsWithText[0]))
            {
                foreach (var translation in all_translations)
                {
                    if (translation.latin == "")
                        continue;

                    if (translation.latin.StartsWith(startsWithText))
                        ComboBox_Search.Items.Add(translation.latin);
                }
            }
            else
            {
                foreach (var translation in all_translations)
                {
                    foreach (string russian in translation.russian)
                    {
                        if (russian == "")
                            continue;

                        if (russian.StartsWith(startsWithText))
                            ComboBox_Search.Items.Add(russian);
                    }
                }
            }


            //foreach (Translation translation in all_translations)
            //{
            //    if (translation.latin == "")
            //        continue;
            //    ComboBox_Search.Items.Add(translation.latin);
            //}

            //foreach (Translation translation in all_translations)
            //{
            //    foreach (string russian in translation.russian)
            //    {
            //        if (russian == "")
            //            continue;
            //        ComboBox_Search.Items.Add(russian);
            //    }
            //}
        }

        bool IsCharLatin(char ch)
        {
            if (ch >= 'a' && ch <= 'z' ||
                ch >= 'A' && ch <= 'Z')
                return true;
            else
                return false;
        }

        private void ComboBox_Search_KeyUp(object sender, KeyEventArgs e)
        {
            var wordToTranslate = ComboBox_Search.Text;

            if (wordToTranslate == "") return;

            LeaveComboBoxItemsThatStartsWith(ComboBox_Search.Text);

            //ComboBox_Search.SelectedIndex = 1;
        }
    }
}
