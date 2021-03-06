﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TagsCloudVisualization
{
    public class MyStemWordLemmatizer : IWordLemmatizer
    {
        private static readonly Regex linePattern = new Regex(@"(?<lemma>[а-я]+)\??=(?<pos>[A-Z]+)");
        private static readonly Dictionary<string, PartOfSpeech> partsOfSpeech = new Dictionary<string, PartOfSpeech>
        {
            ["A"] = PartOfSpeech.Adjective,
            ["ADV"] = PartOfSpeech.Adverb,
            ["CONJ"] = PartOfSpeech.Conjunction,
            ["INTJ"] = PartOfSpeech.Interjection,
            ["NUM"] = PartOfSpeech.Numeral,
            ["PART"] = PartOfSpeech.Particle,
            ["PR"] = PartOfSpeech.Preposition,
            ["S"] = PartOfSpeech.Noun,
            ["V"] = PartOfSpeech.Verb,
            ["APRO"] = PartOfSpeech.Pronoun,
            ["ADVPRO"] = PartOfSpeech.Pronoun,
            ["SPRO"] = PartOfSpeech.Pronoun,
            ["ANUM"] = PartOfSpeech.Numeral,
            ["COM"] = PartOfSpeech.CompositePart
        };

        private readonly string tempFile = "tempFile.txt";
        private readonly Process process;


        public MyStemWordLemmatizer(string mystemPath)
        {
            process = new Process
            {
                StartInfo =
                {
                    FileName = mystemPath,
                    Arguments = $"-nli {tempFile}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
        }

        public IEnumerable<Lexem> LemmatizeWords(IEnumerable<string> words)
        {
            File.WriteAllLines(tempFile, words);
            using (process)
            {
                process.Start();
                using (var textReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = textReader.ReadLine()) != null)
                    {
                        var match = linePattern.Match(line);
                        if (!match.Success) continue;
                        var lemma = match.Groups["lemma"].Value;
                        var partOfSpeech = partsOfSpeech[match.Groups["pos"].Value];
                        var lexem = new Lexem(lemma, partOfSpeech);

                        yield return lexem;
                    }
                }
            }   
            File.Delete(tempFile);
        }
    }
}
