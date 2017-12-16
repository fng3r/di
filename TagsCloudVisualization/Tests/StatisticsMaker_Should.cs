using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using Telerik.JustMock.Expectations;

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
            words = Enumerable.Repeat("слов", 3);
            lexems = Enumerable.Repeat(new Lexem("cлово", PartOfSpeech.Noun), 3);

            lemmatizer = Mock.Create<IWordLemmatizer>();
            Mock.Arrange(() => lemmatizer.LemmatizeWords(words)).Returns(lexems);

            filter1 = Mock.Create<IWordFilter>();
            filter2 = Mock.Create<IWordFilter>();
            Mock.Arrange(() => filter1.Filter(Arg.IsAny<Lexem>())).Returns(true);
            Mock.Arrange(() => filter2.Filter(Arg.IsAny<Lexem>())).Returns(false);

            statisticsMaker = new StatisticsMaker(lemmatizer, new[] { filter1 });
        }

        [Test]
        public void CallFilter_ForEveryWord()
        {
            statisticsMaker.MakeStatistics(words);

            Mock.Assert(() => filter1.Filter(Arg.IsAny<Lexem>()), Occurs.Exactly(words.Count()));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        public void ReturnAsManyWords_AsPassFilter(int passFilterCount)
        {
            Mock.Arrange(() => lemmatizer.LemmatizeWords(words)).Returns(new[]
            {
                new Lexem("a", Arg.IsAny<PartOfSpeech>()),
                new Lexem("б", Arg.IsAny<PartOfSpeech>()),
                new Lexem("в", Arg.IsAny<PartOfSpeech>())
            });

            var filterResults = new bool[3];
            for (var i = 0; i < passFilterCount; i++)
                filterResults[i] = true;

            Mock.Arrange(() => filter1.Filter(Arg.IsAny<Lexem>())).ReturnsMany(filterResults);

            statisticsMaker.MakeStatistics(words)
                .Should().HaveCount(passFilterCount);
        }

        [Test]
        public void ContainOnlyWords_ThatPassAllFilters()
        {
            statisticsMaker = new StatisticsMaker(lemmatizer, new[] { filter1, filter2 });

            statisticsMaker.MakeStatistics(words)
                .Should().BeEmpty();
        }

        [Test]
        public void ReturnEmptyDictionary_WhenWordsIsEmpty()
        {
            words = Array.Empty<string>();

            statisticsMaker.MakeStatistics(words)
                .Should().BeEmpty();
        }

        [Test]
        public void ReturnWordLemma_InsteadOfInitialWord()
        {
            Mock.Arrange(() => lemmatizer.LemmatizeWords(words))
                .Returns(new[] {new Lexem("слово", Arg.IsAny<PartOfSpeech>())});

            statisticsMaker.MakeStatistics(words)
                .Should().ContainKey("слово").And.NotContainKey("слов");
        }

        [Test]
        public void ReturnSameAmountOfWords_AsItsInitialCount_WhenWordsAreDifferent()
        {
            words = new[] { "оп", "ап", "хоп" };
            Mock.Arrange(() => lemmatizer.LemmatizeWords(words)).Returns(new[]
            {
                new Lexem("оп", Arg.IsAny<PartOfSpeech>()),
                new Lexem("ап", Arg.IsAny<PartOfSpeech>()),
                new Lexem("хоп", Arg.IsAny<PartOfSpeech>())
            });

            statisticsMaker.MakeStatistics(words)
                .Should().HaveCount(words.Count());
        }
    }
}