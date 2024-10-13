using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AwakeComponents.DebugUI;
using UnityEngine.UI;

namespace AwakeComponents.Censorship
{
    [ComponentInfo("0.9.7", "12.04.2024")]
    public class CensorshipManager : MonoBehaviour, IDebuggableComponent
    {
        public List<string> words = new();
        public List<string> exceptionWords = new();
        // Для редактирования списка сразу одним текстовым поле и сохранять в PlayerPrefs
        public string savedWords;
        public string savedExceptionWords;
        public void Start()
        {
            savedWords = PlayerPrefs.GetString("savedWords");
            if (savedWords != "")
                words = new List<string>(savedWords.Split(' '));
            
            
            savedExceptionWords = PlayerPrefs.GetString("savedExceptionWords");
            if (savedExceptionWords != "")
                exceptionWords = new List<string>(savedExceptionWords.Split(' '));
        }
        
        public bool Validate(string test)
        {
            string phrase = test.ToLower().Replace(" ", "");
 
            var d = new Dictionary<char, List<char>> {
                {'а', new List<char>{ 'а', 'a', '@' }},
                {'б', new List<char>{ 'б', '6', 'b' }},
                {'в', new List<char>{'в', 'b', 'v' }},
                {'г', new List<char>{'г', 'r', 'g' }},
                {'д', new List<char>{'д', 'd' }},
                {'е', new List<char>{'е', 'e' }},
                {'ё', new List<char>{'ё', 'e' }},
                {'ж', new List<char>{'ж', 'z', '*' }},
                {'з', new List<char>{'з', '3', 'z' }},
                {'и', new List<char>{'и', 'u', 'i' }},
                {'й', new List<char>{'й', 'u', 'i' }},
                {'к', new List<char>{'к', 'k', 'i', '|' }},
                {'л', new List<char>{'л', 'l', 'j' }},
                {'м', new List<char>{'м', 'm' }},
                {'н', new List<char>{'н', 'h', 'n' }},
                {'о', new List<char>{'о', 'o', '0' }},
                {'п', new List<char>{'п', 'n', 'p' }},
                {'р', new List<char>{'р', 'r', 'p' }},
                {'с', new List<char>{'с', 'c', 's' }},
                {'т', new List<char>{'т', 'm', 't' }},
                {'у', new List<char>{'у', 'y', 'u' }},
                {'ф', new List<char>{'ф', 'f' }},
                {'х', new List<char>{'х', 'x', 'h' , '{' }},
                {'ц', new List<char>{'ц', 'c', 'u' }},
                {'ч', new List<char>{'ч'}},
                {'ш', new List<char>{'ш'}},
                {'щ', new List<char>{'щ'}},
                {'ь', new List<char>{'ь', 'b' }},
                {'ы', new List<char>{'ы', 'b' }},
                {'ъ', new List<char>{'ъ' }},
                {'э', new List<char>{'э', 'e' }},
                {'ю', new List<char>{'ю', 'i' }},
                {'я', new List<char>{'я'}}
            };
 
            foreach (var pair in d)
            {
                foreach (var letter in pair.Value)
                {
                    phrase = phrase.Replace(letter, pair.Key);
                }
            }

            if (!FindExWords(phrase))
            {
                if (FindWords(phrase))
                {
                    return false;
                }
            }

            return true;
        }

        private bool FindWords(string phrase)
        {
            foreach (var word in words)
            {
                for (int i = 0; i < phrase.Length; i++)
                {
                    var fragment = phrase.Substring(i, Math.Min(word.Length, phrase.Length - i));
                    
                    if (Distance(fragment, word) == 0) //<= word.Length * 0.25
                    {
                        if (fragment != "" && word != "")
                        {
                            Debug.Log("Найдено " + word + "\nПохоже на " + fragment);
                            return true;
                        }
                    
                    }
                }
            }

            return false;
        }

        private bool FindExWords(string phrase)
        {
            foreach (var exWord in exceptionWords)
            {
                for (int i = 0; i < phrase.Length; i++)
                {
                    var fragment = phrase.Substring(i, Math.Min(exWord.Length, phrase.Length - i));
                    
                    if (Distance(fragment, exWord) == 0) //<= word.Length * 0.25
                    {
                        if (fragment != "" && exWord != "")
                        {
                            Debug.Log("Найдено исключение " + exWord + "\nПохоже на " + fragment);
                            return true;
                        }
                    
                    }
                }
            }

            return false;
        }
 
