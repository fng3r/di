using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class StatisticsMaker_Should
    {
        private IWordLemmatizer lemmatizer;
        private IWordFilter filter1;
        private IWordFilter filter2;
        private IEnumerable<string> words;
        private IEnumerable<Lexem> lexems;
        private StatisticsMaker statisticsMaker;

        [SetUp]
        public void SetUp()
        {
            words = Enumerable.Repeat("слов", 20);
            lexems = Enumerable.Repeat(new Lexem("слово", PartOfSpeech.Noun), 20);

            lemmatizer = Mock.Create<IWordLemmatizer>();
            lemmatizer.Arrange(l => l.LemmatizeWords(words)).Returns(lexems);

            filter1 = Mock.Create<IWordFilter>();
            filter2 = Mock.Create<IWordFilter>();
            Mock.Arrange(() => filter1.Filter(Arg.IsAny<Lexem>())).Returns(true);
            Mock.Arrange(() => filter2.Filter(Arg.IsAny<Lexem>())).Returns(false);

            statisticsMaker = new StatisticsMaker(lemmatizer, new[] { filter1 });
        }

        [Test]
        public void ShouldCallLemmatizer_OnlyOnce()
        {
            statisticsMaker.MakeStatistics(words);

            Mock.Assert(() => lemmatizer.LemmatizeWords(words), Occurs.Once());
        }

        [Test]
        public void ShouldCallFilter_ForEveryWord()
        {
            statisticsMaker.MakeStatistics(words);

            Mock.Assert(() => filter1.Filter(Arg.IsAny<Lexem>()), Occurs.Exactly(words.Count()));
        }

        [Test]
        public void ShouldNotCallOtherFilters_WhenFirstOneFails()
        {
            statisticsMaker = new StatisticsMaker(lemmatizer, new[] { filter2, filter1 });

            statisticsMaker.MakeStatistics(words);

            Mock.Assert(() => filter1.Filter(Arg.IsAny<Lexem>()), Occurs.Never());
        }

        [Test]
        public void ReturnEmptyDictionary_WhenFilterRejectEveryWord()
        {
            Mock.Arrange(() => filter1.Filter(Arg.IsAny<Lexem>())).Returns(false);

            statisticsMaker.MakeStatistics(words).Should().BeEmpty();
        }

        [Test]
        public void ReturnEmptyDictionary_WhenWordsIsEmpty()
        {
            words = Array.Empty<string>();

            statisticsMaker.MakeStatistics(words).Should().BeEmpty();
        }

        [Test]
        public void ContainOnlyWords_WithSpecifiedPartOfSpeech()
        {
            Mock.Arrange(() => lemmatizer.LemmatizeWords(words)).Returns(new[]
                {
                    new Lexem("один", PartOfSpeech.Numeral),
                    new Lexem("неудачливый", PartOfSpeech.Adjective),
                    new Lexem("два", PartOfSpeech.Numeral),
                });

            Mock.Arrange(() => filter1.Filter(Arg.IsAny<Lexem>()))
                .Returns<Lexem>(l => l.PartOfSpeech == PartOfSpeech.Numeral);

            statisticsMaker.MakeStatistics(words).Keys.ShouldBeEquivalentTo(new[] { "один", "два" });
        }

        [Test]
        public void ContainOnlyWords_ThatPassAllFilters()
        {
            statisticsMaker = new StatisticsMaker(lemmatizer, new[] { filter1, filter2 });

            statisticsMaker.MakeStatistics(words).Should().BeEmpty();
        }

        [Test]
        public void ReturnWordLemma_InsteadOfInitialWord()
        {
            statisticsMaker.MakeStatistics(words).Should().ContainKey("слово").And.NotContainKey("слов");
        }

        [Test]
        public void ReturnOnlyOneWord_WhenWordsHaveSameLexem()
        {
            lexems = new [] { new Lexem("слово", PartOfSpeech.Noun) };
            Mock.Arrange(() => lemmatizer.LemmatizeWords(words)).Returns(lexems);

            statisticsMaker.MakeStatistics(words).Count.Should().Be(1);
        }

        [Test]
        public void ReturnSameAmountOfWords_AsItsInitialCount_WhenWordsAreDifferent()
        {
            words = Enumerable.Range('а', 'я' - 'а')
                .Select(c => ((char)c).ToString())
                .ToArray();
            lexems = words.Select(w => new Lexem(w, Arg.IsAny<PartOfSpeech>()));
            Mock.Arrange(() => lemmatizer.LemmatizeWords(words)).Returns(lexems);

            statisticsMaker.MakeStatistics(words).Count.Should().Be(words.Count());
        }
    }
}