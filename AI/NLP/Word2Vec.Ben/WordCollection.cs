﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Word2Vec.Ben
{
    public class WordCollection
    {
        private readonly Dictionary<string, WordInfo> _words;

        public long? this[string index] => _words.ContainsKey(index) ? (long?)_words[index].Position : null;

        public WordCollection() => _words = new Dictionary<string, WordInfo>();

        public void InitWordPositions()
        {
            var wordPosition = 0L;
            foreach (var x in GetWords())
            {
                _words[x].Position = wordPosition++;
            }
        }

        public void AddWords(string line, int maxCodeLength)
            => PopulateWithWords(ParseWords(line), GetWordInfoCreator(maxCodeLength));

        public int GetNumberOfUniqueWords() => _words.Count;

        public IEnumerable<string> GetWords() => _words.Keys;

        public long GetTotalNumberOfWords() => _words.Sum(x => x.Value.Count);

        public double GetTrainWordsPow(double power)
            => _words.Sum(x => Math.Pow(x.Value.Count, power));

        public long GetOccuranceOfWord(string word) => _words[word].Count;

        public void RemoveWordsWithCountLessThanMinCount(int minCount)
        {
            foreach (var word in _words.ToArray())
            {
                if (word.Value.Count < minCount) _words.Remove(word.Key);
            }
            GC.Collect();
        }

        public void SetPoint2(string[] keys, long a, long i, long b, long[] point)
            => _words[keys[a]].Point[i - b] = (int)(point[b] - GetNumberOfUniqueWords());

        public void SetPoint(string[] keys, long a)
            => _words[keys[a]].Point[0] = GetNumberOfUniqueWords() - 2;

        public void SetCode(string[] keys, long a, long i, long b, char[] code)
            => _words[keys[a]].Code[i - b - 1] = code[b];

        private static string Clean(string input)
            => input.Replace("\r", " ")
                    .Replace("\n", " ")
                    .Replace("\t", " ")
                    .Replace(".", " ")
                    .Replace(",", " ")
                    .Replace("\"", " ")
                    .Trim()
                    .ToLower();

        private static IEnumerable<string> ParseWords(string input)
            => input.Split(new[] { "\r", "\n", "\t", " ", ",", "\"" },
                StringSplitOptions.RemoveEmptyEntries).Select(y => y.ToLower());

        private static Func<long, WordInfo> GetWordInfoCreator(int length)
            => x => new WordInfo(new char[length], new int[length], x);

        private void UpsertWord(string word, Func<long, WordInfo> createWordInfo, long position)
        {
            if (_words.ContainsKey(word)) _words[word].IncrementCount();
            else _words.Add(word, createWordInfo(position));
        }

        private void PopulateWithWords(IEnumerable<string> words,
            Func<long, WordInfo> infoCreator)
        {
            var i = 0;
            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word)) continue;
                UpsertWord(Clean(word), infoCreator, i++);
            }
        }
    }
}