        static int Distance(string a, string b)
        {
            int n = a.Length;
            int m = b.Length;
            int[,] d = new int[n + 1, m + 1];
 
            if (n == 0) return m;
            if (m == 0) return n;
 
            for (int i = 0; i <= n; d[i, 0] = i++) {}
            for (int j = 0; j <= m; d[0, j] = j++) {}
 
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (b[j - 1] == a[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }
            return d[n, m];
        }

        #region EditWordList

        // Add One Word
        public void AddWord(string word)
        {
            words.Add(word.ToLower());
            PlayerPrefs.SetString("savedWords", string.Join(" ", words));
            savedWords = PlayerPrefs.GetString("savedWords");
        }
        
        // Add List of Words in One STRING
        public void AddWordsList(List<string> wordsList)
        {
            words = wordsList;
            PlayerPrefs.SetString("savedWords", string.Join(" ", words));
            savedWords = PlayerPrefs.GetString("savedWords");
        }
        
        // Remove One Word
        public void RemoveWord(string word)
        {
            words.Remove(word);
            PlayerPrefs.SetString("savedWords", string.Join(" ", words));
            savedWords = PlayerPrefs.GetString("savedWords");
        }
        
        // Clear All Words
        public void ClearWords()
        {
            words.Clear();
            PlayerPrefs.DeleteKey("savedWords");
            savedWords = PlayerPrefs.GetString("savedWords");
        }
        
        public void OnSaveTextArea()
        {
            words = new List<string>(savedWords.Split(' '));
            PlayerPrefs.SetString("savedWords", savedWords);
        }
        
        // Exception words
        
        // Add One Word
        public void AddExceptionWord(string word)
        {
            exceptionWords.Add(word.ToLower());
            PlayerPrefs.SetString("savedExceptionWords", string.Join(" ", exceptionWords));
            savedExceptionWords = PlayerPrefs.GetString("savedExceptionWords");
        }
        
        // Clear All Words
        public void ClearExceptionWords()
        {
            exceptionWords.Clear();
            PlayerPrefs.DeleteKey("savedExceptionWords");
            savedExceptionWords = PlayerPrefs.GetString("savedExceptionWords");
        }
        
        public void OnSaveExceptionTextArea()
        {
            exceptionWords = new List<string>(savedExceptionWords.Split(' '));
            PlayerPrefs.SetString("savedExceptionWords", savedExceptionWords);
        }

        #endregion

        #region DebugUI

        private string newWord;
        private string testMessage;
        //private bool toggleList = false;
        public bool toggleTextArea = false;
        public bool toggleTestsBlock = false;
        
        
        private string newExWord;
        public bool toggleExWordBlock = false;
        public void RenderDebugUI()
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(10);
            // Menu Bar
            GUILayout.BeginHorizontal();
            
            GUI.backgroundColor = Color.red;
            
            if (GUILayout.Button("Clear All"))
                ClearWords();
            
            GUILayout.EndHorizontal();
            
            
            GUILayout.Space(10);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(10);
            
            // Add Words
            GUILayout.Label("Добавить слово");

            GUILayout.BeginHorizontal();
            newWord = GUILayout.TextField(newWord);
            
            GUI.backgroundColor = new Color(0f, 0.8f, 1f);
            if (GUILayout.Button("Add"))
                AddWord(newWord);
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(10);
            
            // Show List TextArea
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Show List Words TextArea"))
            {
                toggleTextArea = toggleTextArea ? false : true;
            }
            
            // show gui on toggle
            if (toggleTextArea) 
            {
                // List Words
                GUILayout.BeginVertical(GUILayout.Width(500));
                
                GUILayout.Label("Редактировать набор слов текстом разом");
                
                GUILayout.BeginHorizontal();
                
                GUI.backgroundColor = new Color(0f, 0.8f, 1f);
                if (GUILayout.Button("Save"))
                    OnSaveTextArea();
                
                GUILayout.EndHorizontal();
                savedWords = GUILayout.TextArea(savedWords, GUILayout.ExpandHeight(true));
                
                GUILayout.EndVertical();
            }
            
            GUILayout.Space(10);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(10);
            
            // EXCEPTION WORDS
            GUILayout.Label("Блок слов-исключений");
            // Show List TextArea
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Показать блок исключений"))
            {
                toggleExWordBlock = toggleExWordBlock ? false : true;
            }
            
            // show gui on toggle
            if (toggleExWordBlock) 
            {
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                GUILayout.Space(10);
                // Menu Bar
                GUILayout.BeginHorizontal();
            
                GUI.backgroundColor = Color.red;
            
                if (GUILayout.Button("Clear All"))
                    ClearExceptionWords();
            
                GUILayout.EndHorizontal();
            
            
                GUILayout.Space(10);
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                GUILayout.Space(10);
                
                // Add Words
                GUILayout.Label("Добавить слово в исключения");

                GUILayout.BeginHorizontal();
                newExWord = GUILayout.TextField(newExWord);
            
                GUI.backgroundColor = new Color(0f, 0.8f, 1f);
                if (GUILayout.Button("Add"))
                    AddExceptionWord(newExWord);
            
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10);
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                GUILayout.Space(10);
                
                // List Words
                GUILayout.BeginVertical(GUILayout.Width(300));
                
                GUILayout.Label("Редактировать набор слов-исключений");
                
                GUILayout.BeginHorizontal();
                
                GUI.backgroundColor = new Color(0f, 0.8f, 1f);
                if (GUILayout.Button("Save"))
                    OnSaveExceptionTextArea();
                
                GUILayout.EndHorizontal();
                savedExceptionWords = GUILayout.TextArea(savedExceptionWords, GUILayout.ExpandHeight(true));
                
                GUILayout.EndVertical();
            }
            
            GUILayout.Space(10);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(10);
            
            // Show List Words Button
            /*GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Show List Words"))
            {
                toggleList = toggleList ? false : true;
            }
            
            // show gui on toggle
            if (toggleList)
            {
                // List Words
                foreach (var word in words)
                {
                    GUI.backgroundColor = Color.white;

                    GUILayout.BeginHorizontal();

                    GUILayout.Button(word);
                    
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X"))
                    {
                        RemoveWord(word);
                    }
                    GUILayout.EndHorizontal();
                }
            }*/

            
            
            // Тестирование строки
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Test!"))
            {
                toggleTestsBlock = toggleTestsBlock ? false : true;
            }
            
            if (toggleTestsBlock)
            {
                // Add Words
                GUILayout.Label("Введите строку");

                GUILayout.BeginHorizontal();
                testMessage = GUILayout.TextField(testMessage);
            
                GUI.backgroundColor = new Color(0f, 0.8f, 1f);
                if (GUILayout.Button("Add"))
                    Validate(testMessage);
            
                GUILayout.EndHorizontal();
            }
        }

        #endregion
        
    }
}